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
        /// <param name="prefix">Web Prefix [Example: "http://localhost:8080/"]</param>
        public WebServer(string prefix)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(prefix);
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
                //WebSubDirectoryPath? subDirectoryPathAttr = method.GetCustomAttribute<WebSubDirectoryPath>();

                //if (exactPathAttr == null && subDirectoryPathAttr == null) continue;
                if (exactPathAttr == null) continue;

                ControllerMethod cmethod = (ControllerMethod)method.CreateDelegate(typeof(ControllerMethod), instance);



                if (exactPathAttr != null)
                {
                    controller_exactPaths.Add(exactPathAttr.AbsPath, (cmethod, PreExecute));
                    c++;
                }
                //else if (subDirectoryPathAttr != null) { controller_subDirectories.Add(subDirectoryPathAttr.Path, (cmethod, PreExecute)); c++; }



            }

            return c;
        }

        /// <summary>
        /// Starts the WebServer
        /// </summary>
        public void Start()
        {
            listener.Start();

            Task.Run(() => Loop());
        }

        /// <summary>
        /// Stops the WebServer
        /// </summary>
        public void Stop()
        {
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



        private HttpListener listener;

        private Dictionary<string, (ControllerMethod, PreExecuteControllerMethod)> controller_exactPaths = new Dictionary<string, (ControllerMethod, PreExecuteControllerMethod)>();

        //private Dictionary<string, (ControllerMethod, PreExecuteControllerMethod)> controller_subDirectories = new Dictionary<string, (ControllerMethod, PreExecuteControllerMethod)>();

        private void Loop()
        {
            while (listener.IsListening)
            {
                HttpListenerContext context = listener.GetContext();
                HandleIncoming(context);
            }
        }

        private void HandleIncoming(HttpListenerContext ctx)
        {

            HttpListenerRequest req = ctx.Request;
            Uri uri = req.Url;
            if (uri == null) return;
            string path = uri.AbsolutePath;

            (ControllerMethod, PreExecuteControllerMethod) toInvoke;
            if (controller_exactPaths.ContainsKey(path)) toInvoke = controller_exactPaths[path];
            // else if (controller_subDirectories.ContainsKey(req.Url.getFirstSubDirectoryOfAbsPath())) toInvoke = controller_subDirectories[req.Url.getFirstSubDirectoryOfAbsPath()];
            else
            {
                if (On404NotFound == null) ctx.CreateResponse("404", 404);
                else On404NotFound(ctx);

                return;
            }

            if (toInvoke.Item2 != null)
            {
                bool _continue = toInvoke.Item2.Invoke(ctx);
                if (!_continue) return;
            }

            toInvoke.Item1.Invoke(ctx);

        }
    }
}
