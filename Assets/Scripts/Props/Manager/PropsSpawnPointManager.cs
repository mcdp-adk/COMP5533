using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropsSpawnPointManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>(); // 可生成道具的点位列表
    [System.Serializable]
    public class PropPrefabWithWeight
    {
        public GameObject prefab;  // 道具预制体
        public int weight = 1;         // 道具生成的权重
    }
    [SerializeField] private List<PropPrefabWithWeight> propPrefabsWithWeights; // 带有权重的道具列表
    [SerializeField] private int maxTotalProps = 2; // 控制道具的总生成数量
    [SerializeField] private float spawnWaitingTime = 0.3f; // 控制道具的总生成数量

    private int currentTotalProps = 0; // 当前地图中的道具数量

    private class SpawnPointStatus
    {
        public Transform point; // 点位的Transform
        public GameObject generatedProp; // 生成的道具引用
        public bool IsPropDestroyed => generatedProp == null; // 判断道具是否销毁
    }

    private List<SpawnPointStatus> spawnPointStatuses = new List<SpawnPointStatus>(); // 点位状态列表

    void Start()
    {
        // 初始化生成点位的状态
        foreach (var spawnPoint in spawnPoints)
        {
            spawnPointStatuses.Add(new SpawnPointStatus { point = spawnPoint, generatedProp = null });
        }

        SpawnProps(); // 开始时生成道具
        currentTotalProps = spawnPointStatuses.Count(status => status.generatedProp != null); // 初始化当前道具数量
    }

    /// <summary>
    /// 在空闲的点位生成道具
    /// </summary>
    private void SpawnProps()
    {
        if (!this || !gameObject.activeInHierarchy)
        {
            Debug.LogWarning("尝试生成道具时，当前对象无效，停止生成！");
            return;
        }

        // 将点位状态列表随机化
        List<SpawnPointStatus> shuffledSpawnPoints = new List<SpawnPointStatus>(spawnPointStatuses);
        for (int i = 0; i < shuffledSpawnPoints.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledSpawnPoints.Count);
            SpawnPointStatus temp = shuffledSpawnPoints[i];
            shuffledSpawnPoints[i] = shuffledSpawnPoints[randomIndex];
            shuffledSpawnPoints[randomIndex] = temp;
        }

        foreach (var status in shuffledSpawnPoints)
        {
            // 如果当前道具数量已达到最大限制，停止生成
            if (currentTotalProps >= maxTotalProps)
            {
                Debug.Log("道具数量已达到最大限制，停止生成！");
                return;
            }

            // 仅在空闲或道具被销毁的点位生成新的道具
            if (status.IsPropDestroyed)
            {
                GameObject prop = SelectPropByWeight(); // 根据权重选择道具
                if (prop == null) continue;

                GameObject spawnedProp = Instantiate(prop, status.point.position, Quaternion.identity);

                // 绑定销毁事件监听
                var propAction = spawnedProp.GetComponent<PropsBasicAction>();
                if (propAction != null)
                {
                    propAction.OnDestroyed += HandlePropDestroyed; // 注册销毁事件
                }

                // 更新点位状态
                status.generatedProp = spawnedProp;
                currentTotalProps++; // 增加当前道具数量
            }
        }
    }

    /// <summary>
    /// 根据权重选择道具
    /// </summary>
    private GameObject SelectPropByWeight()
    {
        int totalWeight = propPrefabsWithWeights.Sum(prop => prop.weight); // 计算总权重
        int randomValue = Random.Range(0, totalWeight); // 获取0到总权重之间的随机值
        int currentWeightSum = 0;

        foreach (var prop in propPrefabsWithWeights)
        {
            currentWeightSum += prop.weight;
            if (randomValue < currentWeightSum)
            {
                return prop.prefab; // 返回选中的道具
            }
        }

        return null; // 如果权重列表为空，返回null
    }

    /// <summary>
    /// 处理道具销毁事件
    /// </summary>
    private void HandlePropDestroyed(GameObject prop)
    {
        Debug.Log($"道具 {prop.name} 已销毁。");
        if (!this || !gameObject.activeInHierarchy)
        {
            Debug.LogWarning("游戏对象已无效，停止生成逻辑！");
            return;
        }

        foreach (var status in spawnPointStatuses)
        {
            if (status.generatedProp == prop)
            {
                status.generatedProp = null; // 更新点位状态为未生成道具
                currentTotalProps--; // 减少当前道具数量
                break; // 找到对应点位后停止遍历
            }
        }

        // 使用延迟调用生成逻辑，避免在OnDestroy中直接生成
        Invoke(nameof(SpawnProps), spawnWaitingTime); // 延迟调用SpawnProps
    }

    void OnDisable()
    {
        // 取消所有挂起的生成调用
        CancelInvoke(nameof(SpawnProps));
    }
}

