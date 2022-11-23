namespace NetConsoleApp;

public class SettingParameters
{
    public int Port { get; set; } 
    
    public string Directory { get; set; }

    public SettingParameters(int port = 8080, string directory = @"./site/index.html")
    {
        Port = port;
        Directory = directory;
    }
}