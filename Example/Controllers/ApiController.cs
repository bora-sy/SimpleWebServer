using SimpleWebServer.Attributes;
using SimpleWebServer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebServer.Sample.Controllers
{
    internal class ApiController
    {
        // http://localhost:8080/api/generaterandomnumber
        [WebPath("/api/generaterandomnumber")]
        public async Task generateRandomNum(HttpListenerContext ctx)
        {
            int randomNum = new Random().Next(int.MaxValue);

            await ctx.CreateStringResponseAsync(randomNum.ToString());

            Console.WriteLine("Generated Number: " + randomNum.ToString());
        }
    }
}
