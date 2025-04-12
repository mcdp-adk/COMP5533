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
        // 生成玩家
        SpawnPlayer();
    }

    // 生成玩家
    private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);

        // 设置Cinemachine虚拟摄像机的跟随目标
        if (virtualCamera != null)
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
        }
    }

    // 玩家死亡处理方法
    public void PlayerDied()
    {
        StartCoroutine(RespawnPlayer());
    }

    // 玩家重新生成协程
    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(5f);
        SpawnPlayer();
    }
}
