using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropStartActiveEventTreasureMusic : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("TreasureSetting")]
    [SerializeField] private AudioSource treasureSound; // ��Ч�ļ�
    [SerializeField] private float fadeDuration = 2f; // �����������ʱ��
    public void TreasurePlayMusic()
    {
        if (treasureSound != null)
        {
            treasureSound.Play(); // ������Ч
            StartCoroutine(FadeInAudio());
            Debug.Log("�����ѽ��밲ȫ�ݣ���Ч���ڽ������");
        }
        else
        {
            Debug.LogWarning("δ���ñ�����Ч��");
        }
    }

    private IEnumerator FadeInAudio()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            treasureSound.volume = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration); // �� 0 ���� 1
            elapsedTime += Time.deltaTime;
            yield return null; // �ȴ���һ֡
        }
        treasureSound.volume = 1f; // ȷ��������������Ϊ���ֵ
    }
}
