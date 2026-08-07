using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _ctx;
    private IRepository<Project>? _projects;
    private IRepository<TaskItem>? _tasks;
    private IRepository<ProjectMember>? _members;
    private IRepository<RefreshToken>? _tokens;
    public UnitOfWork(ApplicationDbContext ctx) => _ctx = ctx;
    public IRepository<Project> Projects => _projects ??= new Repository<Project>(_ctx);
    public IRepository<TaskItem> Tasks => _tasks ??= new Repository<TaskItem>(_ctx);
    public IRepository<ProjectMember> ProjectMembers => _members ??= new Repository<ProjectMember>(_ctx);
    public IRepository<RefreshToken> RefreshTokens => _tokens ??= new Repository<RefreshToken>(_ctx);
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _ctx.SaveChangesAsync(ct);
    public void Dispose() { _ctx.Dispose(); GC.SuppressFinalize(this); }
}
