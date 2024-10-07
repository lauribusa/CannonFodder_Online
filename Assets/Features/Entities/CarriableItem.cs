using Assets.Features.Fragments.ComponentVariables;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class CarriableItem: MonoBehaviour
    {
        public BoolVariable isCarried;
        public CarriableItem PickUp()
        {
            isCarried.Value = true;
            return this;
        }

        public CarriableItem PutDown()
        {
            isCarried.Value = false;
            return this;
        }
    }
}
