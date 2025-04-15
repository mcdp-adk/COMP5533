using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinInTreasure : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private float minAngle = 90f;
    [SerializeField] private float maxAngle = 270f;
    // AudioSource���
    [SerializeField] private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // ������������Y����ת�Ƕ�
        float randomYRotation = Random.Range(minAngle, maxAngle);
        transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);
        audioSource.Play();
    }
}

