using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Application.Features.Candidates.Queries.GetAllCandidates;
using ApplicantTracking.Application.Mappings;
using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Interfaces.Repositories;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

namespace ApplicantTracking.Tests.Application.Features.Candidates.Queries
{
    public class GetAllCandidatesQueryHandlerTests
    {
        private readonly Mock<ICandidateRepository> _mockCandidateRepo;
        private readonly IMapper _mapper;
        private readonly GetAllCandidatesQueryHandler _handler;

        public GetAllCandidatesQueryHandlerTests()
        {
            _mockCandidateRepo = new Mock<ICandidateRepository>();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mapperConfig.CreateMapper();
            _handler = new GetAllCandidatesQueryHandler(_mockCandidateRepo.Object, _mapper);
        }

        [Fact]
        public async Task Handle_WhenCandidatesExist_ShouldReturnMappedDtos()
        {
            // Arrange
            var candidates = new List<Candidate>
            {
                new Candidate("John", "Doe", new DateTime(1990,1,1), "john@example.com"),
                new Candidate("Jane", "Doe", new DateTime(1992,2,2), "jane@example.com")
            };
            _mockCandidateRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(candidates);
            var query = new GetAllCandidatesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
            result.First().Email.Should().Be("john@example.com");
        }

        [Fact]
        public async Task Handle_WhenNoCandidatesExist_ShouldReturnEmptyList()
        {
            // Arrange
            _mockCandidateRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Candidate>());
            var query = new GetAllCandidatesQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
