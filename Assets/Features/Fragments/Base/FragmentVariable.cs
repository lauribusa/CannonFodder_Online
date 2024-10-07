using System;

namespace Assets.Features.Fragments.Base
{
    public class FragmentVariable<T>
    {
        private T _value;

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnValueChanged?.Invoke(value);
            }
        }

        private event Action<T> OnValueChanged;

        public void Subscribe(Action<T> action)
        {
            OnValueChanged += action;
        }

        public void Unsubscribe(Action<T> action)
        {
            OnValueChanged -= action;
        }
    }
}
