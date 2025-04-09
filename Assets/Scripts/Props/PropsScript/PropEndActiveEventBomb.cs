using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PropEndActiveEventBomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionEffectPrefab; // ��ըЧ����Ԥ����
    public float explosionDuration = 3f;    // ��ըЧ������ʱ��

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
        Debug.Log("ɾ������");
        // ɾ����������
        Destroy(gameObject);
    }
}
