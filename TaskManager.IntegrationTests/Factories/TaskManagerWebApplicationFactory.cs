using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Infrastructure.Data;

namespace TaskManager.IntegrationTests.Factories
{
    public class TaskManagerWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        Environment.GetEnvironmentVariable("TEST_CONNECTION_STRING")
                });
            });

            builder.ConfigureServices(async service =>
            {
                using var serviceProvider = service.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();

                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                context.Database.Migrate();
            });
        }
    }
}