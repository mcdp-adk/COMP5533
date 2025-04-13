using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSceneCamera : MonoBehaviour
{
    // 用于储存摄像机
    public Camera camera;

    // 用于储存玩家的位置
    public Transform player;

    // 记录初始时玩家与摄像机之间的位移
    private Vector3 initialOffset;

    // 在游戏开始时调用
    void Start()
    {
        if (camera == null)
        {
            Debug.LogError("Camera 未分配，请在Inspector中分配摄像机变量！");
        }
        if (player == null)
        {
            Debug.LogError("Player 未分配，请在Inspector中分配玩家变量！");
        }

        // 确保摄像机是正交模式
        if (camera != null && !camera.orthographic)
        {
            Debug.LogError("摄像机需要设置为Orthographic模式！");
        }

        // 计算初始位移
        if (camera != null && player != null)
        {
            initialOffset = camera.transform.position - player.position;
        }
    }

    // 每帧更新
    void Update()
    {
        if (camera != null && player != null)
        {
            // 根据玩家的位置和初始位移，更新摄像机的位置
            Vector3 updatedPosition = player.position + initialOffset;
            camera.transform.position = new Vector3(updatedPosition.x, camera.transform.position.y, updatedPosition.z);
        }
    }
}
