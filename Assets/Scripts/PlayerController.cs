using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 控制玩家角色的移动、攻击和道具交互
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Fields

    // 组件引用
    private Transform _cameraTransform;
    private Animator _animator;
    private PlayerInput _playerInput;
    private CharacterController _controller;

    // 玩家输入
    private InputAction _moveAction;
    private InputAction _attackAction;
    private InputAction _dropAction;
    private Vector2 _moveInput;

    // 持有道具
    private GameObject _currentProp; // 当前持有的道具
    private PropsBasicAction _currentPropAction; // 当前道具的脚本引用
    private bool _isPropEquiped = false; // 记录是否已经装备了道具

    // 处理死亡
    private bool _isDead = false;

    [Header("Player Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Props Settings")]
    [SerializeField] private LayerMask _whatIsProps; // 设置可拾取道具的图层

    #endregion

    #region Properties



    #endregion

    #region Methods: Unity Callbacks

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();

        _moveAction = _playerInput.actions["Move"];
        _attackAction = _playerInput.actions["Attact"];
        _dropAction = _playerInput.actions["Drop"];
    }

    private void Start()
    {
        // 获取主相机的引用
        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (_isDead) return; // 如果玩家已死亡，停止更新

        HandleInputs();
        HandleMovement();
        ApplyGravity();
        UpdateAnimator();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_currentProp != null)
        {
            Debug.Log($"已持有道具 {_currentProp.name}，无法拾取新道具");
            return; // 如果当前已有道具，直接返回
        }

        if (((1 << other.gameObject.layer) & _whatIsProps) != 0)
        {
            _currentProp = other.gameObject; // 记录当前道具
            _currentPropAction = _currentProp.GetComponent<PropsBasicAction>(); // 获取道具脚本引用
            _currentPropAction.PickUpFunction(this.transform.Find("PropPosition").transform);
            _isPropEquiped = true; // 设置道具已装备标志
            Debug.Log($"拾取道具: {_currentProp.name}");
        }
    }

    #endregion

    #region Methods: Public

    /// <summary>
    /// 处理玩家死亡逻辑
    /// </summary>
    public void Die()
    {
        if (_isDead)
        {
            Debug.LogWarning($"玩家 {gameObject.name} 已经死亡，无法再次触发死亡");
            return; // 如果玩家已死亡，直接返回
        }

        _isDead = true; // 设置标志位
        Debug.Log($"玩家 {gameObject.name} 死亡");

        // 触发玩家死亡事件
        OnPlayerDeath?.Invoke();
        Debug.Log("已触发OnPlayerDeath事件");

        // 销毁玩家对象
        Destroy(gameObject);
    }

    #endregion

    #region Methods: Private

    /// <summary>
    /// 处理玩家输入
    /// </summary>
    private void HandleInputs()
    {
        // 获取输入值
        _moveInput = _moveAction.ReadValue<Vector2>();

        // 处理攻击和投掷输入
        if (_attackAction.WasPressedThisFrame())
        {
            // 记录攻击开始时是否持有物体
            if (_currentPropAction != null)
            {
                Debug.Log($"激活道具: {_currentProp.name}");
                _currentPropAction.ActivateButtonPressed();
            }
        }

        if (_attackAction.WasReleasedThisFrame())
        {
            // 只有当攻击开始于持有物体时才执行释放逻辑
            if (_isPropEquiped)
            {
                if (_currentPropAction != null)
                {
                    Debug.Log($"释放道具: {_currentProp.name}");
                    _currentPropAction.ActivateButtonRelease();
                    _currentProp = null; // 清空当前道具引用
                    _currentPropAction = null; // 清空道具脚本引用
                }
            }
            _isPropEquiped = false; // 重置标志
        }

        if (_dropAction.WasPressedThisFrame())
        {
            if (_currentPropAction != null)
            {
                Debug.Log($"丢弃道具: {_currentProp.name}");
                _currentPropAction.DropFunction();
                _currentProp = null; // 清空当前道具引用
                _currentPropAction = null; // 清空道具脚本引用
            }
            else
            {
                Debug.Log("尝试丢弃道具，但当前未持有道具");
            }
        }
    }

    /// <summary>
    /// 处理玩家移动
    /// </summary>
    private void HandleMovement()
    {
        if (_moveInput.magnitude <= 0.1f || _cameraTransform == null) return;

        // 获取相机的前方和右方向（忽略Y轴）
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // 根据输入和相机方向计算移动向量
        Vector3 moveDirection = forward * _moveInput.y + right * _moveInput.x;

        // 应用移动
        _controller.Move(moveDirection * _moveSpeed * Time.deltaTime);

        // 使角色面向移动方向
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveDirection),
                _rotationSpeed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// 应用重力
    /// </summary>
    private void ApplyGravity()
    {
        Vector3 gravityVector = new Vector3(0, _gravity, 0);
        _controller.Move(gravityVector * Time.deltaTime);

        // 如果角色在地面上，重置垂直速度
        if (_controller.isGrounded)
        {
            gravityVector.y = 0;
        }
    }

    /// <summary>
    /// 更新动画参数
    /// </summary>
    private void UpdateAnimator()
    {
        _animator.SetFloat("moveSpeed", _moveInput.magnitude);
    }

    #endregion

    #region Events

    /// <summary>
    /// 玩家死亡事件，当玩家死亡时触发
    /// </summary>
    public static event Action OnPlayerDeath;

    #endregion
}
