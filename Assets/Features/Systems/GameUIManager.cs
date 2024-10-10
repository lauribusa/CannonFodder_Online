using Assets.Features.Fragments.ScriptableObjectVariables;
using TMPro;
using UnityEngine;

namespace Assets.Features.Systems
{
    public class GameUIManager: MonoBehaviour
    {
        public TextMeshProUGUI text;
        public FloatVariableSO value;
        private void OnEnable()
        {
            value.Subscribe(OnValueChanged);
        }

        private void OnDisable()
        {
            value.Unsubscribe(OnValueChanged);
        }

        public void OnValueChanged(float val)
        {
            text.SetText(val.ToString());
        }
    }
}
