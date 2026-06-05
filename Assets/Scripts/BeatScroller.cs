using UnityEngine;
using UnityEngine.InputSystem;

public class BeatScroller : MonoBehaviour
{
    [Header("BPMSettings")]
    public float beatTempo;
    public bool hasStarted;

    void Start()
    {
        beatTempo = beatTempo / 60f;
    }

    void Update()
    {
        if (!hasStarted)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                hasStarted = true;
            }
        }
        else
        {
            transform.position += new Vector3(0f, beatTempo * Time.deltaTime, 0f);
        }
    }
}