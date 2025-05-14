using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ApplicantTracking.Application.Exceptions;
using ApplicantTracking.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApplicantTracking.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception has occurred.");

            var statusCode = HttpStatusCode.InternalServerError;
            object? responseContent = null;

            switch (exception)
            {
                case FluentValidation.ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    responseContent = new { title = "Validation Error", errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) };
                    break;
                case DomainValidationException domainValidationEx:
                    statusCode = HttpStatusCode.BadRequest;
                    responseContent = new { title = "Domain Rule Violation", message = domainValidationEx.Message };
                    break;
                case ApplicationValidationException appValidationEx when appValidationEx.Message.Contains("already exists"):
                    statusCode = HttpStatusCode.Conflict;
                    responseContent = new { title = "Conflict", errors = appValidationEx.Message };
                    break;
                case ApplicationValidationException appValidationEx when appValidationEx.Message.Contains("not found"):
                    statusCode = HttpStatusCode.NotFound;
                    responseContent = new { title = "Not Found", errors = appValidationEx.Message };
                    break;
                case ApplicationException appEx:
                    statusCode = HttpStatusCode.BadRequest;
                    responseContent = new { title = "Application Error", message = appEx.Message };
                    break;
                default:
                    responseContent = new { title = "Internal Server Error", message = "An unexpected error occurred. Please try again later." };
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            if (responseContent != null)
            {
                var jsonResponse = JsonSerializer.Serialize(responseContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
