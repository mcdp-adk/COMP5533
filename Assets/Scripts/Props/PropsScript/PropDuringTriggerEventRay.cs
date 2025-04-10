using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ʹ�ö�̬���ɵ� LineRenderer ��ʾ����Ͷ���켣�����߳��������ȶ�̬�仯
/// </summary>
public class PropDuringTriggerEventRay : MonoBehaviour
{
    [Header("Trajectory Settings")]
    [SerializeField] private float baseThrowForce = 10f; // �����׳���
    [SerializeField] private float throwDirectionRatioUp = 4f; // �׳��������ϣ�
    [SerializeField] private float throwDirectionRatioForward = 3f; // �׳�������ǰ��
    [SerializeField] private int resolution = 30; // �켣�ֱ���
    [SerializeField] private float gravity = -9.8f; // �������ٶ�
    [SerializeField] private Color rayColor = Color.red; // �켣��ɫ
    [SerializeField] private float StartLineWidth = 0.05f; // �켣�߳�ʼ���
    [SerializeField] private float EndLineWidth = 0.00001f; // �켣��ĩ�˿��

    private LineRenderer lineRenderer; // ��̬���ɵ� LineRenderer
    private Vector3 initialVelocity; // ��ʼ�ٶ�

    // Start is called before the first frame update
    void Start()
    {
        // ��̬���� LineRenderer ���
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // ���� LineRenderer �Ļ�������
        lineRenderer.startWidth = StartLineWidth;
        lineRenderer.endWidth = EndLineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // ʹ��֧���������ɫ��
        lineRenderer.material.mainTexture = CreateDashedTexture(); // ��������Ϊ����
        lineRenderer.material.color = rayColor;
        lineRenderer.startColor = rayColor;
        lineRenderer.endColor = rayColor;
        lineRenderer.useWorldSpace = true; // ʹ����������

        // ���� LineRenderer �Ŀ������
        AnimationCurve widthCurve = new AnimationCurve();
        widthCurve.AddKey(0f, StartLineWidth); // ��ʼ���
        widthCurve.AddKey(1f, EndLineWidth);  // ĩ�˿��
        lineRenderer.widthCurve = widthCurve;

        lineRenderer.positionCount = 0; // ��ʼ������Ϊ0
    }

    /// <summary>
    /// ���ƹ켣
    /// </summary>
    /// <param name="forceWeight">����Ȩ��</param>
    public void DrawTrajectory(float forceWeight)
    {
        float finalThrowForce = baseThrowForce * forceWeight;

        // �����ʼ�ٶ�
        Vector3 direction = (transform.forward * throwDirectionRatioForward + transform.up * throwDirectionRatioUp).normalized;
        initialVelocity = direction * finalThrowForce;

        // ����켣��
        Vector3[] points = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float time = i / (float)resolution; // ʱ�����
            points[i] = CalculatePoint(time);
        }

        // ���� forceWeight ��̬�������߳���
        float scaleFactor = resolution / 10f; // ���ڹ켣������������ϵ��
        lineRenderer.material.mainTextureScale = new Vector2(forceWeight * scaleFactor, 1f); // �����������ű���

        // ʹ�� LineRenderer ��ʾ�켣
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    /// <summary>
    /// ����ʱ�����켣���λ��
    /// </summary>
    /// <param name="time">ʱ��</param>
    /// <returns>�켣���λ��</returns>
    private Vector3 CalculatePoint(float time)
    {
        Vector3 displacement = initialVelocity * time + 0.5f * new Vector3(0, gravity, 0) * time * time;
        return transform.position + displacement;
    }

    /// <summary>
    /// ����켣
    /// </summary>
    public void ClearTrajectory()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0; // ����켣
        }
    }

    /// <summary>
    /// ������������
    /// </summary>
    /// <returns>��������</returns>
    private Texture2D CreateDashedTexture()
    {
        int textureWidth = 4; // ����������
        int textureHeight = 1; // �߶�����Ϊ 1 ����
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        Color[] pixels = new Color[textureWidth];
        for (int i = 0; i < textureWidth; i++)
        {
            pixels[i] = i % 2 == 0 ? rayColor : Color.clear; // ��ż���ؼ���γ�����
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
