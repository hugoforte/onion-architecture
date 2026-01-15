using Microsoft.AspNetCore.Mvc;
using Starter.Contracts;
using Starter.Services.Abstractions;

namespace Starter.Web.Controllers;

[ApiController]
[Route("api/todoitems")]
public sealed class TodoItemsController : ControllerBase
{
    private readonly ITodoItemService _todoItemService;

    public TodoItemsController(ITodoItemService todoItemService)
    {
        _todoItemService = todoItemService;
    }

    [HttpGet("~/api/todo-lists/{listId:guid}/items")]
    [ProducesResponseType(typeof(IReadOnlyCollection<TodoItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByList(Guid listId, CancellationToken cancellationToken)
    {
        var items = await _todoItemService.GetByListAsync(listId, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _todoItemService.GetByIdAsync(id, cancellationToken);
        return Ok(item);
    }

    [HttpPost("~/api/todo-lists/{listId:guid}/items")]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(Guid listId, [FromBody] TodoItemForCreationDto dto, CancellationToken cancellationToken)
    {
        dto.TodoListId = listId;
        var item = await _todoItemService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TodoItemForUpdateDto dto, CancellationToken cancellationToken)
    {
        await _todoItemService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        await _todoItemService.CompleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _todoItemService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
