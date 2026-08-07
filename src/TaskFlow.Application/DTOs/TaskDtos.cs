using System.ComponentModel.DataAnnotations;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs;

public record TaskDto(Guid Id, string Title, string? Description, TaskStatus Status, TaskPriority Priority, DateTime? DueDate, int Position, Guid ProjectId, string? AssigneeId, string? AssigneeName, string CreatorId, string CreatorName, DateTime CreatedAt);
public record CreateTaskRequest([Required, MaxLength(200)] string Title, [MaxLength(2000)] string? Description, TaskPriority Priority = TaskPriority.Medium, DateTime? DueDate = null, string? AssigneeId = null);
