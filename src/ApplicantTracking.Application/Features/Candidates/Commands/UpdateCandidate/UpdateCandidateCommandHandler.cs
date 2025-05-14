using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Application.DTOs;
using ApplicantTracking.Application.Exceptions;
using ApplicantTracking.Domain.Events;
using ApplicantTracking.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace ApplicantTracking.Application.Features.Candidates.Commands.UpdateCandidate
{
    public class UpdateCandidateCommandHandler : IRequestHandler<UpdateCandidateCommand, CandidateDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public UpdateCandidateCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<CandidateDto?> Handle(UpdateCandidateCommand request, CancellationToken cancellationToken)
        {
            var candidate = await _unitOfWork.CandidateRepository.GetByIdAsync(request.IdCandidate);
            if (candidate == null)
            {
                throw new ApplicationValidationException($"Candidate with id '{request.IdCandidate}' not found.");
            }

            if (!candidate.Email.Equals(request.Email.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase))
            {
                var existingCandidateWithNewEmail = await _unitOfWork.CandidateRepository.GetByEmailAsync(request.Email.ToLowerInvariant());
                if (existingCandidateWithNewEmail != null && existingCandidateWithNewEmail.IdCandidate != candidate.IdCandidate)
                {
                    throw new ApplicationValidationException($"Another candidate with email '{request.Email}' already exists.");
                }
            }

            var oldCandidateSnapshot = candidate.Clone();

            candidate.UpdateDetails(request.Name, request.Surname, request.Birthdate, request.Email);

            await _unitOfWork.CommitAsync();

            var updatedCandidateSnapshot = candidate.Clone();
            await _mediator.Publish(new CandidateUpdatedEvent(updatedCandidateSnapshot, oldCandidateSnapshot), cancellationToken);

            return _mapper.Map<CandidateDto>(candidate);
        }
    }
}
