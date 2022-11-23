namespace NetConsoleApp.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal class HttpController : Attribute
{
    public readonly string ControllerName;

    public HttpController(string controllerName = "")
    {
        ControllerName = controllerName;
    }
}