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
    [SerializeField] private List<string> targetTags; // ������Ҫ���Ķ�� Tag

    [Header("Parm")]
    private float _time;
    [SerializeField] private Transform currentTarget; // ��ǰ����⵽��Ŀ��
    [SerializeField] private Queue<Transform> targetQueue = new Queue<Transform>(); // �洢Ŀ��Ķ���

    private void LateUpdate()
    {
        // �����ǰĿ��Ϊ null�����ԴӶ����л�ȡ��һ��Ŀ��
        if (currentTarget == null && targetQueue.Count > 0)
        {
            currentTarget = targetQueue.Dequeue(); // �Ӷ�����ȡ����һ��Ŀ��
            Debug.Log($"�л�����һ��Ŀ��: {currentTarget.name}");
        }

        if (currentTarget == null) // ���û��Ŀ�ֱ꣬�ӷ���
            return;

        // ������Ŀ��ķ���
        var lookDelta = currentTarget.position - transform.position;
        var targetRot = Quaternion.LookRotation(lookDelta);
        transform.rotation = targetRot;

        // �����߼�����������
        _time += Time.deltaTime;

        if (_time < FireRate) // ��鿪����
            return;

        // ��������Ч��
        ProjectilePs.Emit(1);
        MuzzleFlashPs.Play(true);
        SleevesPs.Emit(1);

        // ������Ч
        if (AudioSource != null && AudioClip != null)
            AudioSource.PlayOneShot(AudioClip);

        _time = 0; // ����ʱ���Լ�������
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"��⵽��Ŀ��: {other.tag}");
        // ��ⴥ���������Ƿ����ָ���� Tag
        foreach (var tag in targetTags)
        {
            if (other.CompareTag(tag))
            {
                targetQueue.Enqueue(other.transform); // ������������Ŀ��������
                Debug.Log($"Ŀ����뷶Χ: {other.name}");

                // �����ǰû��Ŀ�ֱ꣬������Ϊ��ǰĿ��
                if (currentTarget == null)
                {
                    currentTarget = targetQueue.Dequeue();
                    Debug.Log($"���õ�ǰĿ��: {currentTarget.name}");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ����뿪�������ǵ�ǰĿ�꣬�������ǰĿ��
        if (currentTarget != null && other.transform == currentTarget)
        {
            Debug.Log($"��ǰĿ���뿪: {other.name}");
            currentTarget = null;
        }
        else
        {
            // ����뿪�������ڶ����У����Ƴ�������
            RemoveFromQueue(other.transform);
            Debug.Log($"Ŀ���뿪��Χ: {other.name}");
        }
    }

    private void RemoveFromQueue(Transform target)
    {
        // ����һ���µĶ������Ƴ��ض�Ŀ��
        Queue<Transform> updatedQueue = new Queue<Transform>();

        while (targetQueue.Count > 0)
        {
            Transform queuedTarget = targetQueue.Dequeue();
            if (queuedTarget != target) // �ų�Ҫ�Ƴ���Ŀ��
            {
                updatedQueue.Enqueue(queuedTarget);
            }
        }

        targetQueue = updatedQueue; // ���¶���
    }
}
