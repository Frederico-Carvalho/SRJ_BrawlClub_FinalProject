using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ButtonController : MonoBehaviour
{
    [Header("ArrowSettings")]
    private SpriteRenderer spriteRenderer;
    public Sprite defaultImage;
    public Sprite pressedImage;

    public Key KeyToPress;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        KeyControl key = Keyboard.current[KeyToPress];

        if (key.wasPressedThisFrame)
        {
            spriteRenderer.sprite = pressedImage;
        }
        else if (key.wasReleasedThisFrame)
        {
            spriteRenderer.sprite = defaultImage;
        }
    }
}
