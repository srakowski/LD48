namespace LD48.Core
{
    class Entity
    {
        private EntityDataManager _entityDataManager;

        public Entity(EntityDataManager entityDataManager)
        {
            _entityDataManager = entityDataManager;
        }

        public long Archetype { get; private set; }

        public Entity SetData<T>(T data) where T : struct
        {
            var flag = _entityDataManager.SetData(this, data);
            AddFlagToArchetype(flag);
            return this;
        }

        public T GetData<T>() where T : struct
        {
            return _entityDataManager.GetData<T>(this);
        }

        public Entity RemoveData<T>(T data = default) where T : struct
        {
            var flag = _entityDataManager.RemoveData<T>(this);
            RemoveFlagFromArchetype(flag);
            return this;
        }

        private void AddFlagToArchetype(EntityDataTypeFlag flag)
        {
            Archetype |= flag.Value;
        }

        private void RemoveFlagFromArchetype(EntityDataTypeFlag flag)
        {
            if ((Archetype & flag.Value) != 0)
            {
                Archetype ^= flag.Value;
            }
        }
    }
}
