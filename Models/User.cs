using System;

namespace Task15.Models
{
    public class User
    {
        public virtual int Id { get; set; }
        public virtual string Login { get; set; }
        public virtual string Name { get; set; }
        public virtual string Sex { get; set; }
        public virtual float Salary { get; set; }
        public virtual int Age { get; set; }
        public virtual DateTime BirthDay { get; set; }
    }
}
