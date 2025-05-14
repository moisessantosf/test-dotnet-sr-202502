using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Domain.Events;
using ApplicantTracking.Domain.Interfaces;
using MediatR;

namespace ApplicantTracking.Application.Features.Candidates.Commands.DeleteCandidate
{
    public class DeleteCandidateCommandHandler : IRequestHandler<DeleteCandidateCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public DeleteCandidateCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<bool> Handle(DeleteCandidateCommand request, CancellationToken cancellationToken)
        {
            var candidate = await _unitOfWork.CandidateRepository.GetByIdAsync(request.IdCandidate);
            if (candidate == null)
            {
                return false;
            }

            var candidateSnapshot = candidate.Clone();

            _unitOfWork.CandidateRepository.Remove(candidate);
            await _unitOfWork.CommitAsync();

            await _mediator.Publish(new CandidateDeletedEvent(candidateSnapshot), cancellationToken);

            return true;
        }
    }
}
