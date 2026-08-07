using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Common;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Data;

namespace TaskFlow.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _ctx;
    protected readonly DbSet<T> _set;
    public Repository(ApplicationDbContext ctx) { _ctx = ctx; _set = ctx.Set<T>(); }
    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) => await _set.FindAsync(new object[] { id }, ct);
    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) => await _set.ToListAsync(ct);
    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> p, CancellationToken ct = default) => await _set.Where(p).ToListAsync(ct);
    public async Task<T> AddAsync(T e, CancellationToken ct = default) { await _set.AddAsync(e, ct); return e; }
    public Task UpdateAsync(T e, CancellationToken ct = default) { e.UpdatedAt = DateTime.UtcNow; _set.Update(e); return Task.CompletedTask; }
    public Task DeleteAsync(T e, CancellationToken ct = default) { _set.Remove(e); return Task.CompletedTask; }
    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) => await _set.AnyAsync(x => x.Id == id, ct);
}
