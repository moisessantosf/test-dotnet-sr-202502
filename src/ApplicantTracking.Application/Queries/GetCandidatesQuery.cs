using ApplicantTracking.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace ApplicantTracking.Application.Queries
{
    public class GetCandidatesQuery : IRequest<IEnumerable<Candidate>>
    {
    }
}