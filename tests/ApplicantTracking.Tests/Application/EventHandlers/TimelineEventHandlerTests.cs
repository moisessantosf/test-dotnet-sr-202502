using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Application.EventHandlers;
using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Enumerators;
using ApplicantTracking.Domain.Events;
using ApplicantTracking.Domain.Interfaces;
using ApplicantTracking.Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace ApplicantTracking.Tests.Application.EventHandlers
{
    public class TimelineEventHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ITimelineRepository> _mockTimelineRepo;
        private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IServiceScope> _mockScope;
        private readonly TimelineEventHandler _handler;

        public TimelineEventHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTimelineRepo = new Mock<ITimelineRepository>();
            _mockUnitOfWork.Setup(uow => uow.TimelineRepository).Returns(_mockTimelineRepo.Object);
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).ReturnsAsync(1);

            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockServiceProvider.Setup(sp => sp.GetService(typeof(IUnitOfWork))).Returns(_mockUnitOfWork.Object);

            _mockScope = new Mock<IServiceScope>();
            _mockScope.Setup(s => s.ServiceProvider).Returns(_mockServiceProvider.Object);

            _mockScopeFactory = new Mock<IServiceScopeFactory>();
            _mockScopeFactory.Setup(sf => sf.CreateScope()).Returns(_mockScope.Object);

            _handler = new TimelineEventHandler(_mockScopeFactory.Object);
        }

        private Candidate CreateTestCandidateInstance(int id, string name, string surname, DateTime birthdate, string email)
        {
            var candidate = new Candidate(name, surname, birthdate, email);
            PropertyInfo idProperty = typeof(Candidate).GetProperty("IdCandidate");
            if (idProperty == null)
            {
                throw new InvalidOperationException("Property 'IdCandidate' not found on Candidate entity. Check property name and accessors.");
            }
            idProperty.SetValue(candidate, id);
            return candidate;
        }

        [Fact]
        public async Task Handle_CandidateCreatedEvent_ShouldAddCreatedTimelineEntry()
        {
            // Arrange
            var candidateId = 123;
            var candidateEmail = "created@example.com";
            var candidateSnapshot = CreateTestCandidateInstance(candidateId, "Test", "User", new DateTime(1990, 1, 1), candidateEmail);

            var createdEvent = new CandidateCreatedEvent(candidateSnapshot);

            // Act
            await _handler.Handle(createdEvent, CancellationToken.None);

            // Assert
            _mockTimelineRepo.Verify(repo => repo.AddAsync(It.Is<Timeline>(
                t => t.IdTimelineType == TimelineTypes.Create &&
                     t.IdAggregateRoot == candidateId &&
                     t.OldData == null &&
                     t.NewData != null && JsonConvert.DeserializeObject<Candidate>(t.NewData).Email == candidateEmail
            )), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_CandidateUpdatedEvent_ShouldAddUpdatedTimelineEntry()
        {
            // Arrange
            var candidateId = 456;
            var oldEmail = "old@example.com";
            var newEmail = "new@example.com";

            var oldCandidateSnapshot = CreateTestCandidateInstance(candidateId, "OldName", "OldSurname", new DateTime(1980, 1, 1), oldEmail);
            var newCandidateSnapshot = CreateTestCandidateInstance(candidateId, "NewName", "NewSurname", new DateTime(1990, 1, 1), newEmail);

            var updatedEvent = new CandidateUpdatedEvent(newCandidateSnapshot, oldCandidateSnapshot);

            // Act
            await _handler.Handle(updatedEvent, CancellationToken.None);

            // Assert
            _mockTimelineRepo.Verify(repo => repo.AddAsync(It.Is<Timeline>(
                t => t.IdTimelineType == TimelineTypes.Update &&
                     t.IdAggregateRoot == candidateId &&
                     t.OldData != null && JsonConvert.DeserializeObject<Candidate>(t.OldData).Email == oldEmail &&
                     t.NewData != null && JsonConvert.DeserializeObject<Candidate>(t.NewData).Email == newEmail
            )), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_CandidateDeletedEvent_ShouldAddDeletedTimelineEntry()
        {
            // Arrange
            var candidateId = 789;
            var deletedEmail = "deleted@example.com";

            var deletedCandidateSnapshot = CreateTestCandidateInstance(candidateId, "Deleted", "User", new DateTime(1970, 1, 1), deletedEmail);
            var deletedEvent = new CandidateDeletedEvent(deletedCandidateSnapshot);

            // Act
            await _handler.Handle(deletedEvent, CancellationToken.None);

            // Assert
            _mockTimelineRepo.Verify(repo => repo.AddAsync(It.Is<Timeline>(
               t => t.IdTimelineType == TimelineTypes.Delete &&
                    t.IdAggregateRoot == candidateId &&
                    t.OldData != null && JsonConvert.DeserializeObject<Candidate>(t.OldData).Email == deletedEmail &&
                    t.NewData == null
           )), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
        }
    }
}
