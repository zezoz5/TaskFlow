using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Core.DTOs.Auth;
using TaskManager.Infrastructure.Data;
using TaskManager.IntegrationTests.Factories;

namespace TaskManager.IntegrationTests
{
    public class HealthTests
    {
        [Fact]
        public void Application_CanCreateClient()
        {
            // Arrange
            var factory = new TaskManagerWebApplicationFactory();

            // Act
            var client = factory.CreateClient();

            // Assert
            Assert.NotNull(client);
        }

        [Fact]
        public async Task Register_ReturnsAuthResponse()
        {
            // Arrange
            var factory = new TaskManagerWebApplicationFactory();
            var client = factory.CreateClient();

            var dto = new RegisterDto
            {
                FullName = "Integration Test User",
                Email = $"test-{Guid.NewGuid()}@example.com",
                Password = "Integration@123"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/Auth/register", dto);
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine(error);

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

            // Assert HTTP response
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(dto.Email, result.Email);
            Assert.Equal(dto.FullName, result.FullName);
            Assert.False(string.IsNullOrEmpty(result.Token));

            // Assert Database
            using var scope = factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = await context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);

            Assert.NotNull(user);
            Assert.Equal(dto.FullName, user.FullName);
        }
    }
}