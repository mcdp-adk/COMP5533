using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PropsBasicAction : MonoBehaviour
{
    [Header("Reference")]
    private Rigidbody rb;

    [Header("Situation")]
    [SerializeField] private bool isEnableGrivaty = true;
    [SerializeField] private bool isTouchTriggerLayer = false;

    [Header("Setting")]
    [SerializeField] private float weight = 1.0f;
    [SerializeField] private float value = 1.0f;
    [SerializeField] private float minReActiveTime = 0.5f;  // ��С����ʱ��
    [SerializeField] private float maxReActiveTime = 5f;  // �����ʱ��
    [SerializeField] private float maxActiveDurationTime = 3f;  // �����ʱ��

    [Header("Parameters")]
    [SerializeField] private LayerMask whatIsTriggerLayer; // ���ô���ͼ��
    [SerializeField] private Collider activeCheckCollider; // �ⲿ�������ײ��
    [SerializeField] private UnityEvent<float> onTriggeredAction; // �����밴������ʱ��
    [SerializeField] private UnityEvent endTriggeredAction; // �����밴������ʱ��

    [Header("BindWithCharacter")]
    private Transform bindTargetPoint;

    [Header("ActiveFunction")]
    private float pressDurationTime = 0;
    private float activeStartTime = 0;
    private float buttonPressStartTime; // ��¼�������µ�ʱ���
    private bool isButtonPressed = false; // ��¼��ť�Ƿ񱻰���

    public delegate void DestroyedHandler(GameObject destroyedObject);
    public event DestroyedHandler OnDestroyed;

    [Header("Statement")]
    [SerializeField] private bool isBlocked = false;
    [SerializeField] private bool isActived = false;
    [SerializeField] private bool isBound = false;
    [SerializeField] private PropState currentState = PropState.Default; // ����ĵ�ǰ״̬
    [SerializeField] private PropState lastState = PropState.Default;  // �����ǰ��״̬

    private enum PropState
    {
        Default, // Ĭ��״̬
        Held,    // ������״̬
        Activated, // ������״̬
        Block    // ������״̬
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
    /// �����Ŀ�����߼�
    /// </summary>
    /// 
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & whatIsTriggerLayer) != 0)  // ����Ƿ�����ָ��ͼ��
        {
            isTouchTriggerLayer = true;  // ���ñ�־
            Debug.Log("����������ָ��ͼ��: " + whatIsTriggerLayer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & whatIsTriggerLayer) != 0)  // ����Ƿ�����ָ��ͼ��
        {
            isTouchTriggerLayer = false;  // ���ñ�־
            Debug.Log("�����뿪��ָ��ͼ��: " + whatIsTriggerLayer);
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
            rb.velocity = Vector3.zero; // ��ֹ������ʰȡʱ�в����ٶ�
            rb.angularVelocity = Vector3.zero; // �����ת�ٶ�
        }
    }
    #endregion

    #region Statement Handle
    private void StatementStateHandler()
    {
        if (isBlocked)
        {
            currentState = PropState.Block; // �л����������״̬
        }
        else if (isActived)
        {
            if (currentState != PropState.Activated)
            {
                onTriggeredAction?.Invoke(pressDurationTime);  // �����ȶ��ű�
                activeStartTime = Time.time;
                pressDurationTime = 0;  // ����ʱ��
            }

            if (isBound)  // ���������
            {
                DropFunction();
            }

            currentState = PropState.Activated; // �л����������״̬
        }
        else if (isBound)
        {
            currentState = PropState.Held; // �л����������С�״̬
        }
        else
        {
            currentState = PropState.Default;
        }
    }

    private void StatementEffectHandler()
    {
        if (currentState == PropState.Default)
        {
            isEnableGrivaty = true;  // ��������
        }
        else if (currentState == PropState.Block)
        {
            isEnableGrivaty = false;  // ��������
        }
        else if (currentState == PropState.Activated)
        {
            if (Time.time - activeStartTime > minReActiveTime)  // ����ָ��ͼ���˳�
            {
                if (isTouchTriggerLayer)
                {
                    endTriggeredAction?.Invoke();  // �����ȶ��ű�
                    isActived = false;
                }
            }
            else if (Time.time - activeStartTime > maxReActiveTime)  // ����ָ��ͼ���˳�
            {
                isActived = false;
            }    

            isEnableGrivaty = true;  // ��������
        }
        else if (currentState == PropState.Held)
        {
            if (bindTargetPoint == null)  // ������ֿ��������
            {
                DropFunction();
                currentState = PropState.Default; // �л���Ĭ��״̬
                return;
            }

            transform.position = bindTargetPoint.position;
            transform.rotation = bindTargetPoint.rotation;
            isEnableGrivaty = false;  // ��������
        }
    }
    #endregion

    #region ActiveButton
    /// <summary>
    /// ������������ʱ����
    /// </summary>
    public void ActivateButtonPressed()
    {
        buttonPressStartTime = Time.time; // ��¼��ǰʱ��
        isButtonPressed = true;
    }

    /// <summary>
    /// �������ɿ�����ʱ����������
    /// </summary>
    public void ActivateButtonRelease()
    {
        if (isButtonPressed)
        {
            float pressDuration = Time.time - buttonPressStartTime; // ���㰴סʱ��
            Debug.Log("��������µ�ʱ���ǣ�" + pressDuration);

            if (pressDuration > maxActiveDurationTime)  // �������ʱ��
            {
                pressDuration = maxActiveDurationTime;
            }

            isButtonPressed = false;
            ActivateButtonTriggered(pressDuration); // �����¼����������ʱ��
        }
    }

    /// <summary>
    /// �����󶨵� UnityEvent�������밴ť����ʱ��
    /// </summary>
    private void ActivateButtonTriggered(float pressDuration)
    {
        pressDurationTime = pressDuration;
        //currentState = PropState.Activated; // �л����������״̬
        isActived = true;
    }
    #endregion

    #region PickUp & Drop
    /// <summary>
    /// ��Ŀ�� Transform
    /// </summary>
    public void PickUpFunction(Transform target)
    {
        if (target != null && !isBound)
        {
            bindTargetPoint = target;
            isBound = true;
            //isEnableGrivaty = false;
            //currentState = PropState.Held; // �л����������С�״̬
        }
    }

    /// <summary>
    /// ���Ŀ�� Transform
    /// </summary>
    public void DropFunction()
    {
        isBound = false;
        //isEnableGrivaty = true;
        bindTargetPoint = null;
        //currentState = PropState.Default; // �л��ء�Ĭ�ϡ�״̬
    }
    #endregion

    #region
    public float GetValue()
    {
        return value;
    }

    public float GetWeight()
    {
        return weight;
    }
    #endregion

    #region
    public void OnDestroy()
    {
        Debug.Log("��⵽�����屻���١�");
        OnDestroyed?.Invoke(gameObject); // �����¼�
    }
    #endregion
}
