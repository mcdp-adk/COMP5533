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
        // 生成玩家
        SpawnPlayer();

        // 生成AI敌人
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // 随机选择一个生成点
            Transform spawnPoint = enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            // 获取EnemyAI组件并设置巡逻点
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.patrolPoints = ShufflePatrolPoints(patrolPoints);
            }
        }
    }

    // 生成玩家
    private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);

        // 设置Cinemachine虚拟摄像机的跟随目标
        if (virtualCamera != null)
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
        }
    }

    // 玩家死亡处理方法
    public void PlayerDied()
    {
        StartCoroutine(RespawnPlayer());
    }

    // 玩家重新生成协程
    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(5f);
        SpawnPlayer();
    }

    // 随机打乱巡逻点数组并返回一个新的数组实例
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
