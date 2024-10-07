using Assets.Features.Fragments.Base;
using System;
using UnityEngine;

namespace Assets.Features.Fragments.ScriptableObjectEvents
{
    [CreateAssetMenu(menuName = "Events/Void")]
    public class VoidEventSO : ScriptableObject
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
