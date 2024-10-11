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

        //private Dictionary<int, T> lookups = new();
        [SerializeField]
        private List<T> list = new();

        //private event Action<Dictionary<int, T>> OnPoolUpdated;
        private event Action<List<T>> OnListUpdated;


        private void OnEnable()
        {
            maxId = 0;
            //lookups.Clear();
            list.Clear();
        }

        public void Add(T obj)
        {
            list.Add(obj);
            if(freeIds.Count > 0)
            {
                var freedId = freeIds.First();
                freeIds.Remove(freedId);
                CheckForDuplicateAndAdd(freedId, obj);
                return;
            }
            CheckForDuplicateAndAdd(maxId, obj);
            maxId++;
        }

        private void CheckForDuplicateAndAdd(int id, T obj)
        {
            if (Has(id))
            {
                maxId++;
                CheckForDuplicateAndAdd(maxId, obj);
                return;
            }
            AddToLookups(id, obj);
        }

        private void AddToLookups(int id, T obj)
        {
            //lookups.Add(id, obj);
            Debug.Log($"{obj.GetType().Name} with id {id}");
            obj.SetId((sbyte)id);
            //OnPoolUpdated?.Invoke(lookups);
            OnListUpdated?.Invoke(list);
        }

        public void Remove(int id)
        {
            //if(!lookups.ContainsKey(id)) return;
            var obj = list.FirstOrDefault(x => id == x.GetId());
            if (obj == null) return;
            //var obj = lookups[id];
            list.Remove(obj);
            //lookups.Remove(id);
            freeIds.Add(id);
            //OnPoolUpdated?.Invoke(lookups);
            OnListUpdated?.Invoke(list);
        }

        public void Remove(sbyte id)
        {
            Remove((int)id);
        }

        //public void Subscribe(Action<Dictionary<int, T>> onListUpdated)
        //{
        //    OnPoolUpdated += onListUpdated;
        //}

        //public void Unsubscribe(Action<Dictionary<int, T>> onListUpdated)
        //{
        //    OnPoolUpdated -= onListUpdated;
        //}

        //public Dictionary<int, T> GetPool()
        //{
        //    return new(lookups);
        //}

        public T Get(int id)
        {
            if (id < 0) return default;
            return list.First(x => x.GetId() == (sbyte)id);
            //return lookups[id];
        }

        public bool Has(int id)
        {
            return Get(id) != null;
            //return lookups.ContainsKey(id);
        }

        public bool Has(T obj)
        {
            return list.Contains(obj);
        }

        public int Count()
        {
            return list.Count;
        }

        public List<T> ToList()
        {
            return new(list);
        }
    }
}
