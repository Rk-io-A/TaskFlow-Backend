using TaskFlow.Domain.Common;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public int Position { get; set; }
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string? AssigneeId { get; set; }
    public ApplicationUser? Assignee { get; set; }
    public string CreatorId { get; set; } = string.Empty;
    public ApplicationUser Creator { get; set; } = null!;
    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    public ICollection<TaskLabelAssignment> LabelAssignments { get; set; } = new List<TaskLabelAssignment>();
}
