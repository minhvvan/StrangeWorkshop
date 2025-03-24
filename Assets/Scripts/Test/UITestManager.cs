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
            tmp.ShowUI();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var tmp = UIManager.Instance.GetUI<LoseEffectUIController>(UIType.LoseEffectUI);
            tmp.ShowUI();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var tmp = UIManager.Instance.GetUI<PauseUIController>(UIType.PauseUI);
            tmp.ShowUI();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            var tmp = UIManager.Instance.GetUI<ResultUIContorller>(UIType.ResultUI);
            tmp.ShowUI();
        }
    }

}
