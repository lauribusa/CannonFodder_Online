using Assets.Features.Entities;

namespace Assets.Features.Interfaces
{
    public interface ICarriable
    {
        public ICarriable PickUp(PlayerActor player);

        public void PutDown();
    }
}
