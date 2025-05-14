using ApplicantTracking.Api.Middleware;
using ApplicantTracking.Application.Mappings;
using ApplicantTracking.Domain.Interfaces;
using ApplicantTracking.Domain.Interfaces.Repositories;
using ApplicantTracking.Infrastructure.Context;
using ApplicantTracking.Infrastructure.Persistence;
using ApplicantTracking.Infrastructure.Persistence.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Candidate Management API", Version = "v1" });
});

var connectionString = builder.Configuration.GetConnectionString("ApplicantTrackingDb");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'ApplicantTrackingDb' not found.");
}
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString,
        sqlOptions => sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(ApplicantTracking.Application.Features.Candidates.Commands.CreateCandidate.CreateCandidateCommand).Assembly);
});

builder.Services.AddValidatorsFromAssembly(typeof(ApplicantTracking.Application.Features.Candidates.Commands.CreateCandidate.CreateCandidateCommandValidator).Assembly);

builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<ITimelineRepository, TimelineRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());


var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Candidate Management API v1"));

    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}
else
{ 
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

namespace ApplicantTracking.Api
{
    public partial class Program { } // Adicione esta linha
}
