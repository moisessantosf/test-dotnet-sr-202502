using ApplicantTracking.Domain.Entities;

namespace ApplicantTracking.Domain.Events
{
    public class CandidateUpdatedEvent : CandidateBaseEvent
    {
        public Candidate OldCandidateSnapshot { get; }

        public CandidateUpdatedEvent(Candidate newCandidateSnapshot, Candidate oldCandidateSnapshot) : base(newCandidateSnapshot)
        {
            OldCandidateSnapshot = oldCandidateSnapshot;
        }
    }
}
