using System;
using ApplicantTracking.Domain.Entities;
using MediatR;

namespace ApplicantTracking.Domain.Events
{
    public abstract class CandidateBaseEvent : INotification
    {
        public Candidate CandidateSnapshot { get; }
        public DateTime Timestamp { get; }

        protected CandidateBaseEvent(Candidate candidateSnapshot)
        {
            CandidateSnapshot = candidateSnapshot;
            Timestamp = DateTime.UtcNow;
        }
    }
}
