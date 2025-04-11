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
        // 按下 M 键时显示小地图
        if (Input.GetKeyDown(KeyCode.M))
        {
            //Debug.Log("M");
            miniMap.SetActive(true);
        }

        // 松开 M 键时隐藏小地图
        if (Input.GetKeyUp(KeyCode.M))
        {
            miniMap.SetActive(false);
        }
    }
}
