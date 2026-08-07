using MediatR;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Projects.Queries;

public record GetProjectsQuery(string UserId) : IRequest<IReadOnlyList<ProjectDto>>;

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, IReadOnlyList<ProjectDto>>
{
    private readonly IUnitOfWork _uow;
    public GetProjectsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<ProjectDto>> Handle(GetProjectsQuery q, CancellationToken ct)
    {
        var memberships = await _uow.ProjectMembers.FindAsync(m => m.UserId == q.UserId, ct);
        var ids = memberships.Select(m => m.ProjectId).ToList();
        var projects = await _uow.Projects.FindAsync(p => ids.Contains(p.Id) && !p.IsArchived, ct);
        return projects.Select(p => new ProjectDto(p.Id, p.Name, p.Description, p.Color, p.IsArchived, p.OwnerId, "", 0, 0, p.CreatedAt)).ToList();
    }
}
