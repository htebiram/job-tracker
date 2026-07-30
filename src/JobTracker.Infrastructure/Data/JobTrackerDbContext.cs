using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Data;

/// <summary>
/// Represents the application's database context.
/// </summary>
public sealed class JobTrackerDbContext : DbContext
{
    public JobTrackerDbContext(
        DbContextOptions<JobTrackerDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the DbSet of job applications.
    /// </summary>
    public DbSet<JobApplication> JobApplications =>
        Set<JobApplication>();

    /// <summary>
    /// Configures the model for the database context.
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(JobTrackerDbContext).Assembly);
    }
}
