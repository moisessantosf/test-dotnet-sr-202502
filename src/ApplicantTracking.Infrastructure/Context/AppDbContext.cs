using System.Reflection;
using ApplicantTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicantTracking.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Candidate> Candidates { get; set; } = null!;
        public DbSet<Timeline> Timelines { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
