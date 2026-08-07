using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Auth.Commands;

public record LoginCommand(LoginRequest Request) : IRequest<AuthResponse>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Request.Password).NotEmpty();
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sm, ITokenService ts)
    {
        _userManager = um; _signInManager = sm; _tokenService = ts;
    }

    public async Task<AuthResponse> Handle(LoginCommand command, CancellationToken ct)
    {
        var r = command.Request;
        var user = await _userManager.FindByEmailAsync(r.Email);
        if (user == null || !user.IsActive)
            throw new UnauthorizedException("Invalid email or password.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, r.Password, true);
        if (!result.Succeeded)
            throw new UnauthorizedException("Invalid email or password.");

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var access = _tokenService.GenerateAccessToken(user, roles);
        var refresh = await _tokenService.CreateRefreshTokenAsync(user.Id, ct);
        return new AuthResponse(access, refresh.Token, DateTime.UtcNow.AddMinutes(15),
            new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, null, roles));
    }
}
