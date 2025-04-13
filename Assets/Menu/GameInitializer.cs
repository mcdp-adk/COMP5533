using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private void Start()
    {
        // ��ʼΪ���0���豸
        InputManager.Instance.StartBinding(0);
    }

    public void OnInputDetected()
    {
        // ����⵽�������ɰ�
        InputManager.Instance.StopBinding();

        // ���������Ѱ󶨵����
        InputManager.Instance.SpawnPlayer();
    }
}