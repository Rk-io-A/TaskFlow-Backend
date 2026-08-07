namespace TaskFlow.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Entities.Project> Projects { get; }
    IRepository<Entities.TaskItem> Tasks { get; }
    IRepository<Entities.ProjectMember> ProjectMembers { get; }
    IRepository<Entities.RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
