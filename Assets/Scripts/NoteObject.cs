using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class NoteObject : MonoBehaviour
{
    [Header("InputSettings")]
    public bool canBePressed;
    public Key keytoPress;
    private bool obtained = false;

    [Header("FeedbackEffectsSettings")]
    public GameObject hitEffect, goodEffect, perfectEffect, missEffect;
    public Transform activator;

    private CharacterAnimator characterAnimator;

    void Start()
    {
        characterAnimator = Object.FindFirstObjectByType<CharacterAnimator>();
    }

    void Update()
    {
        KeyControl key = Keyboard.current[keytoPress];
        if (key.wasPressedThisFrame)
        {
            if (canBePressed)
            {
                obtained = true;
                gameObject.SetActive(false);

                characterAnimator.PlayHit(keytoPress);

                float distance = Mathf.Abs(transform.position.y - activator.position.y);
                if (distance > 0.25f)
                {
                    Debug.Log("hit");
                    GameManager.instance.NormalHit();
                    Instantiate(hitEffect, transform.position, hitEffect.transform.rotation);
                    Debug.Log("Distance: " + distance);
                }
                else if (distance > 0.05f)
                {
                    Debug.Log("good hit");
                    GameManager.instance.GoodHit();
                    Instantiate(goodEffect, transform.position, goodEffect.transform.rotation);
                    Debug.Log("Distance: " + distance);
                }
                else
                {
                    Debug.Log("Perfect");
                    GameManager.instance.PerfectHit();
                    Instantiate(perfectEffect, transform.position, perfectEffect.transform.rotation);
                    Debug.Log("Distance: " + distance);
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
                GameManager.instance.NoteMissed();
                characterAnimator.PlayMiss();
                Instantiate(missEffect, transform.position, missEffect.transform.rotation);
            }
        }
    }
}