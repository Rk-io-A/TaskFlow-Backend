using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.DTOs;

public record ProjectDto(Guid Id, string Name, string? Description, string Color, bool IsArchived, string OwnerId, string OwnerName, int MemberCount, int TaskCount, DateTime CreatedAt);
public record CreateProjectRequest([Required, MaxLength(100)] string Name, [MaxLength(500)] string? Description, string? Color);
