using System.Text;
using System.Net;
using System.Text.Json;
using static System.GC;

namespace NetConsoleApp;

internal class HttpServer : IDisposable
{

    private SettingParameters settings;
    private readonly HttpListener listener;
    private const string settingPath = "./settings.json"; 
    
    public HttpServer()
    {
        listener = new HttpListener();
        settings = new SettingParameters();
        Start();
    }

    public void Start()
    {
        if (listener.IsListening)
        {
            Console.WriteLine("Server is already running");
            return;
        }

        if (File.Exists(settingPath))
            settings = JsonSerializer.Deserialize<SettingParameters>(File.ReadAllText(settingPath))
                       ?? new SettingParameters();
        listener.Prefixes.Clear();
        listener.Prefixes.Add($"http://localhost:{settings.Port}/" );
        listener.Start();
        Listen();
    }

    public void Stop()
    {
        if (!listener.IsListening)
        {
            Console.WriteLine("Server is already stopped");
            return;
        }

        try
        {
            listener.Stop();
        }
        catch(Exception e)
        {
            Console.WriteLine($"Stopping end exeption {e.Message}");
        }
    }

    private void Listen() => listener.BeginGetContext(ServerResponse, listener);
    
    private void ServerResponse(IAsyncResult request)
    {
        if (!listener.IsListening) 
            return;
        var httpContext = listener.EndGetContext(request);
        byte[]? buffer;
        var response = httpContext.Response;
        var fullPath = Path.GetFullPath(settings.Directory);
        var rawUrl = httpContext.Request.RawUrl?.Replace("%20", " ");
        var urlReffer = httpContext.Request.UrlReferrer?.AbsolutePath;
        var contentType = "text/plain";
        if (Directory.Exists(fullPath))
        {
            response.Headers.Set("Content-Type", "text/html");
            
            buffer = HttpResponse.GetFile(rawUrl, fullPath, urlReffer, out contentType) 
                     ?? Encoding.UTF8.GetBytes($"File {rawUrl} not found 404.");
            response.StatusCode = (int)HttpStatusCode.OK;
        }
        else
        {
            buffer = Encoding.UTF8.GetBytes("File 'sites' not found 404.");
            response.StatusCode = (int)HttpStatusCode.NotFound;
        }
        
        response.Headers.Set("Content-Type", contentType);
        response.ContentLength64 = buffer!.Length;
        var output = response.OutputStream;
        output.Write(buffer);

        output.Close();
        Listen();
    }


    public void Dispose()
    {
        Stop();
        SuppressFinalize(this);
    }
}