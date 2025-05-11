using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Application.Commands;
using ApplicantTracking.Application.Handlers;
using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Interfaces;
using ApplicantTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ApplicantTracking.Tests.Unit.Handlers
{
    public class CreateCandidateCommandHandlerTests
    {
        private readonly Mock<ICandidateRepository> _mockCandidateRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly ApplicantTrackingContext _context;
        private readonly CreateCandidateCommandHandler _handler;

        public CreateCandidateCommandHandlerTests()
        {
            _mockCandidateRepository = new Mock<ICandidateRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            var options = new DbContextOptionsBuilder<ApplicantTrackingContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicantTrackingContext(options);

            _handler = new CreateCandidateCommandHandler(
                _mockCandidateRepository.Object,
                _mockUnitOfWork.Object,
                _context);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsNewCandidateId()
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

            _mockCandidateRepository
                .Setup(x => x.AddAsync(It.IsAny<Candidate>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result > 0);
            _mockCandidateRepository.Verify(x => x.AddAsync(It.IsAny<Candidate>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Exactly(2));
        }
    }
}