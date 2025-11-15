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

    public async Task<Employee> UpdateEmployee(Employee employee)
    {
        var response = await _http.PutAsJsonAsync($"api/employee/{employee.Id}", employee);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Employee>() ?? employee;
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

    public async Task<bool> MoveEmployee(Guid employeeId, BoardType boardType, string columnId)
    {
        var response = await _http.PutAsync($"api/employee/{employeeId}/move?boardType={boardType}&columnId={columnId}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<Employee> CreateEmployeeAsync(Employee employee)
    {
        var response = await _http.PostAsJsonAsync("api/employee", employee);
        response.EnsureSuccessStatusCode();
        var createdEmployee = await response.Content.ReadFromJsonAsync<Employee>();
        if (createdEmployee != null)
        {
            await GetEmployees();
            return createdEmployee;
        }
        throw new Exception("Failed to create employee");
    }
}