using ApplicantTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantTracking.Infrastructure.Persistence.Configurations
{
    public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
    {
        public void Configure(EntityTypeBuilder<Candidate> builder)
        {
            builder.ToTable("candidates");

            builder.HasKey(c => c.IdCandidate);
            builder.Property(c => c.IdCandidate).ValueGeneratedOnAdd();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(80)
                .HasColumnType("varchar(80)");

            builder.Property(c => c.Surname)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnType("varchar(150)");

            builder.Property(c => c.Birthdate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(250)
                .HasColumnType("varchar(250)");
            builder.HasIndex(c => c.Email).IsUnique();

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(c => c.LastUpdatedAt)
                .HasColumnType("datetime")
                .IsRequired(false);
        }
    }
}
