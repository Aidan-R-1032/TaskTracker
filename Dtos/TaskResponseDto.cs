using TaskTracker.Models;

namespace TaskTracker.Dtos
{
    public class TaskResponseDto
    // What goes OUT - controls what the API exposes to the client
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public Priority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsOverdue => !IsCompleted && DueDate < DateTime.UtcNow; // No DB query is needed
    }
}