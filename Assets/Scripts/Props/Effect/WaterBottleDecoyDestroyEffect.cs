using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBottleDecoyDestroyEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject effectPrefab; // 用于保存特效对象的变量
    [SerializeField] private float effectLifetime = 2f; // 特效的存在时间

    /// <summary>
    /// 当挂载的物体被销毁时触发
    /// </summary>
    private void OnDestroy()
    {
        // 检查是否已经指定了特效对象
        if (effectPrefab != null)
        {
            // 确保生成的特效物体垂直向上
            Quaternion verticalRotation = Quaternion.Euler(0f, 0f, 0f);

            // 生成特效对象
            GameObject effectInstance = Instantiate(effectPrefab, transform.position, verticalRotation);

            // 启动特效销毁倒计时
            Destroy(effectInstance, effectLifetime); // 在特效存在时间后销毁
        }
        else
        {
            Debug.LogWarning("Effect prefab is not assigned!");
        }
    }
}
