using FluentNHibernate.Mapping;

namespace NHibernateCacheTest
{
    public class EntityMap : ClassMap<Entity>
    {
        public EntityMap()
        {
            Id(x => x.Id);

            Map(x => x.Name);
        }
    }
}
