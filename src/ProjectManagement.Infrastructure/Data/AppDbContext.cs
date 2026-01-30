using Microsoft.EntityFrameworkCore;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<ResourceSkill> ResourceSkills => Set<ResourceSkill>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectStatus> ProjectStatuses => Set<ProjectStatus>();
    public DbSet<Period> Periods => Set<Period>();
    public DbSet<Allocation> Allocations => Set<Allocation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ResourceSkill - composite key
        modelBuilder.Entity<ResourceSkill>()
            .HasKey(rs => new { rs.ResourceId, rs.SkillId });

        modelBuilder.Entity<ResourceSkill>()
            .HasOne(rs => rs.Resource)
            .WithMany(r => r.ResourceSkills)
            .HasForeignKey(rs => rs.ResourceId);

        modelBuilder.Entity<ResourceSkill>()
            .HasOne(rs => rs.Skill)
            .WithMany(s => s.ResourceSkills)
            .HasForeignKey(rs => rs.SkillId);

        // Resource relationships
        modelBuilder.Entity<Resource>()
            .HasOne(r => r.Role)
            .WithMany(ro => ro.Resources)
            .HasForeignKey(r => r.RoleId);

        modelBuilder.Entity<Resource>()
            .HasOne(r => r.Vendor)
            .WithMany(v => v.Resources)
            .HasForeignKey(r => r.VendorId)
            .IsRequired(false);

        // Allocation relationships
        modelBuilder.Entity<Allocation>()
            .HasOne(a => a.Resource)
            .WithMany(r => r.Allocations)
            .HasForeignKey(a => a.ResourceId);

        modelBuilder.Entity<Allocation>()
            .HasOne(a => a.Project)
            .WithMany(p => p.Allocations)
            .HasForeignKey(a => a.ProjectId);

        modelBuilder.Entity<Allocation>()
            .HasOne(a => a.Period)
            .WithMany(p => p.Allocations)
            .HasForeignKey(a => a.PeriodId);

        // Unique constraint on allocation (Resource + Project + Period must be unique)
        modelBuilder.Entity<Allocation>()
            .HasIndex(a => new { a.ResourceId, a.ProjectId, a.PeriodId })
            .IsUnique();

        // Project - Status relationship
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Status)
            .WithMany(s => s.Projects)
            .HasForeignKey(p => p.StatusId)
            .IsRequired(false);

        // Indexes
        modelBuilder.Entity<Skill>().HasIndex(s => s.Name).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
        modelBuilder.Entity<Vendor>().HasIndex(v => v.Name).IsUnique();
        modelBuilder.Entity<Project>().HasIndex(p => p.Name).IsUnique();
        modelBuilder.Entity<ProjectStatus>().HasIndex(s => s.Name).IsUnique();
        modelBuilder.Entity<Period>().HasIndex(p => p.Name).IsUnique();
    }
}
