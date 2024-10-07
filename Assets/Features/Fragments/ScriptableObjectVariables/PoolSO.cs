using Assets.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Features.Fragments.ScriptableObjectVariables
{
    public class PoolSO<T>: ScriptableObject where T : IPoolItem
    {
        [SerializeField]
        private int maxId;

        private List<int> freeIds = new();

        private Dictionary<int, T> lookups = new();
        private List<T> list = new();

        private event Action<Dictionary<int, T>> OnPoolUpdated;

        public void Add(T obj)
        {
            list.Add(obj);
            if(freeIds.Count > 0)
            {
                var freedId = freeIds.First();
                freeIds.Remove(freedId);
                AddToLookups(freedId, obj);
                return;
            }
            AddToLookups(maxId, obj);
            maxId++;
        }

        private void AddToLookups(int id, T obj)
        {
            lookups.Add(id, obj);
            OnPoolUpdated?.Invoke(lookups);
        }

        public void Remove(int id)
        {
            if(!lookups.ContainsKey(id)) return;
            var obj = lookups[id];
            list.Remove(obj);
            lookups.Remove(id);
            freeIds.Add(id);
            OnPoolUpdated?.Invoke(lookups);
        }

        public void Subscribe(Action<Dictionary<int, T>> onListUpdated)
        {
            OnPoolUpdated += onListUpdated;
        }

        public void Unsubscribe(Action<Dictionary<int, T>> onListUpdated)
        {
            OnPoolUpdated -= onListUpdated;
        }

        public Dictionary<int, T> GetPool()
        {
            return new(lookups);
        }

        public T Get(int id)
        {
            return lookups[id];
        }

        public bool Has(int id)
        {
            return lookups.ContainsKey(id);
        }

        public void Set(int id, T obj)
        {
            lookups[id] = obj;
        }
    }
}
