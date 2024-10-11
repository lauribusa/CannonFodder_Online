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
        [SerializeField] private VoidEventSO _onLoadingCarReseted;
        [SerializeField] private BoolEventSO _BulletLoadedInCannon;
        [SerializeField] private VoidEventSO _onCannonFired;

        private Item _itemLoaded;
        private bool _isLoadingCarReseted = true;
        private bool _isBulletLoaded;

        private bool IsPowderCharge => _itemLoaded.itemType == ItemType.PowderCharge;
        private bool IsShell => _itemLoaded.itemType == ItemType.Shell;

        private void OnEnable()
        {
            _onLoadingCarCompleted.Subscribe(OnLoadingCarCompleted);
            _onLoadingCarReseted.Subscribe(OnLoadingCarReseted);
            _onCannonFired.Subscribe(OnCannonFired);
        }

        private void OnDisable()
        {
            _onLoadingCarCompleted.Unsubscribe(OnLoadingCarCompleted);
            _onLoadingCarReseted.Unsubscribe(OnLoadingCarReseted);
            _onCannonFired.Unsubscribe(OnCannonFired);
        }

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

        public void LoadItem(sbyte itemID) => LoadItemServerRpc(itemID);

        [Rpc(SendTo.Server)]
        private void LoadItemServerRpc(sbyte itemID)
        {
            Debug.Log($"<color=yellow>Item loaded ID: {itemID}</color>");

            if (!_isLoadingCarReseted)
            {
                Debug.Log("<color=orange>You must reset the loading bullet car</color>");
                return;
            }

            _isLoadingCarReseted = false;

            Item item = _carriableItemsInScene.Get(itemID);
            if (_itemLoaded || !item) return;

            _itemLoaded = item;

            Debug.Log($"<color=green>SERVER: {item.name} ID: {item.Id.Value} was kinematic: {item.GetComponent<Rigidbody>().isKinematic}</color>");
            item.GetComponent<Rigidbody>().isKinematic = true;
            Debug.Log($"<color=green>SERVER: {item.name} ID: {item.Id.Value} is now kinematic: {item.GetComponent<Rigidbody>().isKinematic}</color>");

            var itemTransform = item.transform;
            itemTransform.SetParent(_anchorPoint);
            itemTransform.SetPositionAndRotation(_anchorPoint);

            LoadItemClientRpc(itemID);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void LoadItemClientRpc(sbyte itemID)
        {
            for (int i = 0; i < _carriableItemsInScene.Count(); i++)
            {
                Item pooledItem = _carriableItemsInScene.Get(i);
                Debug.Log($"<color=white>{i} -> CLIENT: {pooledItem.name} has ID: {pooledItem.Id.Value}</color>");
            }

            Item item = _carriableItemsInScene.Get(itemID);
            item.GetComponent<Rigidbody>().isKinematic = true;
            Debug.Log($"<color=cyan>ID {itemID} received - item in client: {item.name}</color>");
            Debug.Log($"<color=cyan>CLIENT: {item.name} ID: {item.Id.Value} is kinematic: {item.GetComponent<Rigidbody>().isKinematic}</color>");

            item.transform.localPosition = Vector3.zero;
        }

        private void OnLoadingCarCompleted()
        {
            if (!_itemLoaded) return;
            
            if (IsShell && _isBulletLoaded)
            {
                Debug.Log("<color=orange>A shell is already loaded</color>");
                return;
            }

            if (IsPowderCharge && !_isBulletLoaded)
            {
                Debug.Log("<color=orange>You must load a shell first</color>");
                return;
            }

            if(IsShell) _isBulletLoaded = true;

            if (IsPowderCharge) _BulletLoadedInCannon.Trigger(true);

            DestroyItemServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void DestroyItemServerRpc() => _itemLoaded.GetComponent<NetworkObject>().Despawn();

        private void OnCannonFired() => _isBulletLoaded = false;

        private void OnLoadingCarReseted() => _isLoadingCarReseted = true;
    }
}