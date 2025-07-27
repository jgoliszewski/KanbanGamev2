using KanbanGame.Shared;

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

    public WorkSimulationService(
        IEmployeeService employeeService,
        ITaskService taskService,
        IFeatureService featureService)
    {
        _employeeService = employeeService;
        _taskService = taskService;
        _featureService = featureService;
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

        Console.WriteLine("Work simulation completed. Updating services...");

        // Update all services
        await _employeeService.UpdateEmployees();
        await _taskService.UpdateTasks();
        await _featureService.UpdateFeatures();

        Console.WriteLine("All services updated.");
    }

    private async Task ProcessWorkItem(Card workItem, Employee employee)
    {
        bool isCompleted = false;

        if (workItem is KanbanTask task)
        {
            // Reduce labor left by employee efficiency
            task.LaborLeft = Math.Max(0, task.LaborLeft - employee.Efficiency);
            isCompleted = task.IsCompleted;

            Console.WriteLine($"Task '{task.Title}' labor left reduced from {task.LaborLeft + employee.Efficiency:F2} to {task.LaborLeft:F2} by {employee.Name} (efficiency: {employee.Efficiency:P0})");
        }
        else if (workItem is Feature feature)
        {
            // Reduce labor left by employee efficiency
            feature.LaborLeft = Math.Max(0, feature.LaborLeft - employee.Efficiency);
            isCompleted = feature.IsCompleted;

            Console.WriteLine($"Feature '{feature.Title}' labor left reduced from {feature.LaborLeft + employee.Efficiency:F2} to {feature.LaborLeft:F2} by {employee.Name} (efficiency: {employee.Efficiency:P0})");
        }

        // If work is completed, move to next column
        if (isCompleted)
        {
            Console.WriteLine($"Work item '{workItem.Title}' completed! Moving to next column.");
            await MoveToNextColumn(workItem);

            // Unassign from employee
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
        }
    }

    private async Task MoveToNextColumn(Card workItem)
    {
        // Define column progression for different board types
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
            workItem.ColumnId = nextColumn;
            workItem.UpdatedAt = DateTime.Now;

            // Reset LaborLeft to original LaborIntensity when moving to next column
            if (workItem is KanbanTask task)
            {
                task.LaborLeft = task.LaborIntensity;
                Console.WriteLine($"Task '{task.Title}' moved to {nextColumn}, labor left reset to {task.LaborLeft:F2}");
            }
            else if (workItem is Feature feature)
            {
                feature.LaborLeft = feature.LaborIntensity;
                Console.WriteLine($"Feature '{feature.Title}' moved to {nextColumn}, labor left reset to {feature.LaborLeft:F2}");
            }
        }
    }
}