using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 为多人游戏管理玩家输入设备和玩家生成
/// </summary>
public class InputManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject[] _playerPrefabs = new GameObject[4]; // 玩家角色预制体

    private PlayerInputManager _playerInputManager; // 玩家输入管理器
    private PlayerInput[] _playerInputs = new PlayerInput[4];   // 玩家输入组件数组
    private InputAction _anyInputAction;    // 任意输入检测的 Action
    private InputDevice[] _boundDevices = new InputDevice[4]; // 每个玩家绑定的设备
    private InputDevice _lastActiveDevice;  // 上一个活动的输入设备

    private int _currentBindingPlayerIndex; // 当前正在绑定的玩家索引
    private bool _isBindingInProgress = false;  // 是否正在进行绑定
    private bool _deviceDetected = false;   // 是否检测到设备

    #endregion

    #region Properties

    /// <summary>
    /// 单例实例
    /// </summary>
    public static InputManager Instance { get; private set; }
    
    /// <summary>
    /// 获取所有玩家的输入组件（直接当 GameObject 使用，比如：对于数组中的某个元素 playerInput，playerInput.transform.position = new Vector3(0, 0, 0)）
    /// </summary>
    public PlayerInput[] PlayerInputs { get { return _playerInputs; } }

    #endregion

    #region Methods: Unity Callbacks

    private void Awake()
    {
        if (!InitializeSingleton()) return;

        _playerInputManager = GetComponent<PlayerInputManager>();

        // 创建用于检测任何输入的Action
        _anyInputAction = new InputAction();
        _anyInputAction.AddBinding("<Keyboard>/anyKey");
        _anyInputAction.AddBinding("<Gamepad>/*");
    }

    #endregion

    #region Methods: Public

    /// <summary>
    /// 开始为指定玩家绑定输入设备
    /// </summary>
    public void StartBinding(int playerIndex)
    {
        if (_isBindingInProgress)
        {
            Debug.LogWarning("已有绑定进行中，请先完成或取消当前绑定");
            return;
        }

        Debug.Log("请按任意键或手柄按钮进行绑定...");

        _currentBindingPlayerIndex = playerIndex;
        _anyInputAction.performed += DetectInputDevice;
        _anyInputAction.Enable();

        _isBindingInProgress = true;
    }

    /// <summary>
    /// 完成当前的设备绑定过程
    /// </summary>
    public void StopBinding()
    {
        if (!_isBindingInProgress)
        {
            Debug.LogWarning("没有正在进行的绑定");
            return;
        }

        if (!_deviceDetected)
        {
            Debug.LogWarning("未检测到任何输入设备，请先按下任意键");
            return;
        }

        // 检查设备是否已被其他玩家使用
        int conflictingPlayerIndex = FindPlayerUsingDevice(_lastActiveDevice, _currentBindingPlayerIndex);
        if (conflictingPlayerIndex != -1)
        {
            Debug.LogWarning($"绑定失败: 设备 {_lastActiveDevice.displayName} 已被玩家 {conflictingPlayerIndex} 使用");
            _boundDevices[_currentBindingPlayerIndex] = null;
            CleanupBinding();
            return;
        }

        // 绑定成功
        Debug.Log($"玩家 {_currentBindingPlayerIndex} 成功绑定设备: {_lastActiveDevice.displayName} (ID: {_lastActiveDevice.deviceId})");
        CleanupBinding();
    }

    /// <summary>
    /// 取消指定玩家的设备绑定
    /// </summary>
    public void CancelBinding(int playerIndex)
    {
        if (_isBindingInProgress)
        {
            Debug.LogWarning("请先完成当前绑定过程");
            return;
        }

        if (_boundDevices[playerIndex] == null)
        {
            Debug.LogWarning($"玩家 {playerIndex} 尚未绑定任何设备");
            return;
        }

        _boundDevices[playerIndex] = null;
        Debug.Log($"已取消玩家 {playerIndex} 的设备绑定");
    }

    /// <summary>
    /// 为所有已绑定设备的玩家生成角色
    /// </summary>
    public void SpawnPlayer()
    {
        if (_isBindingInProgress)
        {
            Debug.LogWarning("请先完成当前绑定过程");
            return;
        }

        for (int i = 0; i < _playerInputs.Length; i++)
        {
            if (_boundDevices[i] == null) continue;

            _playerInputManager.playerPrefab = _playerPrefabs[i];
            _playerInputs[i] = _playerInputManager.JoinPlayer(i, -1, null, _boundDevices[i]);
            Debug.Log($"玩家 {i} 已加入游戏，使用设备: {_boundDevices[i].displayName} (ID: {_boundDevices[i].deviceId})");
        }
    }

    #endregion

    #region Methods: Private

    /// <summary>
    /// 初始化单例模式
    /// </summary>
    /// <returns>如果当前实例是单例则返回true，否则返回false</returns>
    private bool InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 确保单例模式
            return false;
        }
        Instance = this; // 设置单例实例

        DontDestroyOnLoad(gameObject); // 不销毁此对象
        return true;
    }

    /// <summary>
    /// 检测输入设备
    /// </summary>
    /// <param name="ctx"></param>
    private void DetectInputDevice(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device == _lastActiveDevice) return;

        _deviceDetected = true;
        _lastActiveDevice = ctx.control.device;
        _boundDevices[_currentBindingPlayerIndex] = _lastActiveDevice;

        Debug.Log($"检测到设备: {_lastActiveDevice.displayName} (ID: {_lastActiveDevice.deviceId})");
    }

    /// <summary>
    /// 清理绑定状态
    /// </summary>
    private void CleanupBinding()
    {
        _anyInputAction.Disable();
        _anyInputAction.performed -= DetectInputDevice;
        _isBindingInProgress = false;
        _deviceDetected = false;
    }

    /// <summary>
    /// 查找使用指定设备的玩家索引
    /// </summary>
    /// <param name="device"></param>
    /// <param name="excludePlayerIndex"></param>
    /// <returns></returns>
    private int FindPlayerUsingDevice(InputDevice device, int excludePlayerIndex)
    {
        for (int i = 0; i < _boundDevices.Length; i++)
        {
            if (i != excludePlayerIndex && _boundDevices[i] == device)
            {
                return i;
            }
        }
        return -1;
    }

    #endregion
}