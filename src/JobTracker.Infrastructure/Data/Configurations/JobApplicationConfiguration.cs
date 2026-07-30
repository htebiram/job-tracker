using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the JobApplication entity.
/// </summary>
public sealed class JobApplicationConfiguration
    : IEntityTypeConfiguration<JobApplication>
{
    /// <inheritdoc />
    public void Configure(
        EntityTypeBuilder<JobApplication> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("JobApplications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CompanyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.Source)
            .HasMaxLength(100);

        builder.Property(x => x.Location)
            .HasMaxLength(200);

        builder.Property(x => x.Link)
            .HasMaxLength(500);

        builder.Property(x => x.ExpectedSalary)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Notes)
            .HasMaxLength(4000);

        builder.Property(x => x.DateApplied)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.ModifiedAt)
            .IsRequired();
    }
}
