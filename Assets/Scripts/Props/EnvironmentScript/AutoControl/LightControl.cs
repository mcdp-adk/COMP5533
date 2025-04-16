using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer; // ���ڼ�¼ָ����Layer
    private Light lightComponent; // Light���
    private HashSet<GameObject> objectsInTrigger; // ���ڸ����ڴ������е�����
    private Coroutine fadeCoroutine; // ��ǰִ�еĽ���Э��

    [SerializeField] private float fadeDuration = 1.5f; // �������ʱ��
    [SerializeField] private float targetIntensityOn = 1f; // ����ʱ��Ŀ������

    void Start()
    {
        lightComponent = GetComponent<Light>(); // ��ȡLight���

        if (lightComponent == null)
        {
            Debug.LogError("��ǰ������δ�ҵ�Light�����");
        }

        objectsInTrigger = new HashSet<GameObject>(); // ��ʼ��HashSet
        lightComponent.intensity = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            if (objectsInTrigger.Add(other.gameObject))
            {
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine); // ֹͣ��ǰ�Ľ���Э��
                }
                fadeCoroutine = StartCoroutine(FadeLight(true)); // �����ƹ⽥��
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            if (objectsInTrigger.Remove(other.gameObject))
            {
                if (objectsInTrigger.Count == 0)
                {
                    if (fadeCoroutine != null)
                    {
                        StopCoroutine(fadeCoroutine); // ֹͣ��ǰ�Ľ���Э��
                    }
                    fadeCoroutine = StartCoroutine(FadeLight(false)); // �رյƹ⽥��
                }
            }
        }
    }

    private IEnumerator FadeLight(bool turnOn)
    {
        float startIntensity = lightComponent.intensity;
        float targetIntensity = turnOn ? targetIntensityOn : 0f; // Ŀ�������ɲ�������
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            lightComponent.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / fadeDuration);
            yield return null;
        }

        lightComponent.intensity = targetIntensity; // ȷ����������һ��

        // ����ƹ�رգ��������
        if (!turnOn)
        {
            lightComponent.enabled = false;
            lightComponent.intensity = 0;
        }
        else
        {
            lightComponent.enabled = true;
        }

        fadeCoroutine = null; // �����ǰЭ������
    }
}
