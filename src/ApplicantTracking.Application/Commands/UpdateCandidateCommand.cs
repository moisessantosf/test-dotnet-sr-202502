using MediatR;

namespace ApplicantTracking.Application.Commands
{
    public class UpdateCandidateCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string LinkedIn { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}