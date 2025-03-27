using HotelManagementSystem.Server.Models.Hotels;

namespace HotelManagementSystem.Server.Models.Todo;

public class Todo
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string? Note { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateOnly CreatedDate { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public TimeOnly CreatedTime { get; private set; } = TimeOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? CompletedDate { get; private set; }
    public TimeOnly? CompletedTime { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public bool IsArchived { get; private set; }
    public bool IsDeleted { get; private set; }

    // Constructor for new todos
    private Todo() { }

    private Todo(string note)
    {

        if (string.IsNullOrWhiteSpace(note))
            throw new ArgumentException("Note cannot be empty", nameof(note));

        Note = note;
        Logger.Success($"New TODO created: {Note}");
    }

    public static Todo CreateNew(string note)
    {
        return new Todo(note);
    }

    public void UpdateNote(string newNote)
    {
        if (string.IsNullOrWhiteSpace(newNote))
            throw new ArgumentException("Note cannot be empty", nameof(newNote));

        Note = newNote;
        Logger.Info($"TODO note updated: {Note}");
    }

    public void SetDueDate(DateOnly dueDate)
    {
        if (dueDate < CreatedDate)
            throw new ArgumentException("Due date cannot be before the creation date.", nameof(dueDate));

        DueDate = dueDate;
        Logger.Info($"TODO due date set: {DueDate}");
    }

    public bool IsOverdue()
    {
        return DueDate.HasValue && !IsCompleted && DueDate.Value < DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public void Postpone(int days)
    {
        if (!DueDate.HasValue)
            throw new InvalidOperationException("Cannot postpone a TODO without a due date.");

        DueDate = DueDate.Value.AddDays(days);
        Logger.Info($"TODO postponed by {days} days. New due date: {DueDate}");
    }

    public void MarkAsCompleted()
    {
        if (IsCompleted)
        {
            Logger.Warning($"TODO is already completed: {Note}");
            return;
        }

        IsCompleted = true;
        CompletedDate = DateOnly.FromDateTime(DateTime.UtcNow);
        CompletedTime = TimeOnly.FromDateTime(DateTime.UtcNow);
        Logger.Success($"TODO marked as completed: {Note}");
    }

    public void MarkAsNotCompleted()
    {
        if (!IsCompleted)
        {
            Logger.Warning($"TODO is already not completed: {Note}");
            return;
        }

        IsCompleted = false;
        CompletedDate = null;
        CompletedTime = null;
        Logger.Info($"TODO marked as not completed: {Note}");
    }

    public void ToggleCompletion()
    {
        if (IsCompleted)
            MarkAsNotCompleted();
        else
            MarkAsCompleted();
    }

    public void Archive()
    {
        if (IsArchived)
        {
            Logger.Warning($"TODO is already archived: {Note}");
            return;
        }

        IsArchived = true;
        Logger.Info($"TODO archived: {Note}");
    }

    public void Unarchive()
    {
        if (!IsArchived)
        {
            Logger.Warning($"TODO is not archived: {Note}");
            return;
        }

        IsArchived = false;
        Logger.Info($"TODO unarchived: {Note}");
    }

    public void Delete()
    {
        if (IsDeleted)
        {
            Logger.Warning($"TODO is already deleted: {Note}");
            return;
        }

        IsDeleted = true;
        Logger.Info($"TODO deleted: {Note}");
    }

    public void Restore()
    {
        if (!IsDeleted)
        {
            Logger.Warning($"TODO is not deleted: {Note}");
            return;
        }

        IsDeleted = false;
        Logger.Info($"TODO restored: {Note}");
    }

    public string GetSummary()
    {
        string status = IsCompleted ? $"Completed on {CompletedDate} at {CompletedTime}" : "Not completed";
        return $"TODO: {Note}\nCreated: {CreatedDate} at {CreatedTime}\nStatus: {status}";
    }

    public TimeSpan GetTimeSinceCreation()
    {
        return DateTime.UtcNow - new DateTime(CreatedDate.Year, CreatedDate.Month, CreatedDate.Day,
                                              CreatedTime.Hour, CreatedTime.Minute, CreatedTime.Second);
    }

    public TimeSpan? GetTimeToCompletion()
    {
        if (!IsCompleted || !CompletedDate.HasValue || !CompletedTime.HasValue)
            return null;

        DateTime createdDateTime = new(CreatedDate.Year, CreatedDate.Month, CreatedDate.Day,
                                                CreatedTime.Hour, CreatedTime.Minute, CreatedTime.Second);

        DateTime completedDateTime = new(CompletedDate.Value.Year, CompletedDate.Value.Month, CompletedDate.Value.Day,
                                                  CompletedTime.Value.Hour, CompletedTime.Value.Minute, CompletedTime.Value.Second);

        return completedDateTime - createdDateTime;
    }

    public Todo Clone()
    {
        return new Todo(Note!);
    }
}
