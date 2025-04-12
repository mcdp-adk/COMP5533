using UnityEngine;
using Cinemachine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public static PlayerControl Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerDeath += PlayerDied;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerDeath -= PlayerDied;
    }

    void Start()
    {
        // �������
        SpawnPlayer();
    }

    // �������
    private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);

        // ����Cinemachine����������ĸ���Ŀ��
        if (virtualCamera != null)
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
        }
    }

    // �������������
    public void PlayerDied()
    {
        StartCoroutine(RespawnPlayer());
    }

    // �����������Э��
    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(5f);
        SpawnPlayer();
    }
}
