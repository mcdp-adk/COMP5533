using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    // 用于记录指定的Layer
    [SerializeField] private LayerMask targetLayer;
    // Light组件
    private Light lightComponent;
    // HashSet 用于跟踪在触发器中的物体
    private HashSet<GameObject> objectsInTrigger;

    // Start 是脚本开始时调用的函数
    void Start()
    {
        // 获取Light组件
        lightComponent = GetComponent<Light>();

        // 确保Light组件存在
        if (lightComponent == null)
        {
            Debug.LogError("当前对象上未找到Light组件！");
        }

        // 初始化 HashSet
        objectsInTrigger = new HashSet<GameObject>();
    }

    // OnTriggerEnter 是当其他碰撞器进入触发器时调用的函数
    private void OnTriggerEnter(Collider other)
    {
        // 检测进入的对象是否在指定的Layer中
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // 将物体添加到 HashSet 中
            if (objectsInTrigger.Add(other.gameObject))
            {
                // 如果是新物体，激活Light组件
                lightComponent.enabled = true;
            }
        }
    }

    // OnTriggerExit 是当其他碰撞器离开触发器时调用的函数
    private void OnTriggerExit(Collider other)
    {
        // 检测离开的对象是否在指定的Layer中
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // 从 HashSet 中移除物体
            if (objectsInTrigger.Remove(other.gameObject))
            {
                // 当HashSet为空时，关闭Light组件
                if (objectsInTrigger.Count == 0)
                {
                    lightComponent.enabled = false;
                }
            }
        }
    }
}
