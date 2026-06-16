namespace TaskTracker.Models
{
    public class TaskItem
    {
        public int Id { get; set; }                                     // Unique identifier/primary key for the task
        public string Title { get; set; } = string .Empty;              // Title of the task, initialized to an empty string
        public string Description { get; set; } = string.Empty;         // Detailed description of the task, initialized to an empty string
        public DateTime DueDate { get; set; }                           // Due date for the task
        public bool IsCompleted { get; set; }                         // Indicates whether the task is completed or not
        public Priority Priority { get; set; } = Priority.Medium;       // Priority level of the task, initialized to Medium by default
        public DateTime CreatedAt { get; set; } = DateTime.Now;         // Timestamp for when the task was created, initialized to the current date and time
    }
    public enum Priority
    {
        Low,
        Medium,
        High
    }
}
