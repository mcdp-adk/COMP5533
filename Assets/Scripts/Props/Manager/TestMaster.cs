using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMaster : MonoBehaviour
{
    [Header("References")]
    public PropsBasicAction propsAction; // 绑定 `PropsBasicAction` 组件
    public Transform targetBindObj;

    void Start()
    {
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
        ThrowOutTest();
        BindTest();
    }

    private void ThrowOutTest()
    {
        if (propsAction != null)
        {
            // 当鼠标左键按下时，调用 ActivateButtonPressed
            if (Input.GetMouseButtonDown(0))
            {
                propsAction.ActivateButtonPressed();
                Debug.Log("鼠标左键按下：调用 ActivateButtonPressed()");
            }

            // 当鼠标左键松开时，调用 ActivateButtonRelease
            if (Input.GetMouseButtonUp(0))
            {
                propsAction.ActivateButtonRelease();
                Debug.Log("鼠标左键松开：调用 ActivateButtonRelease()");
            }
        }
        else
        {
            Debug.LogWarning("PropsBasicAction 未绑定，请在 Inspector 中指定对象！");
        }
    }

    private void BindTest()
    {
        if (targetBindObj != null)
        {
            // 当鼠标左键按下时，调用 ActivateButtonPressed
            if (Input.GetKeyDown(KeyCode.Q))
            {
                propsAction.PickUpFunction(targetBindObj);
                Debug.Log("鼠标左键按下：调用 ActivateButtonPressed()");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                propsAction.DropFunction();
                Debug.Log("鼠标左键按下：调用 ActivateButtonPressed()");
            }
        }
    }
}

