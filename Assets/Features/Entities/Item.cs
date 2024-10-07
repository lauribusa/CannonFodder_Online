using Assets.Features.Fragments.ComponentVariables;
using Assets.Features.Fragments.ScriptableObjectVariables;
using UnityEngine;

namespace Assets.Features.Entities
{
    [RequireComponent(typeof(Collider))]
    public class Item : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody body;
        [SerializeField]
        private Collider itemCollider;

        public ItemListSO allItems;
        public BoolVariable isCarried;
        public FloatVariableSO floatVariableSO;


        private void OnEnable()
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
        }

        private void OnDisable()
        {
            UnregisterSelfFromItemList();
        }

        private void RegisterSelfToItemList()
        {
            allItems.Add(this);
        }

        private void UnregisterSelfFromItemList()
        {
            allItems.Remove(this);
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
    }
}
