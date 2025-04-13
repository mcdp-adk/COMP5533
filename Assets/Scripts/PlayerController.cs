using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 控制玩家角色的移动、攻击和道具交互
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerController : MonoBehaviour, ICharacter
{
    #region Fields

    // 组件引用
    private Transform _cameraTransform;
    private Animator _animator;
    private PlayerInput _playerInput;
    private CharacterController _controller;
    private Collider _collider;

    // 玩家输入
    private InputAction _moveAction;
    private InputAction _attackAction;
    private InputAction _dropAction;
    private InputAction _crouchAction;
    private InputAction _meleeAction;
    private Vector2 _moveInput;

    [Header("Player Settings")]
    [SerializeField] private Image _healthBar;
    [SerializeField] private Gradient _healthBarGradient; // 生命值渐变色
    [SerializeField] private float _fillSpeed = 0.5f;   // 生命值改变速度
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _crouchSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private int _maxHealth = 100;
    private float _moveSpeed; // 当前移动速度
    private float _moveRate;
    private int _health;
    private bool _isDead = false;
    private bool _isCrouching = false;

    [Header("Melee Settings")]
    [SerializeField] private float _meleeCooldown = 1.5f; // 近战攻击冷却时间(秒)
    [SerializeField] private int _meleeDamage = 100; // 近战攻击伤害值
    [SerializeField] private float _meleeRange = 1.5f; // 近战攻击范围
    [SerializeField] private LayerMask _whatIsEnemy; // 敌人所在层
    private float _meleeTimer = 0f; // 近战冷却计时器
    private bool _meleeOnCooldown = false; // 近战是否在冷却中

    [Header("Props Settings")]
    [SerializeField] private LayerMask _whatIsProps; // 设置可拾取道具的图层
    [SerializeField] private Transform _propPosition; // 道具挂载点
    private GameObject _currentProp; // 当前持有的道具
    private PropsBasicAction _currentPropAction; // 当前道具的脚本引用
    private bool _isPropEquiped = false; // 记录是否已经装备了道具

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _controller = GetComponent<CharacterController>();
        _collider = GetComponent<BoxCollider>();

        // 如果没有指定道具挂载点，尝试查找
        if (_propPosition == null)
        {
            _propPosition = transform.Find("PropPosition");
            if (_propPosition == null)
            {
                Debug.LogWarning("未找到道具挂载点，请手动指定或创建名为'PropPosition'的子物体");
            }
        }

        // 获取输入动作
        _moveAction = _playerInput.actions["Move"];
        _attackAction = _playerInput.actions["Attack"];
        _dropAction = _playerInput.actions["Drop"];
        _crouchAction = _playerInput.actions["Crouch"];
        _meleeAction = _playerInput.actions["Melee"];
    }

    private void OnEnable()
    {
        RegisterInputEvents();
    }

    private void OnDisable()
    {
        UnregisterInputEvents();
    }

    private void Start()
    {
        // 获取主相机的引用
        _cameraTransform = Camera.main.transform;

        // 初始化生命值
        _health = _maxHealth;

        // 设置初始速度
        _moveSpeed = _runSpeed;
    }

    private void Update()
    {
        if (_isDead) return; // 如果玩家已死亡，停止更新

        HandleInputs();
        HandleMovement();
        HandleGravity();
        HandleMeleeCooldown();

        // 更新动画参数
        UpdateAnimationParameters();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否是道具且当前未持有道具
        if (((1 << other.gameObject.layer) & _whatIsProps) != 0 && _currentProp == null)
        {
            PickupProp(other.gameObject);
        }
    }

    #endregion

    #region Inputs

    private void RegisterInputEvents()
    {
        _attackAction.started += OnAttackPressed;
        _attackAction.canceled += OnAttackReleased;
        _dropAction.canceled += ToggleDrop;
        _crouchAction.started += ToggleCrouch;
        _crouchAction.canceled += ToggleCrouch;
        _meleeAction.performed += ToggleMelee;
    }

    private void UnregisterInputEvents()
    {
        _attackAction.started -= OnAttackPressed;
        _attackAction.canceled -= OnAttackReleased;
        _dropAction.canceled -= ToggleDrop;
        _crouchAction.started -= ToggleCrouch;
        _crouchAction.canceled -= ToggleCrouch;
        _meleeAction.performed -= ToggleMelee;
    }

    /// <summary>
    /// 处理玩家输入
    /// </summary>
    private void HandleInputs()
    {
        _moveInput = _moveAction.ReadValue<Vector2>();
    }

    /// <summary>
    /// 处理攻击按下输入
    /// </summary>
    private void OnAttackPressed(InputAction.CallbackContext ctx)
    {
        // 如果持有道具，激活道具
        if (_currentPropAction != null)
        {
            Debug.Log($"激活道具: {_currentProp.name}");
            _currentPropAction.ActivateButtonPressed();
            _isPropEquiped = true; // 设置为已装备道具
        }
    }

    /// <summary>
    /// 处理攻击释放输入
    /// </summary>
    private void OnAttackReleased(InputAction.CallbackContext ctx)
    {
        // 只有当攻击开始于持有物体时才执行释放逻辑
        if (_isPropEquiped && _currentPropAction != null)
        {
            Debug.Log($"释放道具: {_currentProp.name}");
            _currentPropAction.ActivateButtonRelease();
            _currentProp = null;
            _currentPropAction = null;
            _isPropEquiped = false; // 重置标志
        }
    }

    /// <summary>
    /// 丢弃道具
    /// </summary>
    private void ToggleDrop(InputAction.CallbackContext ctx)
    {
        if (_currentPropAction != null)
        {
            Debug.Log($"丢弃道具: {_currentProp.name}");
            _currentPropAction.DropFunction();
            _currentProp = null;
            _currentPropAction = null;
            _isPropEquiped = false;
        }
    }

    /// <summary>
    /// 切换蹲伏状态
    /// </summary>
    private void ToggleCrouch(InputAction.CallbackContext ctx)
    {
        _isCrouching = !_isCrouching;
        _collider.enabled = !_isCrouching;
        _moveSpeed = _isCrouching ? _crouchSpeed : _runSpeed;
        _animator.SetBool("isCrouching", _isCrouching);
    }

    /// <summary>
    /// 执行近战攻击
    /// </summary>
    private void ToggleMelee(InputAction.CallbackContext ctx)
    {
        // 如果在冷却中，不执行攻击
        if (_meleeOnCooldown)
        {
            Debug.Log($"近战攻击冷却中，剩余 {_meleeTimer:F1} 秒");
            return;
        }

        PerformMeleeAttack();
    }

    #endregion

    #region Movement

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

        // 计算持有物体对移动速度的影响
        _moveRate = _currentPropAction != null ? 1f / _currentPropAction.GetWeight() : 1f;

        // 应用移动
        _controller.Move(moveDirection * _moveSpeed * _moveRate * Time.deltaTime);

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
    private void HandleGravity()
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
    private void UpdateAnimationParameters()
    {
        _animator.SetFloat("moveSpeed", _moveInput.magnitude);

        float healthPercentage = (float)_health / _maxHealth;
        _healthBar.fillAmount = healthPercentage; // 更新生命值UI
        _healthBar.color = _healthBarGradient.Evaluate(healthPercentage); // 更新生命值渐变色
    }

    #endregion

    #region Combat

    /// <summary>
    /// 执行近战攻击
    /// </summary>
    private void PerformMeleeAttack()
    {
        // 触发拳击动画，实际伤害将由动画事件触发
        _animator.SetTrigger("triggerPunch");
        
        // 启动冷却
        _meleeOnCooldown = true;
        _meleeTimer = _meleeCooldown;
        Debug.Log("近战攻击已执行，进入冷却");
    }

    /// <summary>
    /// 由拳击动画中的事件触发，在动画中的特定时间点造成伤害
    /// </summary>
    private void OnPunchTriggered()
    {
        // 检测前方是否有敌人
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * (_meleeRange / 2), _meleeRange / 2, _whatIsEnemy);

        // 对命中的敌人造成伤害
        foreach (Collider enemy in hitEnemies)
        {
            ICharacter character = enemy.GetComponent<ICharacter>();
            if (character != null)
            {
                character.CauseDamage(_meleeDamage);
                Debug.Log($"近战攻击命中: {enemy.name}，造成 {_meleeDamage} 点伤害");
            }
        }
    }

    /// <summary>
    /// 处理近战攻击冷却
    /// </summary>
    private void HandleMeleeCooldown()
    {
        if (_meleeOnCooldown)
        {
            _meleeTimer -= Time.deltaTime;
            if (_meleeTimer <= 0)
            {
                _meleeOnCooldown = false;
            }
        }
    }

    #endregion

    #region Props

    /// <summary>
    /// 拾取道具
    /// </summary>
    private void PickupProp(GameObject prop)
    {
        _currentProp = prop;
        _currentPropAction = _currentProp.GetComponent<PropsBasicAction>();

        if (_currentPropAction != null)
        {
            _currentPropAction.PickUpFunction(_propPosition != null ? _propPosition : transform);
            Debug.Log($"拾取道具: {_currentProp.name}");
        }
        else
        {
            Debug.LogWarning($"道具 {prop.name} 缺少 PropsBasicAction 组件");
            _currentProp = null;
        }
    }

    #endregion

    #region Health

    /// <summary>
    /// 设置生命值并触发事件
    /// </summary>
    private void SetHealth(int health)
    {
        int oldHealth = _health;
        _health = health;

        OnHealthChanged?.Invoke();

        if (_health > oldHealth)
        {
            OnHealthIncreased?.Invoke();
        }
        else if (_health < oldHealth)
        {
            OnHealthDecreased?.Invoke();
        }
    }

    /// <summary>
    /// 处理玩家死亡逻辑
    /// </summary>
    private void HandlePlayerDeath()
    {
        if (_isDead) return; // 防止重复执行死亡逻辑

        _isDead = true;
        Debug.Log($"玩家 {gameObject.name} 死亡");

        // 设置死亡动画
        _animator.SetBool("isDead", true);

        // 禁用控制
        _controller.enabled = false;
        _playerInput.enabled = false;

        // 丢弃当前道具
        if (_currentPropAction != null)
        {
            _currentPropAction.DropFunction();
            _currentProp = null;
            _currentPropAction = null;
        }

        // 触发死亡事件
        OnCharacterDeath?.Invoke();
    }

    /// <summary>
    /// 重置玩家状态和位置
    /// </summary>
    public void Respawn(Vector3 respawnPosition)
    {
        transform.position = respawnPosition;
        SetHealth(_maxHealth);
        _isDead = false;
        _isCrouching = false;
        _controller.enabled = true;
        _playerInput.enabled = true;
        _animator.SetBool("isDead", false);
        _moveSpeed = _runSpeed;
    }

    #endregion

    #region ICharacter

    public int MaxHealth
    {
        get { return _maxHealth; }
        set
        {
            if (value < 0) value = 0;
            _maxHealth = value;

            // 如果当前生命值超过新的最大值，调整当前生命值
            if (_health > _maxHealth)
            {
                SetHealth(_maxHealth);
            }
        }
    }

    public int Health { get { return _health; } }

    public void CauseDamage(int damageAmount)
    {
        if (_isDead || damageAmount <= 0) return;

        SetHealth(Math.Max(_health - damageAmount, 0));
        Debug.Log($"玩家 {gameObject.name} 受到了 {damageAmount} 点伤害，剩余生命值: {_health}");

        if (_health <= 0)
        {
            HandlePlayerDeath();
        }
    }

    public void Heal(int healAmount)
    {
        if (_isDead || healAmount <= 0) return;

        SetHealth(Math.Min(_health + healAmount, _maxHealth));
        Debug.Log($"玩家 {gameObject.name} 恢复了 {healAmount} 点生命值，当前生命值: {_health}");
    }

    public event Action OnHealthChanged;
    public event Action OnHealthIncreased;
    public event Action OnHealthDecreased;
    public event Action OnCharacterDeath;

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        // 绘制近战攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * (_meleeRange / 2), _meleeRange / 2);
    }

    #endregion
}
