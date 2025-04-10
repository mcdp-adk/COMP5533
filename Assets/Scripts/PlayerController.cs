using OpenCover.Framework.Model;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector2 moveInput;
    private Animator animator;
    private CharacterController controller;

    private GameObject currentProp; // 当前持有的道具

    [SerializeField] private Vector3 cameraOffset = new(20f, 16f, -20f); // 测试摄像机 - 偏移量

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 10f;
    // [SerializeField] private int playerIndex = 0; // 玩家索引，用于多人游戏

    [Header("Props Settings")]
    [SerializeField] private LayerMask whatIsProps; // 设置地面图层

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
        HandleMovement();
        ApplyGravity();
        UpdateAnimator();

        CameraFollowPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & whatIsProps) != 0)
        {
            currentProp = other.gameObject; // 记录当前道具
            currentProp.GetComponent<PropsBasicAction>().PickUpFunction(this.transform.Find("PropPosition").transform);
        }
    }

    #endregion

    #region Player Input Callbacks

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    #endregion

    #region Public Methods

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

    /// <summary>
    /// 仅供测试：使摄像机跟随玩家
    /// </summary>
    private void CameraFollowPlayer()
    {
        if (cameraTransform != null && controller.isGrounded)
        {
            Vector3 targetPosition = transform.position + cameraOffset;
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * 2f);
        }
    }

    #endregion
}
