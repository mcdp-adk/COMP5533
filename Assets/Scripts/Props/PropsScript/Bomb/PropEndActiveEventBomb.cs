using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PropEndActiveEventBomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private GameObject explosionEffectPrefab; // 爆炸效果的预制体
    [SerializeField] private GameObject explosionSoundPrefab; // 爆炸音效的预制体
    [SerializeField] private float explosionDuration = 3f;    // 爆炸效果持续时间
    [SerializeField] private float explosionRange = 5f;  // 爆炸范围
    [SerializeField] private float explosionDamage = 100f;  // 爆炸伤害
    [SerializeField] private string[] targetTags; // 爆炸范围内目标对象的标签数组

    /// <summary>
    /// 爆炸触发函数
    /// </summary>
    public void BombExplosion()
    {
        Debug.Log("进入爆炸效果，检测预制件的结果为：" + (explosionEffectPrefab != null));

        // 播放爆炸音效
        if (explosionSoundPrefab != null)
        {
            // 实例化音效预制体
            GameObject soundObject = Instantiate(explosionSoundPrefab, transform.position, Quaternion.identity);

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

        // 检测爆炸范围内的目标对象并直接销毁符合标签的对象
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRange);
        foreach (Collider hitCollider in hitColliders)
        {
            // 检查对象是否匹配任意一个目标标签
            foreach (string tag in targetTags)
            {
                if (hitCollider.CompareTag(tag))
                {
                    // 获取EnemyAI组件并修改health属性
                    EnemyAI enemyAI = hitCollider.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.health -= explosionDamage;
                        //Debug.Log($"对带有标签 {tag} 的对象 {hitCollider.gameObject.name} 造成 {explosionDamage} 点伤害，剩余生命值：{enemyAI.health}");
                    }
                    break; // 防止重复处理同一个对象
                }
            }

            // 检查是否是玩家
            if (hitCollider.CompareTag("Player"))
            {
                PlayerController playerController = hitCollider.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.CauseDamage((int)explosionDamage); // 对玩家造成伤害
                }
            }
        }
        Debug.Log("删除自身");
        // 删除自身物体
        Destroy(gameObject);
    }
}
