using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float sightAngle = 60f;    // ������Ұ�Ƕ�
    [SerializeField] private float sightDistance = 10f;  // ��Ұ����
    [SerializeField] private float detectionRadius = 5f; // ���ܼ��뾶
    [SerializeField] private float angularSpeed = 120f;  // ת���ٶ�
    [SerializeField] private float acceleration = 8f;    // ���ٶ�
    [SerializeField] private Transform[] patrolPoints;    // Ѳ��·����

    private NavMeshAgent agent;
    private Transform player;
    private Vector3 lastKnownPosition;
    private bool isChasing;
    private int currentPatrolIndex;

    // AI״̬ö��
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

    // Ѳ���߼�
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

    // ׷���߼�
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

    // �������λ���߼�
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

    // ������Ұ���
    private bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(directionToPlayer, transform.forward);
        float distance = directionToPlayer.magnitude;

        if (angle < sightAngle / 2 && distance < sightDistance)
        {
            // ���߼���ϰ����ڵ�
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

    // ���ܼ��뾶
    private bool IsPlayerInDetectionRadius()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer < detectionRadius;
    }

    // ������һ��Ѳ�ߵ�
    private void SetNextPatrolPoint()
    {
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    // ���ӻ�������Ұ�������ã�
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * sightDistance);
        Gizmos.DrawWireSphere(lastKnownPosition, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
