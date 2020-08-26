using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenSage.Logic.Object
{
    public sealed class GameObjectCollection : DisposableBase
    {
        private readonly GameContext _gameContext;
        private readonly Dictionary<int, GameObject> _items;
        private readonly Dictionary<string, GameObject> _nameLookup;
        private readonly List<int> _destroyList;
        private readonly Player _civilianPlayer;
        private readonly Navigation.Navigation _navigation;
        private int _nextObjectId;

        public IEnumerable<GameObject> Items => _items.Values;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        internal GameObjectCollection(
            GameContext gameContext,
            Player civilianPlayer,
            Navigation.Navigation navigation)
        {
            _gameContext = gameContext;
            _items = new Dictionary<int, GameObject>();
            _nameLookup = new Dictionary<string, GameObject>();
            _destroyList = new List<int>();
            _civilianPlayer = civilianPlayer;
            _navigation = navigation;
            _nextObjectId = 0;
        }

        public GameObject Add(string typeName, Player player)
        {
            var definition = _gameContext.AssetLoadContext.AssetStore.ObjectDefinitions.GetByName(typeName);

            if (definition == null)
            {
                Logger.Warn($"Skipping unknown GameObject \"{typeName}\"");
                return null;
            }

            return Add(definition, player);
        }

        public GameObject Add(string typeName)
        {
            return Add(typeName, _civilianPlayer);
        }

        public GameObject Add(ObjectDefinition objectDefinition, Player player)
        {
            var gameObject = AddDisposable(new GameObject(objectDefinition, _gameContext, player, this));

            _items.Add(_nextObjectId, gameObject);
            _nextObjectId++;

            _gameContext.Radar.AddGameObject(gameObject);

            return gameObject;
        }

        public GameObject Add(ObjectDefinition objectDefinition)
        {
            return Add(objectDefinition, _civilianPlayer);
        }

        // TODO: This is probably not how real SAGE works.
        public int GetObjectId(GameObject gameObject)
        {
            return _items.FirstOrDefault(x => x.Value == gameObject).Key;
        }

        public List<int> GetObjectIds(IEnumerable<GameObject> gameObjects)
        {
            var objIds = new List<int>();
            foreach (var gameObject in gameObjects)
            {
                objIds.Add(GetObjectId(gameObject));
            }

            return objIds;
        }

        public GameObject GetObjectById(int objectId)
        {
            return _items[objectId];
        }

        public bool TryGetObjectByName(string name, out GameObject gameObject)
        {
            return _nameLookup.TryGetValue(name, out gameObject);
        }

        public List<GameObject> GetObjectsByKindOf(ObjectKinds kindOf)
        {
            var result = new List<GameObject>();
            foreach (var match in _items.Where(x => x.Value.Definition.KindOf.Get(kindOf)))
            {
                result.Add(match.Value);
            }
            return result;
        }

        public void AddNameLookup(GameObject gameObject)
        {
            _nameLookup[gameObject.Name ?? throw new ArgumentException("Cannot add lookup for unnamed object.")] = gameObject;
        }

        public void DeleteDestroyed()
        {
            _destroyList.Clear();
            foreach (var item in _items)
            {
                if (item.Value.Destroyed == true)
                {
                    _gameContext.Radar.RemoveGameObject(item.Value);
                    _destroyList.Add(item.Key);
                }
            }

            foreach (var objectId in _destroyList)
            {
                _items.Remove(objectId);
            }
        }
    }
}
