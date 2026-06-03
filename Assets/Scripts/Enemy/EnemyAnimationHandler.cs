using UnityEngine;

public class EnemyAnimationHandler : MonoBehaviour
{
    [SerializeField] private EnemyAnimator animator;

    private PiranhaIA piranha;
    private EnemyBrain siren;

    private void Start()
    {
        piranha = GetComponent<PiranhaIA>();
        siren = GetComponent<EnemyBrain>();

        if (piranha != null)
        {
            piranha.OnAttack += animator.PlayAttack;
           // piranha.OnJumpscare += animator.PlayJumpscare;
        }

        if (siren != null)
        {
            siren.OnAttackTriggered += animator.PlayAttack;
            siren.OnScream += animator.PlayScream;
        }
    }

    private void OnDisable()
    {
        if (piranha != null)
        {
            piranha.OnAttack -= animator.PlayAttack;
            //piranha.OnJumpscare -= animator.PlayJumpscare;
        }

        if (siren != null)
        {
            siren.OnAttackTriggered -= animator.PlayAttack;
            siren.OnScream -= animator.PlayScream;
        }
    }
}