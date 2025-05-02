using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using InfraSim.Models.Capability;
using InfraSim.Models.Server;
using InfraSim.Models.Mediator;
using InfraSim.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace InfraSim
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddScoped<IServerCapability, ServerCapability>(); // Registers the server capability interface with a scoped lifetime 
                    services.AddSingleton<ICapabilityFactory, ServerCapability>(); // Registers the capability factory as a singleton 
                    services.AddSingleton<IServerFactory, ServerFactory>(); // Registers the server factory as a singleton 
                    services.AddSingleton<IInfrastructureMediator, InfrastructureMediator>();
                    services.AddDbContext<InfraSimContext>();
                    services.AddScoped<IRepositoryFactory, RepositoryFactory>();
                    services.AddSingleton<IUnitOfWork, UnitOfWork>(); // Registers the unit of work as a singleton 
                    services.AddSingleton<IServerDataMapper, ServerDataMapper>();
                    services.AddSingleton<ICommandManager, CommandManager>();
                });
    }
}
