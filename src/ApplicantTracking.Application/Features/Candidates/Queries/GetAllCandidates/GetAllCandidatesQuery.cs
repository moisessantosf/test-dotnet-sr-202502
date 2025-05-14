using System.Collections.Generic;
using ApplicantTracking.Application.DTOs;
using MediatR;

namespace ApplicantTracking.Application.Features.Candidates.Queries.GetAllCandidates
{
    public class GetAllCandidatesQuery : IRequest<IEnumerable<CandidateDto>> { }
}
