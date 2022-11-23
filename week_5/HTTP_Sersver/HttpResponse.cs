
namespace NetConsoleApp;

internal static partial class HttpResponse
{
    public static byte[]? GetFile(string? rawUrl, string directory, string urlReffer, out string contentType)
    {
        byte[]? result = null;
        var filePath = directory + urlReffer + rawUrl;
        if (Directory.Exists(filePath))
        {
            filePath += "/index.html";
            if (File.Exists(filePath))
                result = File.ReadAllBytes(filePath);
        }
        else if (File.Exists(filePath))
            result = File.ReadAllBytes(filePath);
        
        contentType = GetContentType(rawUrl);
        return result;
    }
    
    private static string GetContentType(string path)
    {
        var ext = path.Contains('.') ? path.Split('.')[^1] : "html";
        return DictionaryOfTypes.ContainsKey(ext) ? DictionaryOfTypes[ext] : "text/plain";
    }
}