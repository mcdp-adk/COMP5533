using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PropEndActiveEventBomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionEffectPrefab; // 爆炸效果的预制体
    public float explosionDuration = 3f;    // 爆炸效果持续时间

    /// <summary>
    /// 爆炸触发函数
    /// </summary>
    public void BombExplosion()
    {
        Debug.Log("进入爆炸效果，检测预制件的结果为：" + (explosionEffectPrefab != null));
        // 显示爆炸效果
        if (explosionEffectPrefab != null)
        {
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosionEffect, explosionDuration); // 在爆炸效果持续时间后销毁
            Debug.Log("爆炸效果结束！");
        }
        else
        {
            Debug.LogWarning("爆炸效果预制体未设置！");
        }
        Debug.Log("删除自身");
        // 删除自身物体
        Destroy(gameObject);
    }
}
