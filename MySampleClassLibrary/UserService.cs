using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MySampleClassLibrary
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Role
    {
        Admin,
        User,
        Guest // ניתן להוסיף תפקידים נוספים
    }

    public class UserService
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public Role Role { get; set; } = Role.Guest;

        // שדות חדשים
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public void ClearUser()
        {
            UserId = 0;
            UserName = string.Empty;
            Role = Role.Guest;
            FirstName = string.Empty;
            LastName = string.Empty;
        }
    }
}
