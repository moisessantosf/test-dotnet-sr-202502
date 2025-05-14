using System;
using ApplicantTracking.Domain.Exceptions;

namespace ApplicantTracking.Domain.Entities
{
    public class Candidate
    {
        public int IdCandidate { get; set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public DateTime Birthdate { get; private set; }
        public string Email { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }

        private Candidate() { }

        public Candidate(string name, string surname, DateTime birthdate, string email)
        {
            ValidateDomain(name, surname, email, birthdate);

            Name = name;
            Surname = surname;
            Birthdate = birthdate;
            Email = email.ToLowerInvariant();
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(string name, string surname, DateTime birthdate, string email)
        {
            ValidateDomain(name, surname, email, birthdate);

            Name = name;
            Surname = surname;
            Birthdate = birthdate;
            Email = email.ToLowerInvariant();
            LastUpdatedAt = DateTime.UtcNow;
        }

        private void ValidateDomain(string name, string surname, string email, DateTime birthdate)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 80)
                throw new DomainValidationException("Name is required and cannot exceed 80 characters.");
            if (string.IsNullOrWhiteSpace(surname) || surname.Length > 150)
                throw new DomainValidationException("Surname is required and cannot exceed 150 characters.");
            if (string.IsNullOrWhiteSpace(email) || email.Length > 250 || !IsValidEmail(email)) 
                throw new DomainValidationException("A valid email is required and cannot exceed 250 characters.");
            if (birthdate > DateTime.UtcNow.AddYears(-16))
                throw new DomainValidationException("Candidate must be at least 16 years old.");
            if (birthdate < DateTime.UtcNow.AddYears(-100))
                throw new DomainValidationException("Birthdate is not realistic.");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public Candidate Clone()
        {
            return (Candidate)this.MemberwiseClone();
        }
    }
}
