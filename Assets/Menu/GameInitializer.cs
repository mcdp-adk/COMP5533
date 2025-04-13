using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private void Start()
    {
        // 开始为玩家0绑定设备
        InputManager.Instance.StartBinding(0);
    }

    public void OnInputDetected()
    {
        // 当检测到输入后，完成绑定
        InputManager.Instance.StopBinding();

        // 生成所有已绑定的玩家
        InputManager.Instance.SpawnPlayer();
    }
}