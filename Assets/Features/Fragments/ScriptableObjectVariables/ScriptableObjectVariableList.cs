using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Features.Fragments.ScriptableObjectVariables
{
    public class ScriptableObjectVariableList<T>: ScriptableObject
    {
        [SerializeField]
        private List<T> objectList = new();

        private event Action<List<T>> OnListUpdated;

        public void Add(T obj)
        {
            objectList.Add(obj);
            OnListUpdated?.Invoke(objectList);
        }

        public void Remove(T obj)
        {
            objectList.Remove(obj);
            OnListUpdated?.Invoke(objectList);
        }

        public void Subscribe(Action<List<T>> onListUpdated)
        {
            OnListUpdated += onListUpdated;
        }

        public void Unsubscribe(Action<List<T>> onListUpdated)
        {
            OnListUpdated -= onListUpdated;
        }

        public List<T> GetList() => new(objectList);
    }
}
