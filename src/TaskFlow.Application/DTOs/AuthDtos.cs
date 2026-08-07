using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs;

public record RegisterRequest([Required, EmailAddress] string Email, [Required, MinLength(6)] string Password, [Required] string FirstName, [Required] string LastName);
public record LoginRequest([Required, EmailAddress] string Email, [Required] string Password);
public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt, UserDto User);
public record RefreshTokenRequest([Required] string AccessToken, [Required] string RefreshToken);
public record UserDto(string Id, string Email, string FirstName, string LastName, string? AvatarUrl, IList<string> Roles);
