using System;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class CarriedItemLoader : NetworkBehaviour
    {
        public event Action<Item> ItemLoaded;

        [SerializeField] private Transform _anchorPoint;
        public Item _itemTest;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Item item)) return;
            if (!item.isCarried.Value) return;

            Debug.Log($"{item.name} Detected");
        }

        private void OnGUI()
        {
            if (!GUILayout.Button("Load item")) return;

            Load(_itemTest);
        }

        public void Load(Item item)
        {
            var itemTransform = item.transform;
            itemTransform.SetParent(_anchorPoint);
            itemTransform.SetPositionAndRotation(_anchorPoint);

            item.GetComponent<Rigidbody>().isKinematic = true;

            ItemLoaded?.Invoke(item);
        }
    }
}