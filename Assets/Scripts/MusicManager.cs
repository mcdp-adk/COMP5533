using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource chaseMusicAudioSource;
    [SerializeField] private float fadeDuration = 1f; // ��������/��������ʱ��
    public bool isChasing = false; // �Ƿ���׷��״̬

    private Coroutine currentFadeCoroutine = null; // ���ڸ��ٵ�ǰ�������е�Э��

    private void Update()
    {
        if (isChasing && !chaseMusicAudioSource.isPlaying)
        {
            // �������ִ�е�����ȡ����
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
                currentFadeCoroutine = null;
            }
            // ��ʼ����
            currentFadeCoroutine = StartCoroutine(FadeIn(chaseMusicAudioSource, fadeDuration));
        }
        else if (!isChasing && chaseMusicAudioSource.isPlaying)
        {
            // �������ִ�е��룬ȡ����
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
                currentFadeCoroutine = null;
            }
            // ��ʼ����
            currentFadeCoroutine = StartCoroutine(FadeOut(chaseMusicAudioSource, fadeDuration));
        }
    }

    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume; // ��ǰ������Ϊ��ʼֵ
        audioSource.Play(); // ��ʼ������Ƶ

        float elapsedTime = 0f; // ����׷��ʱ��ı���
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime; // ���Ӿ�����ʱ��
            audioSource.volume = Mathf.Clamp01(startVolume + (elapsedTime / duration)); // ��������������
            yield return null;
        }

        audioSource.volume = 1f; // ��������Ϊ1
        currentFadeCoroutine = null; // ������ɺ����Э�̱�־
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume; // ��ǰ������Ϊ��ʼֵ
        float elapsedTime = 0f; // ����׷��ʱ��ı���

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime; // ���Ӿ�����ʱ��
            audioSource.volume = Mathf.Clamp01(startVolume * (1 - (elapsedTime / duration))); // ��������������
            yield return null;
        }

        audioSource.volume = 0f; // ȷ��������ȫΪ0
        audioSource.Stop(); // ֹͣ������Ƶ
        currentFadeCoroutine = null; // ������ɺ����Э�̱�־
    }
}
