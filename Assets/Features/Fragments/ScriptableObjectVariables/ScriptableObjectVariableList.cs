using System.Collections.Generic;
using UnityEngine;

namespace Assets.Features.Fragments.ScriptableObjectVariables
{
    public class ScriptableObjectVariableList<T>: ScriptableObject
    {
        [SerializeField]
        private List<T> objectList = new();

        public void Add(T obj)
        {
            objectList.Add(obj);
        }

        public void Remove(T obj)
        {
            objectList.Remove(obj);
        }

        public List<T> GetList() => new(objectList);
    }
}
