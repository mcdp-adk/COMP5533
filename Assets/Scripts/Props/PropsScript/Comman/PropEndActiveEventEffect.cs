using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropEndActiveEventEffect : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private GameObject effectPrefab; // 爆炸效果的预制体
    [SerializeField] private GameObject soundPrefab; // 爆炸音效的预制体
    [SerializeField] private float effectDuration = 3f;    // 爆炸效果持续时间

    /// <summary>
    /// 爆炸触发函数
    /// </summary>
    public void ShowEffect()
    {

        Debug.Log("进入爆炸效果，检测预制件的结果为：" + (effectPrefab != null));

        // 播放爆炸音效
        if (soundPrefab != null)
        {
            // 实例化音效预制体
            GameObject soundObject = Instantiate(soundPrefab, transform.position, Quaternion.identity);

            // 获取音效预制体上的 AudioSource 组件并播放声音
            AudioSource audioSource = soundObject.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
                Destroy(soundObject, audioSource.clip.length); // 在声音播放完成后销毁音效对象
                Debug.Log("开始播放爆炸声音！");
            }
            else
            {
                Debug.LogWarning("爆炸音效预制体上没有找到 AudioSource 组件！");
            }
        }
        else
        {
            Debug.LogWarning("爆炸音效预制体未设置！");
        }

        // 显示爆炸效果
        if (effectPrefab != null)
        {
            GameObject explosionEffect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
            Destroy(explosionEffect, effectDuration); // 在爆炸效果持续时间后销毁
            Debug.Log("爆炸效果结束！");
        }
        else
        {
            Debug.LogWarning("爆炸效果预制体未设置！");
        }
    }
}
