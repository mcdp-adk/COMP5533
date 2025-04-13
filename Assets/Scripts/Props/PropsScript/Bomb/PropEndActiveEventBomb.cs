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

        // 检测爆炸范围内的目标对象并确保每个物体只被处理一次
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRange);
        HashSet<GameObject> processedObjects = new HashSet<GameObject>();

        foreach (Collider hitCollider in hitColliders)
        {
            GameObject hitObject = hitCollider.gameObject;

            // 如果物体已经被处理过，跳过它
            if (processedObjects.Contains(hitObject))
            {
                continue;
            }

            // 将物体加入已处理集合
            processedObjects.Add(hitObject);

            Debug.Log($"正在检测伤害对象: {hitObject}");

            // 检查对象是否匹配任意一个目标标签
            foreach (string tag in targetTags)
            {
                if (hitObject.CompareTag(tag))
                {
                    // 获取EnemyAI组件并修改health属性
                    EnemyAI enemyAI = hitObject.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.health -= explosionDamage;
                        Debug.Log($"对带有标签 {tag} 的对象 {hitObject.name} 造成 {explosionDamage} 点伤害，剩余生命值：{enemyAI.health}");
                    }
                    break; // 防止重复处理同一个标签
                }
            }

            // 检查是否是玩家
            if (hitObject.CompareTag("Player"))
            {
                PlayerController playerController = hitObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.CauseDamage((int)explosionDamage); // 对玩家造成伤害
                    Debug.Log("爆炸正在给与玩家伤害！");
                }
            }
        }

        Debug.Log("删除自身");
        // 删除自身物体
        Destroy(gameObject);
    }
}
