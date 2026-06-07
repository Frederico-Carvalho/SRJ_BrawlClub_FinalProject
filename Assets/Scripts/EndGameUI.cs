using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class EndGameUI : MonoBehaviour
{
    public void GoToMainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
    }
}