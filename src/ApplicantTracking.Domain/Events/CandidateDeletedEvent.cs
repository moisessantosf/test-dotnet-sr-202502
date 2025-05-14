using ApplicantTracking.Domain.Entities;

namespace ApplicantTracking.Domain.Events
{
    public class CandidateDeletedEvent : CandidateBaseEvent
    {
        public CandidateDeletedEvent(Candidate deletedCandidateSnapshot) : base(deletedCandidateSnapshot) { }
    }
}
