using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UITestManager : MonoBehaviour
{
    void Update()
    {
        //1,2,3,4,5,6,7,8 UI Test
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {   
            Debug.Log(UIManager.Instance);

            var tmp = UIManager.Instance.GetUI<ClearUIController>(UIType.ClearUI);

            Debug.Log(tmp);
            tmp.ShowUI();
        }
    }

}
