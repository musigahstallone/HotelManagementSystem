using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using HotelManagementSystem.Server.Models;
using HotelManagementSystem.Server.Data;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace HotelManagementSystem.Server.Controllers;

[Route("api/todos")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly string _todosCacheKey = "todos";
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public TodoController(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // 🔹 GET ALL TODOS (with caching)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
    {
        return await _cache.GetOrCreateAsync(_todosCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
            return await _context.Todos.ToListAsync();
        });
    }

    // 🔹 GET A SINGLE TODO BY ID (with caching)
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Todo>> GetTodoById(Guid id)
    {
        string cacheKey = $"todo_{id}";

        var todo = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
            return await _context.Todos.FindAsync(id);
        });

        if (todo == null) return NotFound("Todo not found");
        return todo;
    }

    // 🔹 CREATE A NEW TODO (invalidate cache)
    [HttpPost]
    public async Task<ActionResult<Todo>> CreateTodo([FromBody] Todo todoDto)
    {
        if (string.IsNullOrWhiteSpace(todoDto.Note))
            return BadRequest("Note cannot be empty");

        var todo = Todo.CreateNew(todoDto.Note);
        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        _cache.Remove(_todosCacheKey); // Invalidate cached todos

        return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, todo);
    }

    // 🔹 UPDATE A TODO (invalidate cache)
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTodo(Guid id, [FromBody] string newNote)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return NotFound("Todo not found");

        if (string.IsNullOrWhiteSpace(newNote))
            return BadRequest("Note cannot be empty");

        todo.UpdateNote(newNote);
        await _context.SaveChangesAsync();

        _cache.Remove(_todosCacheKey); // Invalidate all todos cache
        _cache.Remove($"todo_{id}");   // Invalidate single todo cache

        return NoContent();
    }

    // 🔹 MARK A TODO AS COMPLETED (invalidate cache)
    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> MarkAsCompleted(Guid id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return NotFound("Todo not found");

        todo.MarkAsCompleted();
        await _context.SaveChangesAsync();

        _cache.Remove(_todosCacheKey);
        _cache.Remove($"todo_{id}");

        return Ok(todo);
    }

    // 🔹 MARK A TODO AS UNCOMPLETED (invalidate cache)
    [HttpPatch("{id}/uncomplete")]
    public async Task<IActionResult> UncompleteTodo(Guid id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return NotFound();

        todo.MarkAsNotCompleted();
        await _context.SaveChangesAsync();

        _cache.Remove(_todosCacheKey);
        _cache.Remove($"todo_{id}");

        return Ok(todo);
    }

    // 🔹 DELETE A TODO (invalidate cache)
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTodo(Guid id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null) return NotFound("Todo not found");

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();

        _cache.Remove(_todosCacheKey);
        _cache.Remove($"todo_{id}");

        return NoContent();
    }
}
