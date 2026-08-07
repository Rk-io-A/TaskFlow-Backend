using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Features.Projects.Commands;
using TaskFlow.Application.Features.Projects.Queries;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _m;
    public ProjectsController(IMediator m) => _m = m;
    private string Uid => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<ActionResult> Get() => Ok(await _m.Send(new GetProjectsQuery(Uid)));

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateProjectRequest req)
        => Ok(await _m.Send(new CreateProjectCommand(req, Uid)));
}
