using UnityEngine;

public class BoxAnimationHandler : MonoBehaviour
{
    private Animator animator;
    private BoxPickup boxPickup;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        boxPickup = GetComponentInParent<BoxPickup>();
    }

    private void OnEnable()
    {
        if (boxPickup != null)
        {
            boxPickup.OnOpen += PlayOpenAnimation;
        }
    }

    private void OnDisable()
    {
        if (boxPickup != null)
        {
            boxPickup.OnOpen -= PlayOpenAnimation;
        }
    }

    private void PlayOpenAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }
    }
}