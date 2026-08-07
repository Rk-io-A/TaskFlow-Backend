using System.Security.Claims;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    Task<RefreshToken> CreateRefreshTokenAsync(string userId, CancellationToken ct = default);
    Task RevokeRefreshTokenAsync(string token, string? reason = null, CancellationToken ct = default);
}
