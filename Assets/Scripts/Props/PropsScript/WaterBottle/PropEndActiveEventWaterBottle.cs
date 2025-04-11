using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropEndActiveEventWaterBottle : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string targetTag = "Player";  // 诱饵的tag
    [SerializeField] private string targetLayer = "Player";  // 诱饵的层
    [SerializeField] private float duringTime = 5f;  // 诱饵的存在时间
    [SerializeField] private float quaternionAngle = 25f;  // 诱饵的存在时间
    [SerializeField] private GameObject decoyPerfab;  // 诱饵的预制件

    /// <summary>
    /// 触发函数
    /// </summary>
    public void DecoyEmerge()
    {
        if (decoyPerfab != null)
        {
            // 调整生成物体的旋转，使其垂直或倾斜角度不超过30°
            Quaternion adjustedRotation = Quaternion.Euler(
                Mathf.Clamp(transform.rotation.eulerAngles.x, -quaternionAngle, quaternionAngle),
                transform.rotation.eulerAngles.y,
                Mathf.Clamp(transform.rotation.eulerAngles.z, -quaternionAngle, quaternionAngle)
            );

            // 生成诱饵
            GameObject decoyInstance = Instantiate(decoyPerfab, transform.position, adjustedRotation);

            // 设置诱饵的Tag
            decoyInstance.tag = targetTag;

            // 设置诱饵的Layer
            decoyInstance.layer = LayerMask.NameToLayer(targetLayer);

            // 启动销毁倒计时
            Destroy(decoyInstance, duringTime); // 在效果持续时间后销毁
        }
        else
        {
            Debug.LogWarning("Decoy prefab is not assigned!");
        }

        // 销毁挂载该脚本的物体
        Destroy(gameObject);
    }
}
