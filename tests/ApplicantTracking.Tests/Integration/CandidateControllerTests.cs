using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ApplicantTracking.Api;
using ApplicantTracking.Application.Commands;
using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ApplicantTracking.Tests.Integration
{
    public class CandidateControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CandidateControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicantTrackingContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApplicantTrackingContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateCandidate_ValidData_ReturnsCreated()
        {
            // Arrange
            var command = new CreateCandidateCommand
            {
                Name = "John Doe",
                Email = "john@example.com",
                Phone = "1234567890",
                LinkedIn = "linkedin.com/johndoe",
                City = "New York",
                State = "NY"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(command),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/candidates", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }

        [Fact]
        public async Task GetCandidates_ReturnsSuccessAndData()
        {
            // Act
            var response = await _client.GetAsync("/candidates");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var candidates = JsonSerializer.Deserialize<Candidate[]>(
                responseString, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.NotNull(candidates);
        }
    }
}