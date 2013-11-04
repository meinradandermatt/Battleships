using System;
using Microsoft.Owin.Hosting;

namespace Battleships.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://localhost:58886";
            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }
    }
}
