using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;

using SimpleWebServer.Extensions;
using SimpleWebServer.Attributes;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SimpleWebServer
{
    /// <summary>
    /// Simple Web Server
    /// </summary>
    public class WebServer
    {

        /// <summary>
        /// Creation of the WebServer
        /// </summary>
        /// <param name="RootURL">Root URL [Example: "http://localhost:8080/"]</param>
        public WebServer(string RootURL)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(RootURL);
        }

        /// <summary>
        /// Adds a controller class to the WebServer
        /// </summary>
        /// <typeparam name="T">Controller Class</typeparam>
        /// <param name="PreExecute">PreExecute method for this controller (This method will be executed before the controller methods to handle bulk authentication/authorization. If it returns true, the specified controller method will be executed; otherwise, the specified controller method won't be executed)</param>
        /// <returns>Added Endpoint Count</returns>
        public int AddController<T>(PreExecuteControllerMethod PreExecute = null)
        {
            Type classType = typeof(T);
            object instance = Activator.CreateInstance(classType);
            MethodInfo[] methods = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod)
                                        .Where(m => m.DeclaringType == classType)
                                        .ToArray();
            int c = 0;
            foreach (MethodInfo method in methods)
            {
                WebPath exactPathAttr = method.GetCustomAttribute<WebPath>();
                if (exactPathAttr == null) continue;

                ControllerMethod cmethod = (ControllerMethod)method.CreateDelegate(typeof(ControllerMethod), instance);

                ControllerEndpoint endpoint = new ControllerEndpoint(
                    exactPathAttr.Path,
                    cmethod,
                    PreExecute,
                    exactPathAttr.AllowedHttpMethods
                    );

                endpoints.Add(endpoint);
                c++;
            }

            return c;
        }

        /// <summary>
        /// Starts the WebServer
        /// </summary>
        public void Start()
        {
            if (listener.IsListening) throw new Exception("WebServer has already been started.");

            listener.Start();

            Task.Run(() => Loop());
        }

        /// <summary>
        /// Stops the WebServer
        /// </summary>
        public void Stop()
        {
            if (!listener.IsListening) throw new InvalidOperationException("WebServer is not currently running.");

            listener.Stop();
        }

        /// <summary>
        /// The method that will be executed when a user sends a request to the specified path.
        /// </summary>
        public delegate void ControllerMethod(HttpListenerContext ctx);

        /// <summary>
        /// This method will be executed before the controller methods to handle bulk authentication/authorization. If it returns true, the specified controller method will be executed; otherwise, the specified controller method won't be executed.
        /// </summary>
        public delegate bool PreExecuteControllerMethod(HttpListenerContext ctx);

        /// <summary>
        /// The event that will be invoked when a user sends a request to an undefined path.
        /// </summary>
        public event ControllerMethod On404NotFound;

        /// <summary>
        /// The event that will be invoked when a user sends a request with an invalid http method
        /// </summary>
        public event ControllerMethod On405MethodNotAllowed;

        private HttpListener listener;

        private class ControllerEndpoint
        {
            private string regexExpression;
            public ControllerMethod controllerMethod { get; private set; }
            public PreExecuteControllerMethod preExecuteControllerMethod { get; private set; }
            public HttpMethods allowedHttpMethods { get; private set; }

            public ControllerEndpoint(string path, ControllerMethod controllerMethod, PreExecuteControllerMethod preExecuteControllerMethod, HttpMethods allowedHttpMethods)
            {
                this.regexExpression = WildCardToRegular(path);
                this.controllerMethod = controllerMethod;
                this.preExecuteControllerMethod = preExecuteControllerMethod;
                this.allowedHttpMethods = allowedHttpMethods;


                string WildCardToRegular(string value)
                {
                    return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
                }
            }

            public bool matchPath(string Path)
            {
                return Regex.IsMatch(Path, this.regexExpression);
            }
        }


        private Dictionary<string, (ControllerMethod, PreExecuteControllerMethod)> controller_paths = new Dictionary<string, (ControllerMethod, PreExecuteControllerMethod)>();

        List<ControllerEndpoint> endpoints = new List<ControllerEndpoint>();

        private void Loop()
        {
            while (listener.IsListening)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    Task.Run(() => HandleIncoming(context));
                }
                catch(Exception ex)
                {
                    if (!listener.IsListening) return;

                    throw ex;
                }
            }
        }

        private void HandleIncoming(HttpListenerContext ctx)
        {
            HttpListenerRequest req = ctx.Request;
            Uri uri = req.Url;
            if (uri == null) return;
            string Path = uri.AbsolutePath;

            ControllerEndpoint endpoint = endpoints.Where(x => x.matchPath(Path)).FirstOrDefault();
            if (endpoint == null)
            {
                if (On404NotFound == null) ctx.CreateStringResponse("404", 404);
                else On404NotFound(ctx);
                return;
            }

            HttpMethods? incomingMethod = getMethodFromString(ctx.Request.HttpMethod);

            if ((endpoint.allowedHttpMethods & HttpMethods.ALLOW_ALL) != 0 || (incomingMethod != null && (endpoint.allowedHttpMethods & incomingMethod) != 0))
            {
                if (endpoint.preExecuteControllerMethod != null)
                {
                    bool _continue = endpoint.preExecuteControllerMethod.Invoke(ctx);
                    if (!_continue) return;
                }

                endpoint.controllerMethod.Invoke(ctx);
            }
            else
            {
                if (On405MethodNotAllowed == null) ctx.CreateStringResponse("405", 405);
                else On405MethodNotAllowed(ctx);
            }


            HttpMethods? getMethodFromString(string method)
            {
                if (method == "ALLOW_ALL") return null;

                if (!Enum.TryParse<HttpMethods>(method, out HttpMethods res)) return null;

                return res;
            }


        }

    }


}
