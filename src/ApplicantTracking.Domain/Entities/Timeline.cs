using System;
using ApplicantTracking.Domain.Enumerators;

namespace ApplicantTracking.Domain.Entities
{
    public class Timeline
    {
        public int IdTimeline { get; set; }
        public TimelineTypes IdTimelineType { get; private set; }
        public int IdAggregateRoot { get; private set; }
        public string? OldData { get; private set; }
        public string? NewData { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Timeline() { }

        public Timeline(TimelineTypes idTimelineType, int idAggregateRoot, string? oldData, string? newData)
        {
            IdTimelineType = idTimelineType;
            IdAggregateRoot = idAggregateRoot;
            OldData = oldData;
            NewData = newData;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
