using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

public class EnemyAI : MonoBehaviour
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
    [SerializeField] private float attackDelay = 0.5f; // 攻击延迟时间
    [SerializeField] private LayerMask playerLayer; // 玩家层
    [SerializeField] private AudioSource attackAudioSource; // 攻击音效
    [SerializeField] public int health = 100; // 敌人生命值

    private NavMeshAgent agent;
    private Transform[] players;
    private Transform targetPlayer;
    private Vector3 lastKnownPosition;
    private bool isChasing;
    private int currentPatrolIndex;

    // AI状态枚举
    private enum AIState { Patrol, Chase, Investigate, Attack, Dead }
    private AIState currentState = AIState.Patrol;

    void Start()
    {
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
                    playerController.Die();
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
        agent.isStopped = true;
        animator.CrossFade("Dying", 0.1f);
        Destroy(gameObject, 2f);
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
}
