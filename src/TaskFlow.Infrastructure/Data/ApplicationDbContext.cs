using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<Project>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Restrict);
        });
        b.Entity<ProjectMember>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.ProjectId, x.UserId }).IsUnique();
            e.HasOne(x => x.Project).WithMany(p => p.Members).HasForeignKey(x => x.ProjectId);
            e.HasOne(x => x.User).WithMany(u => u.ProjectMemberships).HasForeignKey(x => x.UserId);
        });
        b.Entity<TaskItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.HasOne(x => x.Project).WithMany(p => p.Tasks).HasForeignKey(x => x.ProjectId);
            e.HasOne(x => x.Creator).WithMany(u => u.CreatedTasks).HasForeignKey(x => x.CreatorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Assignee).WithMany(u => u.AssignedTasks).HasForeignKey(x => x.AssigneeId).OnDelete(DeleteBehavior.SetNull);
        });
        b.Entity<RefreshToken>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Token).IsUnique();
            e.HasOne(x => x.User).WithMany(u => u.RefreshTokens).HasForeignKey(x => x.UserId);
        });
    }
}
