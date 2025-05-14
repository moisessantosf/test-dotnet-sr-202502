using MediatR;

namespace ApplicantTracking.Application.Features.Candidates.Commands.DeleteCandidate
{
    public class DeleteCandidateCommand : IRequest<bool>
    {
        public int IdCandidate { get; }
        public DeleteCandidateCommand(int idCandidate) => IdCandidate = idCandidate;
    }
}
