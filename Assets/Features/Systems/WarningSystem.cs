using Assets.Features.Fragments.ScriptableObjectEvents;
using Assets.Features.Fragments.ScriptableObjectVariables;
using System.Collections;
using TMPro;
using UnityEngine;

public class WarningSystem : MonoBehaviour
{
    public FloatVariableSO textTimer;
    public TextMeshProUGUI textBox;
    private Coroutine _timer;

    public VoidEventSO onLoadingMustReset;
    public VoidEventSO onShellAlreadyLoaded;
    public VoidEventSO onMustLoadShellFirst;
    public VoidEventSO onScore;

    private void OnEnable()
    {
        onLoadingMustReset.Subscribe(OnLoadingMustReset);
        onShellAlreadyLoaded.Subscribe(OnShellAlreadyLoaded);
        onMustLoadShellFirst.Subscribe(OnMustLoadShellFirst);
        onScore.Subscribe(OnScore);
    }

    private void OnDisable()
    {
        onLoadingMustReset.Unsubscribe(OnLoadingMustReset);
        onShellAlreadyLoaded.Unsubscribe(OnShellAlreadyLoaded);
        onMustLoadShellFirst.Unsubscribe(OnMustLoadShellFirst);
        onScore.Unsubscribe(OnScore);
    }

    private void OnLoadingMustReset()
    {
        DisplayMessage($"Loading car must be reset first!");
    }

    private void OnShellAlreadyLoaded()
    {
        DisplayMessage("A shell is already loaded in the cannon!");
    }

    private void OnMustLoadShellFirst()
    {
        DisplayMessage("You must load a shell first!");
    }

    private void OnScore()
    {
        DisplayMessage("Cannon fired!");
    }

    public void DisplayMessage(string text)
    {
        textBox.SetText(text);
        StartMessageTimer();
    }

    private void StartMessageTimer()
    {
        if(_timer != null)
        {
            StopCoroutine(_timer);
        }
        _timer = StartCoroutine(MessageCoroutine());
    }

    private IEnumerator MessageCoroutine()
    {
        yield return new WaitForSeconds(textTimer.Value);
        textBox.SetText("");
    }
}
