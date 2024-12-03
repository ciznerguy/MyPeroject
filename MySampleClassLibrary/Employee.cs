using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySampleClassLibrary
{
    public class Employee : Person
    {
        public DateTime DateHired { get; set; }
        public Employee(int id, string firstName, string lastName, DateTime birthDate, DateTime dateHired) : base(id, firstName, lastName, birthDate)
        {
            DateHired = dateHired;
        }
    }
}