using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySampleClassLibrary
{
    public class Customer : Person
    {
        public string CustomerIdentifier { get; set; }

        public Customer(int id, string firstName, string lastName, DateTime birthDate, string customerIdentifier) : base(id, firstName, lastName, birthDate)
        {
            CustomerIdentifier = customerIdentifier;
        }

    }
}
