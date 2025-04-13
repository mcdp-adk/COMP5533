using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropStartActiveEventTreasureMusic : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("TreasureSetting")]
    [SerializeField] private AudioSource treasureSound; // 音效文件
    [SerializeField] private float fadeDuration = 2f; // 声音渐大持续时间
    public void TreasurePlayMusic()
    {
        if (treasureSound != null)
        {
            treasureSound.Play(); // 播放音效
            StartCoroutine(FadeInAudio());
            Debug.Log("宝藏已进入安全屋，音效正在渐渐变大！");
        }
        else
        {
            Debug.LogWarning("未设置宝藏音效！");
        }
    }

    private IEnumerator FadeInAudio()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            treasureSound.volume = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration); // 从 0 增大到 1
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }
        treasureSound.volume = 1f; // 确保音量最终设置为最大值
    }
}
