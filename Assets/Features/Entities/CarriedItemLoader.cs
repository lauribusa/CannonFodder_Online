using Assets.Features.Fragments.ScriptableObjectVariables;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class CarriedItemLoader : NetworkBehaviour
    {
        [SerializeField] ItemPool _carriableItemsInScene;
        [SerializeField] private Transform _anchorPoint;

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
            if (!item) return;

            var itemTransform = item.transform;
            itemTransform.SetParent(_anchorPoint);
            itemTransform.SetPositionAndRotation(_anchorPoint);

            item.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}