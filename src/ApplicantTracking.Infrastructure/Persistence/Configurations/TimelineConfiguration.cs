using ApplicantTracking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicantTracking.Infrastructure.Persistence.Configurations
{
    public class TimelineConfiguration : IEntityTypeConfiguration<Timeline>
    {
        public void Configure(EntityTypeBuilder<Timeline> builder)
        {
            builder.ToTable("timelines");

            builder.HasKey(t => t.IdTimeline);
            builder.Property(t => t.IdTimeline).ValueGeneratedOnAdd();

            builder.Property(t => t.IdTimelineType)
                .IsRequired()
                .HasColumnType("tinyint")
                .HasConversion<byte>();

            builder.Property(t => t.IdAggregateRoot)
                .IsRequired();

            builder.Property(t => t.OldData)
                .HasColumnType("varchar(max)")
                .IsRequired(false);

            builder.Property(t => t.NewData)
                .HasColumnType("varchar(max)")
                .IsRequired(false);

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime");
        }
    }
}
