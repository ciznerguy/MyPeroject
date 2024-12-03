using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySampleClassLibrary
{
    public enum UserRole
    {
        User,
        Admin
    }

    public class Person : BaseEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public UserRole Role { get; set; } = UserRole.User;

        public Person(int id, string firstName, string lastName, DateTime birthDate, UserRole role = UserRole.User) : base(id)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            Role = role;
        }

        public Person() : base()
        {
        }
    }
}
