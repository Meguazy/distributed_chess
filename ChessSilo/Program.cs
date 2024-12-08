using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Orleans.Hosting;

namespace ChessSilo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Website: Waiting");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Host.UseOrleans(siloBuilder =>
            {
                siloBuilder.UseLocalhostClustering();
            });

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}