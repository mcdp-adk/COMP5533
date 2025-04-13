using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; // ����Cinemachine�����ռ�

public class CinemachinePlayerTracker : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;


    void Update()
    {
        // ��̬����Ŀ����
        UpdateTargetGroup();

    }

    void UpdateTargetGroup()
    {
        // �������д���Player��ǩ�Ķ���
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // ���Ŀ����
        targetGroup.m_Targets = new CinemachineTargetGroup.Target[players.Length];

        // �����������ӵ�Ŀ����
        for (int i = 0; i < players.Length; i++)
        {
            targetGroup.m_Targets[i].target = players[i].transform;
            targetGroup.m_Targets[i].weight = 1f;
            targetGroup.m_Targets[i].radius = 0f;
        }
    }

   
}
