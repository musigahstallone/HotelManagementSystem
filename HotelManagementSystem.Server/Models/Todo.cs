using System;

namespace HotelManagementSystem.Server.Models;

public class Todo
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string? Note { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateOnly CreatedDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public TimeOnly CreatedTime { get; private set; } = TimeOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? CompletedDate { get; private set; }
    public TimeOnly? CompletedTime { get; private set; }

    // Constructor for new todos
    public Todo(string note)
    {
        if (string.IsNullOrWhiteSpace(note))
            throw new ArgumentException("Note cannot be empty", nameof(note));

        Note = note;
    }

    // Private constructor for EF Core serialization
    private Todo() { }

    // Mark as completed
    public void MarkAsCompleted()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            CompletedDate = DateOnly.FromDateTime(DateTime.UtcNow);
            CompletedTime = TimeOnly.FromDateTime(DateTime.UtcNow);
        }
    }

    // Mark as not completed
    public void MarkAsNotCompleted()
    {
        if (IsCompleted)
        {
            IsCompleted = false;
            CompletedDate = null;
            CompletedTime = null;
        }
    }


    public static Todo CreateNew(string note)
    {
        if (string.IsNullOrWhiteSpace(note))
            throw new ArgumentException("Note cannot be empty", nameof(note));

        return new Todo(note);
    }

    // Update the note
    public void UpdateNote(string newNote)
    {
        if (string.IsNullOrWhiteSpace(newNote))
            throw new ArgumentException("Note cannot be empty", nameof(newNote));

        Note = newNote;
    }
}
