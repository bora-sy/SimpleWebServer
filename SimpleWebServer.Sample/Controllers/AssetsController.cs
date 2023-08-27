using SimpleWebServer.Attributes;
using SimpleWebServer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleWebServer.Sample.Controllers
{
    internal class AssetsController
    {
        // This will capture every incoming request whose path with /assets/css/ (Examples: http://localhost:8080/assets/css/styles.css, http://localhost:8080/assets/css/main.css)
        [WebPath("/assets/css/*")]
        public void handleCSS(HttpListenerContext ctx)
        {
            string fileName = ctx.Request.Url.AbsolutePath.Substring("/assets/css/".Length);

            // Check if the filename matches the pattern of alphabetic characters followed by ".css"
            // NOTE: Performing this regex check is essential to prevent directory traversal attacks where users could potentially access other directories using '..' (e.g., /../../Directory)
            if (!Regex.IsMatch(fileName, @"^[a-zA-Z]+\.css$"))
            {
                ctx.CreateStringResponse("404", 404);
                return;
            }

            string filePath = Path.Combine(Program.WebDataFolderPath, "assets","css", fileName);

            if(!File.Exists(filePath))
            {
                ctx.CreateStringResponse("404", 404);
                return;
            }

            string content = File.ReadAllText(filePath);

            ctx.CreateCSSResponse(content);
        }


        // This will capture every incoming request whose path starts with /assets/js/ (Examples: http://localhost:8080/assets/js/script.js, http://localhost:8080/assets/js/main.js)
        [WebPath("/assets/js/*")]
        public void handleJS(HttpListenerContext ctx)
        {
            string fileName = ctx.Request.Url.AbsolutePath.Substring("/assets/js/".Length);

            // Check if the filename matches the pattern of alphabetic characters followed by ".js"
            // NOTE: Performing this regex check is essential to prevent directory traversal attacks where users could potentially access other directories using '..' (e.g., /../../Directory)
            if (!Regex.IsMatch(fileName, @"^[a-zA-Z]+\.js$"))
            {
                ctx.CreateStringResponse("404", 404);
                return;
            }

            string filePath = Path.Combine(Program.WebDataFolderPath, "assets","js", fileName);

            if (!File.Exists(filePath))
            {
                ctx.CreateStringResponse("404", 404);
                return;
            }

            string content = File.ReadAllText(filePath);

            ctx.CreateJavaScriptResponse(content);
        }
    }
}
