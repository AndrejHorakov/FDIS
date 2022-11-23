using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using NetConsoleApp.Attributes;

namespace NetConsoleApp.ResponseLogic;

internal partial class HttpResponseProvider : IHttpResponseProvider
{
    
    public ServerResponse GetServerResponse(HttpListener listener, SettingParameters settings, HttpListenerContext httpContext)
    {
        var fullPath = Path.GetFullPath(settings.Directory);
        var rawUrl = httpContext?.Request.RawUrl?.Replace("%20", " ");
        var urlRefer = httpContext?.Request.UrlReferrer?.AbsolutePath;
        if (Directory.Exists(fullPath))
        {
            var output = GetStaticFile(rawUrl, fullPath, urlRefer);
            if (output.ResponseCode != HttpStatusCode.NotFound)
                return output;
        }

        return GetDateFromController(httpContext.Request, rawUrl);
    }


    private ServerResponse GetDateFromController(HttpListenerRequest request, string rawUrl)
    {
        if (request.Url!.Segments.Length < 2) return GetResponseNotFound(rawUrl);
        
        using var sr = new StreamReader(request.InputStream, request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        
        var controllerName = request.Url.Segments[1].Replace("/", "");
        var strParams = request.Url.Segments
            .Skip(2)
            .Select(s => s.Replace("/", ""))
            .Concat(bodyParam.Split('&').Select(p => p.Split('=').LastOrDefault()))
            .ToArray();

        var assembly = Assembly.GetExecutingAssembly();
        var controller = assembly.GetTypes()
            .Where(t => Attribute.IsDefined(t, typeof(HttpController)))
            .FirstOrDefault(t => string.Equals(
                (t.GetCustomAttribute(typeof(HttpController)) as HttpController)?.ControllerName,
                controllerName,
                StringComparison.CurrentCultureIgnoreCase));

        var method = controller?.GetMethods()
            .FirstOrDefault(t => t.GetCustomAttributes(true)
                .Any(attr => attr.GetType().Name == $"Http{request.HttpMethod}"
                             && Regex.IsMatch(request.RawUrl ?? "",
                                 attr.GetType()
                                     .GetField("MethodUri")?
                                     .GetValue(attr)?.ToString() ?? "")));
        if (method is null) return GetResponseNotFound(rawUrl);

        var queryParams = method.GetParameters()
            .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
            .ToArray();
        
        var ret = method.Invoke(Activator.CreateInstance(controller!), queryParams);
        var buffer = request.HttpMethod == "POST" ?
            Array.Empty<byte>() :
            Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
        var statusCode = buffer.Length > 0 ? HttpStatusCode.OK : HttpStatusCode.Redirect;
        return new ServerResponse(buffer, "application/json", statusCode);
    }
    private ServerResponse GetStaticFile(string? rawUrl, string fullPath, string urlRefer)
    {
        var buffer = GetFile(rawUrl, fullPath, urlRefer, out var contentType);
        return buffer is null ? 
            GetResponseNotFound(rawUrl) :
            new ServerResponse(buffer, contentType, HttpStatusCode.OK);
    }

    private ServerResponse GetResponseNotFound(string? rawUrl) =>
        new ServerResponse(Encoding.UTF8.GetBytes($"File {rawUrl} not found 404."), 
            "text/plain", HttpStatusCode.NotFound);
    
    private static byte[]? GetFile(string? rawUrl, string directory, string? urlRefer, out string contentType)
    {
        byte[]? result = null;
        var filePath = directory + urlRefer + rawUrl;
        if (Directory.Exists(filePath))
        {
            filePath += "/index.html";
            if (File.Exists(filePath))
                result = File.ReadAllBytes(filePath);
        }
        else if (File.Exists(filePath))
            result = File.ReadAllBytes(filePath);
        
        contentType = GetContentType(rawUrl!);
        return result;
    }
    
    private static string GetContentType(string path)
    {
        var ext = path.Contains('.') ? path.Split('.')[^1] : "html";
        return DictionaryOfTypes.ContainsKey(ext) ? DictionaryOfTypes[ext] : "text/plain";
    }
    
}