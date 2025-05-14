using ApplicantTracking.Domain.Entities;

namespace ApplicantTracking.Domain.Events
{
    public class CandidateCreatedEvent : CandidateBaseEvent
    {
        public CandidateCreatedEvent(Candidate createdCandidateSnapshot) : base(createdCandidateSnapshot) { }
    }
}
