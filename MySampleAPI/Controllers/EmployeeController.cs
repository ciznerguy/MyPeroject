using Microsoft.AspNetCore.Mvc; // שימוש בספריות נדרשות לעבודה עם ASP.NET Core

using MySql.Data.MySqlClient; // חיבור לספריית MySql 

using MySampleClassLibrary; // חיבור לספרייה מותאמת אישית המכילה מחלקות לוגיות

[Route("api/[controller]")] // הגדרת מסלול API עבור הבקר
[ApiController] // תיוג כבקר
public class EmployeeController : ControllerBase
{
    private readonly IConfiguration _configuration; // משתנה פרטי לאחסון הגדרות קובץ התצורה
    private readonly ILogger<EmployeeController> _logger; // משתנה פרטי לאחסון כלי רישום עבור הבקר

    // בנאי של המחלקה שמקבל את ההגדרות ואת כלי הרישום דרך הזרקת תלות
    public EmployeeController(IConfiguration configuration, ILogger<EmployeeController> logger)
    {
        _configuration = configuration; // שמירת ההגדרות במשתנה המקומי
        _logger = logger; // שמירת כלי הרישום במשתנה המקומי
    }

    // פעולה מסוג GET להצגת כל העובדים
    [HttpGet("AllEmployees")]
    public ActionResult GetAllEmployees()
    {
        _logger.LogInformation("הפעולה להצגת כל העובדים מתבצעת"); // רישום הודעת לוג לתחילת הביצוע של הפעולה

        // קבלת מחרוזת חיבור מתוך קובץ ההגדרות
        string connectionString = _configuration.GetConnectionString("DefaultConnection");
        List<Employee> employees = new List<Employee>(); // רשימה לאחסון העובדים שנמצאו

        try
        {
            // יצירת חיבור לבסיס הנתונים MySQL
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open(); // פתיחת החיבור לבסיס הנתונים

                // שאילתה למציאת כל העובדים עם שימוש ב-JOIN בין Person ל-Employee
                string query = "SELECT p.Id, p.FirstName, p.LastName, p.BirthDate, p.CreatedAt, p.UpdatedAt, p.Role, e.DateHired " +
                               "FROM Person p " +
                               "INNER JOIN Employee e ON p.Id = e.Id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // קריאת הנתונים מהתוצאה והוספתם לרשימת העובדים
                        while (reader.Read())
                        {
                            employees.Add(new Employee(
    reader.GetInt32("Id"),
    reader.GetString("FirstName"),
    reader.GetString("LastName"),
    reader.GetDateTime("BirthDate"),
    reader.GetDateTime("DateHired")
));
                        }
                    }
                }

                // רישום הודעת לוג על הצלחה
                _logger.LogInformation("נמצאו {Count} עובדים", employees.Count);
            }
        }
        // טיפול בשגיאות MySQL
        catch (MySqlException ex)
        {
            _logger.LogError(ex, "שגיאה התרחשה בעת חיפוש כל העובדים"); // רישום הודעת שגיאה ללוג
            return StatusCode(500, "שגיאת שרת פנימית"); // החזרת תשובת שגיאת שרת
        }

        // בדיקה האם נמצאו עובדים
        if (employees.Count == 0)
        {
            _logger.LogWarning("לא נמצאו עובדים במערכת"); // רישום אזהרה ללוג במידה ולא נמצאו עובדים
            return NotFound("לא נמצאו עובדים במערכת"); // החזרת תשובת לא נמצא
        }

        // החזרת רשימת העובדים שנמצאו
        return Ok(employees);
    }
}
