using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly TaskFactory _taskFactory;

        public TaskService(AppDbContext context, TaskFactory taskFactory)
        {
            _context = context;
            _taskFactory = taskFactory;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(Priority priority)
        {
            return await _context.Tasks
                .Where(t => t.Priority == priority)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync()
        {
            return await _context.Tasks
                .Where(t => t.DueDate < DateTime.UtcNow && !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetCompletedTasksAsync()
        {
            return await _context.Tasks
                .Where(t => t.IsCompleted)
                .OrderByDescending(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<TaskItem?> CreateTaskAsync(string title, string description, DateTime dueDate, Priority priority)
        {
            try
            {
                var task = _taskFactory.Create(title, description, dueDate, priority);
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
                return task;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<TaskItem?> UpdateTaskAsync(int id, string title, string description, DateTime dueDate, Priority priority)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task is null) return null;
            try
            {
                task.Title = title.Trim();
                task.Description = description.Trim();
                task.DueDate = dueDate;
                task.Priority = priority;

                await _context.SaveChangesAsync();
                return task;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<bool> CompleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task is null) return false;
            try
            {
                task.IsCompleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task is null) return false;
            try
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
