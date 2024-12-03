using MySampleClassLibrary;

public class Employee : Person
{
    public DateTime DateHired { get; set; }

    public Employee(int id, string firstName, string lastName, DateTime birthDate, DateTime dateHired) : base(id, firstName, lastName, birthDate)
    {
        DateHired = dateHired;
    }

    public Employee() : base()
    {
    }
}
