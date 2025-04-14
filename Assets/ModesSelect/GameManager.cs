using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Spawn Point")]
    [SerializeField] private GameObject[] _spawnPoints = new GameObject[4]; // 生成点数组

    [Header("Input Manager")]
    private PlayerInput[] _playerInputs; // 玩家输入组件数组

    [Header("Game Manager")]
    public GameObject spawnBox;
    public GameObject modesMenu;
    public GameObject textContinue;
    public GameObject Grounding;
    public GameObject play;
    private bool waitingForKeyPress = false;
    public Button player1;
    public Button player2;
    public Button player3;
    public Button player4;

    public TextMeshProUGUI countdownText; // 绑定倒计时文本框
    public float countdownTime = 5.0f; // 倒计时总时间
    private float remainingTime; // 剩余时间
    private bool isCountingDown = false; // 是否在倒计时

    void Start()
    {
        Time.timeScale = 0f;

        // player1.onClick.AddListener(OnStartButtonClicked);
        // player2.onClick.AddListener(OnStartButtonClicked);
        // player3.onClick.AddListener(OnStartButtonClicked);
        // player4.onClick.AddListener(OnStartButtonClicked);

        remainingTime = countdownTime;
    }

    void Update()
    {
        if (waitingForKeyPress)
        {
            // 检测是否按下任意键（包括鼠标）
            if (Input.anyKeyDown)
            {
                // 检测是否按下鼠标键（0、1、2 分别对应鼠标左键、右键、中键）
                if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                {
                    waitingForKeyPress = false; // 停止等待按键
                    textContinue.SetActive(false);
                    InputManager.Instance.StopBinding();
                    Grounding.SetActive(true);
                }
            }
        }

        if (isCountingDown && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; // 每帧减少时间
            int seconds = Mathf.CeilToInt(remainingTime); // 获取剩余秒数，向上取整
            countdownText.text = seconds.ToString(); // 更新文本内容
        }
        else if (remainingTime <= 0)
        {
            countdownText.gameObject.SetActive(false); // 倒计时结束，隐藏文本框
            spawnBox.SetActive(false); // 隐藏生成框
            isCountingDown = false; // 重置倒计时状态
        }
    }

    public void GameBegin()
    {
        Time.timeScale = 1f;
        modesMenu.SetActive(false);

        HandleSpawnPlayer();
    }

    public void OnStartButtonClicked()
    {
        waitingForKeyPress = true; // 开始等待按键
        textContinue.SetActive(true); // 显示提示文本
    }

    public void groundingDisapp()
    {
        Grounding.SetActive(false);
    }

    public void StartCountdown()
    {
        countdownText.gameObject.SetActive(true); // 显示文本框
        countdownText.text = countdownTime.ToString(); // 初始化文本内容
        remainingTime = countdownTime; // 初始化剩余时间
        isCountingDown = true; // 开始倒计时
    }

    // #region Handle Players

    // private void HandlePlayerDeath(PlayerController player, Vector3 respawnPosition)
    // {
    //     player.Respawn(respawnPosition);
    // }

    // #endregion

    #region Handle Input Manager

    public void HandleStartBinding(int playerIndex)
    {
        InputManager.Instance.StartBinding(playerIndex);
    }

    public void HandleStopBinding()
    {
        InputManager.Instance.StopBinding();
    }

    public void HandleCancelBinding(int playerIndex)
    {
        InputManager.Instance.CancelBinding(playerIndex);
    }

    public void HandleSpawnPlayer()
    {
        _playerInputs = InputManager.Instance.SpawnPlayer();
        for (int i = 0; i < _playerInputs.Length; ++i)
        {
            if (_playerInputs[i] != null)
            {
                // 设置玩家的生成点
                _playerInputs[i].gameObject.transform.position = _spawnPoints[i].transform.position;
                _playerInputs[i].gameObject.transform.rotation = _spawnPoints[i].transform.rotation;
                Debug.Log($"玩家 {i} 生成在 {_spawnPoints[i].name} 位置");
            }
        }
    }

    #endregion
}
