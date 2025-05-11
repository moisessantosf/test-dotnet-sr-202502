using MediatR;

namespace ApplicantTracking.Application.Commands
{
    public class DeleteCandidateCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}