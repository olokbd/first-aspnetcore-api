using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Run(async (HttpContext context) =>
{
    if (context.Request.Path.StartsWithSegments("/"))
    {
        context.Response.Headers["Content-Type"] = "text/html";

        await context.Response.WriteAsync("<h1>Web Server Running!</h1>");

        await context.Response.WriteAsync("<ul>");

        foreach (var key in context.Request.Headers.Keys)
        {
            await context.Response.WriteAsync($"<li><b>{key}</b>: {context.Request.Headers[key]}</li>");

            await context.Response.WriteAsync("</ul>");
        }
    }
    else if (context.Request.Path.StartsWithSegments("/employees"))
    {
        if (context.Request.Method == "GET")
        {
            if (context.Request.Query.ContainsKey("id"))
            {
                var id = context.Request.Query["id"];

                if (int.TryParse(id, out int employeeId))
                {
                    var employee = EmployeesReporitory.GetSpecificEmployee(employeeId);

                    if (employee is not null)
                    {
                        context.Response.Headers["Content-Type"] = "text/html";
                        await context.Response.WriteAsync($"Name: {employee.Name}</br>Position: {employee.Position}");
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync($"Employee with id: {employeeId} not found.");
                    }
                }
                else
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Please provide a valid id.");
                }
            }
            else
            {
                var employees = EmployeesReporitory.GetEmployees();

                context.Response.StatusCode = 200;

                foreach (var employee in employees)
                {
                    await context.Response.WriteAsync($"{employee.Name} : {employee.Position}\n");
                }
            }
        }
        else if (context.Request.Method == "POST")
        {
            try
            {
                using var reader = new StreamReader(context.Request.Body);
                var body = await reader.ReadToEndAsync();
                var employee = JsonSerializer.Deserialize<Employee>(body);

                if (employee is null || employee.Id <= 0)
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                EmployeesReporitory.AddEmployee(employee);

                context.Response.StatusCode = 201;

                await context.Response.WriteAsync("Employee added successfully.");
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 400;
                context.Response.WriteAsync(ex.ToString());
                return;
            }
        }
        else if (context.Request.Method == "PUT")
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            var employee = JsonSerializer.Deserialize<Employee>(body);

            var flag = EmployeesReporitory.UpdateEmployee(employee);

            if (flag)
            {
                context.Response.StatusCode = 204;
                await context.Response.WriteAsJsonAsync("Employee updated successfully!");
            }
            else
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync("Employee not found.");
            }
        }
        else if (context.Request.Method == "DELETE")
        {
            if (context.Request.Query.ContainsKey("id"))
            {
                if (context.Request.Headers["Authorization"] == "munna")
                {
                    var id = context.Request.Query["id"];
                    if (int.TryParse(id, out int employeeId))
                    {
                        var flag = EmployeesReporitory.DeleteEmployee(employeeId);

                        if (flag)
                        {
                            context.Response.StatusCode = 204;
                            await context.Response.WriteAsync("Employee deleted successfully!");
                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                            await context.Response.WriteAsync("No employee found.");
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync("You do not have permission to delete.");
                }
            }
        }
    }
    else if (context.Request.Path.StartsWithSegments("/redirect"))
    {
        context.Response.Redirect("/employees");
    }
    else
    {
        context.Response.StatusCode = 404;
    }
});

app.Run();

static class EmployeesReporitory
{
    private static List<Employee> Employees = new List<Employee>
    {
        new Employee(1, "Munna", "SWE"),
        new Employee(2, "Monon", "DevOps")
    };
    public static List<Employee> GetEmployees() => Employees;

    public static void AddEmployee(Employee? employee)
    {
        if (employee is not null)
        {
            Employees.Add(employee);
        }
    }

    public static bool UpdateEmployee(Employee? employee)
    {
        if (employee is not null)
        {
            var emp = Employees.FirstOrDefault(x => x.Id == employee.Id);

            if (emp != null)
            {
                emp.Name = employee.Name;
                emp.Position = employee.Position;

                return true;
            }
        }

        return false;
    }

    public static bool DeleteEmployee(int id)
    {
        var employee = Employees.FirstOrDefault(x => x.Id == id);

        if (employee is not null)
        {
            Employees.Remove(employee);

            return true;
        }

        return false;
    }

    public static Employee? GetSpecificEmployee (int id)
    {
        var employee = Employees.FirstOrDefault(x => x.Id == id);

        return employee;
    }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }

    public Employee(int id, string name, string position)
    {
        Id = id;
        Name = name;
        Position = position;
    }
}