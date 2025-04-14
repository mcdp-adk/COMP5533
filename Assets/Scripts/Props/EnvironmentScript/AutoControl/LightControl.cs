using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    // ���ڼ�¼ָ����Layer
    [SerializeField] private LayerMask targetLayer;
    // Light���
    private Light lightComponent;
    // HashSet ���ڸ����ڴ������е�����
    private HashSet<GameObject> objectsInTrigger;

    // Start �ǽű���ʼʱ���õĺ���
    void Start()
    {
        // ��ȡLight���
        lightComponent = GetComponent<Light>();

        // ȷ��Light�������
        if (lightComponent == null)
        {
            Debug.LogError("��ǰ������δ�ҵ�Light�����");
        }

        // ��ʼ�� HashSet
        objectsInTrigger = new HashSet<GameObject>();
    }

    // OnTriggerEnter �ǵ�������ײ�����봥����ʱ���õĺ���
    private void OnTriggerEnter(Collider other)
    {
        // ������Ķ����Ƿ���ָ����Layer��
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // ��������ӵ� HashSet ��
            if (objectsInTrigger.Add(other.gameObject))
            {
                // ����������壬����Light���
                lightComponent.enabled = true;
            }
        }
    }

    // OnTriggerExit �ǵ�������ײ���뿪������ʱ���õĺ���
    private void OnTriggerExit(Collider other)
    {
        // ����뿪�Ķ����Ƿ���ָ����Layer��
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            // �� HashSet ���Ƴ�����
            if (objectsInTrigger.Remove(other.gameObject))
            {
                // ��HashSetΪ��ʱ���ر�Light���
                if (objectsInTrigger.Count == 0)
                {
                    lightComponent.enabled = false;
                }
            }
        }
    }
}
