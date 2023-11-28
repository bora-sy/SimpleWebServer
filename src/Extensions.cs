using System;
using System.Collections.Generic;
using System.IO;
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
        /// Creates an HTML response to the incoming request with necessary headers.
        /// </summary>
        /// <param name="ctx">The main context</param>
        /// <param name="HTMLContent">HTML content in string format</param>
        /// <param name="statusCode">HTTP Status Code, 200 [OK] by default</param>
        /// <param name="additionalHeaders">Additional HTTP headers</param>
        /// <param name="charset">HTTP Charset, UTF-8 by default</param>
        /// <returns>True if the response creation is successful; otherwise, false.</returns>
        public static bool CreateHTMLResponse(this HttpListenerContext ctx, string HTMLContent, int statusCode = 200, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8")
        {
            return ctx.CreateResponse(Encoding.UTF8.GetBytes(HTMLContent), statusCode, $"text/html; charset={charset};", additionalHeaders);
        }


        /// <summary>
        /// Creates a CSS response to the incoming request with necessary headers.
        /// </summary>
        /// <param name="ctx">The main context</param>
        /// <param name="CSSContent">CSS content in string format</param>
        /// <param name="statusCode">HTTP Status Code, 200 [OK] by default</param>
        /// <param name="additionalHeaders">Additional HTTP headers</param>
        /// <param name="charset">HTTP Charset, UTF-8 by default</param>
        /// <returns>True if the response creation is successful; otherwise, false.</returns>
        public static bool CreateCSSResponse(this HttpListenerContext ctx, string CSSContent, int statusCode = 200, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8")
        {
            return ctx.CreateResponse(Encoding.UTF8.GetBytes(CSSContent), statusCode, $"text/css; charset={charset};", additionalHeaders);
        }


        /// <summary>
        /// Creates a JavaScript response to the incoming request with necessary headers.
        /// </summary>
        /// <param name="ctx">The main context</param>
        /// <param name="javaScriptContent">CSS content in string format</param>
        /// <param name="statusCode">HTTP Status Code, 200 [OK] by default</param>
        /// <param name="additionalHeaders">Additional HTTP headers</param>
        /// <param name="charset">HTTP Charset, UTF-8 by default</param>
        /// <returns>True if the response creation is successful; otherwise, false.</returns>
        public static bool CreateJavaScriptResponse(this HttpListenerContext ctx, string javaScriptContent, int statusCode = 200, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8")
        {
            return ctx.CreateResponse(Encoding.UTF8.GetBytes(javaScriptContent), statusCode, $"application/javascript; charset={charset};", additionalHeaders);
        }


        /// <summary>
        /// Creates a plain text response to the incoming request with necessary headers.
        /// </summary>
        /// <param name="ctx">The main context</param>
        /// <param name="content">String Content</param>
        /// <param name="statusCode">HTTP Status Code, 200 [OK] by default</param>
        /// <param name="additionalHeaders">Additional HTTP headers</param>
        /// <param name="charset">HTTP Charset, UTF-8 by default</param>
        /// <returns>True if the response creation is successful; otherwise, false.</returns>
        public static bool CreateStringResponse(this HttpListenerContext ctx, string content, int statusCode = 200, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8")
        {
            return ctx.CreateResponse(Encoding.UTF8.GetBytes(content), statusCode, $"text/plain; charset={charset};", additionalHeaders);
        }


        /// <summary>
        /// Creates a response to the incoming request.
        /// </summary>
        /// <param name="ctx">The main context</param>
        /// <param name="buffer">Bytes to be sent</param>
        /// <param name="statusCode">HTTP Status Code, 200 [OK] by default</param>
        /// <param name="contentTypeHeader">HTTP Content Type header, null by default</param>
        /// <param name="additionalHeaders">Additional HTTP headers</param>
        /// <returns>True if the response creation is successful; otherwise, false.</returns>
        public static bool CreateResponse(this HttpListenerContext ctx, byte[] buffer, int statusCode = 200, string contentTypeHeader = null, Dictionary<string, string> additionalHeaders = null)
        {
            try
            {
                HttpListenerResponse response = ctx.Response;
                if (additionalHeaders != null) additionalHeaders.ToList().ForEach(x => response.Headers[x.Key] = x.Value);
                response.ContentType = contentTypeHeader;
                response.StatusCode = statusCode;
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
        /// Creates a file response to the incoming request.
        /// </summary>
        /// <param name="ctx">The main context</param>
        /// <param name="filePath">Path of the response file</param>
        /// <param name="customFileName">Custom file name for the response</param>
        /// <param name="statusCode">HTTP Status Code, 200 [OK] by default</param>
        /// <param name="additionalHeaders">Additional HTTP headers</param>
        /// <returns>True if the response creation is successful; otherwise, false.</returns>
        public static bool CreateFileResponse(this HttpListenerContext ctx, string filePath, string customFileName = null, int statusCode = 200, Dictionary<string,string> additionalHeaders = null)
        {
            try
            {
                if (!File.Exists(filePath)) return false;

                if (additionalHeaders != null) additionalHeaders.ToList().ForEach(x => ctx.Response.Headers[x.Key] = x.Value);

                ctx.Response.StatusCode = statusCode;
                ctx.Response.ContentLength64 = new FileInfo(filePath).Length;


                string filename = customFileName ?? Path.GetFileName(filePath);
                ctx.Response.Headers["Content-Disposition"] = $"attachment; filename={filename}";


                using (FileStream fs = File.OpenRead(filePath))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ctx.Response.OutputStream.Write(buffer, 0, bytesRead);
                    }
                }

                ctx.Response.OutputStream.Close();
                ctx.Response.Close();
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
        /// <param name="destinationURL">The destination URL for redirection</param>
        public static void Redirect(this HttpListenerContext ctx, string destinationURL)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.Redirect;
            ctx.Response.AddHeader("Location", destinationURL);
            ctx.Response.Close();
        }

    }
}
