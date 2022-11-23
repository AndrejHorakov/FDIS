using System.Net;

namespace NetConsoleApp
{
    internal static class Program
    {
        private static bool isRunning;
        private static bool work = true;

        private static void Main(string[] args)
        {
            var server = new HttpServer();
            server.Start();
            isRunning = true;
            PrintHelpCommands();
            while (work)
                ExecuteCommand(Console.ReadLine()?.ToLower() ?? "", server);
            Console.WriteLine("ServerApp was stopped");
            Console.ReadLine();
        }

        private static void ExecuteCommand(string command, HttpServer server)
        {
            switch (command)
            {
                case "help":
                    PrintHelpCommands();
                    break;
                
                case "status":
                    Console.WriteLine(isRunning ? "Server is running" : "Server is stopped");
                    break;
                    
                case "start":
                    if (isRunning)
                        Console.WriteLine("Server is already running");
                    else
                    {
                        Console.WriteLine("Starting the server");
                        server.Start();
                        isRunning = true;
                    }
                    break;
                
                case "stop":
                    if (!isRunning)
                        Console.WriteLine("Server already is stopped");
                    else
                    {
                        Console.WriteLine("Stopping the server");
                        server.Stop();
                        isRunning = false;
                    }
                    break;
                
                case "restart":
                    if (isRunning)
                        server.Stop();
                    server.Start();
                    Console.WriteLine("Server was restarted");
                    break;;
                
                case "exit":
                    Console.WriteLine("You really want exit? <yes> or <no>");
                    switch (Console.ReadLine())
                    {
                        case "yes":
                            work = false;
                            break;
                        case "no":
                            break;
                    }
                    break;
                
                default:
                    Console.WriteLine("Undefined operation try again");
                    break;
            }
        }
        
        private static void PrintHelpCommands()
        {
            Console.Write("Вам доступны команды:\n" +
                          "start      - запустить сервер (запущен по умолчанию)\n" +
                          "stop       - завершить работу сервера\n" +
                          "restart    - перезапустить сервер\n" +
                          "help       - вывести это сообщение ещё раз\n" +
                          "exit        - завершить работу\n");
        }
    }
}
