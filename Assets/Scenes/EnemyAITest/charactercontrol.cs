using UnityEngine;
using Cinemachine;
using System.Collections;

public class CharactersControl : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private int numberOfEnemies = 5;

    [Header("Patrol Points")]
    [SerializeField] private Transform[] patrolPoints;

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

    void Start()
    {
        // 生成AI敌人
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy();
        }

        // 开始检测敌人数量
        StartCoroutine(CheckEnemyCount());
    }

    // 生成敌人
    private void SpawnEnemy()
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

    // 检测敌人数量并在数量低于设定数量时重新生成敌人
    private IEnumerator CheckEnemyCount()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            int currentEnemyCount = GameObject.FindGameObjectsWithTag("enemy").Length;
            if (currentEnemyCount < numberOfEnemies)
            {
                int enemiesToSpawn = numberOfEnemies - currentEnemyCount;
                for (int i = 0; i < enemiesToSpawn; i++)
                {
                    SpawnEnemy();
                }
            }
        }
    }
}
