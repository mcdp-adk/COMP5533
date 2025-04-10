using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEffectTest : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float maxScale = 5f;           // 圆球最大大小
    public float growthDuration = 2f;    // 放大过程持续时间
    public float disappearDelay = 0.5f;  // 圆球在最大大小停留时间后消失

    public GameObject explosionParticles; // 爆炸效果的粒子系统预制体

    void Start()
    {
        // 开始放大圆球
        StartCoroutine(GrowAndExplode());
    }

    private IEnumerator GrowAndExplode()
    {
        float elapsedTime = 0f;
        Vector3 initialScale = transform.localScale; // 圆球的初始大小

        // 圆球逐渐放大
        while (elapsedTime < growthDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, Vector3.one * maxScale, elapsedTime / growthDuration);
            elapsedTime += Time.deltaTime; // 更新时间
            yield return null; // 等待下一帧
        }

        // 确保圆球达到最大大小
        transform.localScale = Vector3.one * maxScale;

        // 停留一段时间
        yield return new WaitForSeconds(disappearDelay);

        // 显示爆炸效果
        if (explosionParticles != null)
        {
            Instantiate(explosionParticles, transform.position, Quaternion.identity);
        }

        // 销毁圆球
        Destroy(gameObject);
    }
}
