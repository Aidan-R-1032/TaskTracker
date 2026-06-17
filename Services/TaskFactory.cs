using TaskTracker.Models;

namespace TaskTracker.Services
{
    // Handles the creation and validation of TaskItem instances
    public class TaskFactory
    {
        public TaskItem Create(string title, string description, DateTime dueDate, Priority priority)
        {
            if (string.IsNullOrWhiteSpace(title)) 
            {
                throw new ArgumentNullException("Title cannot be null or empty.");
            }

            if (dueDate <= DateTime.UtcNow) 
            {
                throw new ArgumentException("Due date must be in the future.");
            }

            return new TaskItem
            {
                Title = title.Trim(),
                Description = description.Trim(),
                DueDate = dueDate,
                Priority = priority,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
