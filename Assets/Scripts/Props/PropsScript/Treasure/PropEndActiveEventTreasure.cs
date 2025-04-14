using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropEndActiveEventTreasure : MonoBehaviour
{
    [Header("TreasureSetting")]
    [SerializeField] private int scoreToAdd = 1; // 每次增加的得分量
    [SerializeField] private AudioSource treasureSound; // 音效文件
    [SerializeField] private float fadeDuration = 5f; // 声音渐大持续时间
    [SerializeField] private bool isTriggerable = true;

    public void TreasureInSafeHouse()
    {
        SendMsgToGameMaster();

    }

    private void SendMsgToGameMaster()
    {
        // 检查 GameManager 是否存在并调用其方法增加得分
        if (PropGameManager.Instance != null & isTriggerable)
        {
            PropGameManager.Instance.AddScore(scoreToAdd); // 确保 GameManager 有一个 AddScore(int score) 方法
            isTriggerable = false;
            Debug.Log("宝藏已进入安全屋，得分已增加！");
        }
        else
        {
            Debug.LogWarning("GameManager 未找到，无法增加得分！");
        }
    }

    private void TreasurePlayMusic()
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
