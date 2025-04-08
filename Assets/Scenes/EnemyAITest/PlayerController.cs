using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform cameraTransform; // 2.5D视角摄像机
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // WASD输入控制移动
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 根据摄像机方向调整移动方向
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 move = moveDirection.x * cameraTransform.right + moveDirection.z * cameraForward;
        move *= moveSpeed * Time.deltaTime;

        controller.Move(move);
    }
}