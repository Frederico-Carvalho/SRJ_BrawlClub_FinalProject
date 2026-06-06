using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("MusicSettings")]
    public AudioSource theMusic;
    public bool startPlaying;
    public BeatScroller theBS;

    [Header("GameManagerInstance")]
    public static GameManager instance;

    [Header("ScoreSettings")]
    public int currentScore;
    public int scorePerNote = 50;
    public int scorePerGoodNote = 100;
    public int scorePerPerfectNote = 150;

    [Header("MultiplierSettings")]
    public int currentMultiplier;
    public int multipierTracker;
    public int[] multiplierThresholds;

    [Header("UI Settings")]
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI MultiText;

    [Header("SpawnSettings")]
    public NoteSpawner theNoteSpawner;

    public float totalNotes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;

        ScoreText.text = "Score: 0";
        currentMultiplier = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(!startPlaying)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                startPlaying = true;
                theBS.hasStarted = true;
                if (Keyboard.current.anyKey.wasPressedThisFrame)
                {
                    startPlaying = true;
                    theBS.hasStarted = true;
                    theMusic.Play();
                    theNoteSpawner.StartSpawning();
                }

                theMusic.Play();
            }
        }
    }

    public void NoteHit()
    {
        Debug.Log("hit on time");

        if (currentMultiplier - 1 < multiplierThresholds.Length)
        {
            multipierTracker++;

            if (multiplierThresholds[currentMultiplier - 1] <= multipierTracker)
            {
                multipierTracker = 0;
                currentMultiplier++; 
            }
        }

        MultiText.text = "Multiplier: x" + currentMultiplier;

        //currentScore += scorePerNote * currentMultiplier;
        ScoreText.text = "Score: " + currentScore;
    }

    public void NormalHit()
    { 
        currentScore += scorePerNote * currentMultiplier;
        NoteHit();
    }
    public void GoodHit()
    {
        currentScore += scorePerGoodNote * currentMultiplier;
        NoteHit();
    }
    public void PerfectHit()
    {
        currentScore += scorePerPerfectNote * currentMultiplier;
        NoteHit();
    }

    public void NoteMissed()
    {
        Debug.Log("Missed");

        currentMultiplier = 1;
        multipierTracker = 0;
        MultiText.text = "Multiplier: x" + currentMultiplier;
    }
}
