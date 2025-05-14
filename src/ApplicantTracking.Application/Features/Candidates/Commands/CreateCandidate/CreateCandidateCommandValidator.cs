using System;
using FluentValidation;

namespace ApplicantTracking.Application.Features.Candidates.Commands.CreateCandidate
{
    public class CreateCandidateCommandValidator : AbstractValidator<CreateCandidateCommand>
    {
        public CreateCandidateCommandValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(80).WithMessage("Name must not exceed 80 characters.");

            RuleFor(v => v.Surname)
                .NotEmpty().WithMessage("Surname is required.")
                .MaximumLength(150).WithMessage("Surname must not exceed 150 characters.");

            RuleFor(v => v.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.")
                .MaximumLength(250).WithMessage("Email must not exceed 250 characters.");

            RuleFor(v => v.Birthdate)
                .NotEmpty().WithMessage("Birthdate is required.")
                .LessThan(DateTime.UtcNow.AddYears(-16)).WithMessage("Candidate must be at least 16 years old.")
                .GreaterThan(DateTime.UtcNow.AddYears(-100)).WithMessage("Birthdate is not realistic.");
        }
    }
}
