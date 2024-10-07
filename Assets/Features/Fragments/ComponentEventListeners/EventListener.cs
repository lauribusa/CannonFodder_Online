using UnityEngine;
using UnityEngine.Events;

public class EventListener<T> : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<T> unityEvent = new();

    public void Subscribe(UnityAction<T> action)
    {
        unityEvent.AddListener(action);
    }

    public void Unsubscribe(UnityAction<T> action)
    {
        unityEvent.RemoveListener(action);
    }

    public void Trigger(T value)
    {
        unityEvent.Invoke(value);
    }
}
