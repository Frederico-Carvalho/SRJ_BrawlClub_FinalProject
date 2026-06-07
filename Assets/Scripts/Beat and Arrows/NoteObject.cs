using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Unity.Netcode;

public class NoteObject : MonoBehaviour
{
    [Header("PlayerSettings")]
    public bool isPlayer1;

    [Header("InputSettings")]
    public bool canBePressed;
    public Key keytoPress;
    private bool obtained = false;

    [Header("FeedbackEffectsSettings")]
    public GameObject hitEffect, goodEffect, perfectEffect, missEffect;
    public Transform activator;

    public CharacterAnimator characterAnimator;

    void Update()
    {
        if (NetworkManager.Singleton == null) return;
        bool isHost = NetworkManager.Singleton.IsHost;
        if (isPlayer1 != isHost) return;

        KeyControl key = Keyboard.current[keytoPress];
        if (key.wasPressedThisFrame)
        {
            if (canBePressed)
            {
                obtained = true;
                gameObject.SetActive(false);

                float distance = Mathf.Abs(transform.position.y - activator.position.y);
                if (distance > 0.25f)
                {
                    GameManager.instance.NormalHitServerRpc(isHost, transform.position, (int)keytoPress);
                }
                else if (distance > 0.05f)
                {
                    GameManager.instance.GoodHitServerRpc(isHost, transform.position, (int)keytoPress);
                }
                else
                {
                    GameManager.instance.PerfectHitServerRpc(isHost, transform.position, (int)keytoPress);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Activator")
        {
            canBePressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Activator")
        {
            canBePressed = false;
            if (!obtained)
            {
                if (NetworkManager.Singleton == null) return;
                bool isHost = NetworkManager.Singleton.IsHost;
                if (isPlayer1 != isHost) return;
                GameManager.instance.NoteMissedServerRpc(isHost, transform.position);
            }
        }
    }
}