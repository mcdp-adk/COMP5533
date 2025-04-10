using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用动态生成的 LineRenderer 显示物体投掷轨迹，虚线长度随力度动态变化
/// </summary>
public class PropDuringTriggerEventRay : MonoBehaviour
{
    [Header("Trajectory Settings")]
    [SerializeField] private float baseThrowForce = 10f; // 基础抛出力
    [SerializeField] private float throwDirectionRatioUp = 4f; // 抛出方向（向上）
    [SerializeField] private float throwDirectionRatioForward = 3f; // 抛出方向（向前）
    [SerializeField] private int resolution = 30; // 轨迹分辨率
    [SerializeField] private float gravity = -9.8f; // 重力加速度
    [SerializeField] private Color rayColor = Color.red; // 轨迹颜色
    [SerializeField] private float StartLineWidth = 0.05f; // 轨迹线初始宽度
    [SerializeField] private float EndLineWidth = 0.00001f; // 轨迹线末端宽度

    private LineRenderer lineRenderer; // 动态生成的 LineRenderer
    private Vector3 initialVelocity; // 初始速度

    // Start is called before the first frame update
    void Start()
    {
        // 动态创建 LineRenderer 组件
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // 配置 LineRenderer 的基本属性
        lineRenderer.startWidth = StartLineWidth;
        lineRenderer.endWidth = EndLineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 使用支持纹理的着色器
        lineRenderer.material.mainTexture = CreateDashedTexture(); // 设置纹理为虚线
        lineRenderer.material.color = rayColor;
        lineRenderer.startColor = rayColor;
        lineRenderer.endColor = rayColor;
        lineRenderer.useWorldSpace = true; // 使用世界坐标

        // 设置 LineRenderer 的宽度曲线
        AnimationCurve widthCurve = new AnimationCurve();
        widthCurve.AddKey(0f, StartLineWidth); // 起始宽度
        widthCurve.AddKey(1f, EndLineWidth);  // 末端宽度
        lineRenderer.widthCurve = widthCurve;

        lineRenderer.positionCount = 0; // 初始点数量为0
    }

    /// <summary>
    /// 绘制轨迹
    /// </summary>
    /// <param name="forceWeight">力度权重</param>
    public void DrawTrajectory(float forceWeight)
    {
        float finalThrowForce = baseThrowForce * forceWeight;

        // 计算初始速度
        Vector3 direction = (transform.forward * throwDirectionRatioForward + transform.up * throwDirectionRatioUp).normalized;
        initialVelocity = direction * finalThrowForce;

        // 计算轨迹点
        Vector3[] points = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float time = i / (float)resolution; // 时间比例
            points[i] = CalculatePoint(time);
        }

        // 根据 forceWeight 动态调整虚线长度
        float scaleFactor = resolution / 10f; // 基于轨迹点数量的缩放系数
        lineRenderer.material.mainTextureScale = new Vector2(forceWeight * scaleFactor, 1f); // 调整纹理缩放比例

        // 使用 LineRenderer 显示轨迹
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    /// <summary>
    /// 根据时间计算轨迹点的位置
    /// </summary>
    /// <param name="time">时间</param>
    /// <returns>轨迹点的位置</returns>
    private Vector3 CalculatePoint(float time)
    {
        Vector3 displacement = initialVelocity * time + 0.5f * new Vector3(0, gravity, 0) * time * time;
        return transform.position + displacement;
    }

    /// <summary>
    /// 清除轨迹
    /// </summary>
    public void ClearTrajectory()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0; // 清除轨迹
        }
    }

    /// <summary>
    /// 创建虚线纹理
    /// </summary>
    /// <returns>虚线纹理</returns>
    private Texture2D CreateDashedTexture()
    {
        int textureWidth = 4; // 虚线纹理宽度
        int textureHeight = 1; // 高度设置为 1 像素
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        Color[] pixels = new Color[textureWidth];
        for (int i = 0; i < textureWidth; i++)
        {
            pixels[i] = i % 2 == 0 ? rayColor : Color.clear; // 奇偶像素间隔形成虚线
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
