using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SimpleWebServer.Extensions
{
    /// <summary>
    /// extensions
    /// </summary>
    public static class extensions
    {
        /// <summary>
        /// Creates an HTML response to the incoming request.
        /// </summary>
        /// <param name="ctx">The main context</param>
        /// <param name="htmlContent">HTML content</param>
        /// <param name="additionalHeaders">Additional HTTP headers</param>
        /// <param name="charset">HTTP charset, utf-8 by default</param>
        /// <returns>True if the response creation is successful; otherwise, false.</returns>
        public static bool CreateHTMLResponse(this HttpListenerContext ctx, string htmlContent, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8")
        {
            return ctx.CreateResponse(Encoding.UTF8.GetBytes(htmlContent), 200, $"text/html; charset={charset};", additionalHeaders);
        }

        /// <summary>
        /// Creates a simple string response to the incoming request.
        /// </summary>
        /// <param name="ctx">The main context</param>
        /// <param name="content">String Content</param>
        /// <param name="code">HTTP response code, 200 [OK] by default</param>
        /// <param name="additionalHeaders">Additional HTTP headers</param>
        /// <param name="charset">HTTP charset, utf-8 by default</param>
        /// <returns>True if the response creation is successful; otherwise, false.</returns>
        public static bool CreateResponse(this HttpListenerContext ctx, string content, int code = 200, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8")
        {
            return ctx.CreateResponse(Encoding.UTF8.GetBytes(content), code, $"text/plain; charset={charset};", additionalHeaders);
        }

        /// <summary>
        /// Creates a response to the incoming request.
        /// </summary>
        /// <param name="ctx">The main context</param>
        /// <param name="buffer">Bytes to be sent</param>
        /// <param name="code">HTTP response code, 200 [OK] by default</param>
        /// <param name="contentType">HTTP content type, null by default</param>
        /// <param name="additionalHeaders">Additional HTTP headers</param>
        /// <returns>True if the response creation is successful; otherwise, false.</returns>
        public static bool CreateResponse(this HttpListenerContext ctx, byte[] buffer, int code = 200, string contentType = null, Dictionary<string, string> additionalHeaders = null)
        {
            try
            {
                HttpListenerResponse response = ctx.Response;
                if (additionalHeaders != null) additionalHeaders.ToList().ForEach(x => response.Headers.Add(x.Key, x.Value));
                response.ContentType = contentType;
                response.StatusCode = code;
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
                response.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        /// <summary>
        /// Redirects the incoming request to a different URL.
        /// </summary>
        /// <param name="ctx">The main context.</param>
        /// <param name="redirectUrl">The URL to redirect to.</param>
        public static void Redirect(this HttpListenerContext ctx, string redirectUrl)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.Redirect;
            ctx.Response.AddHeader("Location", redirectUrl);
            ctx.Response.Close();
        }

        /*
         public static string getFirstSubDirectoryOfAbsPath(this Uri uri)
         {
             string abspath = uri.AbsolutePath;

             string[] pairs = abspath.Split('/');
             if (pairs.Length == 1) return pairs[0];
             return $"{pairs[0]}/{pairs[1]}";
         }
        */


    }
}
