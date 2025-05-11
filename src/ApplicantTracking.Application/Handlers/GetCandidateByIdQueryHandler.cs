using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Interfaces;
using ApplicantTracking.Application.Queries;
using MediatR;

namespace ApplicantTracking.Application.Handlers
{
    public class GetCandidateByIdQueryHandler : IRequestHandler<GetCandidateByIdQuery, Candidate>
    {
        private readonly ICandidateRepository _candidateRepository;

        public GetCandidateByIdQueryHandler(ICandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }

        public async Task<Candidate> Handle(GetCandidateByIdQuery request, CancellationToken cancellationToken)
        {
            return await _candidateRepository.GetByIdAsync(request.Id);
        }
    }
}