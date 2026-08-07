using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Auth.Commands;

public record RegisterCommand(RegisterRequest Request) : IRequest<AuthResponse>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Request.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Request.FirstName).NotEmpty();
        RuleFor(x => x.Request.LastName).NotEmpty();
    }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand command, CancellationToken ct)
    {
        var r = command.Request;
        if (await _userManager.FindByEmailAsync(r.Email) != null)
            throw new AppException("Email is already registered.");

        var user = new ApplicationUser { UserName = r.Email, Email = r.Email, FirstName = r.FirstName, LastName = r.LastName, EmailConfirmed = true };
        var result = await _userManager.CreateAsync(user, r.Password);
        if (!result.Succeeded)
            throw new AppException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "User");
        var roles = await _userManager.GetRolesAsync(user);
        var access = _tokenService.GenerateAccessToken(user, roles);
        var refresh = await _tokenService.CreateRefreshTokenAsync(user.Id, ct);
        return new AuthResponse(access, refresh.Token, DateTime.UtcNow.AddMinutes(15),
            new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, null, roles));
    }
}
