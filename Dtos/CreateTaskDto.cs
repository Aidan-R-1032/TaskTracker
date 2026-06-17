using TaskTracker.Models;

namespace TaskTracker.Dtos
{
    public class CreateTaskDto
    // what comes IN - controls what the API accepts from the client
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;
    }
}