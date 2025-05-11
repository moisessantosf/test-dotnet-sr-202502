using ApplicantTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApplicantTracking.Infrastructure.Data
{
    public class ApplicantTrackingContext : DbContext
    {
        public ApplicantTrackingContext(DbContextOptions<ApplicantTrackingContext> options)
            : base(options)
        {
        }

        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Timeline> Timelines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.LinkedIn).HasMaxLength(200);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.State).HasMaxLength(2);
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<Timeline>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TimelineType).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.HasOne(e => e.Candidate)
                    .WithMany()
                    .HasForeignKey(e => e.CandidateId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}