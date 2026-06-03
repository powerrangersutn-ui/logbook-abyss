using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;
    private void Awake() => animator = GetComponentInChildren<Animator>();

    public void PlayAttack() => animator.SetTrigger("attack");
    public void PlayScream() => animator.SetTrigger("Scream");
    //public void PlayJumpscare() => animator.SetTrigger("Jumpscare");

    public void UpdateSpeed(float speed, float maxSpeed)
    {
        float norm = Mathf.Clamp01(speed / maxSpeed);
        animator.SetFloat("speed", norm, 0.1f, Time.deltaTime);
    }
}