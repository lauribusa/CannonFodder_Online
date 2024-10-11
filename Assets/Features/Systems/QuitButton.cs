using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    public Button button;
    public void QuitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        button.interactable = !NetworkManager.Singleton.IsConnectedClient;
    }
}
