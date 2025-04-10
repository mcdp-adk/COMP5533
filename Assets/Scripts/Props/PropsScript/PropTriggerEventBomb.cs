using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���屻�ӳ����߼�
/// </summary>
public class PropTriggerEventThrow : MonoBehaviour
{
    [Header("Throw Settings")]
    public float baseThrowForce = 10f;  // �����׳���
    public Vector3 throwDirection = new Vector3(0.5f, 1f, 0f); // б��ǰ�Ϸ��׳�����
    public float randomTorqueRange = 5f; // �����ת����Χ

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogWarning("û���ҵ� Rigidbody ����������޷����ӳ���");
        }
    }

    /// <summary>
    /// ���屻�ӳ���forceWeight Ӱ���׳���ǿ��
    /// </summary>
    /// <param name="forceWeight">�׳�����Ȩ�أ�Ӱ�����Զ��</param>
    public void ThrowOut(float forceWeight)
    {
        Debug.Log($"���������ã�����: {forceWeight}");

        if (rb != null)
        {
            float finalThrowForce = baseThrowForce * forceWeight;

            // ʩ������ʹ������б��ǰ�Ϸ��׳�
            rb.AddForce(throwDirection.normalized * finalThrowForce, ForceMode.Impulse);

            // �����������ת��
            Vector3 randomTorque = new Vector3(
                Random.Range(-randomTorqueRange, randomTorqueRange),
                Random.Range(-randomTorqueRange, randomTorqueRange),
                Random.Range(-randomTorqueRange, randomTorqueRange)
            );

            // ʩ����ת��
            rb.AddTorque(randomTorque, ForceMode.Impulse);

            Debug.Log($"���屻�ӳ��������ת������: {finalThrowForce}����ת��: {randomTorque}");
        }
    }
}

