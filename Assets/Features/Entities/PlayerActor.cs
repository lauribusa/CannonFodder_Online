using Assets.Features.Fragments.ScriptableObjectVariables;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class PlayerActor : MonoBehaviour
    {
        public CarriableItemListSO carriableItemsInScene;
        
        [SerializeField] 
        private CarriableItem carriedItem;
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
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

        private void PerformPickup()
        {
            var existingItems = carriableItemsInScene.GetList();
            foreach (var item in existingItems)
            {
                if (item.isCarried.Value) continue;
                var distance = Vector3.Distance(transform.position, item.transform.position);

                // change this to check which item is closest to player with OrderBy.
                if (distance <= GameSettings.DetectionRange)
                {
                    carriedItem = (CarriableItem)item.PickUp(this);
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
