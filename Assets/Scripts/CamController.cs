using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; // 导入Cinemachine命名空间

public class CinemachinePlayerTracker : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;


    void Update()
    {
        // 动态更新目标组
        UpdateTargetGroup();

    }

    void UpdateTargetGroup()
    {
        // 查找所有带有Player标签的对象
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // 清空目标组
        targetGroup.m_Targets = new CinemachineTargetGroup.Target[players.Length];

        // 将所有玩家添加到目标组
        for (int i = 0; i < players.Length; i++)
        {
            targetGroup.m_Targets[i].target = players[i].transform;
            targetGroup.m_Targets[i].weight = 1f;
            targetGroup.m_Targets[i].radius = 0f;
        }
    }

   
}
