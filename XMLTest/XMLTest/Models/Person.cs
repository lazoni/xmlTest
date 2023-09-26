namespace XMLTest.Models
{
    public class Person
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public Phone Phone { get; set; }
        public Address Address { get; set; }
        public List<Familymember> Family { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string Place { get; set; }
        public string Number { get; set; }

    }
    public class Phone
    {
        public string Mobile { get; set; }
        public string Phone2 { get; set; }

    }

    public class Familymember
    {
        public string Name { get; set; }
        public string Born { get; set; }
        public Address Address { get; set; }
        public Phone Phone { get; set; }
    }
}
