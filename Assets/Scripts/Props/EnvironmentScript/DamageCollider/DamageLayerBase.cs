using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageLayerBase : MonoBehaviour
{
    [Header("Reference")]
    // ���ڴ������㼶�� LayerMask
    [SerializeField] private LayerMask targetLayers;

    [Header("Setting")]
    // ���ڴ����˺�����
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float duringTime = 0.5f;

    // ���浱ǰ�ڴ������еĶ���
    private HashSet<GameObject> objectsInTrigger = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // ʹ�� LayerMask ��������Ƿ���Ŀ��㼶��
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            // �����������Ŀ��㼶������δ��ӵ�������
            if (!objectsInTrigger.Contains(other.gameObject))
            {
                objectsInTrigger.Add(other.gameObject);
                StartCoroutine(ApplyDamageOverTime(other.gameObject));
                Debug.Log($"���� {other.gameObject.name} ���봥��������ʼ�ܵ��˺�");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �������뿪������������Ӽ������Ƴ�
        if (objectsInTrigger.Contains(other.gameObject))
        {
            objectsInTrigger.Remove(other.gameObject);
            Debug.Log($"���� {other.gameObject.name} �뿪�������������ܵ��˺�");
        }
    }

    private IEnumerator ApplyDamageOverTime(GameObject obj)
    {
        // ���������ڴ�������ʱ���������˺�
        while (objectsInTrigger.Contains(obj))
        {
            PlayerController playerController = obj.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.CauseDamage((int)damageAmount); // ���������˺�
                Debug.Log("�������ڸ�������˺���");
            }

            // ��ȡEnemyAI������޸�health����
            EnemyAI enemyAI = obj.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.health -= damageAmount;
            }

            yield return new WaitForSeconds(duringTime); // ÿ�����һ���˺�
        }
    }
}
