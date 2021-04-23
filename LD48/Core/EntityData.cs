using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LD48.Core
{
    interface IEntityData { }

    struct EntityDataTypeFlag
    {
        public EntityDataTypeFlag(long value)
        {
            Value = value;
        }

        public long Value { get; }
    }

    abstract class EntityDataTable
    {
        protected EntityDataTable(EntityDataTypeFlag flag)
        {
            Flag = flag;
        }

        public EntityDataTypeFlag Flag { get; }
    }

    class EntityDataTable<T> : EntityDataTable where T : struct
    {
        private readonly List<T> _data;
        private readonly Dictionary<Entity, int> _entityToDataIdxMap;
        private readonly Dictionary<int, Entity> _dataIdxToEntityMap;

        public EntityDataTable(EntityDataTypeFlag flag) : base(flag)
        {
            _data = new List<T>();
            _entityToDataIdxMap = new Dictionary<Entity, int>();
            _dataIdxToEntityMap = new Dictionary<int, Entity>();
        }

        public T GetData(Entity entity)
        {
            return _data[_entityToDataIdxMap[entity]];
        }

        public Entity SetData(Entity entity, T data)
        {
            if (!_entityToDataIdxMap.ContainsKey(entity))
            {
                AddData(entity, data);
            }

            _data[_entityToDataIdxMap[entity]] = data;

            return new Entity(entity.Id);
        }

        public Entity RemoveData(Entity entity)
        {
            var idx = _entityToDataIdxMap[entity];
            _entityToDataIdxMap.Remove(entity);
            _dataIdxToEntityMap.Remove(idx);

            var lastIdx = _data.Count - 1;
            if (idx != lastIdx)
            {
                _data[idx] = _data[lastIdx];

                var entityAtLastIdx = _dataIdxToEntityMap[lastIdx];

                _dataIdxToEntityMap.Remove(lastIdx);
                _dataIdxToEntityMap.Add(idx, entityAtLastIdx);

                _entityToDataIdxMap[entityAtLastIdx] = idx;
            }

            _data.RemoveAt(_data.Count - 1);

            return new Entity(entity.Id);
        }

        private void AddData(Entity entity, T data)
        {
            var idx = _data.Count;
            _data.Add(data);
            _entityToDataIdxMap[entity] = idx;
            _dataIdxToEntityMap[idx] = entity;
        }
    }
}
