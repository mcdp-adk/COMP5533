using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer; // 用于记录指定的Layer
    private Light lightComponent; // Light组件
    private HashSet<GameObject> objectsInTrigger; // 用于跟踪在触发器中的物体
    private Coroutine fadeCoroutine; // 当前执行的渐变协程

    [SerializeField] private float fadeDuration = 1.5f; // 渐变持续时间
    [SerializeField] private float targetIntensityOn = 1f; // 开启时的目标亮度

    void Start()
    {
        lightComponent = GetComponent<Light>(); // 获取Light组件

        if (lightComponent == null)
        {
            Debug.LogError("当前对象上未找到Light组件！");
        }

        objectsInTrigger = new HashSet<GameObject>(); // 初始化HashSet
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
                    StopCoroutine(fadeCoroutine); // 停止当前的渐变协程
                }
                fadeCoroutine = StartCoroutine(FadeLight(true)); // 开启灯光渐变
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
                        StopCoroutine(fadeCoroutine); // 停止当前的渐变协程
                    }
                    fadeCoroutine = StartCoroutine(FadeLight(false)); // 关闭灯光渐变
                }
            }
        }
    }

    private IEnumerator FadeLight(bool turnOn)
    {
        float startIntensity = lightComponent.intensity;
        float targetIntensity = turnOn ? targetIntensityOn : 0f; // 目标亮度由参数控制
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            lightComponent.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / fadeDuration);
            yield return null;
        }

        lightComponent.intensity = targetIntensity; // 确保最终亮度一致

        // 如果灯光关闭，禁用组件
        if (!turnOn)
        {
            lightComponent.enabled = false;
            lightComponent.intensity = 0;
        }
        else
        {
            lightComponent.enabled = true;
        }

        fadeCoroutine = null; // 清除当前协程引用
    }
}
