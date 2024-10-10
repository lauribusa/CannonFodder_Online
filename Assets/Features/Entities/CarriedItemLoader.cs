using Assets.Features.Fragments.ScriptableObjectVariables;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class CarriedItemLoader : NetworkBehaviour
    {
        [SerializeField] ItemPool _carriableItemsInScene;
        [SerializeField] private Transform _anchorPoint;

        private Item _itemLoaded;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerItemLoader playerItemLoader)) return;

            playerItemLoader.IsReadyToloadItem = true;
            playerItemLoader.ItemLoadingRequested += LoadItem;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out PlayerItemLoader playerItemLoader)) return;

            playerItemLoader.IsReadyToloadItem = false;
            playerItemLoader.ItemLoadingRequested -= LoadItem;
        }

        public void LoadItem(Item item) => LoadItemServerRpc(item.Id);

        [Rpc(SendTo.Server)]
        private void LoadItemServerRpc(sbyte itemID)
        {
            Item item = _carriableItemsInScene.Get(itemID);
            if (_itemLoaded || !item) return;

            _itemLoaded = item;
            item.GetComponent<Rigidbody>().isKinematic = true;

            var itemTransform = item.transform;
            itemTransform.SetParent(_anchorPoint);
            itemTransform.SetPositionAndRotation(_anchorPoint);

            LoadItemClientRpc(itemID);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void LoadItemClientRpc(sbyte itemID)
        {
            Item item = _carriableItemsInScene.Get(itemID);

            item.GetComponent<Rigidbody>().isKinematic = true;

            item.transform.localPosition = Vector3.zero;
        }
    }
}