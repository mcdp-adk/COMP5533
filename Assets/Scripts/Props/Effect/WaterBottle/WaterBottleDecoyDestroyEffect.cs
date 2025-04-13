using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBottleDecoyDestroyEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject effectPrefab; // ���ڱ�����Ч����ı���
    [SerializeField] private float effectLifetime = 2f; // ��Ч�Ĵ���ʱ��

    /// <summary>
    /// �����ص����屻����ʱ����
    /// </summary>
    private void OnDestroy()
    {
        // ����Ƿ��Ѿ�ָ������Ч����
        if (effectPrefab != null)
        {
            // ȷ�����ɵ���Ч���崹ֱ����
            Quaternion verticalRotation = Quaternion.Euler(0f, 0f, 0f);

            // ������Ч����
            GameObject effectInstance = Instantiate(effectPrefab, transform.position, verticalRotation);

            // ������Ч���ٵ���ʱ
            Destroy(effectInstance, effectLifetime); // ����Ч����ʱ�������
        }
        else
        {
            Debug.LogWarning("Effect prefab is not assigned!");
        }
    }
}
