using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using InfraSim.Models;

namespace InfraSim
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].ToLower() == "demo")
            {
                RunDemo();
                return;
            }

            CreateHostBuilder(args).Build().Run();
        }

        private static void RunDemo()
        {
            Console.WriteLine("Running InfraSim Demos");
            Console.WriteLine("======================\n");
            
            ServerCapabilityDemo.RunBasicServerCapabilityDemo();
            
            Console.WriteLine("\n");
            
            ServerCapabilityDemo.RunDecoratorPatternDemo();
            
            Console.WriteLine("\n");
            
            CapabilityFactoryDemo.Run();
            
            Console.WriteLine("\n");
            
            Demo.RunCDNRoutingDemo();
            
            Console.WriteLine("\n");
            
            Demo.RunCacheRoutingDemo();
            
            Console.WriteLine("\n");
            
            Demo.RunFullTrafficRoutingDemo();
            
            Console.WriteLine("\n");
            
            Demo.RunFactoryDemo();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
