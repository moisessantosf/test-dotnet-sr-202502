using System;

namespace ApplicantTracking.Application.DTOs
{
    public class CandidateDto
    {
        public int IdCandidate { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
