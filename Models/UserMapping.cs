using FluentNHibernate.Mapping;

namespace Task15.Models
{
    public class ItemMap : ClassMap<User>
    {
        public ItemMap()
        {
            Id(user => user.Id).GeneratedBy.Increment();
            Map(user => user.Login);
            Map(user => user.Name);
            Map(user => user.Sex);
            Map(user => user.Salary);
            Map(user => user.Age);
            Map(user => user.BirthDay);
            Table("users");
        }
    }
}