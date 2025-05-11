using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Interfaces;
using ApplicantTracking.Application.Queries;
using MediatR;

namespace ApplicantTracking.Application.Handlers
{
    public class GetCandidatesQueryHandler : IRequestHandler<GetCandidatesQuery, IEnumerable<Candidate>>
    {
        private readonly ICandidateRepository _candidateRepository;

        public GetCandidatesQueryHandler(ICandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }

        public async Task<IEnumerable<Candidate>> Handle(GetCandidatesQuery request, CancellationToken cancellationToken)
        {
            return await _candidateRepository.GetAllAsync();
        }
    }
}