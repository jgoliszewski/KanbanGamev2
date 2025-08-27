using KanbanGame.Shared;
using KanbanGamev2.Shared.Services;

namespace KanbanGamev2.Client.Services;

public interface IWorkSimulationService
{
    Task SimulateWorkDay();
}

public class WorkSimulationService : IWorkSimulationService
{
    private readonly IEmployeeService _employeeService;
    private readonly ITaskService _taskService;
    private readonly IFeatureService _featureService;
    private readonly KanbanGamev2.Shared.Services.IGameStateService _gameStateService;
    private readonly IGameStateManager _gameStateManager;

    public WorkSimulationService(
        IEmployeeService employeeService,
        ITaskService taskService,
        IFeatureService featureService,
        KanbanGamev2.Shared.Services.IGameStateService gameStateService,
        IGameStateManager gameStateManager)
    {
        _employeeService = employeeService;
        _taskService = taskService;
        _featureService = featureService;
        _gameStateService = gameStateService;
        _gameStateManager = gameStateManager;
    }

    public async Task SimulateWorkDay()
    {
        Console.WriteLine("Starting work simulation...");

        // Get all employees and their assigned work
        var employees = _employeeService.Employees;
        var tasks = _taskService.Tasks;
        var features = _featureService.Features;

        Console.WriteLine($"Found {employees.Count} employees, {tasks.Count} tasks, {features.Count} features");

        // Process each employee's work
        foreach (var employee in employees)
        {
            if (!employee.IsWorking)
            {
                Console.WriteLine($"Employee {employee.Name} is not working, skipping...");
                continue;
            }

            Console.WriteLine($"Processing work for employee: {employee.Name} (Efficiency: {employee.Efficiency:P0})");

            // Process assigned task
            if (employee.AssignedTaskId.HasValue)
            {
                var task = tasks.FirstOrDefault(t => t.Id == employee.AssignedTaskId.Value);
                if (task != null)
                {
                    Console.WriteLine($"Employee {employee.Name} is working on task: {task.Title}");
                    await ProcessWorkItem(task, employee);
                    // Update the task in the service
                    await _taskService.UpdateTask(task);
                    await _employeeService.UpdateEmployee(employee);
                }
                else
                {
                    Console.WriteLine($"Task with ID {employee.AssignedTaskId.Value} not found!");
                }
            }

            // Process assigned feature
            if (employee.AssignedFeatureId.HasValue)
            {
                var feature = features.FirstOrDefault(f => f.Id == employee.AssignedFeatureId.Value);
                if (feature != null)
                {
                    Console.WriteLine($"Employee {employee.Name} is working on feature: {feature.Title}");
                    await ProcessWorkItem(feature, employee);
                    // Update the feature in the service
                    await _featureService.UpdateFeature(feature);
                    await _employeeService.UpdateEmployee(employee);
                }
                else
                {
                    Console.WriteLine($"Feature with ID {employee.AssignedFeatureId.Value} not found!");
                }
            }
        }

        // Check for completed features and handle task cleanup
        await CheckForCompletedFeatures();

        Console.WriteLine("Work simulation completed.");

        // Refresh data from server to ensure consistency
        await _employeeService.GetEmployees();
        await _taskService.GetTasks();
        await _featureService.GetFeatures();

        Console.WriteLine("All services refreshed from server.");
    }

    private async Task CheckForCompletedFeatures()
    {
        // Check all features that have generated tasks (sent to development)
        var featuresWithTasks = _featureService.Features.Where(f => f.GeneratedTaskIds.Any()).ToList();

        Console.WriteLine($"Checking {featuresWithTasks.Count} features with generated tasks for completion...");

        foreach (var feature in featuresWithTasks)
        {
            Console.WriteLine($"Checking feature '{feature.Title}' with {feature.GeneratedTaskIds.Count} tasks...");

            var allTasksCompleted = await _taskService.AreAllTasksCompleted(feature.GeneratedTaskIds);

            Console.WriteLine($"Feature '{feature.Title}' - All tasks completed: {allTasksCompleted}");

            if (allTasksCompleted)
            {
                Console.WriteLine($"All tasks completed for feature: {feature.Title}. Moving to delivered and adding profit.");

                // Move feature to delivered
                feature.ColumnId = "done";
                feature.Status = Status.Done;
                await _featureService.UpdateFeature(feature);

                // Add profit to company money
                await _gameStateService.AddMoney(feature.Profit, $"Feature completed: {feature.Title}");

                // Delete all tasks for this feature
                await _taskService.DeleteTasks(feature.GeneratedTaskIds);
                feature.GeneratedTaskIds.Clear();
                await _featureService.UpdateFeature(feature);

                // Unlock achievement for feature completion
                var achievement = new Achievement
                {
                    Id = $"feature_completed_{feature.Id}",
                    Name = $"Feature Completed: {feature.Title}",
                    Description = $"Successfully completed feature '{feature.Title}' and earned ${feature.Profit:N0}",
                    Icon = "🎉",
                    Type = AchievementType.Milestone
                };
                await _gameStateService.UnlockAchievement(achievement);

                Console.WriteLine($"Feature {feature.Title} delivered! Added ${feature.Profit:N0} to company money.");
            }
        }
    }

    private async Task ProcessWorkItem(Card workItem, Employee employee)
    {
        bool isCompleted = false;
        double workDone = employee.Efficiency;


        if (workItem is KanbanTask kanbanTask)
        {
            kanbanTask.LaborLeft = Math.Max(0, kanbanTask.LaborLeft - workDone);

            Console.WriteLine($"Task '{kanbanTask.Title}' labor left reduced from {kanbanTask.LaborLeft + workDone:F2} to {kanbanTask.LaborLeft:F2} by {employee.Name} (efficiency: {employee.Efficiency:P0})");

            if (kanbanTask.LaborLeft <= 0)
            {
                kanbanTask.LaborLeft = 0;
                kanbanTask.Status = Status.Done;
                isCompleted = true;
                Console.WriteLine($"Task '{kanbanTask.Title}' completed by {employee.Name}");
            }
        }
        else if (workItem is Feature feature)
        {
            feature.LaborLeft = Math.Max(0, feature.LaborLeft - workDone);

            Console.WriteLine($"Feature '{feature.Title}' labor left reduced from {feature.LaborLeft + workDone:F2} to {feature.LaborLeft:F2} by {employee.Name} (efficiency: {employee.Efficiency:P0})");

            if (feature.LaborLeft <= 0)
            {
                feature.LaborLeft = 0;
                feature.Status = Status.Done;
                isCompleted = true;
                Console.WriteLine($"Feature '{feature.Title}' completed by {employee.Name}");
            }
        }

        // If work is completed, unassign the employee and move to next column
        if (isCompleted)
        {
            if (workItem is KanbanTask completedTask)
            {
                completedTask.AssignedToEmployeeId = null;
                employee.AssignedTaskId = null;
            }
            else if (workItem is Feature completedFeature)
            {
                completedFeature.AssignedToEmployeeId = null;
                employee.AssignedFeatureId = null;
            }

            // Move completed work item to next column
            await MoveToNextColumn(workItem);
        }
    }

    private async Task MoveToNextColumn(Card workItem)
    {
        var columnProgression = new Dictionary<string, string>
        {
            // Analysis Board progression
            { "backlog", "analysis1" },
            { "analysis1", "waiting" },
            { "waiting", "analysis2" },
            { "analysis2", "ready-dev" },
            
            // Backend Board progression
            { "backend-backlog", "backend-analysis" },
            { "backend-analysis", "backend-dev-waiting" },
            { "backend-dev-waiting", "backend-dev-doing" },
            { "backend-dev-doing", "backend-test-waiting" },
            { "backend-test-waiting", "backend-test-doing" },
            { "backend-test-doing", "backend-done" },
            
            // Frontend Board progression
            { "frontend-backlog", "frontend-analysis" },
            { "frontend-analysis", "frontend-dev-waiting" },
            { "frontend-dev-waiting", "frontend-dev-doing" },
            { "frontend-dev-doing", "frontend-test-waiting" },
            { "frontend-test-waiting", "frontend-test-doing" },
            { "frontend-test-doing", "frontend-done" }
        };

        if (columnProgression.TryGetValue(workItem.ColumnId, out var nextColumn))
        {
            var oldColumn = workItem.ColumnId;
            workItem.ColumnId = nextColumn;
            workItem.UpdatedAt = DateTime.Now;

            // Always reset LaborLeft when moving to a new column
            if (workItem is KanbanTask movedTask)
            {
                movedTask.LaborLeft = movedTask.LaborIntensity;
                Console.WriteLine($"Task '{movedTask.Title}' moved from {oldColumn} to {nextColumn}, labor left reset to {movedTask.LaborLeft:F2}");
            }
            else if (workItem is Feature movedFeature)
            {
                movedFeature.LaborLeft = movedFeature.LaborIntensity;
                Console.WriteLine($"Feature '{movedFeature.Title}' moved from {oldColumn} to {nextColumn}, labor left reset to {movedFeature.LaborLeft:F2}");

                // If feature moved to ready-dev column and the ready for development column is not visible,
                // automatically send it to development
                if (nextColumn == "ready-dev" && !_gameStateService.IsReadyForDevelopmentColumnVisible)
                {
                    Console.WriteLine($"Feature '{movedFeature.Title}' automatically sent to development (ready-dev column hidden)");
                    await _featureService.SendFeatureToDevelopment(movedFeature);
                    return; // Don't continue with normal column progression since SendFeatureToDevelopment handles it
                }
            }
        }
        else
        {
            Console.WriteLine($"No progression found for {workItem.GetType().Name} '{workItem.Title}' in column '{workItem.ColumnId}'");
        }
    }
}