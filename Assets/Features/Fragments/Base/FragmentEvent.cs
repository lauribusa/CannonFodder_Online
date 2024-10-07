using System;

namespace Assets.Features.Fragments.Base
{
    public class FragmentEvent
    {
        private event Action @event;

        public void Subscribe(Action action)
        {
            @event += action;
        }

        public void Unsubscribe(Action action)
        {
            @event -= action;
        }

        public void Trigger()
        {
            @event?.Invoke();
        }
    }

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
