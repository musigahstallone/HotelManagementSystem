using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Server.Data;
using HotelManagementSystem.Server.Models.Hotels;
using HotelManagementSystem.Server.Models.Todo;
using HotelManagementSystem.Server.Models.Auth;
namespace HotelManagementSystem.Server.Controllers;
/*
[Route("api/todos")]
[ApiController]
public class TodoController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    private IQueryable<Todo> GetTodosQuery() => _context.Todos.AsQueryable();

    private static async Task<PaginatedResponse<Todo>> GetPaginatedTodos(int page, int pageSize, IQueryable<Todo> query)
    {
        if (page < 1 || pageSize < 1)
            throw new ArgumentException("Page and pageSize must be greater than zero.");

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResponse<Todo>(items, page, pageSize, totalCount);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<Todo>>> GetTodos(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        return Ok(await GetPaginatedTodos(page, pageSize, GetTodosQuery()));
    }

    [HttpGet("all-todos")]
    public async Task<ActionResult<ApiResponse<object>>> GetTodos(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchQuery = "")
    {
        var query = _context.Todos.AsQueryable();

        // Apply search filter if searchQuery is provided (ignoring case)
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            //var loweredSearchQuery = searchQuery.ToLower();
            //query = query.Where(t => t.Note.ToLower().Contains(loweredSearchQuery));
            query = query.Where(t => (t.Note ?? string.Empty).Contains(searchQuery, StringComparison.CurrentCultureIgnoreCase));
        }

        // Sort: Prioritize uncompleted todos first, then by ID
        query = query.OrderBy(t => t.IsCompleted).ThenBy(t => t.Id);

        // If page or pageSize is zero or negative, return all todos
        if (page < 1 || pageSize < 1)
        {
            var allTodos = await query.ToListAsync();
            return Ok(ApiResponse<object>.SuccessResponse(allTodos, "Retrieved all todos"));
        }

        // Otherwise, apply pagination
        var totalCount = await query.CountAsync();
        var paginatedTodos = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(new PaginatedResponse<Todo>(paginatedTodos, page, pageSize, totalCount));
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<Todo>>> GetTodoById(Guid id)
    {
        var todo = await _context.Todos.FindAsync(id);
        return todo != null
            ? Ok(ApiResponse<Todo>.SuccessResponse(todo))
            : NotFound(ApiResponse<Todo>.FailureResponse("Todo not found"));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Todo>>> CreateTodo(
        [FromBody] Todo todoDto)
    {
        if (string.IsNullOrWhiteSpace(todoDto.Note))
            return BadRequest(ApiResponse<Todo>.FailureResponse("Note cannot be empty"));

        var todo = Todo.CreateNew(todoDto.Note);
        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, ApiResponse<Todo>.SuccessResponse(todo));
    }

 
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTodo(
        Guid id, 
        [FromBody] UpdateTodoRequest request)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
            return NotFound(ApiResponse<Todo>.FailureResponse("Todo not found"));

        if (string.IsNullOrWhiteSpace(request.NewNote))
            return BadRequest(ApiResponse<Todo>.FailureResponse("Note cannot be empty"));

        todo.UpdateNote(request.NewNote);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpPatch("{id:guid}/toggle")]
    public async Task<IActionResult> ToggleCompletion(Guid id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
            return NotFound(ApiResponse<Todo>.FailureResponse("Todo not found"));

        todo.ToggleCompletion();
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<Todo>.SuccessResponse(todo));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTodo(Guid id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
            return NotFound(ApiResponse<Todo>.FailureResponse("Todo not found"));

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("completed")]
    public async Task<ActionResult<PaginatedResponse<Todo>>> GetCompletedTodos(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        return Ok(await GetPaginatedTodos(page, pageSize, GetTodosQuery().Where(t => t.IsCompleted)));
    }

    [HttpGet("incomplete")]
    public async Task<ActionResult<PaginatedResponse<Todo>>> GetIncompleteTodos(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        return Ok(await GetPaginatedTodos(page, pageSize, GetTodosQuery().Where(t => !t.IsCompleted)));
    }

    [HttpDelete("bulk-delete")]
    public async Task<IActionResult> BulkDelete(
        [FromBody] List<Guid> ids)
    {
        var todos = await _context.Todos.Where(t => ids.Contains(t.Id)).ToListAsync();

        if (todos.Count == 0)
            return NotFound(ApiResponse<string>.FailureResponse("No matching todos found"));

        _context.Todos.RemoveRange(todos);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("bulk-add")]
    public async Task<IActionResult> BulkAdd([FromBody] List<string> notes)
    {
        if (notes == null || notes.Count == 0)
            return BadRequest(ApiResponse<string>.FailureResponse("No todo items provided."));

        var newTodos = notes.Select(Todo.CreateNew).ToList();
        _context.Todos.AddRange(newTodos);
        await _context.SaveChangesAsync();

        return Created("bulk-add", ApiResponse<List<Todo>>.SuccessResponse(newTodos));
    }

    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> MarkAsCompleted(Guid id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return NotFound(ApiResponse<Todo>.FailureResponse("Todo not found"));

        todo.MarkAsCompleted();
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<Todo>.SuccessResponse(todo, "Todo marked as completed"));
    }

    [HttpPatch("{id:guid}/uncomplete")]
    public async Task<IActionResult> MarkAsNotCompleted(Guid id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return NotFound(ApiResponse<Todo>.FailureResponse("Todo not found"));

        todo.MarkAsNotCompleted();
        await _context.SaveChangesAsync();

        return Ok(ApiResponse<Todo>.SuccessResponse(todo, "Todo marked as not completed"));
    }
}
*/