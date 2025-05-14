using System;
using ApplicantTracking.Application.DTOs;
using MediatR;

namespace ApplicantTracking.Application.Features.Candidates.Commands.UpdateCandidate
{
    public class UpdateCandidateCommand : IRequest<CandidateDto?>
    {
        public int IdCandidate { get; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
        public string Email { get; set; }

        public UpdateCandidateCommand(int id, UpdateCandidateDto dto)
        {
            IdCandidate = id;
            Name = dto.Name;
            Surname = dto.Surname;
            Birthdate = dto.Birthdate;
            Email = dto.Email;
        }
    }
}
