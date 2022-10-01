using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace NetConsoleApp;

internal class HttpServer
{
    private readonly HttpListener listener;
    
    public HttpServer(string port, string name)
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:" + port + "/" + name + "/");
        // listener.Prefixes.Add("http://localhost:8888/");
    }

    public void Start()
    {
        listener.Start();
    }

    public void Stop()
    {
        listener.Stop();
    }
    
    public void giveResponse(string responseStr)
    {
        HttpListenerContext _httpContext = listener.GetContext();

        HttpListenerRequest request = _httpContext.Request;

        HttpListenerResponse response = _httpContext.Response;
        
        byte[] buffer = Encoding.UTF8.GetBytes(responseStr);

        response.ContentLength64 = buffer.Length;
        Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);

        output.Close();
    }
}