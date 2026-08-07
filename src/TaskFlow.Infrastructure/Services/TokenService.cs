using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _cfg;
    private readonly IUnitOfWork _uow;
    public TokenService(IConfiguration cfg, IUnitOfWork uow) { _cfg = cfg; _uow = uow; }

    public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        foreach (var role in roles) claims.Add(new Claim(ClaimTypes.Role, role));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_cfg["Jwt:Issuer"], _cfg["Jwt:Audience"], claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_cfg["Jwt:AccessTokenExpirationMinutes"] ?? "15")),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        try
        {
            var p = new TokenValidationParameters
            {
                ValidateAudience = true, ValidateIssuer = true, ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!)),
                ValidateLifetime = false, ValidIssuer = _cfg["Jwt:Issuer"], ValidAudience = _cfg["Jwt:Audience"]
            };
            return new JwtSecurityTokenHandler().ValidateToken(token, p, out _);
        }
        catch { return null; }
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(string userId, CancellationToken ct = default)
    {
        var rt = new RefreshToken
        {
            Token = GenerateRefreshToken(), UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(double.Parse(_cfg["Jwt:RefreshTokenExpirationDays"] ?? "7"))
        };
        await _uow.RefreshTokens.AddAsync(rt, ct);
        await _uow.SaveChangesAsync(ct);
        return rt;
    }

    public async Task RevokeRefreshTokenAsync(string token, string? reason = null, CancellationToken ct = default)
    {
        var list = await _uow.RefreshTokens.FindAsync(t => t.Token == token, ct);
        var rt = list.FirstOrDefault();
        if (rt == null) return;
        rt.IsRevoked = true; rt.ReasonRevoked = reason; rt.UpdatedAt = DateTime.UtcNow;
        await _uow.RefreshTokens.UpdateAsync(rt, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
