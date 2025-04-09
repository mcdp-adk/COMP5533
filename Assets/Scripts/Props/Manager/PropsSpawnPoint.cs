using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsSpawnPoint : MonoBehaviour
{
    [Header("Reference")]
    // ��������Ԥ������
    [SerializeField] private GameObject prefab;

    [Header("Parm")]
    // ��ǰʵ����������
    private GameObject currentInstance;

    // Start is called before��һ������ʱ��ִ��
    void Start()
    {
        SpawnObject(); // �ڿ�ʼʱ����һ������
    }

    // ���ڼ�������Ƿ�����
    void Update()
    {
        if (currentInstance == null) // �����ǰʵ���������Ѿ�����
        {
            SpawnObject(); // ��������һ������
        }
    }

    // ��������ķ���
    void SpawnObject()
    {
        if (prefab != null) // ���prefab�Ƿ�������
        {
            currentInstance = Instantiate(prefab, transform.position, transform.rotation);
        }
    }
}

