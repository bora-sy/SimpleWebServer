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
            object controllerInstance = Activator.CreateInstance(classType);
            MethodInfo[] methods = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod)
                                        .Where(m => m.DeclaringType == classType)
                                        .ToArray();

            List<ControllerEndpoint> newEndpoints = new List<ControllerEndpoint>();

            for(int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];

                WebPath webPath = method.GetCustomAttribute<WebPath>();
                if (webPath == null) continue;

                if (!IsMethodInfoValidControllerMethod(method)) 
                    throw new InvalidOperationException($"The method '{method.Name}' has invalid parameters or an invalid return type. Please review the method signature. (Controller Name: '{classType.Name}')");

                if (newEndpoints.Where(x => x.checkConflict(webPath)).FirstOrDefault() != null || endpoints.Where(x => x.checkConflict(webPath)).FirstOrDefault() != null)
                    throw new Exception($"The method '{method.Name}' has a conflicting Controller Method that captures the same path using the same HTTP method(s). Please consider either altering the target path or adjusting the allowed HTTP methods.");

                ControllerMethod controllerMethod = (ControllerMethod)method.CreateDelegate(typeof(ControllerMethod), controllerInstance);
                
                ControllerEndpoint endpoint = new ControllerEndpoint(
                    webPath.Path,
                    controllerMethod,
                    PreExecute,
                    webPath.AllowedHttpMethods
                    );

                newEndpoints.Add(endpoint);
            }

            endpoints.AddRange(newEndpoints);

            return newEndpoints.Count;



            bool IsMethodInfoValidControllerMethod(MethodInfo methodInfo)
            {
                Type delegateType = typeof(ControllerMethod);

                if (!delegateType.IsSubclassOf(typeof(Delegate)) || !methodInfo.IsPublic)
                {
                    return false;
                }

                MethodInfo invokeMethod = delegateType.GetMethod("Invoke");
                ParameterInfo[] delegateParameters = invokeMethod.GetParameters();
                ParameterInfo[] methodParameters = methodInfo.GetParameters();

                if (delegateParameters.Length != methodParameters.Length)
                {
                    return false;
                }

                for (int i = 0; i < delegateParameters.Length; i++)
                {
                    if (delegateParameters[i].ParameterType != methodParameters[i].ParameterType)
                    {
                        return false;
                    }
                }

                Type delegateReturnType = invokeMethod.ReturnType;
                Type methodReturnType = methodInfo.ReturnType;

                return delegateReturnType == methodReturnType || delegateReturnType == typeof(void);
            }
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
        public delegate Task ControllerMethod(HttpListenerContext ctx);

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
                }
            }
        }

        private void HandleIncoming(HttpListenerContext ctx)
        {
            HttpListenerRequest req = ctx.Request;
            Uri uri = req.Url;
            if (uri == null) return;
            string Path = uri.AbsolutePath;

            ControllerEndpoint[] pathMatchedEndpoints = endpoints.Where(x => x.matchPath(Path)).ToArray();
            if (pathMatchedEndpoints.Length == 0)
            {
                if (On404NotFound == null) ctx.CreateStringResponse("404", 404);
                else On404NotFound(ctx);
                return;
            }

            HttpMethods? incomingMethod = getMethodFromString(ctx.Request.HttpMethod);

            ControllerEndpoint targetEndpoint = pathMatchedEndpoints.Where(x => x.matchHttpMethod(incomingMethod)).FirstOrDefault();

            if(targetEndpoint == null)
            {
                if (On405MethodNotAllowed == null) ctx.CreateStringResponse("405", 405);
                else On405MethodNotAllowed(ctx);
                return;
            }

            if(targetEndpoint.preExecuteControllerMethod != null)
            {
                bool _continue = targetEndpoint.preExecuteControllerMethod.Invoke(ctx);
                if (!_continue) return;
            }

            targetEndpoint.controllerMethod.Invoke(ctx);


            HttpMethods? getMethodFromString(string method)
            {
                if (method == "ALLOW_ALL") return null;

                if (!Enum.TryParse<HttpMethods>(method, out HttpMethods res)) return null;

                return res;
            }


        }

    }


}
