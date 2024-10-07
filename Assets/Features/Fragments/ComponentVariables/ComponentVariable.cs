using Assets.Features.Fragments.Base;
using Assets.Features.Fragments.Interfaces;
using System;
using UnityEngine;

public class ComponentVariable<T> : MonoBehaviour, IVariable<T>
{
    [SerializeField]
    private bool debug;
    [SerializeField]
    private FragmentVariable<T> fragment = new();

    public T Value {
        get { return fragment.Value; }
        set {
            fragment.Value = value; 
            if (debug) Debug.Log($"{gameObject.name} value set to {fragment.Value}", gameObject);
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
