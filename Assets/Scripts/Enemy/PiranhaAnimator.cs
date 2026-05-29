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
    public void UpdateSpeed(float speed, float maxSpeed)
    {
        float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);

        animator.SetFloat("speed", normalizedSpeed, 0.1f, Time.deltaTime);
    }
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
}
