using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PropEndActiveEventBomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private GameObject explosionEffectPrefab; // ��ըЧ����Ԥ����
    [SerializeField] private float explosionDuration = 3f;    // ��ըЧ������ʱ��
    [SerializeField] private float explosionRange = 5f;  // ��ը��Χ
    [SerializeField] private float explosionDamage = 100f;  // ��ը��Χ
    [SerializeField] private string[] targetTags; // ��ը��Χ��Ŀ�����ı�ǩ����

    /// <summary>
    /// ��ը��������
    /// </summary>
    public void BombExplosion()
    {
        Debug.Log("���뱬ըЧ�������Ԥ�Ƽ��Ľ��Ϊ��" + (explosionEffectPrefab != null));
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
                    // ���ٷ��ϱ�ǩ�Ķ���
                    Debug.Log($"���ٴ��б�ǩ {tag} �Ķ���: {hitCollider.gameObject.name}");
                    Destroy(hitCollider.gameObject); // ֱ�����ٶ���
                    break; // ��ֹ�ظ�����ͬһ������
                }
            }
        }


        Debug.Log("ɾ������");
        // ɾ����������
        Destroy(gameObject);
    }
}
