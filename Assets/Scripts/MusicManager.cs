using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource chaseMusicAudioSource;
    [SerializeField] private float fadeDuration = 1f; // 音量淡入/淡出持续时间
    public bool isChasing = false; // 是否处于追逐状态

    private Coroutine currentFadeCoroutine = null; // 用于跟踪当前正在运行的协程

    private void Update()
    {
        if (isChasing && !chaseMusicAudioSource.isPlaying)
        {
            // 如果正在执行淡出，取消它
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
                currentFadeCoroutine = null;
            }
            // 开始淡入
            currentFadeCoroutine = StartCoroutine(FadeIn(chaseMusicAudioSource, fadeDuration));
        }
        else if (!isChasing && chaseMusicAudioSource.isPlaying)
        {
            // 如果正在执行淡入，取消它
            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
                currentFadeCoroutine = null;
            }
            // 开始淡出
            currentFadeCoroutine = StartCoroutine(FadeOut(chaseMusicAudioSource, fadeDuration));
        }
    }

    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume; // 当前音量作为初始值
        audioSource.Play(); // 开始播放音频

        float elapsedTime = 0f; // 用于追踪时间的变量
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime; // 增加经过的时间
            audioSource.volume = Mathf.Clamp01(startVolume + (elapsedTime / duration)); // 按比例增加音量
            yield return null;
        }

        audioSource.volume = 1f; // 最终音量为1
        currentFadeCoroutine = null; // 淡入完成后清空协程标志
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume; // 当前音量作为初始值
        float elapsedTime = 0f; // 用于追踪时间的变量

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime; // 增加经过的时间
            audioSource.volume = Mathf.Clamp01(startVolume * (1 - (elapsedTime / duration))); // 按比例减少音量
            yield return null;
        }

        audioSource.volume = 0f; // 确保音量完全为0
        audioSource.Stop(); // 停止播放音频
        currentFadeCoroutine = null; // 淡出完成后清空协程标志
    }
}
