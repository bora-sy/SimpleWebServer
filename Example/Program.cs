using SimpleWebServer;
using SimpleWebServer.Sample.Controllers;

namespace SimpleWebServer.Sample
{
    internal class Program
    {
        public static readonly string WebDataFolderPath = Path.Combine("..", "..", "..", "WebData"); // Your WebData folder path
        static async Task Main(string[] args)
        {
            // Creating the server
            WebServer server = new WebServer("http://localhost:8080/");

            // Adding the PageController class as a controller to handle HTML page requests
            server.AddController<PagesController>();

            // Adding the ApiController class as a controller to handle API requests
            server.AddController<ApiController>();

            // Adding thee AssetsController class as a controller to handle JS / CSS or any additional asset requests
            server.AddController<AssetsController>();

            // Starting the server
            server.Start();

            Console.WriteLine("Server Started");

            // Using this prevents the console application from closing
            await Task.Delay(-1);
        }
    }
}