namespace NetConsoleApp.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class HttpGET : Attribute
{
    public readonly string MethodUri;

    public HttpGET(string methodUri = "")
    {
        MethodUri = methodUri;
    }
}