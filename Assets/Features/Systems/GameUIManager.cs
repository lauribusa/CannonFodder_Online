using TMPro;
using UnityEngine;

namespace Assets.Features.Systems
{
    public class GameUIManager: MonoBehaviour
    {
        public TextMeshProUGUI timer;

        public void Update()
        {
            timer.text = NetworkSingleton.Instance.Time.Value.ToString();
        }

        private void OnEnable()
        {
            NetworkSingleton.Instance.Time.OnValueChanged += OnTimerUpdate;
        }

        private void OnDisable()
        {
            NetworkSingleton.Instance.Time.OnValueChanged -= OnTimerUpdate;
        }

        public void OnTimerUpdate(int prev, int next)
        {
            timer.text = next.ToString();
        }
    }
}
