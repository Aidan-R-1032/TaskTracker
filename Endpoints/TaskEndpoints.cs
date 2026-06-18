using TaskTracker.Dtos;
using TaskTracker.Models;
using TaskTracker.Services;


namespace TaskTracker.Endpoints
{    public static class TaskEndpoints
    {
        public static void MapTaskEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/tasks").WithTags("Tasks");

            // Get all tasks
            group.MapGet("/", async (ITaskService taskService) =>
            {
                var tasks = await taskService.GetAllTasksAsync();
                return Results.Ok(TaskMapper.ToResponseDtoList(tasks));
            })
                .WithName("GetAllTasks")
                .WithSummary("Get all tasks ordered by due date");

            group.MapGet("/{id:int}", async (int id, ITaskService taskService) =>
            {
                var task = await taskService.GetTaskByIdAsync(id);

                if (task is null)
                    return Results.NotFound();

                return Results.Ok(TaskMapper.ToResponseDto(task));
            })
                .WithName("GetTaskById")
                .WithSummary("Get a single task by ID");


            group.MapGet("/priority/{priority}", async (string priority, ITaskService taskService) =>
            {
                if (!Enum.TryParse<Priority>(priority, ignoreCase: true, out var parsedPriority))
                {
                    return Results.BadRequest($"Invalid priority value: {priority}. Valid values are: Low, Medium, High.");
                }
                var tasks = await taskService.GetTasksByPriorityAsync(parsedPriority);
                return Results.Ok(TaskMapper.ToResponseDtoList(tasks));
            })
                .WithName("GetTasksByPriority")
                .WithSummary("Get tasks filtered by priority");

            group.MapGet("/overdue", async (ITaskService taskService) =>
            {
                var tasks = await taskService.GetOverdueTasksAsync();
                return Results.Ok(TaskMapper.ToResponseDtoList(tasks));
            })
                .WithName("GetOverdueTasks")
                .WithSummary("Get all overdue tasks");

            group.MapGet("/completed", async (ITaskService taskService) =>
            {
                var tasks = await taskService.GetCompletedTasksAsync();
                return Results.Ok(TaskMapper.ToResponseDtoList(tasks));
            })
                .WithName("GetCompletedTasks")
                .WithSummary("Get all completed tasks");

            group.MapPost("/", async (CreateTaskDto dto, ITaskService taskService) =>
            {
                try
                {
                    var task = await taskService.CreateTaskAsync(
                            dto.Title,
                            dto.Description,
                            dto.DueDate,
                            dto.Priority
                        );
                    if (task is null)
                    {
                        return Results.BadRequest("Failed to create task. Please check the provided data.");
                    }
                    return Results.Created($"/api/tasks/{task.Id}", TaskMapper.ToResponseDto(task));
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            })
                .WithName("CreateTask")
                .WithSummary("Create a new task");

            group.MapPut("/{id:int}", async (int id, CreateTaskDto? dto, ITaskService taskService) =>
            {
                if (dto is null)
                {
                    return Results.BadRequest("Request body cannot be null");
                }
                try
                {
                    var task = await taskService.UpdateTaskAsync(
                            id,
                            dto.Title,
                            dto.Description,
                            dto.DueDate,
                            dto.Priority
                        );

                    return task is null ?
                        Results.NotFound($"Task with ID {id} was not found.") :
                        Results.Ok(TaskMapper.ToResponseDto(task));
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });

            group.MapPatch("/{id:int}/complete", async (int id, ITaskService taskService) =>
            {
                var success = await taskService.CompleteTaskAsync(id);
                return success ?
                    Results.NoContent() :
                    Results.NotFound($"Task with ID {id} was not found.");
            })
                .WithName("MarkTaskAsCompleted")
                .WithSummary("Mark a task as completed");

            group.MapDelete("/{id:int}", async (int id, ITaskService taskService) =>
            {
                var success = await taskService.DeleteTaskAsync(id);
                return success ?
                    Results.NoContent() :
                    Results.NotFound($"Task with ID {id} was not found.");
            })
                .WithName("DeleteTask")
                .WithSummary("Delete a task by ID");
        }
    }
}
