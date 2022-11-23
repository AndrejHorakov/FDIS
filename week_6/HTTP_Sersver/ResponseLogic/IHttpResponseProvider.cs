using System.Net;

namespace NetConsoleApp.ResponseLogic;

public interface IHttpResponseProvider
{
    ServerResponse GetServerResponse(HttpListener listener, SettingParameters settings, HttpListenerContext request);
}