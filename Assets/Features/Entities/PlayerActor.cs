using Assets.Features.Fragments.ScriptableObjectVariables;
using System.Globalization;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class PlayerActor : NetworkBehaviour
    {
        [SerializeField]
        private bool debug;
        public ItemPool carriableItemsInScene;

        public Transform itemAnchorPoint;

        private NetworkVariable<int> carriedItemId = new();

        private Item carriedItem => carriableItemsInScene.Get(carriedItemId.Value);
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PerformAction();
            }
        }

        private void PerformAction()
        {
            if (carriedItemId.Value == -1)
            {
                PerformPickupRpc();
                return;
            }

            PerformPutDownRpc();

        }

        public override void OnNetworkSpawn()
        {
            carriedItemId.OnValueChanged += OnCarriedItemIdUpdate;
            carriedItemId.Value = -1;
        }

        public override void OnNetworkDespawn()
        {
            carriedItemId.OnValueChanged -= OnCarriedItemIdUpdate;
        }

        [Rpc(SendTo.Server)]
        private void PerformPickupRpc()
        {
            if (debug) Debug.Log($"{carriableItemsInScene.Count()} items in scene", gameObject);
            var existingItems = carriableItemsInScene.ToList();
            if (debug) Debug.Log($"{existingItems.Count} : {transform.position}");
            existingItems = GameHelpers.SortByDistance(existingItems, transform.position);
            foreach (var item in existingItems)
            {
                if (item.isCarried.Value) continue;
                var distance = Vector3.Distance(transform.position, item.transform.position);
                if (debug) Debug.Log($"Distance between {gameObject.name} and {item.name}: {distance} (required: {GameHelpers.DetectionRange})", gameObject);
                if (distance <= GameHelpers.DetectionRange)
                {
                    carriedItemId.Value = item.PickUp().Id;
                    item.transform.SetParent(itemAnchorPoint);
                    if (debug) Debug.Log($"Carrying {item.name}", gameObject);
                    break;
                }
            }
        }

        private void OnCarriedItemIdUpdate(int prev, int next)
        {
            if(next <0 )
            {
                if (debug) Debug.Log($"Carried item is null (id {next})", gameObject);
                return;
            }
            if (debug) Debug.Log($"Carried item is not null (id {next})", gameObject);
        }

        [Rpc(SendTo.Server)]
        private void PerformPutDownRpc()
        {
            if (debug) Debug.Log($"Putting {carriedItem.name} down.", gameObject);
            
            carriedItemId.Value = -1;
        }
    }
}
