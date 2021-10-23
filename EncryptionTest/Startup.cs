using System;
using EncryptionTest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.DependencyInjection;
using Xunit.DependencyInjection.Logging;

namespace EncryptionTest
{
    public class Startup
    {
        public void ConfigureHost(IHostBuilder hostBuilder) =>
            hostBuilder
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var environmentName = Environment.GetEnvironmentVariable("Hosting:Environment");
                    builder
                        .AddJsonFile("appsettings.json")
                        .AddEnvironmentVariables();
                    context.Configuration = builder.Build();
                    
                });


        public void ConfigureServices(IServiceCollection services, HostBuilderContext context)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(
                        context.Configuration.GetConnectionString("DefaultConnection"));
                    //options.AddRelationalTypeMappingSourcePlugin<EncryptAttributeTypeMappingPlugin>();
                }
            );
            services.AddLogging();
        }
    }
}