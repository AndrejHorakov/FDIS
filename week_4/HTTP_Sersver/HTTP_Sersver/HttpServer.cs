using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace NetConsoleApp;

internal class HttpServer
{
    private static bool isRunning;
    private static bool isStoping;
    private readonly HttpListener listener;
    private string responseStr;
    
    public HttpServer()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8888/" );
        // listener.Prefixes.Add("http://localhost:8888/");
        String path = "../../../../html/google/index.html";
        if (File.Exists(path))
            responseStr = File.ReadAllText(path); 
        else
        {
            Stop();
            Console.WriteLine("Файл не был найден, сервер остановлен");
        }
    }

    public void Start()
    {
        listener.Start();
        isRunning = true;
        var answer = new Thread(Listen);
        answer.Start();
        while (isRunning)
        {
            switch (Console.ReadLine())
            {
                case "start":
                    Console.WriteLine("Сервер уже запущен");
                    break;
                case "stop":
                    isRunning = false;
                    isStoping = true;
                    break;
                case "end":
                    Console.WriteLine("You realy want exit? <yes> or <no>");
                    switch (Console.ReadLine())
                    {
                        case "yes":
                            isRunning = false;
                            isStoping = false;
                            listener.Stop();
                            break;
                        case "no":
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Undefind operation try again");
                    break;
            }
            if (isStoping)
                Stop();
        }
    }

    public void Stop()
    {
        if (!listener.IsListening)
            return;
        listener.Stop();
        Console.WriteLine("Сервер остановлен");
        while (isStoping)
        {
            switch (Console.ReadLine())
            {
                case "start":
                    isRunning = true;
                    isStoping = false;
                    break;
                case "stop":
                    Console.WriteLine("Сервер уже остановлен");
                    break;
                case "end":
                    Console.WriteLine("You realy want exit? <yes> or <no>");
                    switch (Console.ReadLine())
                    {
                        case "yes":
                            isRunning = false;
                            isStoping = false;
                            break;
                        case "no":
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Undefind operation try again");
                    break;
            }

            if (isRunning)
                Start();
        }
    }

    private void Listen()
    {
        try
        {
            HttpListenerContext _httpContext = listener.GetContext();

            HttpListenerRequest request = _httpContext.Request;

            HttpListenerResponse response = _httpContext.Response;
        
            byte[] buffer = Encoding.UTF8.GetBytes(responseStr);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer);

            output.Close();
            Listen();
        }
        catch 
        {
            if (!listener.IsListening)
                return;
            Console.WriteLine("Server crashed");
            var response = listener.GetContext().Response;
            response.StatusCode = 500;
            Stop();
        }
    }

    private static void printHelpCommands()
    {
        Console.Write("Вам доступны команды:\n" +
                      "start      - запустить сервер (запущен по умолчанию)\n" +
                      "stop       - завершить работу сервера\n" +
                      "restart    - перезапустить сервер\n" +
                      "help       - вывести это сообщение ещё раз\n" +
                      "end        - завершить работу\n");
    }
}