using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using KanbanGame.Shared;

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
        var result = await _http.GetFromJsonAsync<List<Employee>>("api/employee/available");
        return result ?? new List<Employee>();
    }
} 