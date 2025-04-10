using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsSpawnPoint : MonoBehaviour
{
    [Header("Reference")]
    // 用于设置预制物体
    [SerializeField] private GameObject prefab;

    [Header("Parm")]
    // 当前实例化的物体
    private GameObject currentInstance;

    // Start is called before第一次运行时会执行
    void Start()
    {
        SpawnObject(); // 在开始时生成一个物体
    }

    // 用于检查物体是否被销毁
    void Update()
    {
        if (currentInstance == null) // 如果当前实例的物体已经销毁
        {
            SpawnObject(); // 重新生成一个物体
        }
    }

    // 生成物体的方法
    void SpawnObject()
    {
        if (prefab != null) // 检查prefab是否已设置
        {
            currentInstance = Instantiate(prefab, transform.position, transform.rotation);
        }
    }
}

