using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform cameraTransform; // 2.5D�ӽ������
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // WASD��������ƶ�
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // �����������������ƶ�����
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 move = moveDirection.x * cameraTransform.right + moveDirection.z * cameraForward;
        move *= moveSpeed * Time.deltaTime;

        controller.Move(move);
    }
}