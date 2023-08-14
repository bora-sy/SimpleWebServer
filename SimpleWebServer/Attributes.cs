using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWebServer.Attributes
{
    /// <summary>
    /// WebPath Attribute
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method)]
    public class WebPath : Attribute
    {
        /// <summary>
        /// Specify the absolute endpoint path by using this attribute
        /// </summary>
        /// <param name="AbsPath">Absolute Path of the endpoint [Examples: "/", "/index", "/api/login"]</param>
        public WebPath(string AbsPath)
        {
            this.absPath = AbsPath;
        }

        /// <summary>
        /// Absolute Path of the endpoint [Examples: "/", "/index", "/api/login"]
        /// </summary>
        public string AbsPath
        {
            get { return this.absPath; }
        }
        private string absPath;
    }


    /*
    [System.AttributeUsage(AttributeTargets.Method)]
    public class WebSubDirectoryPath : Attribute
    {
        public WebSubDirectoryPath(string Path)
        {
            this.path = Path;
        }
        public string Path
        {
            get { return this.path; }
        }
        private string path;
    }
    */
}
