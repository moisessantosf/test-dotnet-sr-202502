using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Enumerators;
using ApplicantTracking.Domain.Interfaces;
using ApplicantTracking.Application.Commands;
using MediatR;

namespace ApplicantTracking.Application.Handlers
{
    public class DeleteCandidateCommandHandler : IRequestHandler<DeleteCandidateCommand, bool>
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicantTrackingContext _context;

        public DeleteCandidateCommandHandler(
            ICandidateRepository candidateRepository,
            IUnitOfWork unitOfWork,
            ApplicantTrackingContext context)
        {
            _candidateRepository = candidateRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<bool> Handle(DeleteCandidateCommand request, CancellationToken cancellationToken)
        {
            var candidate = await _candidateRepository.GetByIdAsync(request.Id);
            if (candidate == null) return false;

            var oldData = JsonSerializer.Serialize(candidate);

            _candidateRepository.Delete(candidate);
            await _unitOfWork.SaveChangesAsync();

            var timeline = new Timeline
            {
                CandidateId = candidate.Id,
                TimelineType = TimelineTypes.Delete,
                OldData = oldData,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Timelines.AddAsync(timeline);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}