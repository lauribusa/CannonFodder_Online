using System;

namespace Assets.Features.Fragments.Interfaces
{
    public interface IEvent<T>
    {
        public void Subscribe(Action<T> action);

        public void Unsubscribe(Action<T> action);

        public void Trigger(T value);
    }
}
