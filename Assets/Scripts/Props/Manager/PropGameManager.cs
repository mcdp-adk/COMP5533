using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropGameManager : MonoBehaviour
{
    public static PropGameManager Instance;

    private int score;

    private void Awake()
    {
        Instance = this; // 设置静态实例
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        Debug.Log($"当前得分: {score}");
    }

    public int GetScore()
    {
        return score;
    }
}
