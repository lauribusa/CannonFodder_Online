using Assets.Features.Fragments.ScriptableObjectVariables;
using Assets.Features.Interfaces;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations;

namespace Assets.Features.Entities
{
    public enum ItemType
    {
        Shell,
        PowderCharge
    }
    public class Item : NetworkBehaviour, IEquatable<Item>, IPoolItem
    {
        public ItemType itemType;
        public PositionConstraint constraint;
        [SerializeField]
        private bool debug;
        [SerializeField]
        private Rigidbody body;
        [SerializeField]
        private Collider itemCollider;

        public ItemPool allItems;
        public NetworkVariable<bool> isCarried = new();
        public FloatVariableSO weight;

        public NetworkVariable<sbyte> Id = new(-2);

        private void RegisterSelfToItemList()
        {
            if (allItems.Has(this)) return;
            allItems.Add(this);
            if (debug) Debug.Log($"Adding {name} (ID: {Id}) to pool", gameObject);
        }

        private void UnregisterSelfFromItemList()
        {
            if (debug) Debug.Log($"Removing {name} from pool", gameObject);
            allItems.Remove(Id.Value);
        }

        public override void OnNetworkSpawn()
        {
            RegisterSelfToItemList();
            if (body == null && TryGetComponent<Rigidbody>(out var rb))
            {
                body = rb;
            }

            if (itemCollider == null)
            {
                itemCollider = GetComponent<Collider>();
            }
            RegisterSelfToItemList();
            if(IsServer) isCarried.Value = false;
        }

        public override void OnNetworkDespawn()
        {
            UnregisterSelfFromItemList();
        }

        public Item PickUp()
        {
            SetCarriedRpc();
            PickUpRpc();
            return this;
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void PickUpRpc()
        {
            //if (IsServer) return;
            
            itemCollider.isTrigger = true;
            body.isKinematic = true;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        [Rpc(SendTo.Server)]
        public void SetCarriedRpc()
        {
            if (!IsServer) return;
            isCarried.Value = true;
        }

        [Rpc(SendTo.Server)]
        public void SetNotCarriedRpc()
        {
            if (!IsServer) return;
            isCarried.Value = false;
        }

        public Item PutDown()
        {
            SetNotCarriedRpc();
            PutDownRpc();
            return this;
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void PutDownRpc()
        {
            itemCollider.isTrigger = false;
            body.isKinematic = false;
        }

        public bool Equals(Item other)
        {
            if (this != other) return false;
            return true;
        }

        public sbyte GetId()
        {
            return Id.Value;
        }

        public void SetId(sbyte itemId)
        {
            Id.Value = itemId;
        }
    }
}
