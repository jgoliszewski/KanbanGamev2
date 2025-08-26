# Kanban Game v2

A Blazor-based Kanban board game application with real-time collaboration features.

## Repository Structure

### Client
- **Components/**
  - **UI/** - Reusable UI components
    - `NotificationPopup.razor` - Notification display component
  - **Game/** - Game-specific components
    - `DraggableCard.razor` - Draggable task cards
    - `DroppableColumn.razor` - Kanban columns that accept dropped cards
    - `GameCalendar.razor` - Game calendar component
    - `GameStatus.razor` - Game status display
    - `KanbanBoard.razor` - Main kanban board component
    - `MoneyHistory.razor` - Financial transaction history
  - **Modals/** - Modal dialog components
    - `MoveEmployeeModal.razor` - Employee movement modal
    - `RestartConfirmationModal.razor` - Game restart confirmation
  - **Layout/** - Layout and navigation components
- **Services/**
  - **Interfaces/** - Service contracts
    - `IDragDropService.cs` - Drag and drop functionality
    - `IEmployeeService.cs` - Employee management
    - `IFeatureService.cs` - Feature management
    - `IGameRestartService.cs` - Game restart functionality
    - `INotificationService.cs` - Notification system
    - `ISignalRService.cs` - Real-time communication
    - `ITaskService.cs` - Task management
  - Service implementations
    - `DragDropService.cs` - Drag and drop implementation
    - `EmployeeService.cs` - Employee service implementation
    - `FeatureService.cs` - Feature service implementation
    - `GameRestartService.cs` - Game restart service
    - `GameStateManager.cs` - Game state management
    - `GameStateService.cs` - Game state service
    - `GlobalLoaderService.cs` - Global loading indicator
    - `NotificationService.cs` - Notification service
    - `SignalRService.cs` - SignalR communication
    - `TaskService.cs` - Task service implementation
    - `WorkSimulationService.cs` - Work simulation logic

### Server
- **Services/**
  - **Interfaces/** - Service contracts
    - `IEmployeeService.cs` - Employee management interface
    - `IFeatureService.cs` - Feature management interface
    - `IGameRestartService.cs` - Game restart interface
    - `INotificationService.cs` - Notification interface
    - `ITaskService.cs` - Task management interface
  - Service implementations
    - `EmployeeService.cs` - Employee service implementation
    - `FeatureService.cs` - Feature service implementation
    - `GameRestartService.cs` - Game restart service
    - `GameStateService.cs` - Game state service
    - `NotificationService.cs` - Notification service
    - `TaskService.cs` - Task service implementation
    - `VacationBackgroundService.cs` - Vacation processing service
- **Controllers/** - API endpoints
  - `EmployeeController.cs` - Employee management API
  - `FeatureController.cs` - Feature management API
  - `GameStateController.cs` - Game state API
  - `TaskController.cs` - Task management API
- **SignalR/** - Real-time communication hubs
  - `GameHub.cs` - Game state updates
  - `NotificationHub.cs` - Notification broadcasting

### Shared
- **Models/** - Data models
  - `Board.cs` - Kanban board model
  - `Card.cs` - Task card model
  - `Column.cs` - Board column model
  - `Employee.cs` - Employee model
  - `Feature.cs` - Feature model
  - `KanbanTask.cs` - Task model
  - `MoneyTransaction.cs` - Financial transaction model
- **Enums/** - Enumeration types
  - `BoardType.cs` - Board type definitions
  - `Department.cs` - Department types
  - `EmployeeStatus.cs` - Employee status
  - `Priority.cs` - Task priority levels
  - `Role.cs` - Employee roles
  - `Seniority.cs` - Seniority levels
  - `Status.cs` - Task status
- **Extensions/** - Extension methods
  - `EnumExtensions.cs` - Enum utility methods

## Getting Started

1. Ensure you have .NET 7.0 or later installed
2. Clone the repository
3. Navigate to the solution directory
4. Run `dotnet restore` to restore packages
5. Run `dotnet run --project Server` to start the backend
6. Run `dotnet run --project Client` to start the frontend

## Features

- Real-time Kanban board with drag and drop
- Employee management and role assignment
- Feature and task management
- Game state persistence
- Real-time notifications via SignalR
- Financial simulation with money tracking
- Vacation and time-off management

## Architecture

The application follows a clean architecture pattern with:
- **Client**: Blazor WebAssembly frontend
- **Server**: ASP.NET Core Web API backend
- **Shared**: Common models and contracts
- **SignalR**: Real-time communication layer
