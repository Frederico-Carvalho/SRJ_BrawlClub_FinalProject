using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine.InputSystem.Controls;

public class GameManager : NetworkBehaviour
{
    [Header("MusicSettings")]
    public AudioSource theMusic;
    public bool startPlaying;
    public BeatScroller theBS;

    [Header("GameManagerInstance")]
    public static GameManager instance;

    [Header("ScoreSettings")]
    public int scorePerNote = 50;
    public int scorePerGoodNote = 100;
    public int scorePerPerfectNote = 150;

    [Header("MultiplierSettings")]
    public int currentMultiplier;
    public int multipierTracker;
    public int[] multiplierThresholds;

    [Header("UI Settings")]
    public TextMeshProUGUI ScoreTextP1;
    public TextMeshProUGUI MultiTextP1;
    public TextMeshProUGUI ScoreTextP2;
    public TextMeshProUGUI MultiTextP2;

    [Header("SpawnSettings")]
    public NoteSpawner theNoteSpawner;
    public NoteSpawner theNoteSpawnerP2;
    public float totalNotes;

    [Header("AnimatorSettings")]
    public CharacterAnimator characterAnimatorP1;
    public CharacterAnimator characterAnimatorP2;

    [Header("EffectSettings")]
    public GameObject hitEffectP1;
    public GameObject goodEffectP1;
    public GameObject perfectEffectP1;
    public GameObject missEffectP1;
    public GameObject hitEffectP2;
    public GameObject goodEffectP2;
    public GameObject perfectEffectP2;
    public GameObject missEffectP2;

    [Header("EndGame Settings")]
    public GameObject endGamePanel;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI finalScoreTextP1;
    public TextMeshProUGUI finalScoreTextP2;
    public TextMeshProUGUI winnerText;

    //Networked Variables
    private NetworkVariable<int> scoreP1 = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> scoreP2 = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> multiplierP1 = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> multiplierP2 = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> multipierTrackerP1 = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> multipierTrackerP2 = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> playersReady = new NetworkVariable<int>(0);

    private bool playerReady = false;

    void Start()
    {
        instance = this;
        currentMultiplier = 1;
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        ScoreTextP1.text = "Score: 0";
        ScoreTextP2.text = "Score: 0";
        MultiTextP1.text = "Multiplier: x1";
        MultiTextP2.text = "Multiplier: x1";

        scoreP1.OnValueChanged += (oldVal, newVal) =>
            ScoreTextP1.text = "Score: " + newVal;

        scoreP2.OnValueChanged += (oldVal, newVal) =>
            ScoreTextP2.text = "Score: " + newVal;

        multiplierP1.OnValueChanged += (oldVal, newVal) =>
            MultiTextP1.text = "Multiplier: x" + newVal;

        multiplierP2.OnValueChanged += (oldVal, newVal) =>
            MultiTextP2.text = "Multiplier: x" + newVal;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    void OnClientConnected(ulong clientId)
    {
        Debug.Log("Cliente conectado: " + clientId);
    }

    void Update()
    {
        if (!startPlaying && !playerReady)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                playerReady = true;
                PlayerReadyServerRpc();
            }
        }

        if (startPlaying && !theMusic.isPlaying)
        {
            startPlaying = false;
            ShowEndGameClientRpc(scoreP1.Value, scoreP2.Value);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void PlayerReadyServerRpc()
    {
        playersReady.Value++;
        Debug.Log("Jogadores prontos: " + playersReady.Value);
        if (playersReady.Value >= 2)
        {
            StartGameClientRpc();
        }
    }

    [ClientRpc]
    void StartGameClientRpc()
    {
        startPlaying = true;
        theBS.hasStarted = true;
        theMusic.Play();
        theNoteSpawner.StartSpawning();
        theNoteSpawnerP2.StartSpawning();
        Debug.Log("Jogo começou!");
    }

    [ClientRpc]
    public void PlayHitAnimationClientRpc(int keyIndex, bool isPlayer1Anim)
    {
        Key key = (Key)keyIndex;
        if (isPlayer1Anim)
            characterAnimatorP1.PlayHit(key);
        else
            characterAnimatorP2.PlayHit(key);
    }

    [ClientRpc]
    public void PlayMissAnimationClientRpc(bool isPlayer1Anim)
    {
        if (isPlayer1Anim)
            characterAnimatorP1.PlayMiss();
        else
            characterAnimatorP2.PlayMiss();
    }

    [ClientRpc]
    public void SpawnEffectClientRpc(Vector3 position, int effectType, bool isPlayer1Side)
    {
        GameObject effect = null;

        if (isPlayer1Side)
        {
            switch (effectType)
            {
                case 0: effect = hitEffectP1; break;
                case 1: effect = goodEffectP1; break;
                case 2: effect = perfectEffectP1; break;
                case 3: effect = missEffectP1; break;
            }
        }
        else
        {
            switch (effectType)
            {
                case 0: effect = hitEffectP2; break;
                case 1: effect = goodEffectP2; break;
                case 2: effect = perfectEffectP2; break;
                case 3: effect = missEffectP2; break;
            }
        }

        if (effect != null)
            Instantiate(effect, position, Quaternion.identity);
    }

    [ClientRpc]
    public void ShowEndGameClientRpc(int finalScoreP1, int finalScoreP2)
    {
        endGamePanel.SetActive(true);
        finalScoreTextP1.text = "Player 1 Score: " + finalScoreP1;
        finalScoreTextP2.text = "Player 2 Score: " + finalScoreP2;

        if (finalScoreP1 > finalScoreP2)
            winnerText.text = "Player 1 Wins!";
        else if (finalScoreP2 > finalScoreP1)
            winnerText.text = "Player 2 Wins!";
        else
            winnerText.text = "It's a Draw!";
    }

    public void NoteHit(bool isHost)
    {
        if (isHost)
        {
            if (multiplierP1.Value - 1 < multiplierThresholds.Length)
            {
                multipierTrackerP1.Value++;
                if (multiplierThresholds[multiplierP1.Value - 1] <= multipierTrackerP1.Value)
                {
                    multipierTrackerP1.Value = 0;
                    multiplierP1.Value++;
                }
            }
        }
        else
        {
            if (multiplierP2.Value - 1 < multiplierThresholds.Length)
            {
                multipierTrackerP2.Value++;
                if (multiplierThresholds[multiplierP2.Value - 1] <= multipierTrackerP2.Value)
                {
                    multipierTrackerP2.Value = 0;
                    multiplierP2.Value++;
                }
            }
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void NormalHitServerRpc(bool isHost, Vector3 position, int keyIndex)
    {
        if (isHost) scoreP1.Value += scorePerNote * multiplierP1.Value;
        else scoreP2.Value += scorePerNote * multiplierP2.Value;
        NoteHit(isHost);
        PlayHitAnimationClientRpc(keyIndex, isHost);
        SpawnEffectClientRpc(position, 0, isHost);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void GoodHitServerRpc(bool isHost, Vector3 position, int keyIndex)
    {
        if (isHost) scoreP1.Value += scorePerGoodNote * multiplierP1.Value;
        else scoreP2.Value += scorePerGoodNote * multiplierP2.Value;
        NoteHit(isHost);
        PlayHitAnimationClientRpc(keyIndex, isHost);
        SpawnEffectClientRpc(position, 1, isHost);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void PerfectHitServerRpc(bool isHost, Vector3 position, int keyIndex)
    {
        if (isHost) scoreP1.Value += scorePerPerfectNote * multiplierP1.Value;
        else scoreP2.Value += scorePerPerfectNote * multiplierP2.Value;
        NoteHit(isHost);
        PlayHitAnimationClientRpc(keyIndex, isHost);
        SpawnEffectClientRpc(position, 2, isHost);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void NoteMissedServerRpc(bool isHost, Vector3 position)
    {
        if (isHost)
        {
            multiplierP1.Value = 1;
            multipierTrackerP1.Value = 0;
        }
        else
        {
            multiplierP2.Value = 1;
            multipierTrackerP2.Value = 0;
        }
        PlayMissAnimationClientRpc(isHost);
        SpawnEffectClientRpc(position, 3, isHost);
    }
}