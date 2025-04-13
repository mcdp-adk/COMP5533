using System.Collections.Generic;
using UnityEngine;

public class MinigunBase : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private ParticleSystem ProjectilePs;
    [SerializeField] private ParticleSystem MuzzleFlashPs;
    [SerializeField] private ParticleSystem SleevesPs;

    [SerializeField] private AudioClip AudioClip;
    [SerializeField] private AudioSource AudioSource;

    [Header("Setting")]
    [SerializeField] private float FireRate = 0.08f;
    [SerializeField] private List<string> targetTags; // 储存需要检测的多个 Tag

    [Header("Parm")]
    private float _time;
    [SerializeField] private Transform currentTarget; // 当前被检测到的目标
    [SerializeField] private Queue<Transform> targetQueue = new Queue<Transform>(); // 存储目标的队列

    private void LateUpdate()
    {
        // 如果当前目标为 null，尝试从队列中获取下一个目标
        if (currentTarget == null && targetQueue.Count > 0)
        {
            currentTarget = targetQueue.Dequeue(); // 从队列中取出下一个目标
            Debug.Log($"切换到下一个目标: {currentTarget.name}");
        }

        if (currentTarget == null) // 如果没有目标，直接返回
            return;

        // 计算与目标的方向
        var lookDelta = currentTarget.position - transform.position;
        var targetRot = Quaternion.LookRotation(lookDelta);
        transform.rotation = targetRot;

        // 开火逻辑：持续开火
        _time += Time.deltaTime;

        if (_time < FireRate) // 检查开火间隔
            return;

        // 发射粒子效果
        ProjectilePs.Emit(1);
        MuzzleFlashPs.Play(true);
        SleevesPs.Emit(1);

        // 播放音效
        if (AudioSource != null && AudioClip != null)
            AudioSource.PlayOneShot(AudioClip);

        _time = 0; // 重置时间以继续开火
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"检测到新目标: {other.tag}");
        // 检测触发的物体是否具有指定的 Tag
        foreach (var tag in targetTags)
        {
            if (other.CompareTag(tag))
            {
                targetQueue.Enqueue(other.transform); // 将符合条件的目标加入队列
                Debug.Log($"目标进入范围: {other.name}");

                // 如果当前没有目标，直接设置为当前目标
                if (currentTarget == null)
                {
                    currentTarget = targetQueue.Dequeue();
                    Debug.Log($"设置当前目标: {currentTarget.name}");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 如果离开的物体是当前目标，则清除当前目标
        if (currentTarget != null && other.transform == currentTarget)
        {
            Debug.Log($"当前目标离开: {other.name}");
            currentTarget = null;
        }
        else
        {
            // 如果离开的物体在队列中，则移除该物体
            RemoveFromQueue(other.transform);
            Debug.Log($"目标离开范围: {other.name}");
        }
    }

    private void RemoveFromQueue(Transform target)
    {
        // 创建一个新的队列以移除特定目标
        Queue<Transform> updatedQueue = new Queue<Transform>();

        while (targetQueue.Count > 0)
        {
            Transform queuedTarget = targetQueue.Dequeue();
            if (queuedTarget != target) // 排除要移除的目标
            {
                updatedQueue.Enqueue(queuedTarget);
            }
        }

        targetQueue = updatedQueue; // 更新队列
    }
}
