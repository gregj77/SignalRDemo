using System;
using Microsoft.Owin.Hosting;
using WebApplication.Infrastructure;

namespace WebApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://*:8089";

            using (WebApp.Start<InfrastructureStartup>(url))
            {
                Console.ReadLine();
            }
        }
    }
}
