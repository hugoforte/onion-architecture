using System.ComponentModel.DataAnnotations;
using Starter.Domain.Entities;

namespace Starter.Contracts;

public class TodoItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public TodoPriority Priority { get; set; }
    public Guid TodoListId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class TodoItemForCreationDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public TodoPriority Priority { get; set; } = TodoPriority.Medium;

    [Required]
    public Guid TodoListId { get; set; }
}

public class TodoItemForUpdateDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public TodoPriority Priority { get; set; } = TodoPriority.Medium;
}

public class TodoListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IReadOnlyCollection<TodoItemDto> Items { get; set; } = Array.Empty<TodoItemDto>();
}

public class TodoListForCreationDto
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}

public class TodoListForUpdateDto
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
