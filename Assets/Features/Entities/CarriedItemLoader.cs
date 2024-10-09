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

        private Rigidbody _rigidbody;

        private void Awake() => _rigidbody = GetComponent<Rigidbody>();

        private void OnGUI()
        {
            if (!GUILayout.Button("Load item")) return;

            LoadItem(_itemTest);
        }

        private void LoadItem(Item item)
        {
            var itemTransform = item.transform;
            itemTransform.SetParent(_anchorPoint);
            itemTransform.SetPositionAndRotation(_anchorPoint);

            item.GetComponent<Rigidbody>().isKinematic = true;

            ItemLoaded?.Invoke(item);
        }
    }
}