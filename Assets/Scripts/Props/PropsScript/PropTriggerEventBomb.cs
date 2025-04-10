using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物体被扔出的逻辑
/// </summary>
public class PropTriggerEventThrow : MonoBehaviour
{
    [Header("Throw Settings")]
    public float baseThrowForce = 10f;  // 基础抛出力
    public Vector3 throwDirection = new Vector3(0.5f, 1f, 0f); // 斜向前上方抛出方向
    public float randomTorqueRange = 5f; // 随机旋转力范围

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogWarning("没有找到 Rigidbody 组件，物体无法被扔出！");
        }
    }

    /// <summary>
    /// 物体被扔出，forceWeight 影响抛出的强度
    /// </summary>
    /// <param name="forceWeight">抛出力度权重，影响距离远近</param>
    public void ThrowOut(float forceWeight)
    {
        Debug.Log($"函数被调用！力度: {forceWeight}");

        if (rb != null)
        {
            float finalThrowForce = baseThrowForce * forceWeight;

            // 施加力，使物体沿斜向前上方抛出
            rb.AddForce(throwDirection.normalized * finalThrowForce, ForceMode.Impulse);

            // 生成随机的旋转力
            Vector3 randomTorque = new Vector3(
                Random.Range(-randomTorqueRange, randomTorqueRange),
                Random.Range(-randomTorqueRange, randomTorqueRange),
                Random.Range(-randomTorqueRange, randomTorqueRange)
            );

            // 施加旋转力
            rb.AddTorque(randomTorque, ForceMode.Impulse);

            Debug.Log($"物体被扔出并随机旋转！力度: {finalThrowForce}，旋转力: {randomTorque}");
        }
    }
}

