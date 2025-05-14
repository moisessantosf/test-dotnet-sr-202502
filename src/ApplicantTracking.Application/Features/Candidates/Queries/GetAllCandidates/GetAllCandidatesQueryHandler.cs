using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Application.DTOs;
using ApplicantTracking.Domain.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace ApplicantTracking.Application.Features.Candidates.Queries.GetAllCandidates
{
    public class GetAllCandidatesQueryHandler : IRequestHandler<GetAllCandidatesQuery, IEnumerable<CandidateDto>>
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IMapper _mapper;

        public GetAllCandidatesQueryHandler(ICandidateRepository candidateRepository, IMapper mapper)
        {
            _candidateRepository = candidateRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CandidateDto>> Handle(GetAllCandidatesQuery request, CancellationToken cancellationToken)
        {
            var candidates = await _candidateRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CandidateDto>>(candidates);
        }
    }
}
