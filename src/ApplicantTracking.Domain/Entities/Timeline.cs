using ApplicantTracking.Domain.Enumerators;
using System;

namespace ApplicantTracking.Domain.Entities
{
    public class Timeline
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public TimelineTypes TimelineType { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public virtual Candidate Candidate { get; set; }
    }
}