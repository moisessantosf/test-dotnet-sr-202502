using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Application.DTOs;
using ApplicantTracking.Application.Exceptions;
using ApplicantTracking.Application.Features.Candidates.Commands.UpdateCandidate;
using ApplicantTracking.Application.Mappings;
using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Events;
using ApplicantTracking.Domain.Interfaces;
using ApplicantTracking.Domain.Interfaces.Repositories;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace ApplicantTracking.Tests.Application.Features.Candidates.Commands
{
    public class UpdateCandidateCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICandidateRepository> _mockCandidateRepo;
        private readonly IMapper _mapper;
        private readonly Mock<IMediator> _mockMediator;
        private readonly UpdateCandidateCommandHandler _handler;

        public UpdateCandidateCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCandidateRepo = new Mock<ICandidateRepository>();
            _mockUnitOfWork.Setup(uow => uow.CandidateRepository).Returns(_mockCandidateRepo.Object);
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).ReturnsAsync(1);

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mapperConfig.CreateMapper();
            _mockMediator = new Mock<IMediator>();
            _handler = new UpdateCandidateCommandHandler(_mockUnitOfWork.Object, _mapper, _mockMediator.Object);
        }

        [Fact]
        public async Task Handle_WhenCandidateExistsAndDataIsValid_ShouldUpdateAndPublishEvent_AndReturnDto()
        {
            // Arrange
            var candidateId = 1;
            var originalEmail = "original@example.com";
            var originalName = "OldName";
            var originalSurname = "OldSurname";
            var originalBirthdate = new DateTime(1990, 1, 1);

            var existingCandidate = new Candidate(originalName, originalSurname, originalBirthdate, originalEmail);

            PropertyInfo idProperty = typeof(Candidate).GetProperty("IdCandidate");
            if (idProperty == null)
            {
                throw new InvalidOperationException("Property 'IdCandidate' not found on Candidate entity. Check property name and accessors.");
            }
            idProperty.SetValue(existingCandidate, candidateId);

            _mockCandidateRepo.Setup(repo => repo.GetByIdAsync(candidateId)).ReturnsAsync(existingCandidate);

            var updateDto = new UpdateCandidateDto { Name = "NewName", Surname = "NewSurname", Email = "newemail@example.com", Birthdate = new DateTime(1991, 2, 2) };
            var command = new UpdateCandidateCommand(candidateId, updateDto);

            if (updateDto.Email.ToLowerInvariant() != originalEmail.ToLowerInvariant())
            {
                _mockCandidateRepo.Setup(repo => repo.GetByEmailAsync(updateDto.Email.ToLowerInvariant())).ReturnsAsync((Candidate)null);
            }

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result?.Name.Should().Be(updateDto.Name);
            result?.Email.Should().Be(updateDto.Email.ToLowerInvariant());

            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
            _mockMediator.Verify(m => m.Publish(
                It.Is<CandidateUpdatedEvent>(e =>
                    e.CandidateSnapshot.IdCandidate == candidateId &&
                    e.CandidateSnapshot.Name == updateDto.Name &&
                    e.OldCandidateSnapshot.Name == originalName),
                CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenCandidateDoesNotExist_ShouldThrowApplicationException_AndNotUpdateOrPublish()
        {
            // Arrange
            var candidateId = 99;
            _mockCandidateRepo.Setup(repo => repo.GetByIdAsync(candidateId)).ReturnsAsync((Candidate)null);
            var command = new UpdateCandidateCommand(candidateId, new UpdateCandidateDto());

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApplicationValidationException>()
                .WithMessage($"Candidate with id '{candidateId}' not found.");

            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never);
            _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUpdatingToEmailThatExistsForAnotherCandidate_ShouldThrowApplicationException()
        {
            // Arrange
            var candidateIdBeingUpdated = 1;
            var originalEmail = "original@example.com";

            var candidateBeingUpdated = new Candidate("Original Name", "Surname", new DateTime(1990, 1, 1), originalEmail);
            typeof(Candidate).GetProperty("IdCandidate")!.SetValue(candidateBeingUpdated, candidateIdBeingUpdated);
            _mockCandidateRepo.Setup(repo => repo.GetByIdAsync(candidateIdBeingUpdated)).ReturnsAsync(candidateBeingUpdated);

            var conflictingEmail = "conflicting@example.com";

            var anotherCandidateWithEmail = new Candidate("Another", "User", new DateTime(1985, 1, 1), conflictingEmail);
            var anotherCandidateId = 2;

            typeof(Candidate).GetProperty("IdCandidate")!.SetValue(anotherCandidateWithEmail, anotherCandidateId);

            _mockCandidateRepo.Setup(repo => repo.GetByEmailAsync(conflictingEmail.ToLowerInvariant())).ReturnsAsync(anotherCandidateWithEmail);

            var updateDto = new UpdateCandidateDto { Name = "NewName", Surname = "NewSurname", Email = conflictingEmail, Birthdate = new DateTime(1991, 2, 2) };
            var command = new UpdateCandidateCommand(candidateIdBeingUpdated, updateDto);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApplicationValidationException>()
                .WithMessage($"Another candidate with email '{conflictingEmail}' already exists.");

            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never);
            _mockMediator.Verify(m => m.Publish(It.IsAny<CandidateUpdatedEvent>(), CancellationToken.None), Times.Never);
        }
    }
}
