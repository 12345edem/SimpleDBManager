using FluentNHibernate.Mapping;

namespace Task15.Models
{
    public class ItemMap : ClassMap<User>
    {
        public ItemMap()
        {
            Id(user => user.Id).GeneratedBy.Increment();
            Map(user => user.Name);
            Map(user => user.Sex);
            Map(user => user.Weight);
            Table("users");
        }
    }
}