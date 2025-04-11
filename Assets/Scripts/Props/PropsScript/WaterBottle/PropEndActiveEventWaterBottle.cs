using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropEndActiveEventWaterBottle : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string targetTag = "Player";  // �ն���tag
    [SerializeField] private string targetLayer = "Player";  // �ն��Ĳ�
    [SerializeField] private float duringTime = 5f;  // �ն��Ĵ���ʱ��
    [SerializeField] private float quaternionAngle = 25f;  // �ն��Ĵ���ʱ��
    [SerializeField] private GameObject decoyPerfab;  // �ն���Ԥ�Ƽ�

    /// <summary>
    /// ��������
    /// </summary>
    public void DecoyEmerge()
    {
        if (decoyPerfab != null)
        {
            // ���������������ת��ʹ�䴹ֱ����б�ǶȲ�����30��
            Quaternion adjustedRotation = Quaternion.Euler(
                Mathf.Clamp(transform.rotation.eulerAngles.x, -quaternionAngle, quaternionAngle),
                transform.rotation.eulerAngles.y,
                Mathf.Clamp(transform.rotation.eulerAngles.z, -quaternionAngle, quaternionAngle)
            );

            // �����ն�
            GameObject decoyInstance = Instantiate(decoyPerfab, transform.position, adjustedRotation);

            // �����ն���Tag
            decoyInstance.tag = targetTag;

            // �����ն���Layer
            decoyInstance.layer = LayerMask.NameToLayer(targetLayer);

            // �������ٵ���ʱ
            Destroy(decoyInstance, duringTime); // ��Ч������ʱ�������
        }
        else
        {
            Debug.LogWarning("Decoy prefab is not assigned!");
        }

        // ���ٹ��ظýű�������
        Destroy(gameObject);
    }
}
