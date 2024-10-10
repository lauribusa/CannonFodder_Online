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

        private void Update()
        {
            //if (!Input.GetKeyDown(KeyCode.L)) return;
            //if (!_itemTest.isCarried.Value) return;

            //LoadItem(_itemTest);
        }

        private void OnGUI()
        {
            if (!GUILayout.Button("Load item")) return;

            LoadItem(_itemTest);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerItemLoader playerItemLoader)) return;

            playerItemLoader.IsReadyToloadItem = true;
            Debug.Log($"Player enter trigger - ready to load: {playerItemLoader.IsReadyToloadItem}");
            playerItemLoader.ItemLoadingRequested += OnPlayerItemLoadingRequested;

            //if (!other.TryGetComponent(out Item item)) return;
            //if (!item.isCarried.Value) return;
            //Debug.Log("carried item trigger stay");

            //LoadItem(item);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out PlayerItemLoader playerItemLoader)) return;

            playerItemLoader.IsReadyToloadItem = false;
            Debug.Log($"Player exit trigger - ready to load: {playerItemLoader.IsReadyToloadItem}");
            playerItemLoader.ItemLoadingRequested -= OnPlayerItemLoadingRequested;
        }

        private void OnPlayerItemLoadingRequested(Item item)
        {
            Debug.Log($"try loading: {item.name}");
            LoadItem(item);
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
