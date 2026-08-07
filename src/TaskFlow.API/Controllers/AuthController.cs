using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Features.Auth.Commands;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
        => Ok(await _mediator.Send(new RegisterCommand(req)));

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
        => Ok(await _mediator.Send(new LoginCommand(req)));

    [HttpGet("me")]
    [Authorize]
    public ActionResult Me()
    {
        var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value!;
        var fn = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value!;
        var ln = User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value!;
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();
        return Ok(new UserDto(id, email, fn, ln, null, roles));
    }
}
