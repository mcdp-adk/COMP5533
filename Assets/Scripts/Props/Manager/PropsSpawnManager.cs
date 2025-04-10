using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsSpawnManager : MonoBehaviour
{
    [Header("Reference")]
    // ��������Ԥ������
    [SerializeField] private GameObject prefab;
    // ����ָ����Layerͼ��
    [SerializeField] private LayerMask targetLayer;
    // ���÷�Χ
    [SerializeField] private List<Bounds> spawnRanges;
    // ����Prefab���������
    [SerializeField] private int maxCount;
    // ����Prefab֮���Լ�������ǽ�����С����
    [SerializeField] private float minDistance;

    // ��ǰ���ɵ�Prefab�б�
    private List<GameObject> spawnedPrefabs = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnPrefabs());
    }

    void Update()
    {
        // ��������������Ƿ�����
        spawnedPrefabs.RemoveAll(prefab => prefab == null);

        // ����������㣬��������
        if (spawnedPrefabs.Count < maxCount)
        {
            StartCoroutine(SpawnPrefabs());
        }
    }

    IEnumerator SpawnPrefabs()
    {
        // ��������������
        while (spawnedPrefabs.Count < maxCount)
        {
            bool spawned = TrySpawnPrefab();
            if (!spawned) break; // ����޷������µ�Prefab���˳�ѭ��
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
        return false; // û�ҵ���Чλ��
    }
}
