using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Application.Features.Candidates.Commands.DeleteCandidate;
using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Events;
using ApplicantTracking.Domain.Interfaces;
using ApplicantTracking.Domain.Interfaces.Repositories;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace ApplicantTracking.Tests.Application.Features.Candidates.Commands
{
    public class DeleteCandidateCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICandidateRepository> _mockCandidateRepo;
        private readonly Mock<IMediator> _mockMediator;
        private readonly DeleteCandidateCommandHandler _handler;

        public DeleteCandidateCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCandidateRepo = new Mock<ICandidateRepository>();
            _mockUnitOfWork.Setup(uow => uow.CandidateRepository).Returns(_mockCandidateRepo.Object);
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).ReturnsAsync(1);
            _mockMediator = new Mock<IMediator>();
            _handler = new DeleteCandidateCommandHandler(_mockUnitOfWork.Object, _mockMediator.Object);
        }

        [Fact]
        public async Task Handle_WhenCandidateExists_ShouldDeleteAndPublishEvent_AndReturnTrue()
        {
            // Arrange
            var candidateId = 1;
            var candidateToDelete = new Candidate("Test", "User", new DateTime(1990, 1, 1), "test@test.com");
            _mockCandidateRepo.Setup(repo => repo.GetByIdAsync(candidateId)).ReturnsAsync(candidateToDelete);

            var command = new DeleteCandidateCommand(candidateId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _mockCandidateRepo.Verify(repo => repo.Remove(candidateToDelete), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
            _mockMediator.Verify(m => m.Publish(It.Is<CandidateDeletedEvent>(e => e.CandidateSnapshot.Email == candidateToDelete.Email), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCandidateDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var candidateId = 99;
            _mockCandidateRepo.Setup(repo => repo.GetByIdAsync(candidateId)).ReturnsAsync((Candidate)null);
            var command = new DeleteCandidateCommand(candidateId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
            _mockCandidateRepo.Verify(repo => repo.Remove(It.IsAny<Candidate>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never);
            _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Never);
        }
    }
}
