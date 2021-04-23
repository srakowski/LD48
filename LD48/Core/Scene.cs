using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LD48.Core
{
    enum SceneState
    {
        Unset = 0,
        Loading,
        Active,
        Unloading,
        Inactive
    }

    class Scene
    {
        private readonly List<GameSystem> _gameSystems;
        private readonly List<Entity> _entities;
        private readonly Dictionary<Type, EntityDataTypeFlag> _typeToFlag;
        private readonly Dictionary<EntityDataTypeFlag, EntityDataTable> _flagToEntityDataTable;

        public Scene()
        {
            _gameSystems = new();
            _entities = new();
            _typeToFlag = new();
            _flagToEntityDataTable = new();
            PopulateEntityDataTables();
        }

        public SceneState State { get; private set; }

        public void BeginLoad()
        {
            State = SceneState.Loading;
        }

        public void BeginUnload()
        {
            State = SceneState.Unloading;
        }

        public Scene AddSystem<T>() where T : GameSystem
        {
            var gameSystem = Activator.CreateInstance(typeof(T)) as GameSystem;
            return AddSystem(gameSystem);
        }

        public Scene AddSystem(GameSystem gameSystem)
        {
            _gameSystems.Add(gameSystem);
            return this;
        }

        public Scene AddEntity(EntityBuilder entityBuilder)
        {
            var idx = _entities.Count;
            var entity = new Entity(idx);
            _entities.Add(entity);

            foreach (dynamic ed in entityBuilder.EntityData)
            {
                SetData(entity, ed);
            }

            return this;
        }

        private void PopulateEntityDataTables()
        {
            var entityDataTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsValueType && t.GetInterfaces().Contains(typeof(IEntityData)))
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

        private T GetData<T>(Entity entity) where T : struct, IEntityData
        {
            return GetDataTable<T>().GetData(entity);
        }

        private void SetData<T>(Entity entity, T data) where T : struct, IEntityData
        {
            var changedEntity = GetDataTable<T>().SetData(entity, data);
            _entities[entity.Id] = changedEntity;
            foreach (var gs in _gameSystems)
            {
                gs.ProcessEntityChange(changedEntity);
            }
        }

        private void RemoveData<T>(Entity entity) where T : struct, IEntityData
        {
            var changedEntity = GetDataTable<T>().RemoveData(entity);
            foreach (var gs in _gameSystems)
            {
                gs.ProcessEntityChange(changedEntity);
            }
        }

        private EntityDataTable<T> GetDataTable<T>() where T : struct, IEntityData
        {
            return (_flagToEntityDataTable[_typeToFlag[typeof(T)]] as EntityDataTable<T>);
        }

        public static Scene operator +(Scene scene, EntityBuilder eb) => scene.AddEntity(eb);
    }
}
