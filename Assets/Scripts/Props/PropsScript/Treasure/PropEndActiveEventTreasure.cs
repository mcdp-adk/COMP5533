using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropEndActiveEventTreasure : MonoBehaviour
{
    [Header("TreasureSetting")]
    [SerializeField] private int scoreToAdd = 1; // ÿ�����ӵĵ÷���
    [SerializeField] private AudioSource treasureSound; // ��Ч�ļ�
    [SerializeField] private float fadeDuration = 5f; // �����������ʱ��
    [SerializeField] private bool isTriggerable = true;

    public void TreasureInSafeHouse()
    {
        SendMsgToGameMaster();

    }

    private void SendMsgToGameMaster()
    {
        // ��� GameManager �Ƿ���ڲ������䷽�����ӵ÷�
        if (PropGameManager.Instance != null & isTriggerable)
        {
            PropGameManager.Instance.AddScore(scoreToAdd); // ȷ�� GameManager ��һ�� AddScore(int score) ����
            isTriggerable = false;
            Debug.Log("�����ѽ��밲ȫ�ݣ��÷������ӣ�");
        }
        else
        {
            Debug.LogWarning("GameManager δ�ҵ����޷����ӵ÷֣�");
        }
    }

    private void TreasurePlayMusic()
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
