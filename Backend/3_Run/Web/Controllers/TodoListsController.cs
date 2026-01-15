using Microsoft.AspNetCore.Mvc;
using Starter.Contracts;
using Starter.Services.Abstractions;

namespace Starter.Web.Controllers;

[ApiController]
[Route("api/todo-lists")]
public sealed class TodoListsController : ControllerBase
{
    private readonly ITodoListService _todoListService;

    public TodoListsController(ITodoListService todoListService)
    {
        _todoListService = todoListService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TodoListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var lists = await _todoListService.GetAllAsync(cancellationToken);
        return Ok(lists);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TodoListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var list = await _todoListService.GetByIdAsync(id, cancellationToken);
        return Ok(list);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoListDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] TodoListForCreationDto dto, CancellationToken cancellationToken)
    {
        var list = await _todoListService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = list.Id }, list);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TodoListForUpdateDto dto, CancellationToken cancellationToken)
    {
        await _todoListService.UpdateAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _todoListService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
