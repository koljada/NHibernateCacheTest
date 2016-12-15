namespace NHibernateCacheTest
{
    public class Entity
    {
        public Entity() { }
        public Entity(string name)
        {
            Name = name;
        }
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}
