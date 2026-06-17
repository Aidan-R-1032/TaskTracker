using TaskTracker.Models;

namespace TaskTracker.Dtos
{
    public class TaskMapper
    {
        public static TaskResponseDto ToResponseDto(TaskItem task)
        {
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                Priority = task.Priority,
                CreatedAt = task.CreatedAt
            };
        }

        public static IEnumerable<TaskResponseDto> ToResponseDtoList(IEnumerable<TaskItem> tasks)
        {
            return tasks.Select(t => ToResponseDto(t)).ToList();
        }
    }
}
