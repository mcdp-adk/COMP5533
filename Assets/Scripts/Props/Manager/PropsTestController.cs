using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsTestController : MonoBehaviour
{
    [Header("Choice")]
    [SerializeField] private bool isNeedDestroedSelf = false;

    // Update is called once per frame
    void Update()
    {
        CheckCommand();
    }

    private void CheckCommand()
    {
        if (isNeedDestroedSelf)
        {
            Destroy(gameObject); // 销毁挂载的对象
        }
    }
}

