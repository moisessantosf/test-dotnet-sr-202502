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
    public class UpdateCandidateCommandHandler : IRequestHandler<UpdateCandidateCommand, bool>
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicantTrackingContext _context;

        public UpdateCandidateCommandHandler(
            ICandidateRepository candidateRepository,
            IUnitOfWork unitOfWork,
            ApplicantTrackingContext context)
        {
            _candidateRepository = candidateRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<bool> Handle(UpdateCandidateCommand request, CancellationToken cancellationToken)
        {
            var candidate = await _candidateRepository.GetByIdAsync(request.Id);
            if (candidate == null) return false;

            var oldData = JsonSerializer.Serialize(candidate);

            candidate.Name = request.Name;
            candidate.Email = request.Email;
            candidate.Phone = request.Phone;
            candidate.LinkedIn = request.LinkedIn;
            candidate.City = request.City;
            candidate.State = request.State;
            candidate.UpdatedAt = DateTime.UtcNow;

            _candidateRepository.Update(candidate);
            await _unitOfWork.SaveChangesAsync();

            var timeline = new Timeline
            {
                CandidateId = candidate.Id,
                TimelineType = TimelineTypes.Update,
                OldData = oldData,
                NewData = JsonSerializer.Serialize(candidate),
                CreatedAt = DateTime.UtcNow
            };

            await _context.Timelines.AddAsync(timeline);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}