using UnityEngine;
using Cinemachine;
using System.Collections;

public class CharactersControl : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private int numberOfEnemies = 5;

    [Header("Patrol Points")]
    [SerializeField] private Transform[] patrolPoints;

    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public static CharactersControl Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerDeath += PlayerDied;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerDeath -= PlayerDied;
    }

    void Start()
    {
        // �������
        SpawnPlayer();

        // ����AI����
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // ���ѡ��һ�����ɵ�
            Transform spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // ��ȡEnemyAI���������Ѳ�ߵ�
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.patrolPoints = ShufflePatrolPoints(patrolPoints);
            }
        }
    }

    // �������
    private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);

        // ����Cinemachine����������ĸ���Ŀ��
        if (virtualCamera != null)
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
        }
    }

    // �������������
    public void PlayerDied()
    {
        StartCoroutine(RespawnPlayer());
    }

    // �����������Э��
    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(5f);
        SpawnPlayer();
    }

    // �������Ѳ�ߵ����鲢����һ���µ�����ʵ��
    private Transform[] ShufflePatrolPoints(Transform[] points)
    {
        Transform[] shuffledPoints = (Transform[])points.Clone();
        for (int i = shuffledPoints.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Transform temp = shuffledPoints[i];
            shuffledPoints[i] = shuffledPoints[randomIndex];
            shuffledPoints[randomIndex] = temp;
        }
        return shuffledPoints;
    }
}
