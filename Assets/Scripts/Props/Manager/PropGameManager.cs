using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropGameManager : MonoBehaviour
{
    public static PropGameManager Instance;

    private int score;

    private void Awake()
    {
        Instance = this; // ���þ�̬ʵ��
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        Debug.Log($"��ǰ�÷�: {score}");
    }

    public int GetScore()
    {
        return score;
    }
}
