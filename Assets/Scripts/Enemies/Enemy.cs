using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : BaseAnimationController
{
    private enum EnemyState { Idle, Wander, Taunt, Chase, Dead }
    private EnemyState currentState = EnemyState.Idle;

    [Header("Components")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private SaveableEntity saveableEntity;

    [Header("Base Settings")]
    [SerializeField] private float maxHealth;
    private float currentHealth;

    [Header("AI Settings")]
    [SerializeField] private float idleDuration = 2f;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float wanderSpeed = 1.5f;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private LayerMask targetLayers;

    private Transform target;
    private Vector3 wanderDest;
    private float stateTimer = 0f;
    private float attackTimer = 0f;

    private bool isTaunting = false;


    public override void Awake()
    {
        base.Awake();

        EnsureComponents();
        saveableEntity?.GenerateID();
        
        // TriggerRandomIdle();
        // StartCoroutine(CycleIdleAnimations());
    }

    private void EnsureComponents()
    {
        if (!agent) { agent = GetComponent<NavMeshAgent>(); }
        if (!saveableEntity) { saveableEntity = GetComponent<SaveableEntity>(); }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (!agent) { agent = GetComponent<NavMeshAgent>(); }
    }

    public void Init(Transform _target)
    {
        target = _target;
        agent.speed = wanderSpeed;
        agent.stoppingDistance = 0.5f;
        agent.updateRotation = true;
    }

    private void Update()
    {
        bool targetDetected = IsTargetWithinDetectionRadius();

        if (currentState != EnemyState.Dead)
        {
            stateTimer += Time.deltaTime;
            attackTimer += Time.deltaTime;
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                base.UpdateMovement(0f);

                if (stateTimer >= idleDuration)
                {
                    currentState = EnemyState.Wander;
                    stateTimer = 0f;
                    ChooseNewWanderDestination();
                }

                break;
            case EnemyState.Wander:
                base.UpdateMovement(0.5f);

                agent.speed = wanderSpeed;
                agent.SetDestination(wanderDest);

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = EnemyState.Idle;
                    stateTimer = 0f;
                }

                break;
            case EnemyState.Taunt:
                base.UpdateMovement(0f);

                agent.ResetPath();
                //agent.SetDestination(target.transform.position);
                FaceTarget();

                break;
            case EnemyState.Chase:
                if (target)
                {
                    //FaceTarget();

                    if (!targetDetected)
                    {
                        currentState = EnemyState.Idle;
                        stateTimer = 0f;
                    }
                    else if (Vector3.Distance(transform.position, target.position) > agent.stoppingDistance)
                    {
                        base.UpdateMovement(1.0f);
                        agent.speed = chaseSpeed;
                        agent.SetDestination(target.position);
                    }
                    else
                    {
                        base.UpdateMovement(0f);

                        if (attackTimer >= attackCooldown)
                        {
                            base.TriggerRandomAttack();
                            attackTimer = 0f;
                        }
                    }
                }
                break;
            case EnemyState.Dead:
                base.UpdateMovement(0f);

                stateTimer = 0f;

                break;
        }

        if (targetDetected && (currentState == EnemyState.Idle || currentState == EnemyState.Wander) && !isTaunting)
        {
            stateTimer = 0f;
            isTaunting = true;
            currentState = EnemyState.Taunt;
            StartCoroutine(TauntPlayer());
        }
    }

    private void FaceTarget()
    {
        if (!target) { return; }
        Vector3 dir = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0f, dir.z), Vector3.up);
    }

    private void ChooseNewWanderDestination()
    {
        // Try up to 10 times to find a new spot.
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                wanderDest = hit.position;
                return;
            }
        }

        // Fallback: stay idle and eventually try again.
        currentState = EnemyState.Idle;
    }

    private bool IsTargetWithinDetectionRadius()
    {
        if (!target) { return false; }

        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = (target.position - transform.position).normalized;

        if (Physics.SphereCast(origin, detectionRadius, direction, out RaycastHit hit, detectionRadius, targetLayers))
        {
            return hit.transform == target;
        }

        return false;
    }

    private IEnumerator TauntPlayer()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Taunt");
    }

    public void EndTaunt()
    {
        Debug.LogError("THIS IS BEING CALLED WOOO");
        currentState = EnemyState.Chase;
        stateTimer = 0f;
        isTaunting = false;
    }

    public void TakeDamage(float _damage)
    {
        currentHealth -= _damage;
        if (currentHealth <= 0f)
        {
            base.TriggerRandomDeath();
            currentState = EnemyState.Dead;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

    #if UNITY_EDITOR
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, $"State: {currentState}");
    #endif
    }
}
