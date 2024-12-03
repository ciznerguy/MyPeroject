using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySampleClassLibrary;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Logging;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmployeeController> _logger;

    public EmployeeController(IConfiguration configuration, ILogger<EmployeeController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Employee>> GetEmployees()
    {
        _logger.LogInformation("Executing GetEmployees action");

        var employees = new List<Employee>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(
                "SELECT p.Id, p.FirstName, p.LastName, p.BirthDate, e.DateHired " +
                "FROM Person p INNER JOIN Employee e ON p.Id = e.Id", conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Checking if the returned values are not NULL
                    if (reader["Id"] == DBNull.Value || reader["FirstName"] == DBNull.Value || reader["LastName"] == DBNull.Value || reader["BirthDate"] == DBNull.Value || reader["DateHired"] == DBNull.Value)
                    {
                        _logger.LogWarning("One or more of the returned fields are NULL.");
                        continue;
                    }

                    // Creating a new Employee object with public property access
                    var employee = new Employee
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                        DateHired = Convert.ToDateTime(reader["DateHired"])
                    };

                    _logger.LogInformation("Added employee: {Id}, {FirstName} {LastName}", employee.Id, employee.FirstName, employee.LastName);

                    employees.Add(employee);
                }
            }
        }

        if (employees.Count == 0)
        {
            _logger.LogWarning("No employees found to return.");
        }

        return Ok(employees);
    }

    [HttpGet("{id}")]
    public ActionResult<Employee> GetEmployee(int id)
    {
        _logger.LogInformation("Executing GetEmployee action with Id {Id}", id);

        Employee employee = null;
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(
                "SELECT p.Id, p.FirstName, p.LastName, p.BirthDate, e.DateHired " +
                "FROM Person p INNER JOIN Employee e ON p.Id = e.Id " +
                "WHERE p.Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    // Checking if the returned values are not NULL
                    if (reader["Id"] == DBNull.Value || reader["FirstName"] == DBNull.Value || reader["LastName"] == DBNull.Value || reader["BirthDate"] == DBNull.Value || reader["DateHired"] == DBNull.Value)
                    {
                        _logger.LogWarning("One or more of the returned fields are NULL for employee with Id {Id}", id);
                        return NotFound();
                    }

                    // Creating a new Employee object with public property access
                    employee = new Employee
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                        DateHired = Convert.ToDateTime(reader["DateHired"])
                    };

                    _logger.LogInformation("Retrieved employee: {Id}, {FirstName} {LastName}", employee.Id, employee.FirstName, employee.LastName);
                }
            }
        }

        if (employee == null)
        {
            _logger.LogWarning("Employee with Id {Id} not found.", id);
            return NotFound();
        }

        return Ok(employee);
    }
}
