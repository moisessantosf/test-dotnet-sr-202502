using System;
using ApplicantTracking.Application.DTOs;
using MediatR;

namespace ApplicantTracking.Application.Features.Candidates.Commands.CreateCandidate
{
    public class CreateCandidateCommand : IRequest<CandidateDto>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
        public string Email { get; set; }

        public CreateCandidateCommand(CreateCandidateDto dto)
        {
            Name = dto.Name;
            Surname = dto.Surname;
            Birthdate = dto.Birthdate;
            Email = dto.Email;
        }
    }
}
