using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform cameraTransform; // 2.5D视角摄像机
    private CharacterController controller;

    // 定义玩家死亡事件
    public static event Action OnPlayerDeath;

    private bool isDead = false; // 标志位

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (isDead) return; // 如果玩家已死亡，停止更新

        // WASD输入控制移动
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 根据摄像机方向调整移动方向
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 move = moveDirection.x * cameraTransform.right + moveDirection.z * cameraForward;
        move *= moveSpeed * Time.deltaTime;

        controller.Move(move);
    }

    // 玩家死亡处理方法
    public void Die()
    {
        if (isDead) return; // 如果玩家已死亡，直接返回

        isDead = true; // 设置标志位
        // 触发玩家死亡事件
        OnPlayerDeath?.Invoke();
        // 销毁玩家对象
        Destroy(gameObject);
    }
}
