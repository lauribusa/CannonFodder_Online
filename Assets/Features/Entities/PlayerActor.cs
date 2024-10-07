using Assets.Features.Fragments.ScriptableObjectVariables;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class PlayerActor : NetworkBehaviour
    {
        [SerializeField]
        private bool debug;
        public ItemListSO carriableItemsInScene;

        public Transform itemAnchorPoint;

        [SerializeField]
        private Item carriedItem;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PerformAction();
            }
        }

        private void PerformAction()
        {
            if (carriedItem == null)
            {
                PerformPickupRpc();
                return;
            }

            PerformPutDownRpc();

        }

        [Rpc(SendTo.ClientsAndHost)]
        private void SetItemParentRpc()
        {
            carriedItem.transform.SetParent(itemAnchorPoint);
            carriedItem = carriedItem.PickUp();
        }

        [Rpc(SendTo.Server)]
        private void PerformPickupRpc()
        {
            if (debug) Debug.Log($"{carriableItemsInScene.GetList().Count} items in scene", gameObject);
            var existingItems = carriableItemsInScene.GetList();
            existingItems = GameHelpers.SortByDistance(existingItems, transform.position);
            foreach (var item in existingItems)
            {
                if (item.isCarried.Value) continue;
                var distance = Vector3.Distance(transform.position, item.transform.position);
                if (debug) Debug.Log($"Distance between {gameObject.name} and {item.name}: {distance} (required: {GameHelpers.DetectionRange})", gameObject);
                if (distance <= GameHelpers.DetectionRange)
                {
                    SetItemParentRpc();
                    if (debug) Debug.Log($"Carrying {item.name}", gameObject);
                    break;
                }
            }
        }

        [Rpc(SendTo.Server)]
        private void PerformPutDownRpc()
        {
            if (debug) Debug.Log($"Putting {carriedItem.name} down.", gameObject);
            carriedItem.transform.SetParent(null);
            carriedItem.PutDown();
            carriedItem = null;
        }
    }
}
