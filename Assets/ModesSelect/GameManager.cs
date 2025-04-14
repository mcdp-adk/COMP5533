using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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

    public TextMeshProUGUI countdownText; // �󶨵���ʱ�ı���
    public float countdownTime = 5.0f; // ����ʱ��ʱ��
    private float remainingTime; // ʣ��ʱ��
    private bool isCountingDown = false; // �Ƿ��ڵ���ʱ

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;

        player1.onClick.AddListener(OnStartButtonClicked);
        player2.onClick.AddListener(OnStartButtonClicked);
        player3.onClick.AddListener(OnStartButtonClicked);
        player4.onClick.AddListener(OnStartButtonClicked);

        remainingTime = countdownTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingForKeyPress)
        {
            // ����Ƿ����������������꣩
            if (Input.anyKeyDown)
            {
                // ����Ƿ���������0��1��2 �ֱ��Ӧ���������Ҽ����м���
                if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                {
                    waitingForKeyPress = false; // ֹͣ�ȴ�����
                    textContinue.SetActive(false);
                    InputManager.Instance.StopBinding();
                    Grounding.SetActive(true);
                }
            }
        }

        if (isCountingDown && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; // ÿ֡����ʱ��
            int seconds = Mathf.CeilToInt(remainingTime); // ��ȡʣ������������ȡ��
            countdownText.text = seconds.ToString(); // �����ı�����
        }
        else if (remainingTime <= 0)
        {
            countdownText.gameObject.SetActive(false); // ����ʱ�����������ı���
            spawnBox.SetActive(false); // �������ɿ�
            isCountingDown = false; // ���õ���ʱ״̬
        }
    }

    public void GameBegin()
    {
        Time.timeScale = 1f;
        modesMenu.SetActive(false);

        // ��ȡ������Ҷ��󲢽������ƶ��� spawnBox �в���������
        foreach (var playerInput in InputManager.Instance.PlayerInputs)
        {
            if (playerInput == null) continue;

            GameObject player = playerInput.gameObject;
            if (player != null)
            {
                player.transform.position = spawnBox.transform.position;
                player.SetActive(true);
            }
        }
    }

    public void OnStartButtonClicked()
    {
        waitingForKeyPress = true; // ��ʼ�ȴ�����
        textContinue.SetActive(true); // ��ʾ��ʾ�ı�
    }

    public void groundingDisapp()
    {
        Grounding.SetActive(false);
    }

    public void StartCountdown()
    {
        countdownText.gameObject.SetActive(true); // ��ʾ�ı���
        countdownText.text = countdownTime.ToString(); // ��ʼ���ı�����
        remainingTime = countdownTime; // ��ʼ��ʣ��ʱ��
        isCountingDown = true; // ��ʼ����ʱ
    }
}
