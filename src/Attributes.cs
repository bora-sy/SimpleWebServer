using System;
using System.Collections.Generic;
using System.Text;
using static SimpleWebServer.WebServer;

namespace SimpleWebServer.Attributes
{
    /// <summary>
    /// WebPath Attribute
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method)]
    public class WebPath: Attribute
    {
        /// <summary>
        /// Specify the endpoint path by using this attribute
        /// </summary>
        /// <param name="Path">Path of the endpoint (asterisk (*) wildcard is supported) [Examples: "/", "/index", "/api/users", "/assets/*", "/users/modify/*"]</param>
        /// <param name="allowedMethods">Allowed HTTP Methods, ALLOW_ALL by default (Can be stacked using the | (bitwise or) character)</param>
        public WebPath(string Path, HttpMethod allowedMethods = HttpMethod.ALLOW_ALL)
        {
            this.path = Path;
            this.allowedHttpMethods = allowedMethods;
        }

        /// <summary>
        /// Absolute Path of the endpoint [Examples: "/", "/index", "/api/login"]
        /// </summary>
        public string Path
        {
            get { return this.path; }
        }
        private string path;

        /// <summary>
        /// Allowed HTTP Methods
        /// </summary>
        public HttpMethod AllowedHttpMethods
        {
            get { return this.allowedHttpMethods; }
        }
        private HttpMethod allowedHttpMethods;
    }

}
