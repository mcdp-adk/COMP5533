using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

public class EnemyAI : MonoBehaviour, ICharacter
{
    [Header("AI Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float sightAngle = 60f;    // 扇形视野角度
    [SerializeField] private float sightDistance = 10f;  // 视野距离
    [SerializeField] private float detectionRadius = 5f; // 四周检测半径
    [SerializeField] private float angularSpeed = 120f;  // 转向速度
    [SerializeField] private float acceleration = 8f;    // 加速度
    [SerializeField] private float attackDistance = 1f;  // 攻击距离
    [SerializeField] public Transform[] patrolPoints;    // 巡逻路径点
    [SerializeField] private Animator animator;
    [SerializeField] private float attackDelay = 0.8f; // 攻击延迟时间
    [SerializeField] private LayerMask playerLayer; // 玩家层
    [SerializeField] private AudioSource attackAudioSource; // 攻击音效
    [SerializeField] private MusicManager musicManager;
    public float health = 100f; // 敌人生命值

    private NavMeshAgent agent;
    private Transform[] players;
    private Transform targetPlayer;
    private Vector3 lastKnownPosition;
    private int currentPatrolIndex;

    // 用于跟踪是否有敌人处于追击或攻击状态的静态变量
    private static int chasingEnemiesCount = 0;

    // AI状态枚举
    private enum AIState { Patrol, Chase, Investigate, Attack, Dead }
    private AIState currentState = AIState.Patrol;

    public event System.Action<ICharacter> OnCharacterDeath;
    public event System.Action OnHealthChanged;
    public event System.Action OnHealthIncreased;
    public event System.Action OnHealthDecreased;

    void Start()
    {
        musicManager = FindObjectOfType<MusicManager>();
        if (musicManager == null)
        {
            Debug.LogError("MusicManager 未找到！");
        }
        agent = GetComponent<NavMeshAgent>();
        currentPatrolIndex = 0;
        agent.speed = patrolSpeed;
        agent.angularSpeed = angularSpeed;
        agent.acceleration = acceleration;
        SetNextPatrolPoint();
    }

    void Update()
    {
        if (health <= 0)
        {
            currentState = AIState.Dead;
        }

        players = GameObject.FindGameObjectsWithTag("Player").Select(go => go.transform).ToArray();
        targetPlayer = GetClosestPlayer();

        AIState previousState = currentState;

        switch (currentState)
        {
            case AIState.Patrol:
                PatrolBehavior();
                break;
            case AIState.Chase:
                ChaseBehavior();
                break;
            case AIState.Investigate:
                InvestigateBehavior();
                break;
            case AIState.Attack:
                AttackBehavior();
                break;
            case AIState.Dead:
                DeadBehavior();
                break;
        }

        // 更新 chasingEnemiesCount
        if ((previousState == AIState.Chase || previousState == AIState.Attack || previousState == AIState.Investigate) &&
            (currentState != AIState.Chase && currentState != AIState.Attack && currentState != AIState.Investigate))
        {
            chasingEnemiesCount--;
        }
        else if ((previousState != AIState.Chase && previousState != AIState.Attack && previousState != AIState.Investigate) &&
                 (currentState == AIState.Chase || currentState == AIState.Attack || currentState == AIState.Investigate))
        {
            chasingEnemiesCount++;
        }
        else if (chasingEnemiesCount < 0)
        {
            chasingEnemiesCount = 0;
        }

        // 更新 MusicManager 中的 isChasing 变量
        musicManager.isChasing = chasingEnemiesCount > 0;
    }

    private Transform GetClosestPlayer()
    {
        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (var player in players)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        return closestPlayer;
    }

    // 巡逻逻辑
    private void PatrolBehavior()
    {
        if (patrolPoints.Length <= 1)
        {
            Debug.LogWarning("巡逻点数量不足，无法进行巡逻");
            return;
        }

        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            SetNextPatrolPoint();
        }

        if (IsPlayerInSight() || IsPlayerInDetectionRadius())
        {
            currentState = AIState.Chase;
            animator.CrossFade("Running", 0.1f);
            agent.speed = chaseSpeed;
        }
    }

    // 追逐逻辑
    private void ChaseBehavior()
    {
        if (targetPlayer == null)
        {
            currentState = AIState.Patrol;
            animator.CrossFade("Walking", 0.1f);
            agent.speed = patrolSpeed;
            SetNextPatrolPoint();
            return;
        }

        agent.SetDestination(targetPlayer.position);
        lastKnownPosition = targetPlayer.position;

        if (Vector3.Distance(transform.position, targetPlayer.position) < attackDistance)
        {
            currentState = AIState.Attack;
            animator.CrossFade("Standing Melee Attack Downward", 0.1f);
            StartCoroutine(PerformAttack());
            PlayAttackSound();
        }
        else if (!IsPlayerInSight() && !IsPlayerInDetectionRadius())
        {
            currentState = AIState.Investigate;
            agent.SetDestination(lastKnownPosition);
        }
    }

    // 攻击逻辑
    private void AttackBehavior()
    {
        if (Vector3.Distance(transform.position, targetPlayer.position) > attackDistance)
        {
            currentState = AIState.Chase;
            animator.CrossFade("Running", 0.1f);
            return;
        }
    }

    // 执行攻击判定的协程
    private IEnumerator PerformAttack()
    {
        yield return new WaitForSeconds(attackDelay);
        currentState = AIState.Chase;
        animator.CrossFade("Running", 0.1f);
    }

    // 攻击命中判定
    public void EnemyAttackHit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackDistance, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerController playerController = hitCollider.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    // playerController.Die();
                    playerController.CauseDamage(20);
                }
            }
        }
    }

    // 调查最后位置逻辑
    private void InvestigateBehavior()
    {
        if (agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            if (IsPlayerInSight() || IsPlayerInDetectionRadius())
            {
                currentState = AIState.Chase;
            }
            else
            {
                currentState = AIState.Patrol;
                animator.CrossFade("Walking", 0.1f);
                agent.speed = patrolSpeed;
                SetNextPatrolPoint();
            }
        }
    }

    // 扇形视野检测
    private bool IsPlayerInSight()
    {
        if (targetPlayer == null) return false;

        Vector3 directionToPlayer = targetPlayer.position - transform.position;
        float angle = Vector3.Angle(directionToPlayer, transform.forward);
        float distance = directionToPlayer.magnitude;

        if (angle < sightAngle / 2 && distance < sightDistance)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 四周检测半径
    private bool IsPlayerInDetectionRadius()
    {
        if (targetPlayer == null) return false;

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);
        return distanceToPlayer < detectionRadius;
    }

    // 设置下一个巡逻点
    private void SetNextPatrolPoint()
    {
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    // 播放攻击音效
    private void PlayAttackSound()
    {
        attackAudioSource.Play();
    }

    // 死亡逻辑
    private void DeadBehavior()
    {
        chasingEnemiesCount--;
        agent.isStopped = true;
        animator.SetBool("Dying", true);
        Destroy(gameObject, 4f);
    }

    // 可视化扇形视野（调试用）
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * sightDistance);
        Gizmos.DrawWireSphere(lastKnownPosition, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
    public int MaxHealth
    {
        get { return 100; }
        set { }
    }

    public int Health { get { return (int)health; } }

    public void CauseDamage(int damageAmount)
    {
        if (health <= 0) return;

        health -= damageAmount;
        OnHealthChanged?.Invoke();
        if (health <= 0)
        {
            currentState = AIState.Dead;
            OnCharacterDeath?.Invoke(this);
        }
    }

    public void Heal(int healAmount)
    {
        if (health <= 0) return;

        health += healAmount;
        if (health > MaxHealth)
        {
            health = MaxHealth;
        }
        OnHealthChanged?.Invoke();
        OnHealthIncreased?.Invoke();
    }

    public void Respawn(Vector3 position)
    {
        return;
    }
}
