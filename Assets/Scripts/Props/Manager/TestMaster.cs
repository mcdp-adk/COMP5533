using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMaster : MonoBehaviour
{
    [Header("References")]
    public PropsBasicAction propsAction; // �� `PropsBasicAction` ���
    public Transform targetBindObj;

    void Start()
    {
        if (propsAction == null)
        {
            Debug.LogError("PropsBasicAction δ��ֵ������ Inspector �а󶨶���");
        }
        else
        {
            Debug.Log("��ʼ�����׳��ű���");
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
            // ������������ʱ������ ActivateButtonPressed
            if (Input.GetMouseButtonDown(0))
            {
                propsAction.ActivateButtonPressed();
                Debug.Log("���������£����� ActivateButtonPressed()");
            }

            // ���������ɿ�ʱ������ ActivateButtonRelease
            if (Input.GetMouseButtonUp(0))
            {
                propsAction.ActivateButtonRelease();
                Debug.Log("�������ɿ������� ActivateButtonRelease()");
            }
        }
        else
        {
            Debug.LogWarning("PropsBasicAction δ�󶨣����� Inspector ��ָ������");
        }
    }

    private void BindTest()
    {
        if (targetBindObj != null)
        {
            // ������������ʱ������ ActivateButtonPressed
            if (Input.GetKeyDown(KeyCode.Q))
            {
                propsAction.PickUpFunction(targetBindObj);
                Debug.Log("���������£����� ActivateButtonPressed()");
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                propsAction.DropFunction();
                Debug.Log("���������£����� ActivateButtonPressed()");
            }
        }
    }
}

