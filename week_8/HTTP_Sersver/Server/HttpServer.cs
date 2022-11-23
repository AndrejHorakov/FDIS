using System.Net;
using System.Text.Json;
using static System.GC;
using NetConsoleApp.ResponseLogic;

namespace NetConsoleApp;

internal class HttpServer : IDisposable
{

    private SettingParameters _settings;
    private readonly HttpListener _listener;
    private readonly IHttpResponseProvider _provider;
    private const string SettingPath = "./Settings/settings.json"; 
    
    public HttpServer(IHttpResponseProvider provider)
    {
        _provider = provider;
        _listener = new HttpListener();
        _settings = new SettingParameters();
        Start();
    }

    public void Start()
    {
        if (_listener.IsListening)
        {
            Console.WriteLine("Server is already running");
            return;
        }

        if (File.Exists(Path.GetFullPath(SettingPath)))
            _settings = JsonSerializer.Deserialize<SettingParameters>(File.ReadAllText(SettingPath))
                       ?? new SettingParameters();
        _listener.Prefixes.Clear();
        _listener.Prefixes.Add($"http://localhost:{_settings.Port}/" );
        _listener.Start();
        Listen();
    }

    public void Stop()
    {
        if (!_listener.IsListening)
        {
            Console.WriteLine("Server is already stopped");
            return;
        }

        try
        {
            _listener.Stop();
        }
        catch(Exception e)
        {
            Console.WriteLine($"Stopping end exception {e.Message}");
        }
    }

    private void Listen() => _listener.BeginGetContext(ServerResponse, _listener);
    
    private void ServerResponse(IAsyncResult request)
    {
        try
        {
            if (_listener is null) return;
        
            var httpContext = _listener?.EndGetContext(request);
            var response = httpContext.Response;

            var serverResponse = _provider.GetServerResponse(_listener, _settings, httpContext);
            response.Headers.Set("Content-Type", serverResponse.ContentType);
            response.StatusCode = (int)serverResponse.ResponseCode;

            if (serverResponse.ResponseCode is HttpStatusCode.Redirect)
                response.Redirect(@"http://steampowered.com");

            var output = response.OutputStream;
            output.WriteAsync(serverResponse.Buffer, 0, serverResponse.Buffer.Length);
            Task.WaitAll();

            output.Close();
            response.Close();

            Listen();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }


    public void Dispose()
    {
        Stop();
        SuppressFinalize(this);
    }
}