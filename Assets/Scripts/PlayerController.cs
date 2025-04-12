using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 控制玩家角色的移动、攻击和道具交互
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, ICharacter
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


    [Header("Player Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private int _maxHealth = 100;
    private int _health;
    private bool _isDead = false;

    [Header("Props Settings")]
    [SerializeField] private LayerMask _whatIsProps; // 设置可拾取道具的图层

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

        // 初始化生命值
        _health = _maxHealth;
    }

    private void Update()
    {
        if (_isDead) return; // 如果玩家已死亡，停止更新

        HandleInputs();
        HandleMovement();
        ApplyGravity();

        _animator.SetFloat("moveSpeed", _moveInput.magnitude); // 更新动画参数
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
            Debug.Log($"拾取道具: {_currentProp.name}");
        }
    }

    #endregion

    #region Methods: Public

    /// <summary>
    /// 重置玩家状态和位置
    /// /// </summary>
    /// <param name="respawnPosition">重生位置</param>
    public void Respawn(Vector3 respawnPosition)
    {
        // 重置玩家位置和状态
        transform.position = respawnPosition;
        SetHealth(_maxHealth); // 重置生命值
        _isDead = false; // 重置死亡状态
        _controller.enabled = true; // 启用角色控制器
        _playerInput.enabled = true; // 启用玩家输入
        _animator.SetBool("isDead", false); // 重置死亡动画状态
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
                _isPropEquiped = true; // 设置为已装备道具
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
                _isPropEquiped = false; // 重置标志
            }
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
    /// 设置生命值并触发事件
    /// </summary>
    /// <param name="health">新的生命值</param>
    private void SetHealth(int health)
    {
        _health = health;
        Debug.Log($"玩家 {gameObject.name} 的生命值设置为 {_health}");

        OnHealthChanged?.Invoke(); // 触发生命值改变事件
        Debug.Log("已触发 OnHealthChanged 事件");
    }

    /// <summary>
    /// 处理玩家死亡逻辑
    /// </summary>
    private void HandlePlayerDeath()
    {
        _isDead = true; // 设置标志位
        Debug.Log($"玩家 {gameObject.name} 死亡");

        // 触发玩家死亡事件
        OnCharacterDeath?.Invoke();
        Debug.Log("已触发 OnCharacterDeath 事件");

        _animator.SetBool("isDead", true);

        _controller.enabled = false; // 禁用角色控制器以避免碰撞
        _playerInput.enabled = false; // 禁用玩家输入
    }

    #endregion

    #region ICharacter Implementation

    public int MaxHealth
    {
        get { return _maxHealth; }
        set
        {
            if (value < 0) value = 0; // 确保最大生命值不小于 0
            _maxHealth = value;
        }
    }

    public int Health { get { return _health; } }

    public void CauseDamage(int damageAmount)
    {
        if (_isDead) return; // 如果玩家已死亡，停止处理伤害

        if (damageAmount > 0)
        {
            SetHealth(Math.Max(_health - damageAmount, 0)); // 减少生命值，确保不小于0
            Debug.Log($"玩家 {gameObject.name} 受到了 {damageAmount} 点伤害");
            OnHealthDecreased?.Invoke(); // 触发生命值减少事件
            Debug.Log($"玩家 {gameObject.name} 受到了 {damageAmount} 点伤害");
        }
        else
        {
            Debug.LogWarning("伤害小于或等于 0，不造成伤害。");
        }

        if (_health <= 0)
        {
            HandlePlayerDeath(); // 调用死亡方法
        }
    }

    public void Heal(int healAmount)
    {
        if (_isDead) return; // 如果玩家已死亡，停止处理治疗

        if (healAmount > 0)
        {
            SetHealth(Math.Min(_health + healAmount, _maxHealth)); // 增加生命值，确保不超过最大生命值
            Debug.Log($"玩家 {gameObject.name} 治疗了 {healAmount} 点生命值");

            OnHealthIncreased?.Invoke(); // 触发生命值增加事件
            Debug.Log("已触发 OnHealthIncreased 事件");
        }
        else
        {
            Debug.LogWarning("治疗小于或等于 0，不进行治疗。");
        }
    }

    public event Action OnHealthChanged;
    public event Action OnHealthIncreased;
    public event Action OnHealthDecreased;
    public event Action OnCharacterDeath;

    #endregion
}
