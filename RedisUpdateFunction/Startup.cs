using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RyderGyde.ShopNotes.RedisUpdateTrigger;
using RyderGyde.ShopNotes.RedisUpdateTrigger.Persistence;
using RyderGyde.ShopNotes.RedisUpdateTrigger.Services;
using System;
using System.Reflection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace RyderGyde.ShopNotes.RedisUpdateTrigger
{
    internal class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var context = builder.GetContext();
            var environment = Environment.GetEnvironmentVariable("AZURE_FUNCTION_ENVIRONMENT") ?? context.EnvironmentName;

            builder.ConfigurationBuilder
                .SetBasePath(context.ApplicationRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environment}.json", true, true)
                .AddEnvironmentVariables();

            _configuration = builder.ConfigurationBuilder.Build();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Register AutoMapper to Service Collection
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register DBContext to service collection
            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseCosmos(_configuration["poc-alfre_DOCUMENTDB"], _configuration.GetConnectionString("CosmosDb:DataBase"));
            });

            // Register REDIS Cache to Service Collection
            builder.Services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = $"{_configuration.GetConnectionString("Redis:DefaultConnection")},defaultDatabase={_configuration.GetConnectionString("Redis:DataBase")}";
                opt.InstanceName = "shop_notes_";
            });

            // Register Services
            builder.Services.AddScoped<IVehicleService, VehicleService>();
            builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        }
    }
}
