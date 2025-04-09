using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsSpawnManager : MonoBehaviour
{
    [Header("Reference")]
    // 用于设置预制物体
    [SerializeField] private GameObject prefab;
    // 设置指定的Layer图层
    [SerializeField] private LayerMask targetLayer;
    // 设置范围
    [SerializeField] private List<Bounds> spawnRanges;
    // 设置Prefab的最大数量
    [SerializeField] private int maxCount;
    // 设置Prefab之间以及与其他墙体的最小距离
    [SerializeField] private float minDistance;

    // 当前生成的Prefab列表
    private List<GameObject> spawnedPrefabs = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnPrefabs());
    }

    void Update()
    {
        // 检查已生成物体是否被销毁
        spawnedPrefabs.RemoveAll(prefab => prefab == null);

        // 如果数量不足，重新生成
        if (spawnedPrefabs.Count < maxCount)
        {
            StartCoroutine(SpawnPrefabs());
        }
    }

    IEnumerator SpawnPrefabs()
    {
        // 按数量需求生成
        while (spawnedPrefabs.Count < maxCount)
        {
            bool spawned = TrySpawnPrefab();
            if (!spawned) break; // 如果无法生成新的Prefab，退出循环
            yield return null;
        }
    }

    bool TrySpawnPrefab()
    {
        foreach (var range in spawnRanges)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(range.min.x, range.max.x),
                Random.Range(range.min.y, range.max.y),
                Random.Range(range.min.z, range.max.z)
            );

            Collider[] colliders = Physics.OverlapSphere(randomPosition, minDistance);

            bool positionIsValid = true;
            foreach (var collider in colliders)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Walls") ||
                    collider.gameObject.layer == targetLayer)
                {
                    positionIsValid = false;
                    break;
                }
            }

            if (positionIsValid)
            {
                GameObject newPrefab = Instantiate(prefab, randomPosition, Quaternion.identity);
                spawnedPrefabs.Add(newPrefab);
                return true;
            }
        }
        return false; // 没找到有效位置
    }
}
