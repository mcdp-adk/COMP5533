using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropEndActiveEventEffect : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private GameObject effectPrefab; // ��ըЧ����Ԥ����
    [SerializeField] private GameObject soundPrefab; // ��ը��Ч��Ԥ����
    [SerializeField] private float effectDuration = 3f;    // ��ըЧ������ʱ��

    /// <summary>
    /// ��ը��������
    /// </summary>
    public void ShowEffect()
    {

        Debug.Log("���뱬ըЧ�������Ԥ�Ƽ��Ľ��Ϊ��" + (effectPrefab != null));

        // ���ű�ը��Ч
        if (soundPrefab != null)
        {
            // ʵ������ЧԤ����
            GameObject soundObject = Instantiate(soundPrefab, transform.position, Quaternion.identity);

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
        if (effectPrefab != null)
        {
            GameObject explosionEffect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
            Destroy(explosionEffect, effectDuration); // �ڱ�ըЧ������ʱ�������
            Debug.Log("��ըЧ��������");
        }
        else
        {
            Debug.LogWarning("��ըЧ��Ԥ����δ���ã�");
        }
    }
}
