using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;

    [Header("Animation Names")]
    public string idleAnim = "Idle";
    public string hitLeftAnim = "Left";
    public string hitRightAnim = "Right";
    public string hitUpAnim = "Up";
    public string hitDownAnim = "Down";
    public string missAnim = "Missed";

    void Start()
    {
        animator = GetComponent<Animator>();
        PlayIdle();
    }

    public void PlayIdle()
    {
        animator.Play(idleAnim);
    }

    public void PlayHit(Key key)
    {
        switch (key)
        {
            case Key.LeftArrow:
                animator.Play(hitLeftAnim);
                break;
            case Key.DownArrow:
                animator.Play(hitDownAnim);
                break;
            case Key.UpArrow:
                animator.Play(hitUpAnim);
                break;
            case Key.RightArrow:
                animator.Play(hitRightAnim);
                break;
        }
    }

    public void PlayMiss()
    {
        animator.Play(missAnim);
    }
}