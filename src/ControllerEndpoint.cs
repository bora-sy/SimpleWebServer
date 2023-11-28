using SimpleWebServer.Attributes;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using static SimpleWebServer.WebServer;

namespace SimpleWebServer
{

    internal class ControllerEndpoint
    {
        public string path { get; private set; }
        private string regexExpression;
        public ControllerMethod controllerMethod { get; private set; }
        public PreExecuteControllerMethod preExecuteControllerMethod { get; private set; }
        public HttpMethods allowedHttpMethods { get; private set; }
        public ControllerEndpoint(string path, ControllerMethod controllerMethod, PreExecuteControllerMethod preExecuteControllerMethod, HttpMethods allowedHttpMethods)
        {
            this.path = path;
            this.regexExpression = WildCardToRegular(path);
            this.controllerMethod = controllerMethod;
            this.preExecuteControllerMethod = preExecuteControllerMethod;
            this.allowedHttpMethods = allowedHttpMethods;


            string WildCardToRegular(string value)
            {
                return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
            }
        }

        public bool checkConflict(WebPath wp)
        {
            if (path != wp.Path) return false;

            if(wp.AllowedHttpMethods == HttpMethods.ALLOW_ALL) return true;

            bool isMethodMatch = matchHttpMethod(wp.AllowedHttpMethods);
            return isMethodMatch;
        }

        public bool matchHttpMethod(HttpMethods? incomingRequestMethod)
        {
            if ((allowedHttpMethods & HttpMethods.ALLOW_ALL) != 0) return true;

            if(incomingRequestMethod == null) return false;

            if ((allowedHttpMethods & incomingRequestMethod) != 0) return true;

            return false;
        }
        
        public bool matchPath(string Path)
        {
            return Regex.IsMatch(Path, this.regexExpression);
        }
    }
}
