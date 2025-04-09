using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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
    [SerializeField] private Transform[] patrolPoints;    // 巡逻路径点
    [SerializeField] private Animator animator;
    [SerializeField] private float attackDelay = 0.5f; // 攻击延迟时间
    [SerializeField] private LayerMask playerLayer; // 玩家层

    private NavMeshAgent agent;
    private Transform player;
    private Vector3 lastKnownPosition;
    private bool isChasing;
    private int currentPatrolIndex;

    // AI状态枚举
    private enum AIState { Patrol, Chase, Investigate, Attack }
    private AIState currentState = AIState.Patrol;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentPatrolIndex = 0;
        agent.speed = patrolSpeed;
        agent.angularSpeed = angularSpeed;
        agent.acceleration = acceleration;
        SetNextPatrolPoint();
    }

    void Update()
    {
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
        }
    }

    // 巡逻逻辑
    private void PatrolBehavior()
    {
        if (agent.remainingDistance < 0.5f)
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
        agent.SetDestination(player.position);
        lastKnownPosition = player.position;

        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            currentState = AIState.Attack;
            animator.CrossFade("Standing Melee Attack Downward", 0.1f);
            StartCoroutine(PerformAttack());
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
        if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            currentState = AIState.Chase;
            animator.CrossFade("Running", 0.1f);
            return;
        }
    }

    // 执行攻击判定的协程
    private IEnumerator PerformAttack()
    {
        // 等待攻击动画播放完成
        yield return new WaitForSeconds(attackDelay);

        // 攻击动画结束后恢复到追逐状态
        currentState = AIState.Chase;
        animator.CrossFade("Running", 0.1f);
    }

    // 攻击命中判定
    public void EnemyAttackHit()
    {
        // 攻击判定
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackDistance, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // 删除玩家对象
                Destroy(hitCollider.gameObject);
            }
        }
    }

    // 调查最后位置逻辑
    private void InvestigateBehavior()
    {
        if (agent.remainingDistance < 0.5f)
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
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(directionToPlayer, transform.forward);
        float distance = directionToPlayer.magnitude;

        if (angle < sightAngle / 2 && distance < sightDistance)
        {
            // 射线检测障碍物遮挡
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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
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

    // 可视化扇形视野（调试用）
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * sightDistance);
        Gizmos.DrawWireSphere(lastKnownPosition, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackDistance); // 可视化攻击范围
    }
}
