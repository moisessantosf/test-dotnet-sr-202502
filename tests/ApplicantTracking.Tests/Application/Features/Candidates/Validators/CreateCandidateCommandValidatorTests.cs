using System;
using ApplicantTracking.Application.DTOs;
using ApplicantTracking.Application.Features.Candidates.Commands.CreateCandidate;
using FluentValidation.TestHelper;
using Xunit;

namespace ApplicantTracking.Tests.Application.Features.Candidates.Validators
{
    public class CreateCandidateCommandValidatorTests
    {
        private readonly CreateCandidateCommandValidator _validator;

        public CreateCandidateCommandValidatorTests()
        {
            _validator = new CreateCandidateCommandValidator();
        }

        [Fact]
        public void ShouldHaveError_WhenNameIsEmpty()
        {
            var command = new CreateCandidateCommand(new CreateCandidateDto { Name = "" });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Name)
                .WithErrorMessage("Name is required.");
        }

        [Fact]
        public void ShouldHaveError_WhenNameExceedsMaxLength()
        {
            var command = new CreateCandidateCommand(new CreateCandidateDto { Name = new string('A', 81) });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Name)
                .WithErrorMessage("Name must not exceed 80 characters.");
        }

        [Fact]
        public void ShouldNotHaveError_WhenNameIsValid()
        {
            var command = new CreateCandidateCommand(new CreateCandidateDto { Name = "Valid Name" });
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(c => c.Name);
        }

        [Fact]
        public void ShouldHaveError_WhenEmailIsInvalidFormat()
        {
            var command = new CreateCandidateCommand(new CreateCandidateDto { Email = "invalidemail" });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Email)
                .WithErrorMessage("A valid email is required.");
        }

        [Fact]
        public void ShouldHaveError_WhenBirthdateIsLessThan16YearsAgo()
        {
            var command = new CreateCandidateCommand(new CreateCandidateDto { Birthdate = DateTime.UtcNow.AddYears(-15) });
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(c => c.Birthdate)
                .WithErrorMessage("Candidate must be at least 16 years old.");
        }
    }
}
