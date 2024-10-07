using Assets.Features.Fragments.Base;
using System;

namespace Assets.Features.Fragments.Interfaces
{
    public interface IVariable<T>
    {
        public T Value { get; set; }

        public void Subscribe(Action<T> action);

        public void Unsubscribe(Action<T> action);
    }
}
