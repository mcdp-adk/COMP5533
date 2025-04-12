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
        // ����AI����
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy();
        }

        // ��ʼ����������
        StartCoroutine(CheckEnemyCount());
    }

    // ���ɵ���
    private void SpawnEnemy()
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

    // �����������������������趨����ʱ�������ɵ���
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
