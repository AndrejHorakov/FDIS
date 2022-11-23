namespace NetConsoleApp;

public class SettingParameters
{
    public int Port { get; set; } 
    
    public string Directory { get; set; } = null!;

    public SettingParameters(int port, string directory)
    {
        Port = port;
        Directory = directory;
    }

    public SettingParameters() { }
}