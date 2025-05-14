using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Application.DTOs;
using ApplicantTracking.Application.Exceptions;
using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Events;
using ApplicantTracking.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace ApplicantTracking.Application.Features.Candidates.Commands.CreateCandidate
{
    public class CreateCandidateCommandHandler : IRequestHandler<CreateCandidateCommand, CandidateDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public CreateCandidateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<CandidateDto> Handle(CreateCandidateCommand request, CancellationToken cancellationToken)
        {
            var existingCandidate = await _unitOfWork.CandidateRepository.GetByEmailAsync(request.Email.ToLowerInvariant());
            if (existingCandidate != null)
            {
                throw new ApplicationValidationException($"A candidate with email '{request.Email}' already exists.");
            }

            var candidate = new Candidate(request.Name, request.Surname, request.Birthdate, request.Email);

            await _unitOfWork.CandidateRepository.AddAsync(candidate);
            await _unitOfWork.CommitAsync();

            var candidateSnapshot = candidate.Clone();
            await _mediator.Publish(new CandidateCreatedEvent(candidateSnapshot), cancellationToken);

            return _mapper.Map<CandidateDto>(candidate);
        }
    }
}
