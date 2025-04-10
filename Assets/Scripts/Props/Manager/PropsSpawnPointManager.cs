using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropsSpawnPointManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>(); // �����ɵ��ߵĵ�λ�б�
    [SerializeField] private GameObject[] propPrefabs; // �����ɵ�Ԥ�Ƽ���������
    [SerializeField] private int maxTotalProps = 2; // ���Ƶ��ߵ�����������
    [SerializeField] private float spawnWaitingTime = 0.3f; // ���Ƶ��ߵ�����������

    private int currentTotalProps = 0; // ��ǰ��ͼ�еĵ�������

    private class SpawnPointStatus
    {
        public Transform point; // ��λ��Transform
        public GameObject generatedProp; // ���ɵĵ�������
        public bool IsPropDestroyed => generatedProp == null; // �жϵ����Ƿ�����
    }

    private List<SpawnPointStatus> spawnPointStatuses = new List<SpawnPointStatus>(); // ��λ״̬�б�

    void Start()
    {
        // ��ʼ�����ɵ�λ��״̬
        foreach (var spawnPoint in spawnPoints)
        {
            spawnPointStatuses.Add(new SpawnPointStatus { point = spawnPoint, generatedProp = null });
        }

        SpawnProps(); // ��ʼʱ���ɵ���
        currentTotalProps = spawnPointStatuses.Count(status => status.generatedProp != null); // ��ʼ����ǰ��������
    }

    /// <summary>
    /// �ڿ��еĵ�λ���ɵ���
    /// </summary>
    private void SpawnProps()
    {
        if (!this || !gameObject.activeInHierarchy)
        {
            Debug.LogWarning("�������ɵ���ʱ����ǰ������Ч��ֹͣ���ɣ�");
            return;
        }

        // ����λ״̬�б������
        List<SpawnPointStatus> shuffledSpawnPoints = new List<SpawnPointStatus>(spawnPointStatuses);
        for (int i = 0; i < shuffledSpawnPoints.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledSpawnPoints.Count);
            SpawnPointStatus temp = shuffledSpawnPoints[i];
            shuffledSpawnPoints[i] = shuffledSpawnPoints[randomIndex];
            shuffledSpawnPoints[randomIndex] = temp;
        }

        foreach (var status in shuffledSpawnPoints)
        {
            // �����ǰ���������Ѵﵽ������ƣ�ֹͣ����
            if (currentTotalProps >= maxTotalProps)
            {
                Debug.Log("���������Ѵﵽ������ƣ�ֹͣ���ɣ�");
                return;
            }

            // ���ڿ��л���߱����ٵĵ�λ�����µĵ���
            if (status.IsPropDestroyed)
            {
                int randomIndex = Random.Range(0, propPrefabs.Length);
                GameObject prop = Instantiate(propPrefabs[randomIndex], status.point.position, Quaternion.identity);

                // �������¼�����
                var propAction = prop.GetComponent<PropsBasicAction>();
                if (propAction != null)
                {
                    propAction.OnDestroyed += HandlePropDestroyed; // ע�������¼�
                }

                // ���µ�λ״̬
                status.generatedProp = prop;
                currentTotalProps++; // ���ӵ�ǰ��������
            }
        }
    }


    /// <summary>
    /// ������������¼�
    /// </summary>
    private void HandlePropDestroyed(GameObject prop)
    {
        Debug.Log($"���� {prop.name} �����١�");
        if (!this || !gameObject.activeInHierarchy)
        {
            Debug.LogWarning("��Ϸ��������Ч��ֹͣ�����߼���");
            return;
        }

        foreach (var status in spawnPointStatuses)
        {
            if (status.generatedProp == prop)
            {
                status.generatedProp = null; // ���µ�λ״̬Ϊδ���ɵ���
                currentTotalProps--; // ���ٵ�ǰ��������
                break; // �ҵ���Ӧ��λ��ֹͣ����
            }
        }

        // ʹ���ӳٵ��������߼���������OnDestroy��ֱ������
        Invoke(nameof(SpawnProps), spawnWaitingTime); // �ӳ�0.1�����SpawnProps
    }


    void OnDisable()
    {
        // ȡ�����й�������ɵ���
        CancelInvoke(nameof(SpawnProps));
    }

}
