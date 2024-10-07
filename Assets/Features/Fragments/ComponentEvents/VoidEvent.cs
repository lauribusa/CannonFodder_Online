using Assets.Features.Fragments.Base;
using System;
using UnityEngine;

namespace Assets.Features.Fragments.ComponentEvents
{
    public class VoidEvent: MonoBehaviour
    {
        private FragmentEvent _event = new();

        public void Subscribe(Action action)
        {
            _event.Subscribe(action);
        }

        public void Trigger()
        {
            _event.Trigger();
        }

        public void Unsubscribe(Action action)
        {
            _event.Unsubscribe(action);
        }
    }
}
