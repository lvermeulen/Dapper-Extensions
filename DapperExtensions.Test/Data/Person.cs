using System;
using System.Collections.Generic;
using DapperExtensions.Mapper;

namespace DapperExtensions.Test.Data
{
    public class Person
    {
	    public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Active { get; set; }
        public IEnumerable<Phone> Phones { get; } = new List<Phone>();
    }

    public class Phone
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

    public class PersonMapper : ClassMapper<Person>
    {
        public PersonMapper()
        {
            Table("Person");
            Map(m => m.Phones).Ignore();
            AutoMap();
        }
    }
}