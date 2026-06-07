using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : MonoBehaviour
{
    [Header("UI")]
    public Button hostButton;
    public Button joinButton;
    public Button confirmButton;
    public TMP_InputField ipInputField;

    [Header("Scene")]
    public string gameSceneName = "OnlineTest";

    void Start()
    {
        ipInputField.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);

        hostButton.onClick.AddListener(StartHost);
        joinButton.onClick.AddListener(ShowJoinUI);
        confirmButton.onClick.AddListener(StartClient);
    }

    void StartHost()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>()
       .SetConnectionData("0.0.0.0", 7777);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    void ShowJoinUI()
    {
        ipInputField.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(false);
    }

    void StartClient()
    {
        string ip = ipInputField.text;

        if (string.IsNullOrEmpty(ip))
            ip = "127.0.0.1";

        NetworkManager.Singleton.GetComponent<UnityTransport>()
            .SetConnectionData(ip, 7777);

        NetworkManager.Singleton.StartClient();
    }
}