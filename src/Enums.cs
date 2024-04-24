using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWebServer
{
    /// <summary>
    /// Http Methods
    /// </summary>
    [Flags]
    public enum HttpMethod
    {
        /// <summary>
        /// Http Method GET
        /// </summary>
        GET = 1,

        /// <summary>
        /// Http Method POST
        /// </summary>
        POST = 2,

        /// <summary>
        /// Http Method PUT
        /// </summary>
        PUT = 4,

        /// <summary>
        /// Http Method PATCH
        /// </summary>
        PATCH = 8,

        /// <summary>
        /// Http Method DELETE
        /// </summary>
        DELETE = 16,

        /// <summary>
        /// Http Method HEAD
        /// </summary>
        HEAD = 32,

        /// <summary>
        /// Http Method OPTIONS
        /// </summary>
        OPTIONS = 64,

        /// <summary>
        /// Using this will allow any valid / invalid Http Methods
        /// </summary>
        ALLOW_ALL = 128
    }
}
