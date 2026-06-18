using TaskTracker.Models;

namespace TaskTracker.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(Priority priority);
        Task<IEnumerable<TaskItem>> GetOverdueTasksAsync();
        Task<IEnumerable<TaskItem>> GetCompletedTasksAsync();
        Task<TaskItem?> CreateTaskAsync(string title, string description, DateTime dueDate, Priority priority);
        Task<TaskItem?> UpdateTaskAsync(int id, string title, string description, DateTime dueDate, Priority priority);
        Task<bool> CompleteTaskAsync(int id);
        Task<bool> DeleteTaskAsync(int id);
    }
}
