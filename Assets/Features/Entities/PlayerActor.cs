using Assets.Features.Fragments.ScriptableObjectVariables;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Features.Entities
{
    public class PlayerActor : MonoBehaviour
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
                PerformPickup();
                return;
            }

            PerformPutDown();

        }

        private void SetItem()
        {
            carriedItem.transform.SetParent(itemAnchorPoint);
            carriedItem.transform.localPosition = Vector3.zero;
        }

        private void PerformPickup()
        {
            if (debug) Debug.Log($"{carriableItemsInScene.GetList().Count} items in scene", gameObject);
            var existingItems = carriableItemsInScene.GetList();
            existingItems = GameHelpers.SortByDistance(existingItems, transform.position);
            foreach (var item in existingItems)
            {
                if (item.isCarried.Value) continue;
                var distance = Vector3.Distance(transform.position, item.transform.position);
                if (debug) Debug.Log($"Distance between player and {item.name}: {distance} (required: {GameHelpers.DetectionRange})", gameObject);
                if (distance <= GameHelpers.DetectionRange)
                {
                    carriedItem = item.PickUp();
                    carriedItem.transform.SetParent(itemAnchorPoint);
                    carriedItem.transform.localPosition = Vector3.zero;
                    
                    if(debug) Debug.Log($"Carrying {item.name}", gameObject);
                    break;
                }
            }
        }

        private void PerformPutDown()
        {
            if (debug) Debug.Log($"Putting {carriedItem.name} down.", gameObject);
            carriedItem.transform.SetParent(null);
            carriedItem.PutDown();
            carriedItem = null;
        }
    }
}
