using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Application.DTOs;
using ApplicantTracking.Application.Features.Candidates.Commands.CreateCandidate;
using ApplicantTracking.Application.Mappings;
using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Events;
using ApplicantTracking.Domain.Exceptions;
using ApplicantTracking.Domain.Interfaces;
using ApplicantTracking.Domain.Interfaces.Repositories;
using AutoMapper;
using MediatR;
using Moq;
using Xunit;
using FluentAssertions;
using ApplicantTracking.Application.Exceptions;

namespace ApplicantTracking.Tests.Application.Features.Candidates.Commands
{
    public class CreateCandidateCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICandidateRepository> _mockCandidateRepo;
        private readonly IMapper _mapper;
        private readonly Mock<IMediator> _mockMediator;
        private readonly CreateCandidateCommandHandler _handler;

        public CreateCandidateCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCandidateRepo = new Mock<ICandidateRepository>();
            _mockUnitOfWork.Setup(uow => uow.CandidateRepository).Returns(_mockCandidateRepo.Object);
            _mockUnitOfWork.Setup(uow => uow.CommitAsync()).ReturnsAsync(1);

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mapperConfig.CreateMapper();
            _mockMediator = new Mock<IMediator>();

            _handler = new CreateCandidateCommandHandler(_mockUnitOfWork.Object, _mapper, _mockMediator.Object);
        }

        [Fact]
        public async Task Handle_WhenEmailIsUnique_ShouldCreateCandidateAndPublishEvent_AndReturnCandidateDto()
        {
            // Arrange
            var command = new CreateCandidateCommand(new CreateCandidateDto { Name = "Moisés", Surname = "Santos", Email = "moises.santos@test.com", Birthdate = new DateTime(1990, 1, 1) });
            _mockCandidateRepo.Setup(repo => repo.GetByEmailAsync(command.Email.ToLowerInvariant())).ReturnsAsync((Candidate)null);
            _mockCandidateRepo.Setup(repo => repo.AddAsync(It.IsAny<Candidate>())).Returns(Task.CompletedTask);

            // Act
            var resultDto = await _handler.Handle(command, CancellationToken.None);

            // Assert
            resultDto.Should().NotBeNull();
            resultDto.Name.Should().Be(command.Name);
            resultDto.Email.Should().Be(command.Email.ToLowerInvariant());

            _mockCandidateRepo.Verify(repo => repo.AddAsync(It.Is<Candidate>(c => c.Email == command.Email.ToLowerInvariant())), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Once);
            _mockMediator.Verify(m => m.Publish(It.Is<CandidateCreatedEvent>(e => e.CandidateSnapshot.Email == command.Email.ToLowerInvariant()), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEmailAlreadyExists_ShouldThrowApplicationException_AndNotCreateOrPublish()
        {
            // Arrange
            var command = new CreateCandidateCommand(new CreateCandidateDto { Name = "Moisés", Surname = "Santos", Email = "moises.santos@test.com", Birthdate = new DateTime(1991, 2, 2) });
            var existingCandidate = new Candidate("Existing", "User", new System.DateTime(1980, 1, 1), command.Email.ToLowerInvariant());
            _mockCandidateRepo.Setup(repo => repo.GetByEmailAsync(command.Email.ToLowerInvariant())).ReturnsAsync(existingCandidate);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApplicationValidationException>()
                .WithMessage($"A candidate with email '{command.Email}' already exists.");

            _mockCandidateRepo.Verify(repo => repo.AddAsync(It.IsAny<Candidate>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never);
            _mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenCandidateConstructorThrowsDomainValidationException_ShouldPropagateException()
        {
            // Arrange
            var command = new CreateCandidateCommand(new CreateCandidateDto { Name = new string('A', 81), Surname = "Doe", Email = "moises@test.com", Birthdate = new DateTime(1990, 1, 1) });
            _mockCandidateRepo.Setup(repo => repo.GetByEmailAsync(command.Email.ToLowerInvariant())).ReturnsAsync((Candidate)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainValidationException>();
            _mockUnitOfWork.Verify(uow => uow.CommitAsync(), Times.Never);
        }
    }
}
