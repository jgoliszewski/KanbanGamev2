using KanbanGame.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace KanbanGamev2.Client.Services;

public class EmployeeService : IEmployeeService
{
    private readonly HttpClient _http;
    private readonly NavigationManager _navigationManager;

    public List<Employee> Employees { get; set; } = new();

    public EmployeeService(HttpClient http, NavigationManager navigationManager)
    {
        _http = http;
        _navigationManager = navigationManager;
    }

    public async Task GetEmployees()
    {
        var result = await _http.GetFromJsonAsync<List<Employee>>("api/employee");
        if (result is not null)
            Employees = result;
    }

    public async Task<Employee?> GetEmployee(Guid id)
    {
        return await _http.GetFromJsonAsync<Employee>($"api/employee/{id}");
    }

    public async Task<Employee> CreateEmployee(Employee employee)
    {
        var response = await _http.PostAsJsonAsync("api/employee", employee);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Employee>() ?? employee;
    }

    public async Task<Employee> UpdateEmployee(Employee employee)
    {
        var response = await _http.PutAsJsonAsync($"api/employee/{employee.Id}", employee);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Employee>() ?? employee;
    }

    public async Task<bool> DeleteEmployee(Guid id)
    {
        var response = await _http.DeleteAsync($"api/employee/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<Employee>> GetAvailableEmployees()
    {
        return await GetAvailableEmployeesAsync();
    }

    public async Task<List<Employee>> GetAvailableEmployeesAsync()
    {
        await GetEmployees();
        return Employees.Where(e => e.Status == EmployeeStatus.Active && !e.IsWorking).ToList();
    }

    public async Task<List<Employee>> GetEmployeesByColumnAsync(string columnId)
    {
        await GetEmployees();
        return Employees.Where(e => e.ColumnId == columnId && e.Status == EmployeeStatus.Active).ToList();
    }

    public async Task<bool> SendEmployeeOnVacationAsync(Guid employeeId, int days)
    {
        try
        {
            var response = await _http.PostAsync($"api/employee/{employeeId}/vacation?days={days}", null);
            if (response.IsSuccessStatusCode)
            {
                await GetEmployees();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending employee on vacation: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> EndEmployeeVacationAsync(Guid employeeId)
    {
        try
        {
            var response = await _http.PostAsync($"api/employee/{employeeId}/end-vacation", null);
            if (response.IsSuccessStatusCode)
            {
                await GetEmployees();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error ending employee vacation: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> FireEmployeeAsync(Guid employeeId)
    {
        try
        {
            var response = await _http.PostAsync($"api/employee/{employeeId}/fire", null);
            if (response.IsSuccessStatusCode)
            {
                await GetEmployees();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error firing employee: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RehireEmployeeAsync(Guid employeeId)
    {
        try
        {
            var response = await _http.PostAsync($"api/employee/{employeeId}/rehire", null);
            if (response.IsSuccessStatusCode)
            {
                await GetEmployees();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rehiring employee: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UnassignWorkFromEmployee(Guid employeeId)
    {
        var response = await _http.PutAsync($"api/employee/{employeeId}/unassign", null);
        return response.IsSuccessStatusCode;
    }

    public async Task UpdateEmployees()
    {
        // This method is called after work simulation to persist changes
        await Task.CompletedTask;
    }

    public async Task<bool> MoveEmployee(Guid employeeId, BoardType boardType, string columnId)
    {
        var response = await _http.PutAsync($"api/employee/{employeeId}/move?boardType={boardType}&columnId={columnId}", null);
        return response.IsSuccessStatusCode;
    }
}