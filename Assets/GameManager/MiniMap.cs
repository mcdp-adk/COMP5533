using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public GameObject miniMap;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ���� M ��ʱ��ʾС��ͼ
        if (Input.GetKeyDown(KeyCode.M))
        {
            //Debug.Log("M");
            miniMap.SetActive(true);
        }

        // �ɿ� M ��ʱ����С��ͼ
        if (Input.GetKeyUp(KeyCode.M))
        {
            miniMap.SetActive(false);
        }
    }
}
