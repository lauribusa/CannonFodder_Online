using System;

namespace Assets.Features.Fragments.Base
{
    public class FragmentEvent<T>
    {
        private event Action<T> @event;

        public void Subscribe(Action<T> action)
        {
            @event += action;
        }

        public void Unsubscribe(Action<T> action)
        {
            @event -= action;
        }

        public void Trigger(T value)
        {
            @event?.Invoke(value);
        }
    }
}
