using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using SimpleWebServer.Attributes;
using SimpleWebServer.Extensions;

namespace SimpleWebServer.Sample.Controllers
{
    internal class PagesController
    {
        // http://localhost:8080/ AND http://localhost:8080 (They both are captured by "/" )
        [WebPath("/")]
        public void main(HttpListenerContext ctx)
        {
            ctx.Redirect("/index");
        }

        // http://localhost:8080/index
        [WebPath("/index")]
        public void index(HttpListenerContext ctx)
        {
            string filePath = Path.Combine(Program.WebDataFolderPath, "pages", "index.html");

            string htmlContent = File.ReadAllText(filePath);

            ctx.CreateHTMLResponse(htmlContent);
        }

        // http://localhost:8080/randomnumbergenerator
        [WebPath("/randomnumbergenerator")]
        public void randomnumbergenerator(HttpListenerContext ctx)
        {
            string filePath = Path.Combine(Program.WebDataFolderPath, "pages", "randomnumbergenerator.html");

            string htmlContent = File.ReadAllText(filePath);

            ctx.CreateHTMLResponse(htmlContent);
        }

    }
}
