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
    public LayerMask whatIsTriggerLayer; // ���ô���ͼ��
    public UnityEvent<float> onTriggeredAction; // �����밴������ʱ��

    [Header("BindWithCharacter")]
    private Transform bindTargetPoint;
    private bool isBound = false;

    [Header("ActiveFunction")]
    private float buttonPressStartTime; // ��¼�������µ�ʱ���
    private bool isButtonPressed = false; // ��¼��ť�Ƿ񱻰���

    [Header("Statement")]
    private PropState currentState = PropState.Default; // ����ĵ�ǰ״̬

    private enum PropState
    {
        Default, // Ĭ��״̬
        Held,    // ������״̬
        Activated, // ������״̬
        Block    // ���׳�״̬
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
    /// �����Ŀ�����߼�
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
            rb.velocity = Vector3.zero; // ��ֹ������ʰȡʱ�в����ٶ�
            rb.angularVelocity = Vector3.zero; // �����ת�ٶ�
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
            isButtonPressed = false;
            ActivateButtonTriggered(pressDuration); // �����¼����������ʱ��
        }
    }

    /// <summary>
    /// �����󶨵� UnityEvent�������밴ť����ʱ��
    /// </summary>
    private void ActivateButtonTriggered(float pressDuration)
    {
        onTriggeredAction?.Invoke(pressDuration);
        currentState = PropState.Activated; // �л����������״̬
    }
    #endregion

    #region PIckUp & Drop
    /// <summary>
    /// ��Ŀ�� Transform
    /// </summary>
    public void PickUpFunction(Transform target)
    {
        if (target != null && !isBound)
        {
            bindTargetPoint = target;
            isBound = true;
            isEnableGrivaty = false;
            currentState = PropState.Held; // �л����������С�״̬
        }
    }

    /// <summary>
    /// ���Ŀ�� Transform
    /// </summary>
    public void DropFunction()
    {
        isBound = false;
        isEnableGrivaty = true;
        bindTargetPoint = null;
        currentState = PropState.Default; // �л��ء�Ĭ�ϡ�״̬
    }
    #endregion
}
