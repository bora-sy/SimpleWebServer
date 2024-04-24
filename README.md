<img src="Icon.png" alt="Alt Text" width="150">

# SimpleWebServer
[![NuGet](https://img.shields.io/nuget/v/SimpleWebServer.svg?label=NuGet)](https://nuget.org/packages/SimpleWebServer)

**Simple & easy to use library for creating web servers in C#**


Contact me from discord for additional help & suggestions

Discord: borasy

## Simple Usage

Program.cs:

```csharp
using System;

using SimpleWebServer;
using SimpleWebServer.Extensions;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            WebServer server = new WebServer("http://localhost:8080/");

            server.AddController<Controller1>();

            server.Start();

            Console.WriteLine("Started Listening");

            await Task.Delay(-1);
        }
    }
}
```

<br>

Controller1.cs:

```csharp
using System;
using System.Net;

using SimpleWebServer.Attributes;
using SimpleWebServer.Extensions;

namespace ConsoleApp1
{
    internal class Controller1
    {

        [WebPath("/")]
        public async Task Main(HttpListenerContext ctx) => ctx.Redirect("/index");

        [WebPath("/index")]
        public async Task Index(HttpListenerContext ctx)
        {
            string? name = ctx.Request.QueryString["name"];

            string response = $"Hello, {name ?? "World"}";

            bool success = await ctx.CreateHTMLResponseAsync(response);

            Console.WriteLine(success ? "Successfully created response" : "An error occured while creating response");
        }
    }
}
```

## Detailed Documentation

### WebServer

**Namespace: SimpleWebServer**

<br>

*Creating The Server:*

```csharp
WebServer server = new WebServer(string RootURL);
```

* ***string RootURL:*** The host name with scheme, subdomain *(optional)* and port *(optional)* *[Examples: "http://localhost:8080/", "https://test.testdomain.com/"]*

<br>

*Starting The Server:*

```csharp
server.Start();
```

<br>

*Stopping The Server:*

```csharp
server.Stop();
```

<br>

*Adding Controller Class To The Server:*

```csharp
server.AddController<T>(PreExecuteControllerMethod PreExecute = null);
```

* ***T:*** The Controller Class
* ***PreExecuteControllerMethod PreExecute (Optional):*** PreExecute method for this controller (This method will be executed before the controller methods to handle bulk authentication/authorization. If it returns true, the specified controller method will be executed; otherwise, the specified controller method won't be executed)
  <br>
* ***RETURNS: Added endpoint count (int)***

  <br>

*Adding single API Endpoint To The Server:*

```csharp
server.AddRoute(string path, ControllerMethod controllerMethod, HttpMethod allowedMethods = HttpMethod.ALLOW_ALL);
```

* ***string path:*** Path of the endpoint (asterisk (\*) wildcard is supported) [Examples: "/", "/index", "/api/users", "/assets/\*", "/users/modify/\*"]
* ***ControllerMethod controllerMethod:*** The method that will be executed when a user sends a request to the specified path.
* ***HttpMethod allowedMethods (Optional):*** Allowed HTTP Methods, ALLOW_ALL by default (Can be stacked using the | *(bitwise or)* character)
  <br>

#### WebServer Events

* `WebServer.On404NotFound`: This event that will be invoked when a user sends a request to an undefined path. Use this event to create custom 405 response.
  <br>
* `WebServer.On405MethodNotAllolwed`: The event that will be invoked when a user sends a request with an invalid http method. Use this event to create custom 405 response.
  <br>

#### WebServer Delegates

* `WebServer.ControllerMethod`: The method that will be executed when a user sends a request to the specified path.

  ***INPUT PARAMETER:*** HttpListenerContext
  <br>
  <br>
* `WebServer.PreExecuteControllerMethod`: This method will be executed before the controller methods to handle bulk authentication/authorization. If it returns true, the specified controller method will be executed; otherwise, the specified controller method won't be executed.

  ***INPUT PARAMETER:*** HttpListenerContext

  ***MUST RETURN:*** BOOL
  <br>

### Attributes

**Namespace: SimpleWebServer.Attributes**

```csharp
[WebPath(string Path, HttpMethod allowedMethods = HttpMethod.ALLOW_ALL)]
```

Use this attribute on Controller Methods inside Controller Classes to specify the path and HttpMethod.

* ***string Path:*** Path of the endpoint (asterisk (\*) wildcard is supported) [Examples: "/", "/index", "/api/users", "/assets/\*", "/users/modify/\*"]
* ***HttpMethod allowedMethods (Optional):*** Allowed HTTP Methods, ALLOW_ALL by default (Can be stacked using the | *(bitwise or)* character)
  <br>

### Extensions

**Namespace: SimpleWebServer.Extensions**

```csharp
HttpListenerResponse.CreateHTMLResponse(string HTMLContent, int statusCode = 200, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8");
HttpListenerResponse.CreateHTMLResponseAsync(string HTMLContent, int statusCode = 200, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8");
```

Creates an HTML response to the incoming request with necessary headers.

* ***string HTMLContent:*** HTML content in string format
* ***int statusCode (Optional):*** HTTP Status Code, 200 [OK] by default
* ***Dictionary<string, string> additionalHeaders (Optional):*** Additional HTTP headers, none by default
* ***string charset (Optional)*** HTTP Charset, UTF-8 by default
  <br>
* ***RETURNS: True if the response creation is successful; otherwise, false. (bool)***
  <br>
  <br>
  
```csharp
HttpListenerResponse.CreateCSSResponse(string CSSContent, int statusCode = 200, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8");
HttpListenerResponse.CreateCSSResponseAsync(string CSSContent, int statusCode = 200, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8");
```

Creates a CSS response to the incoming request with necessary headers.

* ***string CSSContent:*** CSS content in string format
* ***int statusCode (Optional):*** HTTP Status Code, 200 [OK] by default
* ***Dictionary<string, string> additionalHeaders (Optional):*** Additional HTTP headers, none by default
* ***string charset (Optional)*** HTTP Charset, UTF-8 by default
  <br>
* ***RETURNS: True if the response creation is successful; otherwise, false. (bool)***
  <br>
  <br>
  
```csharp
HttpListenerResponse.CreateJavaScriptResponse(string javaScriptContent, int statusCode = 200, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8");
HttpListenerResponse.CreateJavaScriptResponseAsync(string javaScriptContent, int statusCode = 200, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8");
```

Creates a JavaScript response to the incoming request with necessary headers.

* ***string javaScriptContent:*** JavaScript content in string format
* ***int statusCode (Optional):*** HTTP Status Code, 200 [OK] by default
* ***Dictionary<string, string> additionalHeaders (Optional):*** Additional HTTP headers, none by default
* ***string charset (Optional)*** HTTP Charset, UTF-8 by default
  <br>
* ***RETURNS: True if the response creation is successful; otherwise, false. (bool)***
  <br>
  <br>

```csharp
HttpListenerResponse.CreateFileResponse(string filePath, string customFileName = null, int statusCode = 200, Dictionary<string,string> additionalHeaders = null);
HttpListenerResponse.CreateFileResponseAsync(string filePath, string customFileName = null, int statusCode = 200, Dictionary<string,string> additionalHeaders = null);
```

Creates a file response to the incoming request.

* ***string filePath:*** Path of the response file
* ***string customFileName (Optional):*** Custom file name for the response file
* ***int statusCode (Optional):*** HTTP Status Code, 200 [OK] by default
* ***Dictionary<string, string> additionalHeaders (Optional):*** Additional HTTP headers, none by default
  <br>
* ***RETURNS: True if the response creation is successful; otherwise, false. (bool)***
  <br>
  <br>

```csharp
HttpListenerResponse.CreateStringResponse(string content, int statusCode = 200, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8");
HttpListenerResponse.CreateStringResponseAsync(string content, int statusCode = 200, Dictionary<string, string> additionalHeaders = null, string charset = "utf-8");
```

Creates a plain text response to the incoming request with necessary headers.

* ***string content:*** String Content
* ***int statusCode (Optional):*** HTTP Status Code, 200 [OK] by default
* ***Dictionary<string, string> additionalHeaders (Optional):*** Additional HTTP headers, none by default
* ***string charset (Optional):*** HTTP Charset, UTF-8 by default
  <br>
* ***RETURNS: True if the response creation is successful; otherwise, false. (bool)***
  <br>
  <br>

```csharp
HttpListenerResponse.CreateResponse(byte[] buffer, int statusCode = 200, string contentType = null, Dictionary<string, string> additionalHeaders = null);
HttpListenerResponse.CreateResponseAsync(byte[] buffer, int statusCode = 200, string contentType = null, Dictionary<string, string> additionalHeaders = null);
```

Creates a response to the incoming request.

* ***byte[] buffer:*** Bytes to be sent
* ***int statusCode (Optional):*** HTTP Status Code, 200 [OK] by default
* ***string contentTypeHeader (Optional):*** HTTP Content Type header, null by default
* ***Dictionary<string, string> additionalHeaders (Optional):*** Additional HTTP headers, none by default
  <br>
* ***RETURNS: True if the response creation is successful; otherwise, false. (bool)***
  <br>
  <br>

```csharp
HttpListenerResponse.Redirect(string destinationURL);
```

Redirects the incoming request to a different URL.

* ***string destinationURL:*** The destination URL for redirection
