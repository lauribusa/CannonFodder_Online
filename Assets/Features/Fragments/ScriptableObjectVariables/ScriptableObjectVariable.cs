using Assets.Features.Fragments.Base;
using Assets.Features.Fragments.Interfaces;
using System;
using UnityEngine;

public class ScriptableObjectVariable<T> : ScriptableObject, IVariable<T>
{
    [SerializeField]
    private bool debug;
    [SerializeField]
    private FragmentVariable<T> fragment = new();

    public T Value { get => fragment.Value; set
        {
            fragment.Value = value;
            if (debug) Debug.Log($"{this.name} value set to {fragment.Value}", this);
        }
    }

    public void Subscribe(Action<T> action)
    {
        fragment.Subscribe(action);
    }

    public void Unsubscribe(Action<T> action)
    {
        fragment.Unsubscribe(action);
    }
}
