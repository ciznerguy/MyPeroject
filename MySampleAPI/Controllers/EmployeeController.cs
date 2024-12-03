using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using MySampleClassLibrary;

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

    [HttpPost]
    public ActionResult AddEmployee([FromBody] Employee employee)
    {
        _logger.LogInformation("Executing AddEmployee action");

        if (employee == null)
        {
            _logger.LogWarning("Employee object is null.");
            return BadRequest("Employee data is null");
        }

        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        int newId;

        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // שלב 1: הוספה לטבלת BaseEntity ולקבלת ה-Id שנוצר
                using (MySqlCommand baseEntityCmd = new MySqlCommand(
                    "INSERT INTO BaseEntity (CreatedAt, UpdatedAt) VALUES (@CreatedAt, @UpdatedAt); SELECT LAST_INSERT_ID();", conn))
                {
                    baseEntityCmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    baseEntityCmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    newId = Convert.ToInt32(baseEntityCmd.ExecuteScalar());
                }

                // שלב 2: הוספה לטבלת Person תוך שימוש ב-Id שנוצר
                using (MySqlCommand personCmd = new MySqlCommand(
                    "INSERT INTO Person (Id, FirstName, LastName, BirthDate, CreatedAt, UpdatedAt, Role) " +
                    "VALUES (@Id, @FirstName, @LastName, @BirthDate, @CreatedAt, @UpdatedAt, @Role);", conn))
                {
                    personCmd.Parameters.AddWithValue("@Id", newId);
                    personCmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    personCmd.Parameters.AddWithValue("@LastName", employee.LastName);
                    personCmd.Parameters.AddWithValue("@BirthDate", employee.BirthDate);
                    personCmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    personCmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    personCmd.Parameters.AddWithValue("@Role", employee.Role.ToString()); // המרת ה-Role למחרוזת בדיוק כפי שהוגדר

                    personCmd.ExecuteNonQuery();
                }

                // שלב 3: הוספה לטבלת Employee תוך שימוש ב-Id שנוצר
                using (MySqlCommand employeeCmd = new MySqlCommand(
                    "INSERT INTO Employee (Id, DateHired) VALUES (@Id, @DateHired);", conn))
                {
                    employeeCmd.Parameters.AddWithValue("@Id", newId);
                    employeeCmd.Parameters.AddWithValue("@DateHired", employee.DateHired);
                    employeeCmd.ExecuteNonQuery();
                }

                _logger.LogInformation("Employee added successfully: {FirstName} {LastName}", employee.FirstName, employee.LastName);
            }
        }
        catch (MySqlException ex)
        {
            _logger.LogError(ex, "Error occurred while adding employee: {FirstName} {LastName}", employee.FirstName, employee.LastName);
            return StatusCode(500, "Internal server error");
        }

        return Ok("Employee added successfully");
    }
}
