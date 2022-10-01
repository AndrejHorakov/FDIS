using System.Text.RegularExpressions;

namespace NetConsoleApp;

public static class Server_Logic
{
    private static HttpServer server;
    private static bool isRestarted;
    private static bool isRunning;
    private static string port;
    private static string name;
    public static void Start()
    {
        var enter = GetPortAndName();
        port = enter[0];
        name = enter[1];
        server = new HttpServer(port, name);
        server.Start();
        if (!isRestarted)
            printHelpCommands();

        while (processRequest()) ;
    }

    private static string[] GetPortAndName()
    {
        Console.WriteLine("Пожалуйста, введите \"порт/имя сервера\"\n");
        var enter = Console.ReadLine().Split('/', StringSplitOptions.RemoveEmptyEntries); 
        Regex regexPort = new Regex(@"[0-9]{4,5}");
        while (!regexPort.IsMatch(enter[0]))
        {
            Console.WriteLine("Пожалуйста, введите <порт/имя сервера>\n");
            enter = Console.ReadLine().Split('/', StringSplitOptions.RemoveEmptyEntries);
        }

        return enter;
    }

    private static bool processRequest()
    {
        var enter = Console.ReadLine();
        switch (enter)
        {
            case "start":
                if (isRunning)
                    Console.WriteLine("Сервер уже запущен");
                else
                {
                    Console.WriteLine("Вы хотите ввести новые параматры? (<yes> or <no>)");
                    if (Console.ReadLine() == "yes")
                        CreateNewServer();
                    isRestarted = true;
                    server.Start();
                }
                break;
            case "stop":
                if (isRunning)
                {
                    server.Stop();
                    Console.WriteLine("Сервер остановлен");
                }
                else
                    Console.WriteLine("Сервер уже остановлен");
                break;
            case "restart":
                if (!isRunning)
                {
                    Console.WriteLine("Сервер не запущен");
                    break;
                }
                server.Stop();
                server.Start();
                break;
            case "help":
                printHelpCommands();
                break;
            case "end":
                return false;
            default:
                SendRequest(enter);
                break;
        }

        return true;
    }

    private static void printHelpCommands()
    {
        Console.Write("Вам доступны команды:\n" +
                      "<name page>     - открыть html файл по адресу " +
                      "(адрес одним словом без заглавных букв, например: \"google\"\n" +
                      "start      - запустить сервер (запущен по умолчанию)\n" +
                      "stop       - завершить работу сервера\n" +
                      "restart    - перезапустить сервер\n" +
                      "help       - вывести это сообщение ещё раз\n" +
                      "end        - завершить работу\n");
    }

    private static void CreateNewServer()
    {
        var enter = GetPortAndName();
        port = enter[0];
        name = enter[1];
        server = new HttpServer(port, name);
    }

    private static void SendRequest(string namePage)
    {
        String responseStr;
        String path = "../../../../html/" + namePage + "/index.html";
        if (File.Exists(path))
        {
            responseStr = File.ReadAllText(path);
            server.giveResponse(responseStr);
        }
        else
        {
            server.Stop();
            Console.WriteLine("Файл не был найден, сервер остановлен");
        }
    }
}