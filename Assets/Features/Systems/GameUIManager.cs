using Assets.Features.Fragments.ScriptableObjectEvents;
using TMPro;
using UnityEngine;

namespace Assets.Features.Systems
{
    public class GameUIManager: MonoBehaviour
    {
        public TextMeshProUGUI timer;
        public VoidEventSO onNetworkSpawned;
        public VoidEventSO onNetworkDespawned;

        

        private void OnEnable()
        {
            onNetworkSpawned.Subscribe(SubscribeAction);
            onNetworkDespawned.Subscribe(UnsubscribeAction);
        }

        private void OnDisable()
        {
            onNetworkDespawned.Unsubscribe(UnsubscribeAction);
            onNetworkSpawned.Unsubscribe(SubscribeAction);
        }

        public void UnsubscribeAction()
        {
           // NetworkSingleton.Instance.Time.OnValueChanged -= OnTimerUpdate;
        }

        public void SubscribeAction()
        {
           // NetworkSingleton.Instance.Time.OnValueChanged += OnTimerUpdate;
            onNetworkSpawned.Unsubscribe(SubscribeAction);
        }

        public void OnTimerUpdate(int prev, int next)
        {
            timer.text = next.ToString();
        }
    }
}
