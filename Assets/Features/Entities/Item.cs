using Assets.Features.Fragments.ScriptableObjectVariables;
using Assets.Features.Interfaces;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class Item : NetworkBehaviour, IEquatable<Item>, IPoolItem
    {
        [SerializeField]
        private bool debug;
        [SerializeField]
        private Rigidbody body;
        [SerializeField]
        private Collider itemCollider;

        public ItemPool allItems;
        public NetworkVariable<bool> isCarried;
        public FloatVariableSO weight;

        public int Id { get; set; }

        private void OnEnable()
        {
            RegisterSelfToItemList();
        }

        private void OnDisable()
        {
            UnregisterSelfFromItemList();
        }

        private void RegisterSelfToItemList()
        {
            if (allItems.Has(this)) return;
            if (debug) Debug.Log($"Adding {name} to pool", gameObject);
            allItems.Add(this);
        }

        private void UnregisterSelfFromItemList()
        {
            if (debug) Debug.Log($"Removing {name} from pool", gameObject);
            allItems.Remove(Id);
        }

        public override void OnNetworkSpawn()
        {
            if (body == null && TryGetComponent<Rigidbody>(out var rb))
            {
                body = rb;
            }

            if (itemCollider == null)
            {
                itemCollider = GetComponent<Collider>();
            }
            RegisterSelfToItemList();
            isCarried.Value = false;
        }

        public override void OnNetworkDespawn()
        {
            UnregisterSelfFromItemList();
        }

        public Item PickUp()
        {
            itemCollider.enabled = false;
            body.isKinematic = true;
            isCarried.Value = true;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            return this;
        }

        public Item PutDown()
        {
            itemCollider.enabled = true;
            body.isKinematic = false;
            isCarried.Value = false;
            return this;
        }

        public bool Equals(Item other)
        {
            if (this != other) return false;
            return true;
        }
    }
}
