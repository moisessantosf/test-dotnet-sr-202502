using ApplicantTracking.Application.DTOs;
using MediatR;

namespace ApplicantTracking.Application.Features.Candidates.Queries.GetCandidateById
{
    public class GetCandidateByIdQuery : IRequest<CandidateDto?>
    {
        public int IdCandidate { get; }
        public GetCandidateByIdQuery(int idCandidate) => IdCandidate = idCandidate;
    }
}
