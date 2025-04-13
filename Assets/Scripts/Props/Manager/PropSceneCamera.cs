using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSceneCamera : MonoBehaviour
{
    // ���ڴ��������
    public Camera camera;

    // ���ڴ�����ҵ�λ��
    public Transform player;

    // ��¼��ʼʱ����������֮���λ��
    private Vector3 initialOffset;

    // ����Ϸ��ʼʱ����
    void Start()
    {
        if (camera == null)
        {
            Debug.LogError("Camera δ���䣬����Inspector�з��������������");
        }
        if (player == null)
        {
            Debug.LogError("Player δ���䣬����Inspector�з�����ұ�����");
        }

        // ȷ�������������ģʽ
        if (camera != null && !camera.orthographic)
        {
            Debug.LogError("�������Ҫ����ΪOrthographicģʽ��");
        }

        // �����ʼλ��
        if (camera != null && player != null)
        {
            initialOffset = camera.transform.position - player.position;
        }
    }

    // ÿ֡����
    void Update()
    {
        if (camera != null && player != null)
        {
            // ������ҵ�λ�úͳ�ʼλ�ƣ������������λ��
            Vector3 updatedPosition = player.position + initialOffset;
            camera.transform.position = new Vector3(updatedPosition.x, camera.transform.position.y, updatedPosition.z);
        }
    }
}
