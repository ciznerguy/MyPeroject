using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySampleClassLibrary;  // כאן נוסיף שימוש בספרייה שלך
using System.Collections.Generic;
using System.Data;

[Route("api/[controller]")]
[ApiController]
public class PersonController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public PersonController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Person>> GetPeople()
    {
        var people = new List<Person>();
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Person", conn);
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
            }
        }
        return Ok(people);
    }

    [HttpGet("{id}")]
    public ActionResult<Person> GetPerson(int id)
    {
        Person person = null;
        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Person WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    person = new Person
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        BirthDate = Convert.ToDateTime(reader["BirthDate"])
                    };
                }
            }
        }

        if (person == null)
        {
            return NotFound();
        }
        return Ok(person);
    }
}
