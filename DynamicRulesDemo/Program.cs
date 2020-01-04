using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicRulesDemo.Models.Db;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DynamicRulesDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //1. Get the IHost which will host this application.
            var host = CreateHostBuilder(args).Build();
            //2. Find the service layer within our scope.
            using (var scope = host.Services.CreateScope())
            {
                //3. Get the instance of OneTechDbContext in our services layer
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<OneTechDbContext>();

                //4. Call the DataGenerator to create sample data
                DataGenerator.Initialize(services);
            }
            //5.Continue to run the application
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
