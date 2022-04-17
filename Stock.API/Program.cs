using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stock.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var a = CreateHostBuilder(args).Build();
            //bu scope sayesinde iþlem bitince memoryden düsürcek otomatik
                using(var scope=a.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                context.Stocks.Add(new Models.Stocks() { Id = 1, ProductId = 1, Count = 200 });
                context.Stocks.Add(new Models.Stocks() { Id = 2, ProductId = 2, Count = 500 });
                context.SaveChanges();

            }


            a.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
