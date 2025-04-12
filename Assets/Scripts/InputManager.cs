using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // 组件引用
    private PlayerInputManager playerInputManager;

    // 玩家引用
    [SerializeField] private GameObject playerPrefab_1;
    [SerializeField] private GameObject playerPrefab_2;

    // 临时存储
    private InputAction anyAction;
    private InputDevice lastDevice;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();

        anyAction = new();
        anyAction.AddBinding("<Keyboard>/anyKey"); // 监听任意按键
        anyAction.AddBinding("<Gamepad>/*"); // 监听任意手柄操作
    }

    #region Public Methods

    public void StartBinding()
    {
        Debug.Log("请按下按键连接设备...");

        anyAction.performed += GetLastDevice; // 监听按键事件
        anyAction.Enable(); // 启用全局输入事件
    }

    public void StopBinding()
    {
        Debug.Log("停止连接设备...");

        anyAction.Disable(); // 禁用全局输入事件
        anyAction.performed -= GetLastDevice; // 取消监听按键事件
        Debug.Log($"最后连接的设备: {lastDevice.displayName}");
    }

    public void SpawnPlayer()
    {
        Debug.Log("生成玩家...");

        if (lastDevice == null) return; // 如果没有设备则返回

        // 生成玩家
        GameObject player = playerInputManager.playerPrefab = playerPrefab_1;
        playerInputManager.JoinPlayer(-1, -1, null, lastDevice); // 加入玩家
        Debug.Log($"玩家已加入: {player.name}"); // 输出玩家信息

        lastDevice = null; // 重置最后连接的设备
    }

    #endregion

    #region Private Methods

    private void GetLastDevice(InputAction.CallbackContext ctx)
    {
        lastDevice = ctx.control.device; // 获取最后一个按下的设备
        Debug.Log($"最后连接的设备: {lastDevice.displayName}");
    }

    #endregion
}
