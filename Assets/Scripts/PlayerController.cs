using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector2 moveInput;
    private Animator animator;
    private CharacterController controller;
    // 定义玩家死亡事件
    public static event Action OnPlayerDeath;

    private bool isDead = false; // 标志位

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private int playerIndex = 0; // 玩家索引，用于多人游戏

    #region Unity Callbacks

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (isDead) return; // 如果玩家已死亡，停止更新
        // 处理玩家输入
        HandleMovement();
        ApplyGravity();
        UpdateAnimator();
    }

    #endregion

    #region Player Input Callbacks

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    #endregion

    #region Public Methods

    public void SetPlayerIndex(int index)
    {
        playerIndex = index;
    }
    public void Die()
    {
        if (isDead) return; // 如果玩家已死亡，直接返回

        isDead = true; // 设置标志位
        // 触发玩家死亡事件
        OnPlayerDeath?.Invoke();
        // 销毁玩家对象
        Destroy(gameObject);
    }

    #endregion

    #region Private Methods

    private void HandleMovement()
    {
        if (moveInput.magnitude <= 0.1f || cameraTransform == null) return;

        // 获取相机的前方和右方向（忽略Y轴）
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // 根据输入和相机方向计算移动向量
        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        // 应用移动
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // 使角色面向移动方向
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveDirection),
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void ApplyGravity()
    {
        Vector3 gravityVector = new Vector3(0, gravity, 0);
        controller.Move(gravityVector * Time.deltaTime);

        // 如果角色在地面上，重置垂直速度
        if (controller.isGrounded)
        {
            gravityVector.y = 0;
        }
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("moveSpeed", moveInput.magnitude);
    }

    #endregion
}
