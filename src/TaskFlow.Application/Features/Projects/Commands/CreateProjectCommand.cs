using MediatR;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Projects.Commands;

public record CreateProjectCommand(CreateProjectRequest Request, string UserId) : IRequest<ProjectDto>;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IUnitOfWork _uow;
    public CreateProjectCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ProjectDto> Handle(CreateProjectCommand cmd, CancellationToken ct)
    {
        var p = new Project
        {
            Name = cmd.Request.Name,
            Description = cmd.Request.Description,
            Color = cmd.Request.Color ?? "#3B82F6",
            OwnerId = cmd.UserId,
            CreatedBy = cmd.UserId
        };
        await _uow.Projects.AddAsync(p, ct);
        await _uow.ProjectMembers.AddAsync(new ProjectMember { ProjectId = p.Id, UserId = cmd.UserId, Role = "Owner" }, ct);
        await _uow.SaveChangesAsync(ct);
        return new ProjectDto(p.Id, p.Name, p.Description, p.Color, p.IsArchived, p.OwnerId, "", 1, 0, p.CreatedAt);
    }
}
