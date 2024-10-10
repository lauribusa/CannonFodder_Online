using Assets.Features.Fragments.ComponentEvents;
using Assets.Features.Fragments.ComponentVariables;
using Assets.Features.Fragments.ScriptableObjectVariables;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations;

namespace Assets.Features.Entities
{
    public class PlayerNetworkClient : NetworkBehaviour
    {
        [SerializeField]
        private Transform playerHand;
        private ConstraintSource _handConstraint;
        [SerializeField]
        private PlayerNetworkServer server;
        [SerializeField]
        private bool debug;
        public ItemPool carriableItemsInScene;

        public Transform itemAnchorPoint;

        [SerializeField]
        private NetworkVariable<sbyte> carriedItemId = new(writePerm: NetworkVariableWritePermission.Server);

        public Item carriedItem => carriableItemsInScene.Get(carriedItemId.Value);

        public IntVariable carriedItem_Id;

        #region Events
        public IntEvent onSetItemParent;
        #endregion

        private void Awake()
        {
            _handConstraint = new ConstraintSource { sourceTransform = playerHand, weight = 1 };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PerformAction();
            }
        }

        private void PerformAction()
        {
            if (carriedItemId.Value < 0)
            {
                PerformPickup();
                return;
            }

            PerformPutDownRpc();
        }

        public override void OnNetworkSpawn()
        {
            carriedItemId.OnValueChanged += OnCarriedItemIdUpdate;
            if (IsServer) carriedItemId.Value = -1;
        }

        public override void OnNetworkDespawn()
        {
            if (carriedItemId.Value >= 0)
            {
                PerformPutDownRpc();
            }
            carriedItemId.OnValueChanged -= OnCarriedItemIdUpdate;
        }

        private void PerformPickup()
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
                    if (!IsLocalPlayer) return;
                    var id = item.Id;
                    SetCarriedItemRpc(id);
                    item.PickUp();
                    server.SetItemParentServerSideRpc(id);
                    if (debug) Debug.Log($"LOCAL FUNC: Setting item parent {id}({item.name}) for {gameObject.name}", gameObject);
                    break;
                }
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SetItemParentRpc(sbyte id)
        {
            if (IsLocalPlayer) Debug.Log($"LOCAL PLAYER: SET_ITEM_PARENT_RPC FROM {gameObject.name}: {id}", gameObject);
            if (IsOwner) Debug.Log($"OWNER: SET_ITEM_PARENT_RPC FROM {gameObject.name}: {id}", gameObject);
            if (IsServer) Debug.Log($"SERVER: SET_ITEM_PARENT_RPC FROM {gameObject.name}: {id}", gameObject);
            if (!IsLocalPlayer) return;
            Debug.Log($"TO CLIENT: Setting item parent {id} for {gameObject.name}", gameObject);
            var item = carriableItemsInScene.Get(id);
            if (item == null) return;
            item.constraint.AddSource(_handConstraint);
            item.constraint.constraintActive = true;
        }

        private void OnCarriedItemIdUpdate(sbyte prev, sbyte next)
        {
            carriedItem_Id.Value = next;
            if (next < 0)
            {
                if (debug) Debug.Log($"Carried item is null (id {next})", gameObject);
                return;
            }
            if (debug) Debug.Log($"Carried item is not null (id {next})", gameObject);
        }

        [Rpc(SendTo.Server)]
        public void SetCarriedItemRpc(sbyte id)
        {
            carriedItemId.Value = id;
            var item = carriableItemsInScene.Get(id);
        }

        [Rpc(SendTo.Server)]
        public void PerformPutDownRpc()
        {
            if (debug) Debug.Log($"Putting {carriedItem.name} ({carriedItemId.Value}) down. Performed by {gameObject.name}", gameObject);
            carriedItem.PutDown();
            carriedItem.constraint.constraintActive = false;
            carriedItem.constraint.RemoveSource(0);
            carriedItemId.Value = -1;
        }

    }
}
