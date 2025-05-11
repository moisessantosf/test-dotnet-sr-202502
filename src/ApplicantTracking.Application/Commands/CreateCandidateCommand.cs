using MediatR;
using System;

namespace ApplicantTracking.Application.Commands
{
    public class CreateCandidateCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string LinkedIn { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}