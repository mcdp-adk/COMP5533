using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEffectTest : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float maxScale = 5f;           // Բ������С
    public float growthDuration = 2f;    // �Ŵ���̳���ʱ��
    public float disappearDelay = 0.5f;  // Բ��������Сͣ��ʱ�����ʧ

    public GameObject explosionParticles; // ��ըЧ��������ϵͳԤ����

    void Start()
    {
        // ��ʼ�Ŵ�Բ��
        StartCoroutine(GrowAndExplode());
    }

    private IEnumerator GrowAndExplode()
    {
        float elapsedTime = 0f;
        Vector3 initialScale = transform.localScale; // Բ��ĳ�ʼ��С

        // Բ���𽥷Ŵ�
        while (elapsedTime < growthDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, Vector3.one * maxScale, elapsedTime / growthDuration);
            elapsedTime += Time.deltaTime; // ����ʱ��
            yield return null; // �ȴ���һ֡
        }

        // ȷ��Բ��ﵽ����С
        transform.localScale = Vector3.one * maxScale;

        // ͣ��һ��ʱ��
        yield return new WaitForSeconds(disappearDelay);

        // ��ʾ��ըЧ��
        if (explosionParticles != null)
        {
            Instantiate(explosionParticles, transform.position, Quaternion.identity);
        }

        // ����Բ��
        Destroy(gameObject);
    }
}
