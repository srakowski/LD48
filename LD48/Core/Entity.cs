namespace LD48.Core
{
    using System.Collections.Generic;

    struct Entity
    {
        public Entity(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public static EntityBuilder New => new EntityBuilder();
    }

    class EntityBuilder
    {
        private List<IEntityData> _entityData = new();

        public EntityBuilder AddData<T>(T gameData) where T : IEntityData
        {
            _entityData.Add(gameData);
            return this;
        }

        public IEnumerable<IEntityData> EntityData => _entityData;

        public static EntityBuilder operator +(EntityBuilder eb, IEntityData data) => eb.AddData(data);
    }
}
