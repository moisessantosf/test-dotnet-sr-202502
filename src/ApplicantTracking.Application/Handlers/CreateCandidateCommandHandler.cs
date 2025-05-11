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
    public class CreateCandidateCommandHandler : IRequestHandler<CreateCandidateCommand, int>
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicantTrackingContext _context;

        public CreateCandidateCommandHandler(
            ICandidateRepository candidateRepository,
            IUnitOfWork unitOfWork,
            ApplicantTrackingContext context)
        {
            _candidateRepository = candidateRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<int> Handle(CreateCandidateCommand request, CancellationToken cancellationToken)
        {
            var candidate = new Candidate
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                LinkedIn = request.LinkedIn,
                City = request.City,
                State = request.State,
                CreatedAt = DateTime.UtcNow
            };

            await _candidateRepository.AddAsync(candidate);
            await _unitOfWork.SaveChangesAsync();

            var timeline = new Timeline
            {
                CandidateId = candidate.Id,
                TimelineType = TimelineTypes.Create,
                NewData = JsonSerializer.Serialize(candidate),
                CreatedAt = DateTime.UtcNow
            };

            await _context.Timelines.AddAsync(timeline);
            await _unitOfWork.SaveChangesAsync();

            return candidate.Id;
        }
    }
}