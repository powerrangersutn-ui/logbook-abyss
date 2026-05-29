using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    [SerializeField] private PiranhaIA piranhaAI;
    [SerializeField] private PiranhaAnimator piranhaAnimator; 

    private void OnEnable()
    {
        piranhaAI.OnJumpscare += piranhaAnimator.PlayJumpscare;
        piranhaAI.OnAttack += () => {
            Debug.Log("Evento OnAttack detectado en el puente");
            piranhaAnimator.PlayAttack();
        };
    }

    private void OnDisable()
    {
        piranhaAI.OnJumpscare -= piranhaAnimator.PlayJumpscare;
        piranhaAI.OnAttack -= piranhaAnimator.PlayAttack;
    }

}