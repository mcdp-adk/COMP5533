using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMaster : MonoBehaviour
{
    [Header("References")]
    public PropsBasicAction propsAction; // 绑定 `PropsBasicAction` 组件
    public Transform targetBindObj; // 用于绑定目标对象
    public bool isNeedTesting = true; // 是否启用测试功能

    void Start()
    {
        // 检查 PropsBasicAction 是否已绑定
        if (propsAction == null)
        {
            Debug.LogError("PropsBasicAction 未赋值，请在 Inspector 中绑定对象！");
        }
        else
        {
            Debug.Log("开始测试抛出脚本。");
        }
    }

    void Update()
    {
        if (isNeedTesting)
        {
            ThrowOutTest(); // 测试抛出功能
            BindTest();     // 测试绑定功能
        }
    }

    /// <summary>
    /// 测试抛出功能，检测鼠标左键按下或松开的动作。
    /// </summary>
    private void ThrowOutTest()
    {
        if (propsAction != null)
        {
            // 鼠标左键按下时调用 ActivateButtonPressed
            if (Input.GetMouseButtonDown(0))
            {
                propsAction.ActivateButtonPressed();
                Debug.Log("鼠标左键按下：调用 ActivateButtonPressed()");
            }

            // 鼠标左键松开时调用 ActivateButtonRelease
            if (Input.GetMouseButtonUp(0))
            {
                propsAction.ActivateButtonRelease();
                Debug.Log("鼠标左键松开：调用 ActivateButtonRelease()");
            }
        }
        else
        {
            // 如果未绑定 PropsBasicAction，关闭测试功能
            isNeedTesting = false;
            Debug.LogWarning("PropsBasicAction 未绑定，请在 Inspector 中指定对象！或目标物体已经消失，脚本运行已被关闭。");
        }
    }

    /// <summary>
    /// 测试绑定功能，检测按键 Q 和 E 的动作。
    /// </summary>
    private void BindTest()
    {
        if (targetBindObj != null)
        {
            // 按下 Q 键时调用 PickUpFunction
            if (Input.GetKeyDown(KeyCode.Q))
            {
                propsAction.PickUpFunction(targetBindObj);
                Debug.Log("按下 Q 键：调用 PickUpFunction()");
            }

            // 按下 E 键时调用 DropFunction
            if (Input.GetKeyDown(KeyCode.E))
            {
                propsAction.DropFunction();
                Debug.Log("按下 E 键：调用 DropFunction()");
            }
        }
    }
}
