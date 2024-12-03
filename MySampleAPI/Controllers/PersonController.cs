using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc; // הוספת תמיכה ב-ASP.NET Core MVC
using MySql.Data.MySqlClient; // הוספת תמיכה בספריית MySQL

using System.Data;
using MySampleClassLibrary;


[Route("api/[controller]")]
[ApiController]
public class PersonController : ControllerBase
{
    private readonly IConfiguration _configuration; // תלות בקובץ הקונפיגורציה
    private readonly ILogger<PersonController> _logger; // תלות בלוגים לצורך מעקב ותיעוד

    // בנאי לקבלת התלויות
    public PersonController(IConfiguration configuration, ILogger<PersonController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    // פעולה לבדיקת שם משתמש וסיסמה באמצעות GET
    [HttpGet("ValidateLogin")]
    public IActionResult ValidateLogin([FromQuery] string userName, [FromQuery] string password)
    {
        string connectionString = _configuration.GetConnectionString("DefaultConnection"); // שליפת מחרוזת החיבור מבסיס הנתונים

        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open(); // פתיחת חיבור לבסיס הנתונים

                string query = "SELECT COUNT(*) FROM person WHERE UserName = @UserName AND Password = @Password"; // שאילתה לספירת משתמשים תואמים

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName); // הכנסת פרמטר שם משתמש
                    cmd.Parameters.AddWithValue("@Password", password); // הכנסת פרמטר סיסמה

                    int userCount = Convert.ToInt32(cmd.ExecuteScalar()); // הפעלת השאילתה וקבלת מספר משתמשים

                    if (userCount > 0)
                    {
                        return Ok(true); // משתמש תקין
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while validating login."); // רישום שגיאה בלוג
            return StatusCode(500, "Internal server error"); // שגיאת שרת פנימית
        }

        return Ok(false); // משתמש לא תקין
    }

    // פעולה להתחברות משתמש והחזרת נתוני המשתמש
    [HttpGet("Login")]
    public async Task<IActionResult> Login(
     [FromQuery] string userName,
     [FromQuery] string password,
     [FromServices] IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection"); // שליפת מחרוזת החיבור מבסיס הנתונים

        try
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync(); // פתיחת חיבור אסינכרוני לבסיס הנתונים

                // שאילתה לשליפת נתוני המשתמש כולל שדות FirstName ו-LastName
                string query = "SELECT Id, UserName, Role, FirstName, LastName FROM person WHERE UserName = @UserName AND Password = @Password";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserName", userName); // הכנסת פרמטר שם משתמש
                    cmd.Parameters.AddWithValue("@Password", password); // הכנסת פרמטר סיסמה

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // יצירת אובייקט אנונימי להחזרת נתוני המשתמש
                            var userResponse = new
                            {
                                UserId = reader.GetInt32("Id"), // שליפת מזהה המשתמש
                                UserName = reader.GetString("UserName"), // שליפת שם המשתמש
                                Role = reader.GetString("Role"), // שליפת תפקיד
                                FirstName = reader.GetString("FirstName"), // שליפת שם פרטי
                                LastName = reader.GetString("LastName") // שליפת שם משפחה
                            };

                            return Ok(userResponse); // החזרת נתוני המשתמש
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred", Error = ex.Message }); // טיפול בשגיאה
        }

        return Unauthorized(new { Message = "Invalid credentials" }); // הרשאות לא תקינות
    }
}