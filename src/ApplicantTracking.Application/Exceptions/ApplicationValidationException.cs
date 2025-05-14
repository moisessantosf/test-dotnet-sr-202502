using System;

namespace ApplicantTracking.Application.Exceptions
{
    public class ApplicationValidationException : Exception
    {
        public ApplicationValidationException(string message) : base(message) { }
        public ApplicationValidationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
