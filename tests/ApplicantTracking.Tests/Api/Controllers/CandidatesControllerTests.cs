using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ApplicantTracking.Application.DTOs;
using ApplicantTracking.Domain.Enumerators;
using ApplicantTracking.Infrastructure.Context;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ApplicantTracking.Tests.Api.Controllers
{
    public class CandidatesControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public CandidatesControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostCandidate_WithValidData_ShouldReturnCreatedAndCandidateDto_AndCreateTimeline()
        {
            // Arrange
            var uniqueEmail = $"test.user.{Guid.NewGuid()}@example.com";
            var createDto = new CreateCandidateDto
            {
                Name = "Valid",
                Surname = "User",
                Email = uniqueEmail,
                Birthdate = new DateTime(1990, 1, 15)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/candidates", createDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdCandidate = await response.Content.ReadFromJsonAsync<CandidateDto>();

            createdCandidate.Should().NotBeNull();
            createdCandidate?.Name.Should().Be(createDto.Name);
            createdCandidate?.Email.Should().Be(uniqueEmail.ToLowerInvariant());
            createdCandidate?.IdCandidate.Should().BeGreaterThan(0);

            response.Headers.Location.Should().NotBeNull();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var timelineEntry = await context.Timelines
                    .FirstOrDefaultAsync(t => t.IdAggregateRoot == createdCandidate!.IdCandidate && t.IdTimelineType == TimelineTypes.Create);
                timelineEntry.Should().NotBeNull();
                timelineEntry?.NewData.Should().Contain(uniqueEmail.ToLowerInvariant());
            }
        }

        [Fact]
        public async Task PostCandidate_WithExistingEmail_ShouldReturnConflict()
        {
            // Arrange
            var existingEmail = $"existing.{Guid.NewGuid()}@example.com";
            var initialCreateDto = new CreateCandidateDto { Name = "Existing", Surname = "User", Email = existingEmail, Birthdate = new DateTime(1980, 1, 1) };
            await _client.PostAsJsonAsync("/candidates", initialCreateDto);

            var duplicateCreateDto = new CreateCandidateDto { Name = "Duplicate", Surname = "User", Email = existingEmail, Birthdate = new DateTime(1995, 1, 1) };

            // Act
            var response = await _client.PostAsJsonAsync("/candidates", duplicateCreateDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problemDetails.Should().NotBeNull();
            problemDetails?.Title.Should().Be("Conflict");
        }

        [Fact]
        public async Task PostCandidate_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidCreateDto = new CreateCandidateDto { Name = "", Surname = "User", Email = "notanemail", Birthdate = DateTime.Now.AddYears(-10) };

            // Act
            var response = await _client.PostAsJsonAsync("/candidates", invalidCreateDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            validationProblemDetails.Should().NotBeNull();
        }


        [Fact]
        public async Task GetAllCandidates_ShouldReturnOkAndListOfCandidates()
        {
            // Arrange
            await _client.PostAsJsonAsync("/candidates", new CreateCandidateDto { Name = "Test1", Surname = "User1", Email = $"test1.{Guid.NewGuid()}@test.com", Birthdate = new DateTime(1991, 1, 1) });
            await _client.PostAsJsonAsync("/candidates", new CreateCandidateDto { Name = "Test2", Surname = "User2", Email = $"test2.{Guid.NewGuid()}@test.com", Birthdate = new DateTime(1992, 1, 1) });

            // Act
            var response = await _client.GetAsync("/candidates");

            // Assert
            response.EnsureSuccessStatusCode();
            var candidates = await response.Content.ReadFromJsonAsync<List<CandidateDto>>();
            candidates.Should().NotBeNull();
            candidates?.Count.Should().BeGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task GetCandidateById_WhenCandidateExists_ShouldReturnOkAndCandidate()
        {
            // Arrange
            var email = $"get.{Guid.NewGuid()}@test.com";
            var createResponse = await _client.PostAsJsonAsync("/candidates", new CreateCandidateDto { Name = "GetTest", Surname = "User", Email = email, Birthdate = new DateTime(1993, 1, 1) });
            var createdCandidate = await createResponse.Content.ReadFromJsonAsync<CandidateDto>();
            var candidateId = createdCandidate!.IdCandidate;

            // Act
            var response = await _client.GetAsync($"/candidates/{candidateId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var fetchedCandidate = await response.Content.ReadFromJsonAsync<CandidateDto>();
            fetchedCandidate.Should().NotBeNull();
            fetchedCandidate?.IdCandidate.Should().Be(candidateId);
            fetchedCandidate?.Email.Should().Be(email.ToLowerInvariant());
        }

        [Fact]
        public async Task GetCandidateById_WhenCandidateDoesNotExist_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync("/candidates/999999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        [Fact]
        public async Task PutCandidate_WithValidDataAndExistingCandidate_ShouldReturnNoContent_AndCreateTimeline()
        {
            // Arrange
            var email = $"update.{Guid.NewGuid()}@example.com";
            var createResponse = await _client.PostAsJsonAsync("/candidates", new CreateCandidateDto { Name = "ToUpdate", Surname = "Initial", Email = email, Birthdate = new DateTime(1994, 1, 1) });
            var createdCandidate = await createResponse.Content.ReadFromJsonAsync<CandidateDto>();
            var candidateId = createdCandidate!.IdCandidate;

            var updateDto = new UpdateCandidateDto { Name = "UpdatedName", Surname = "UpdatedSurname", Email = email, Birthdate = new DateTime(1995, 1, 1) };

            // Act
            var response = await _client.PutAsJsonAsync($"/candidates/{candidateId}", updateDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var updatedDbCandidate = await context.Candidates.FindAsync(candidateId);
                updatedDbCandidate?.Name.Should().Be("UpdatedName");

                var timelineEntry = await context.Timelines
                    .FirstOrDefaultAsync(t => t.IdAggregateRoot == candidateId && t.IdTimelineType == TimelineTypes.Update);
                timelineEntry.Should().NotBeNull();
                timelineEntry?.NewData.Should().Contain("UpdatedName");
                timelineEntry?.OldData.Should().Contain("ToUpdate");
            }
        }


        [Fact]
        public async Task DeleteCandidate_WhenCandidateExists_ShouldReturnNoContent_AndCreateTimeline()
        {
            // Arrange
            var email = $"delete.{Guid.NewGuid()}@example.com";
            var createResponse = await _client.PostAsJsonAsync("/candidates", new CreateCandidateDto { Name = "ToDelete", Surname = "User", Email = email, Birthdate = new DateTime(1996, 1, 1) });
            var createdCandidate = await createResponse.Content.ReadFromJsonAsync<CandidateDto>();
            var candidateId = createdCandidate!.IdCandidate;

            // Act
            var response = await _client.DeleteAsync($"/candidates/{candidateId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var deletedDbCandidate = await context.Candidates.FindAsync(candidateId);
                deletedDbCandidate.Should().BeNull();

                var timelineEntry = await context.Timelines
                    .FirstOrDefaultAsync(t => t.IdAggregateRoot == candidateId && t.IdTimelineType == TimelineTypes.Delete);
                timelineEntry.Should().NotBeNull();
                timelineEntry?.OldData.Should().Contain("ToDelete");
            }
        }
    }
}
