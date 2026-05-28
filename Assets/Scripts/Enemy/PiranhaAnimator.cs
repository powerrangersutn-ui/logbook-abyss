using UnityEngine;

public class PiranhaAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake() => animator = GetComponent<Animator>();
    public void PlayJumpscare() => animator.SetTrigger("Scream");
    public void PlayAttack() => animator.SetTrigger("Attack");
    public void UpdateSpeed(float speed) => animator.SetFloat("Speed", speed);
}
