using Microsoft.AspNetCore.Mvc; // Include ASP.NET Core MVC
using MySql.Data.MySqlClient; // Include MySQL library
using MySampleClassLibrary; // Custom library import
using System.Collections.Generic;
using System.Data;

[Route("api/[controller]")]
[ApiController]
public class PersonController : ControllerBase
{
    private readonly IConfiguration _configuration; // Inject configuration
    private readonly ILogger<PersonController> _logger; // Inject logger
 

    // Constructor to get dependencies
    public PersonController(IConfiguration configuration, ILogger<PersonController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Person>> GetPeople()
    {
        var people = new List<Person>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        try
        {
            _logger.LogInformation("Attempting to connect to the database.");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                _logger.LogInformation("Database connection opened successfully.");

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Person", conn);
                _logger.LogInformation("Executing query: SELECT * FROM Person");

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var person = new Person
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            BirthDate = Convert.ToDateTime(reader["BirthDate"])
                        };

                        people.Add(person);
                    }
                    _logger.LogInformation("Query executed successfully. Retrieved {Count} records.", people.Count);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while accessing the database.");
            return StatusCode(500, "Internal server error");
        }

        return Ok(people);
    }
}
