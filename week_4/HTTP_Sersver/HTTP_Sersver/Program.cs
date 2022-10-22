using System.Net;

namespace NetConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            new HttpServer().Start();
        }
    }
}