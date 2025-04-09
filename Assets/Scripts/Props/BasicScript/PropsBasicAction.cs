using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PropsBasicAction : MonoBehaviour
{
    [Header("Reference")]
    private Rigidbody rb;

    [Header("Situation")]
    public bool isEnableGrivaty = true;

    [Header("Parameters")]
    public LayerMask whatIsTriggerLayer; // 设置触发图层
    public UnityEvent<float> onTriggeredAction; // 允许传入按键持续时间

    [Header("BindWithCharacter")]
    private Transform bindTargetPoint;
    private bool isBound = false;

    [Header("ActiveFunction")]
    private float buttonPressStartTime; // 记录按键按下的时间戳
    private bool isButtonPressed = false; // 记录按钮是否被按下

    [Header("Statement")]
    private PropState currentState = PropState.Default; // 物体的当前状态

    private enum PropState
    {
        Default, // 默认状态
        Held,    // 被持有状态
        Activated, // 被激活状态
        Block    // 被抛出状态
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        HandleBounds();
        HandleGrivaty();
    }

    #region Runtime Working
    /// <summary>
    /// 处理绑定目标点的逻辑
    /// </summary>
    private void HandleBounds()
    {
        if (isBound && bindTargetPoint != null)
        {
            transform.position = bindTargetPoint.position;
            transform.rotation = bindTargetPoint.rotation;
        }
    }
    #endregion

    #region Situation Handle
    private void HandleGrivaty()
    {
        if (isEnableGrivaty)
        {
            rb.useGravity = true;
        }
        else
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero; // 防止物体在拾取时有残留速度
            rb.angularVelocity = Vector3.zero; // 清除旋转速度
        }
    }
    #endregion

    #region ActiveButton
    /// <summary>
    /// 当按键被按下时调用
    /// </summary>
    public void ActivateButtonPressed()
    {
        buttonPressStartTime = Time.time; // 记录当前时间
        isButtonPressed = true;
    }

    /// <summary>
    /// 当按键松开或按下时间过长后调用
    /// </summary>
    public void ActivateButtonRelease()
    {
        if (isButtonPressed)
        {
            float pressDuration = Time.time - buttonPressStartTime; // 计算按住时间
            Debug.Log("激活按键按下的时间是：" + pressDuration);
            isButtonPressed = false;
            ActivateButtonTriggered(pressDuration); // 触发事件并传入持续时间
        }
    }

    /// <summary>
    /// 触发绑定的 UnityEvent，并传入按钮按下时间
    /// </summary>
    private void ActivateButtonTriggered(float pressDuration)
    {
        onTriggeredAction?.Invoke(pressDuration);
        currentState = PropState.Activated; // 切换到“被激活”状态
    }
    #endregion

    #region PIckUp & Drop
    /// <summary>
    /// 绑定目标 Transform
    /// </summary>
    public void PickUpFunction(Transform target)
    {
        if (target != null && !isBound)
        {
            bindTargetPoint = target;
            isBound = true;
            isEnableGrivaty = false;
            currentState = PropState.Held; // 切换到“被持有”状态
        }
    }

    /// <summary>
    /// 解绑目标 Transform
    /// </summary>
    public void DropFunction()
    {
        isBound = false;
        isEnableGrivaty = true;
        bindTargetPoint = null;
        currentState = PropState.Default; // 切换回“默认”状态
    }
    #endregion
}
