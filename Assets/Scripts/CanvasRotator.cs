using UnityEngine;

/// <summary>
/// 使Canvas始终与主摄像机保持平行的控制器
/// </summary>
public class CanvasRotator : MonoBehaviour
{
    private Transform _cameraTransform;

    /// <summary>
    /// 在启动时初始化
    /// </summary>
    void Start()
    {
        // 获取主摄像机引用
        _cameraTransform = Camera.main.transform;
        
        if (_cameraTransform == null)
        {
            Debug.LogWarning("未找到主摄像机，Canvas 将无法正确旋转");
        }
    }

    /// <summary>
    /// 在所有 Update 调用完成后执行，确保摄像机位置已更新
    /// </summary>
    void LateUpdate()
    {
        if (_cameraTransform != null)
        {
            // 让Canvas与摄像机保持相同的旋转（平行）
            transform.rotation = _cameraTransform.rotation;
        }
    }
}