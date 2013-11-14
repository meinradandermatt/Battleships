using System;
using Microsoft.Owin.Hosting;

namespace Battleships.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            string port = "58886";
            if (args.Length == 1)
            {
                port = args[0];
            }

            string url = "http://localhost:" + port;
            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }
    }
}
