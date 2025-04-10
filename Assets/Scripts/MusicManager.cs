using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource chaseMusicAudioSource;
    [SerializeField] private float fadeDuration = 1f;
    public bool isChasing = false;

    private void Update()
    {
        if (isChasing && !chaseMusicAudioSource.isPlaying)
        {
            StartCoroutine(FadeIn(chaseMusicAudioSource, fadeDuration));
        }
        else if (!isChasing && chaseMusicAudioSource.isPlaying)
        {
            StartCoroutine(FadeOut(chaseMusicAudioSource, fadeDuration));
        }
    }

    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        float startVolume = 0f;
        audioSource.volume = startVolume;
        audioSource.Play();

        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.deltaTime / duration;
            yield return null;
        }

        audioSource.volume = 1f;
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}
