using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropEndActiveEventHealthCare : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private GameObject explosionEffectPrefab; // ��ըЧ����Ԥ����
    [SerializeField] private GameObject healthCareEffectPrefab; // ����Ч����Ԥ����
    [SerializeField] private GameObject explosionSoundPrefab; // ��ը��Ч��Ԥ����
    [SerializeField] private float explosionDuration = 3f;    // ��ըЧ������ʱ��
    [SerializeField] private float explosionRange = 5f;  // ��ը��Χ
    [SerializeField] private float explosionHealthCare = 100f;  // ��ը�˺�
    [SerializeField] private Vector3 healthCareEffectOffset = new Vector3(0, 1.5f, 0); // Ĭ��ƫ��ֵ
    [SerializeField] private string[] targetTags; // ��ը��Χ��Ŀ�����ı�ǩ����

    /// <summary>
    /// ��ը��������
    /// </summary>
    public void HealthcareExplosion()
    {
        Debug.Log("���뱬ըЧ�������Ԥ�Ƽ��Ľ��Ϊ��" + (explosionEffectPrefab != null));

        // ���ű�ը��Ч
        if (explosionSoundPrefab != null)
        {
            // ʵ������ЧԤ����
            GameObject soundObject = Instantiate(explosionSoundPrefab, transform.position, Quaternion.identity);

            // ��ȡ��ЧԤ�����ϵ� AudioSource �������������
            AudioSource audioSource = soundObject.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
                Destroy(soundObject, audioSource.clip.length); // ������������ɺ�������Ч����
                Debug.Log("��ʼ���ű�ը������");
            }
            else
            {
                Debug.LogWarning("��ը��ЧԤ������û���ҵ� AudioSource �����");
            }
        }
        else
        {
            Debug.LogWarning("��ը��ЧԤ����δ���ã�");
        }

        // ��ʾ��ըЧ��
        if (explosionEffectPrefab != null)
        {
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosionEffect, explosionDuration); // �ڱ�ըЧ������ʱ�������
            Debug.Log("��ըЧ��������");
        }
        else
        {
            Debug.LogWarning("��ըЧ��Ԥ����δ���ã�");
        }

        // ��ⱬը��Χ�ڵ�Ŀ�����ֱ�����ٷ��ϱ�ǩ�Ķ���
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRange);
        foreach (Collider hitCollider in hitColliders)
        {
            // �������Ƿ�ƥ������һ��Ŀ���ǩ
            foreach (string tag in targetTags)
            {
                if (hitCollider.CompareTag(tag))
                {
                    // ��ȡEnemyAI������޸�health����
                    EnemyAI enemyAI = hitCollider.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.Heal((int)explosionHealthCare);
                        //Debug.Log($"�Դ��б�ǩ {tag} �Ķ��� {hitCollider.gameObject.name} ��� {explosionHealthCare} ���˺���ʣ������ֵ��{enemyAI.health}");
                    }

                    if (healthCareEffectPrefab != null)
                    {
                        Vector3 effectPosition = hitCollider.transform.position + healthCareEffectOffset;
                        Vector3 directionToExplosion = (effectPosition - transform.position).normalized;

                        // ��������ը����ת
                        Quaternion rotationToExplosion = Quaternion.LookRotation(directionToExplosion);

                        // ��Ӷ������תƫ���������Ƕȣ������� x �� z ����ת��
                        Quaternion rotationFix = Quaternion.Euler(0, 90, 0); // ����ʵ��ƫ������������� Y ������ 90��
                        Quaternion finalRotation = rotationToExplosion * rotationFix;

                        GameObject healthCareEffect = Instantiate(healthCareEffectPrefab, effectPosition, finalRotation);
                        Destroy(healthCareEffect, explosionDuration);
                    }


                    break; // ��ֹ�ظ�����ͬһ������
                }
            }

            // ����Ƿ������
            if (hitCollider.CompareTag("Player"))
            {
                PlayerController playerController = hitCollider.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.Heal((int)(explosionHealthCare)); // ���������˺�
                }
                if (healthCareEffectPrefab != null)
                {
                    Vector3 effectPosition = hitCollider.transform.position + healthCareEffectOffset;
                    Vector3 directionToExplosion = (effectPosition - transform.position).normalized;

                    // ��������ը����ת
                    Quaternion rotationToExplosion = Quaternion.LookRotation(directionToExplosion);

                    // ��Ӷ������תƫ���������Ƕȣ������� x �� z ����ת��
                    Quaternion rotationFix = Quaternion.Euler(0, 90, 0); // ����ʵ��ƫ������������� Y ������ 90��
                    Quaternion finalRotation = rotationToExplosion * rotationFix;

                    GameObject healthCareEffect = Instantiate(healthCareEffectPrefab, effectPosition, finalRotation);
                    Destroy(healthCareEffect, explosionDuration);
                }

            }
        }
        Debug.Log("ɾ������");
        // ɾ����������
        Destroy(gameObject);
    }
}
