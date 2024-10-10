using Assets.Features.Fragments.ScriptableObjectVariables;
using TMPro;
using UnityEngine;

namespace Assets.Features.Systems
{
    public class GameUIManager: MonoBehaviour
    {
        public TextMeshProUGUI timer;
        public FloatVariableSO time;
        private void OnEnable()
        {
            time.Subscribe(OnTimeChanged);
        }

        private void OnDisable()
        {
            time.Unsubscribe(OnTimeChanged);
        }

        public void OnTimeChanged(float time)
        {
            timer.SetText(time.ToString());
        }
    }
}
