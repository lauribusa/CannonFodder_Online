using Assets.Features.Fragments.ComponentVariables;
using Assets.Features.Interfaces;
using UnityEngine;

namespace Assets.Features.Entities
{
    public class CarriableItem: MonoBehaviour, ICarriable
    {
        public BoolVariable isCarried;
        public ICarriable PickUp(PlayerActor player)
        {
            transform.SetParent(player.transform);
            isCarried.Value = true;
            return this;
        }

        public void PutDown()
        {
            transform.SetParent(null);
            isCarried.Value = false;
        }
    }
}
