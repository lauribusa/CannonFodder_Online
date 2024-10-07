using Assets.Features.Fragments.ScriptableObjectVariables;
using System.Linq;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class PlayerActor : MonoBehaviour
    {
        public CarriableItemListSO carriableItemsInScene;

        public Transform itemAnchorPoint;

        [SerializeField]
        private CarriableItem carriedItem;
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
            var existingItems = carriableItemsInScene.GetList();
            existingItems = GameHelpers.SortByDistance(existingItems, transform.position);
            foreach (var item in existingItems)
            {
                if (item.isCarried.Value) continue;
                var distance = Vector3.Distance(transform.position, item.transform.position);

                if (distance <= GameHelpers.DetectionRange)
                {
                    carriedItem = item.PickUp();
                    break;
                }
            }
        }

        private void PerformPutDown()
        {
            carriedItem.PutDown();
            carriedItem = null;
        }
    }
}
