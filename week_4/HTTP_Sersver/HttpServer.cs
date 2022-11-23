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
            settings = JsonSerializer.Deserialize<SettingParameters>(File.ReadAllBytes(settingPath))
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

        listener.Stop();
    }

    private void Listen() => listener.BeginGetContext(ServerResponse, listener);
    
    private void ServerResponse(IAsyncResult request)
    {
        if (!listener.IsListening) 
            return;
        var _httpContext = listener.EndGetContext(request);
        byte[] buffer;
        var response = _httpContext.Response;
        var path = Path.GetFullPath(@"../../../../sites");
        if (Directory.Exists(path))
        {
            response.Headers.Set("Content-Type", "text/html");
            buffer = File.ReadAllBytes(path + "/google/index.html");
            response.StatusCode = (int)HttpStatusCode.OK;
        }
        else
        {
            Stop();
            response.Headers.Set("Content-Type", "text/plain");
            buffer = Encoding.UTF8.GetBytes("File not found 404. Server was stopped");
        }
        
        response.ContentLength64 = buffer.Length;
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