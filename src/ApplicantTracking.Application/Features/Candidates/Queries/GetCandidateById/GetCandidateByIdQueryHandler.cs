using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Application.DTOs;
using ApplicantTracking.Domain.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace ApplicantTracking.Application.Features.Candidates.Queries.GetCandidateById
{
    public class GetCandidateByIdQueryHandler : IRequestHandler<GetCandidateByIdQuery, CandidateDto?>
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IMapper _mapper;

        public GetCandidateByIdQueryHandler(ICandidateRepository candidateRepository, IMapper mapper)
        {
            _candidateRepository = candidateRepository;
            _mapper = mapper;
        }

        public async Task<CandidateDto?> Handle(GetCandidateByIdQuery request, CancellationToken cancellationToken)
        {
            var candidate = await _candidateRepository.GetByIdAsync(request.IdCandidate);
            if (candidate == null) return null;
            return _mapper.Map<CandidateDto>(candidate);
        }
    }
}
