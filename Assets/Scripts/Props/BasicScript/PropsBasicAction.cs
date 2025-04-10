using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PropsBasicAction : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private List<Collider> componentCollidersBox; // 储存需要启用或禁用的碰撞箱
    private Rigidbody rb;

    [Header("Situation")]
    [SerializeField] private bool isEnableGrivaty = true;
    [SerializeField] private bool isTouchTriggerLayer = false;

    [Header("Setting")]
    [SerializeField] private float weight = 1.0f;
    [SerializeField] private float value = 1.0f;
    [SerializeField] private float minReActiveTime = 0.5f;  // 最小重置时间
    [SerializeField] private float maxReActiveTime = 5f;  // 最长激活时间
    [SerializeField] private float maxActiveDurationTime = 3f;  // 最长蓄力时间

    [Header("Parameters")]
    [SerializeField] private LayerMask whatIsTriggerLayer; // 设置触发图层
    [SerializeField] private Collider activeCheckCollider; // 外部传入的碰撞器
    [SerializeField] private UnityEvent<float> onTriggeredAction; // 允许传入按键持续时间
    [SerializeField] private UnityEvent endTriggeredAction; // 允许传入按键持续时间

    [Header("BindWithCharacter")]
    private Transform bindTargetPoint;

    [Header("ActiveFunction")]
    private float pressDurationTime = 0;
    private float activeStartTime = 0;
    private float buttonPressStartTime; // 记录按键按下的时间戳
    private bool isButtonPressed = false; // 记录按钮是否被按下

    public delegate void DestroyedHandler(GameObject destroyedObject);
    public event DestroyedHandler OnDestroyed;

    [Header("Statement")]
    [SerializeField] private bool isBlocked = false;
    [SerializeField] private bool isActived = false;
    [SerializeField] private bool isBound = false;
    [SerializeField] private PropState currentState = PropState.Default; // 物体的当前状态
    [SerializeField] private PropState lastState = PropState.Default;  // 物体的前置状态

    private enum PropState
    {
        Default, // 默认状态
        Held,    // 被持有状态
        Activated, // 被激活状态
        Block    // 被锁定状态
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleGrivaty();
        StatementStateHandler();
        StatementEffectHandler();
    }

    #region Runtime Working
    /// <summary>
    /// 处理绑定目标点的逻辑
    /// </summary>
    /// 
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & whatIsTriggerLayer) != 0)  // 检查是否属于指定图层
        {
            isTouchTriggerLayer = true;  // 设置标志
            Debug.Log("物体碰到了指定图层: " + whatIsTriggerLayer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & whatIsTriggerLayer) != 0)  // 检查是否属于指定图层
        {
            isTouchTriggerLayer = false;  // 设置标志
            Debug.Log("物体离开了指定图层: " + whatIsTriggerLayer);
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

    #region Statement Handle
    private void StatementStateHandler()
    {
        if (isBlocked)
        {
            currentState = PropState.Block; // 切换到“被激活”状态
        }
        else if (isActived)
        {
            if (currentState != PropState.Activated)
            {
                onTriggeredAction?.Invoke(pressDurationTime);  // 触发既定脚本
                activeStartTime = Time.time;
                pressDurationTime = 0;  // 重置时间
            }

            if (isBound)  // 处理绑定锁定
            {
                DropFunction();
            }

            currentState = PropState.Activated; // 切换到“被激活”状态
        }
        else if (isBound)
        {
            currentState = PropState.Held; // 切换到“被持有”状态
        }
        else
        {
            currentState = PropState.Default;
        }
    }

    private void StatementEffectHandler()
    {
        if (lastState != currentState)
        {
            if (lastState == PropState.Held)
            {
                SwitchCollidersState(true);  // 禁用碰撞箱
            }
        }

        lastState = currentState;  // 保存状态用于更改状态时触发

        if (currentState == PropState.Default)
        {
            isEnableGrivaty = true;  // 禁用重力
        }
        else if (currentState == PropState.Block)
        {
            isEnableGrivaty = false;  // 禁用重力
        }
        else if (currentState == PropState.Activated)
        {
            if (Time.time - activeStartTime > minReActiveTime)  // 碰到指定图层退出
            {
                if (isTouchTriggerLayer)
                {
                    endTriggeredAction?.Invoke();  // 触发既定脚本
                    isActived = false;
                }
            }
            else if (Time.time - activeStartTime > maxReActiveTime)  // 碰到指定图层退出
            {
                isActived = false;
            }    

            isEnableGrivaty = true;  // 禁用重力
        }
        else if (currentState == PropState.Held)
        {
            if (bindTargetPoint == null)  // 避免出现空物体错误
            {
                DropFunction();
                currentState = PropState.Default; // 切换到默认状态
                return;
            }

            SwitchCollidersState(false);  // 禁用碰撞箱

            transform.position = bindTargetPoint.position;
            transform.rotation = bindTargetPoint.rotation;
            isEnableGrivaty = false;  // 禁用重力
        }
    }
    #endregion

    #region Component Function

    #endregion
    /// <summary>
    /// 开启或关闭碰撞箱避免错误
    /// </summary>
    private void SwitchCollidersState(bool target)
    {
        // 禁用所有碰撞箱
        foreach (var collider in componentCollidersBox)
        {
            collider.enabled = target;
        }
    }

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

            if (pressDuration > maxActiveDurationTime)  // 限制最大时间
            {
                pressDuration = maxActiveDurationTime;
            }

            isButtonPressed = false;
            ActivateButtonTriggered(pressDuration); // 触发事件并传入持续时间
        }
    }

    /// <summary>
    /// 触发绑定的 UnityEvent，并传入按钮按下时间
    /// </summary>
    private void ActivateButtonTriggered(float pressDuration)
    {
        pressDurationTime = pressDuration;
        //currentState = PropState.Activated; // 切换到“被激活”状态
        isActived = true;
    }
    #endregion

    #region PickUp & Drop
    /// <summary>
    /// 绑定目标 Transform
    /// </summary>
    public void PickUpFunction(Transform target)
    {
        if (target != null && !isBound)
        {
            bindTargetPoint = target;
            isBound = true;
            //isEnableGrivaty = false;
            //currentState = PropState.Held; // 切换到“被持有”状态
        }
    }

    /// <summary>
    /// 解绑目标 Transform
    /// </summary>
    public void DropFunction()
    {
        isBound = false;
        //isEnableGrivaty = true;
        bindTargetPoint = null;
        //currentState = PropState.Default; // 切换回“默认”状态
    }
    #endregion

    #region Public Get Value
    public float GetValue()
    {
        return value;
    }

    public float GetWeight()
    {
        return weight;
    }
    #endregion

    #region Destroy Event
    public void OnDestroy()
    {
        Debug.Log("检测到了物体被销毁。");
        OnDestroyed?.Invoke(gameObject); // 触发事件
    }
    #endregion
}
