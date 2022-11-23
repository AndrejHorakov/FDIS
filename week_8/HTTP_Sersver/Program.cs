using NetConsoleApp.ResponseLogic;

namespace NetConsoleApp
{
    internal static class Program
    {
        private static bool _isRunning;
        private static bool _work = true;
        private static readonly IHttpResponseProvider Provider = new HttpResponseProvider();

        private static void Main()
        {
            var server = new HttpServer(Provider);
            server.Start();
            _isRunning = true;
            PrintHelpCommands();
            while (_work)
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
                    Console.WriteLine(_isRunning ? "Server is running" : "Server is stopped");
                    break;
                    
                case "start":
                    if (_isRunning)
                        Console.WriteLine("Server is already running");
                    else
                    {
                        Console.WriteLine("Starting the server");
                        server.Start();
                        _isRunning = true;
                    }
                    break;
                
                case "stop":
                    if (!_isRunning)
                        Console.WriteLine("Server already is stopped");
                    else
                    {
                        Console.WriteLine("Stopping the server");
                        server.Stop();
                        _isRunning = false;
                    }
                    break;
                
                case "restart":
                    if (_isRunning)
                        server.Stop();
                    server.Start();
                    Console.WriteLine("Server was restarted");
                    break;
                
                case "exit":
                    Console.WriteLine("You really want exit? <yes> or <no>");
                    switch (Console.ReadLine())
                    {
                        case "yes":
                            _work = false;
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
