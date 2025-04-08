using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private Transform[] patrolPoints;    // 巡逻路径点

    private NavMeshAgent agent;
    private Transform player;
    private Vector3 lastKnownPosition;
    private bool isChasing;
    private int currentPatrolIndex;

    // AI状态枚举
    private enum AIState { Patrol, Chase, Investigate }
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
            agent.speed = chaseSpeed;
        }
    }

    // 追逐逻辑
    private void ChaseBehavior()
    {
        agent.SetDestination(player.position);
        lastKnownPosition = player.position;

        if (!IsPlayerInSight() && !IsPlayerInDetectionRadius())
        {
            currentState = AIState.Investigate;
            agent.SetDestination(lastKnownPosition);
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
    }
}
