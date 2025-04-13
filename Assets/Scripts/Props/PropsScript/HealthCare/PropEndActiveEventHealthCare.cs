using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropEndActiveEventHealthCare : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private GameObject explosionEffectPrefab; // 爆炸效果的预制体
    [SerializeField] private GameObject healthCareEffectPrefab; // 治疗效果的预制体
    [SerializeField] private GameObject explosionSoundPrefab; // 爆炸音效的预制体
    [SerializeField] private float explosionDuration = 3f;    // 爆炸效果持续时间
    [SerializeField] private float explosionRange = 5f;  // 爆炸范围
    [SerializeField] private float explosionHealthCare = 100f;  // 爆炸伤害
    [SerializeField] private Vector3 healthCareEffectOffset = new Vector3(0, 1.5f, 0); // 默认偏移值
    [SerializeField] private string[] targetTags; // 爆炸范围内目标对象的标签数组

    /// <summary>
    /// 爆炸触发函数
    /// </summary>
    public void HealthcareExplosion()
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
                        enemyAI.Heal((int)explosionHealthCare);
                        //Debug.Log($"对带有标签 {tag} 的对象 {hitCollider.gameObject.name} 造成 {explosionHealthCare} 点伤害，剩余生命值：{enemyAI.health}");
                    }

                    if (healthCareEffectPrefab != null)
                    {
                        Vector3 effectPosition = hitCollider.transform.position + healthCareEffectOffset;
                        Vector3 directionToExplosion = (effectPosition - transform.position).normalized;

                        // 创建朝向爆炸的旋转
                        Quaternion rotationToExplosion = Quaternion.LookRotation(directionToExplosion);

                        // 添加额外的旋转偏移来修正角度（例如绕 x 或 z 轴旋转）
                        Quaternion rotationFix = Quaternion.Euler(0, 90, 0); // 根据实际偏差调整，例如绕 Y 轴增加 90°
                        Quaternion finalRotation = rotationToExplosion * rotationFix;

                        GameObject healthCareEffect = Instantiate(healthCareEffectPrefab, effectPosition, finalRotation);
                        Destroy(healthCareEffect, explosionDuration);
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
                    playerController.Heal((int)(explosionHealthCare)); // 对玩家造成伤害
                }
                if (healthCareEffectPrefab != null)
                {
                    Vector3 effectPosition = hitCollider.transform.position + healthCareEffectOffset;
                    Vector3 directionToExplosion = (effectPosition - transform.position).normalized;

                    // 创建朝向爆炸的旋转
                    Quaternion rotationToExplosion = Quaternion.LookRotation(directionToExplosion);

                    // 添加额外的旋转偏移来修正角度（例如绕 x 或 z 轴旋转）
                    Quaternion rotationFix = Quaternion.Euler(0, 90, 0); // 根据实际偏差调整，例如绕 Y 轴增加 90°
                    Quaternion finalRotation = rotationToExplosion * rotationFix;

                    GameObject healthCareEffect = Instantiate(healthCareEffectPrefab, effectPosition, finalRotation);
                    Destroy(healthCareEffect, explosionDuration);
                }

            }
        }
        Debug.Log("删除自身");
        // 删除自身物体
        Destroy(gameObject);
    }
}
