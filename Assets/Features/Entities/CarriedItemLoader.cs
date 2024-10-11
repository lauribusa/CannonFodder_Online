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

        [SerializeField] private VoidEventSO _onLoadindCarMustBeReseted;
        [SerializeField] private VoidEventSO _onShellAlreadyLoaded;
        [SerializeField] private VoidEventSO _onMustLoadShellFirst;

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
            playerItemLoader.ItemUnloadingRequested += UnloadItem;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out PlayerInteraction playerItemLoader)) return;

            playerItemLoader.IsReadyToloadItem = false;
            playerItemLoader.ItemLoadingRequested -= LoadItem;
            playerItemLoader.ItemUnloadingRequested -= UnloadItem;
        }

        public void LoadItem(sbyte itemID) => LoadItemServerRpc(itemID);

        [Rpc(SendTo.Server)]
        private void LoadItemServerRpc(sbyte itemID)
        {
            if (!_isLoadingCarReseted)
            {
                SendWarningOnMustResetLoadingCarClientRpc();
                return;
            }

            _isLoadingCarReseted = false;

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
            for (int i = 0; i < _carriableItemsInScene.Count(); i++)
            {
                Item pooledItem = _carriableItemsInScene.Get(i);
            }

            Item item = _carriableItemsInScene.Get(itemID);
            item.GetComponent<Rigidbody>().isKinematic = true;

            item.transform.localPosition = Vector3.zero;
        }

        public void UnloadItem()
        {
            if (!_itemLoaded) return;

            if (!_isLoadingCarReseted)
            {
                SendWarningOnMustResetLoadingCarClientRpc();
                return;
            }

            UnloadItemServerRpc();
        }

        [Rpc(SendTo.Server)]
        private void UnloadItemServerRpc()
        {
            _itemLoaded.transform.SetParent(null);

            _itemLoaded.GetComponent<Rigidbody>().isKinematic = false;
            _itemLoaded = null;
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void UnloadItemClientRpc() => _itemLoaded.GetComponent<Rigidbody>().isKinematic = false;

        private void OnLoadingCarCompleted()
        {
            if (!_itemLoaded) return;
            
            if (IsShell && _isBulletLoaded)
            {
                SendWarningShellAlreadyLoadedClientRpc();
                return;
            }

            if (IsPowderCharge && !_isBulletLoaded)
            {
                SendWarningMustLoadShellFirstClientRpc();
                return;
            }

            if(IsShell) _isBulletLoaded = true;

            if (IsPowderCharge) _BulletLoadedInCannon.Trigger(true);

            DestroyItemServerRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SendWarningOnMustResetLoadingCarClientRpc()
        {
            Debug.Log("<color=orange>You must reset the loading bullet car</color>");
            _onLoadindCarMustBeReseted.Trigger();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SendWarningShellAlreadyLoadedClientRpc()
        {
            Debug.Log("<color=orange>A shell is already loaded</color>");
            _onLoadindCarMustBeReseted.Trigger();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SendWarningMustLoadShellFirstClientRpc()
        {
            Debug.Log("<color=orange>You must load a shell first</color>");
            _onLoadindCarMustBeReseted.Trigger();
        }

        [Rpc(SendTo.Server)]
        private void DestroyItemServerRpc() => _itemLoaded.GetComponent<NetworkObject>().Despawn();

        private void OnCannonFired() => _isBulletLoaded = false;

        private void OnLoadingCarReseted() => _isLoadingCarReseted = true;
    }
}