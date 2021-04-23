using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LD48.Core
{
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

        public void SetData(Entity entity, T data)
        {
            if (!_entityToDataIdxMap.ContainsKey(entity))
            {
                AddData(entity, data);
            }

            _data[_entityToDataIdxMap[entity]] = data;
        }

        public void RemoveData(Entity entity)
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
        }

        private void AddData(Entity entity, T data)
        {
            var idx = _data.Count;
            _data.Add(data);
            _entityToDataIdxMap[entity] = idx;
            _dataIdxToEntityMap[idx] = entity;
        }
    }

    class EntityDataManager
    {
        private Dictionary<Type, EntityDataTypeFlag> _typeToFlag;
        private Dictionary<EntityDataTypeFlag, EntityDataTable> _flagToEntityDataTable;

        public EntityDataManager()
        {
            _typeToFlag = new Dictionary<Type, EntityDataTypeFlag>();
            _flagToEntityDataTable = new Dictionary<EntityDataTypeFlag, EntityDataTable>();
        }

        public void Initialize(string dataNamespace)
        {
            var entityDataTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    t.Namespace == dataNamespace &&
                    t.IsValueType)
                .ToArray();

            var i = 0;
            foreach (var edt in entityDataTypes)
            {
                var flag = new EntityDataTypeFlag((long)Math.Pow(2, i));
                i++;

                var table = Activator.CreateInstance(
                    typeof(EntityDataTable<>).MakeGenericType(edt),
                    args: flag) as EntityDataTable;

                _typeToFlag.Add(edt, flag);
                _flagToEntityDataTable.Add(flag, table);
            }
        }

        public T GetData<T>(Entity entity) where T : struct
        {
            return GetDataTable<T>().GetData(entity);
        }

        public EntityDataTypeFlag SetData<T>(Entity entity, T data) where T : struct
        {
            var dt = GetDataTable<T>();
            dt.SetData(entity, data);
            return dt.Flag;
        }

        public EntityDataTypeFlag RemoveData<T>(Entity entity) where T : struct
        {
            var dt = GetDataTable<T>();
            dt.RemoveData(entity);
            return dt.Flag;
        }

        private EntityDataTable<T> GetDataTable<T>() where T : struct
        {
            return (_flagToEntityDataTable[_typeToFlag[typeof(T)]] as EntityDataTable<T>);
        }
    }
}
