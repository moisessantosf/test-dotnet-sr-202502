using ApplicantTracking.Domain.Entities;
using MediatR;

namespace ApplicantTracking.Application.Queries
{
    public class GetCandidateByIdQuery : IRequest<Candidate>
    {
        public int Id { get; set; }
    }
}