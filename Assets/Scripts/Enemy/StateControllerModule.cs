// StateControllerModule.cs - Controlador de estados y prioridades
using UnityEngine;

public class StateControllerModule : EnemyModule
{
    public enum AIState
    {
        Idle,
        Patrol,
        Detect,
        Chase,
        Attack,
        Retreat,
        CallForHelp
    }

    [Header("State Settings")]
    [SerializeField] private AIState currentState = AIState.Patrol;
    [SerializeField] private bool debugState = false;

    private AIState previousState;

    // Referencias a módulos
    private PatrolModule patrolModule;
    private ChaseModule chaseModule;
    private MeleeAttackModule attackModule;
    private RetreatModule retreatModule;
    private GroupBehaviorModule groupModule;

    protected override void OnInitialize()
    {
        patrolModule = core.GetModule<PatrolModule>();
        chaseModule = core.GetModule<ChaseModule>();
        attackModule = core.GetModule<MeleeAttackModule>();
        retreatModule = core.GetModule<RetreatModule>();
        groupModule = core.GetModule<GroupBehaviorModule>();

        ChangeState(currentState);
    }

    public override void OnUpdate()
    {
        if (!isActive) return;

        // Evaluar transiciones de estado
        EvaluateStateTransitions();

        if (debugState && currentState != previousState)
        {
            Debug.Log($"{gameObject.name}: {previousState} -> {currentState}");
            previousState = currentState;
        }
    }

    private void EvaluateStateTransitions()
    {
        // Prioridad 1: Retirada (si está en peligro)
        if (retreatModule != null && retreatModule.IsRetreating)
        {
            if (currentState != AIState.Retreat)
            {
                ChangeState(AIState.Retreat);
            }
            return;
        }

        // Prioridad 2: Pedir ayuda (si está solo y detecta enemigo)
        if (groupModule != null && !sharedData.hasCalledForHelp &&
            sharedData.HasTarget && sharedData.nearbyAlliesCount == 0)
        {
            if (currentState != AIState.CallForHelp)
            {
                ChangeState(AIState.CallForHelp);
                groupModule.CallForHelp();
                sharedData.hasCalledForHelp = true;
            }
        }

        // Si no hay objetivo, patrullar o estar idle
        if (!sharedData.HasTarget)
        {
            sharedData.hasCalledForHelp = false;

            if (currentState != AIState.Patrol && currentState != AIState.Idle)
            {
                ChangeState(patrolModule != null ? AIState.Patrol : AIState.Idle);
            }
            return;
        }

        // Tenemos objetivo - decidir entre Attack y Chase
        if (attackModule != null && attackModule.IsInAttackRange())
        {
            if (currentState != AIState.Attack)
            {
                ChangeState(AIState.Attack);
            }
        }
        else
        {
            // Fuera de rango - perseguir
            if (currentState != AIState.Chase && currentState != AIState.Detect)
            {
                ChangeState(AIState.Chase);
            }
        }
    }

    private void ChangeState(AIState newState)
    {
        // Desactivar módulos del estado anterior
        DisableModulesForState(currentState);

        previousState = currentState;
        currentState = newState;

        // Activar módulos del nuevo estado
        EnableModulesForState(newState);
    }

    private void DisableModulesForState(AIState state)
    {
        switch (state)
        {
            case AIState.Patrol:
                if (patrolModule != null) patrolModule.enabled = false;
                break;
            case AIState.Chase:
                if (chaseModule != null) chaseModule.enabled = false;
                break;
        }
    }

    private void EnableModulesForState(AIState state)
    {
        switch (state)
        {
            case AIState.Idle:
                // No activar ningún módulo de movimiento
                break;

            case AIState.Patrol:
                if (patrolModule != null) patrolModule.enabled = true;
                break;

            case AIState.Detect:
                // Solo observar, sin movimiento especial
                break;

            case AIState.Chase:
                if (chaseModule != null) chaseModule.enabled = true;
                break;

            case AIState.Attack:
                // El attackModule siempre está activo, maneja su propia lógica
                break;

            case AIState.Retreat:
                // El retreatModule maneja su propia activación
                break;

            case AIState.CallForHelp:
                // Acción instantánea, volverá a Chase o Attack
                break;
        }
    }

    public AIState GetCurrentState()
    {
        return currentState;
    }

    public void ForceState(AIState newState)
    {
        ChangeState(newState);
    }
}