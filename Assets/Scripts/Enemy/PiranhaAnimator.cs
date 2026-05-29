using UnityEngine;

public class PiranhaAnimator : MonoBehaviour
{
    private Animator animator;
    public void PlayJumpscare() => animator.SetTrigger("Scream");
    public void PlayAttack()
    {
        Debug.Log("¡PiranhaAnimator recibió la orden de atacar!");

        if (animator != null)
        {
            animator.SetTrigger("attack");
        }
        else
        {
            Debug.LogError("¡No hay Animator asignado o encontrado en los hijos!");
        }
    }
    public void UpdateSpeed(float speed) => animator.SetFloat("Speed", speed);

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
}
