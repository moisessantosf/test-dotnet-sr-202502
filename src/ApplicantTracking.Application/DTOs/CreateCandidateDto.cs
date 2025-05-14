using System;

namespace ApplicantTracking.Application.DTOs
{
    public class CreateCandidateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
