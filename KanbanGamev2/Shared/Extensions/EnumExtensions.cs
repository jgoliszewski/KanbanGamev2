using System.ComponentModel;

namespace KanbanGame.Shared;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());
        var attributes = field?.GetCustomAttributes(typeof(DescriptionAttribute), false);
        
        if (attributes != null && attributes.Length > 0)
        {
            return ((DescriptionAttribute)attributes[0]).Description;
        }
        
        return enumValue.ToString().Replace("_", " ");
    }

    public static string GetBadgeClass(this Priority priority)
    {
        return priority switch
        {
            Priority.Low => "bg-secondary",
            Priority.Medium => "bg-primary",
            Priority.High => "bg-warning",
            Priority.Critical => "bg-danger",
            _ => "bg-secondary"
        };
    }

    public static string GetBadgeClass(this Priority? priority)
    {
        if (!priority.HasValue)
            return "bg-secondary";
        return priority.Value.GetBadgeClass();
    }

    public static string GetBadgeClass(this Status status)
    {
        return status switch
        {
            Status.New => "bg-info",
            Status.ToDo => "bg-secondary",
            Status.InProgress => "bg-primary",
            Status.Waiting => "bg-warning",
            Status.Testing => "bg-info",
            Status.Done => "bg-success",
            Status.Cancelled => "bg-danger",
            _ => "bg-secondary"
        };
    }
} 