using Assets.Features.Fragments.Base;
using Assets.Features.Fragments.Interfaces;
using System;
using UnityEngine;

public class ScriptableObjectEvent<T> : ScriptableObject, IEvent<T>
{
    private FragmentEvent<T> _event = new();

    public void Subscribe(Action<T> action)
    {
        _event.Subscribe(action);
    }

    public void Trigger(T value)
    {
        _event.Trigger(value);
    }

    public void Unsubscribe(Action<T> action)
    {
        _event.Unsubscribe(action);
    }
}
