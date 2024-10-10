using Assets.Features.Fragments.ScriptableObjectEvents;
using Assets.Features.Fragments.ScriptableObjectVariables;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class CarriedItemLoader : NetworkBehaviour
    {
        [SerializeField] ItemPool _carriableItemsInScene;
        [SerializeField] private Transform _anchorPoint;

        [SerializeField] private VoidEventSO _onLoadingCarCompleted;
        [SerializeField] private BoolEventSO _BulletLoadedInCannon;

        private Item _itemLoaded;

        private void OnEnable() => _onLoadingCarCompleted.Subscribe(OnLoadingCarCompleted);

        private void OnDisable() => _onLoadingCarCompleted.Unsubscribe(OnLoadingCarCompleted);

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerInteraction playerItemLoader)) return;

            playerItemLoader.IsReadyToloadItem = true;
            playerItemLoader.ItemLoadingRequested += LoadItem;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out PlayerInteraction playerItemLoader)) return;

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

        private void OnLoadingCarCompleted()
        {
            if (!_itemLoaded) return;

            if (true) //TODO: Check for bullet or powder
            {
                _BulletLoadedInCannon.Trigger(true);
            }
            
            DestroyItemServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void DestroyItemServerRpc() => _itemLoaded.GetComponent<NetworkObject>().Despawn();
    }
}