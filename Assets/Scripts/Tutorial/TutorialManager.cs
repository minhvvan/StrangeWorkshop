using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    // 튜토리얼 UI 패널과 텍스트 컴포넌트를 연결합니다.
    public GameObject[] tutorialUIPanel;

    private int steps = 0;
    public bool flag = false;

    private void Start()
    {
        ShowManualTutorial();
        TutorialEventManager.OnFirstInput += ShowCounterTutorial;
        /*TutorialEventManager.OnFirstInteract += ShowTutorial;
        TutorialEventManager.OnTurretMade += ShowTutorial;
        TutorialEventManager.OnBulletEmpty += ShowTutorial;*/
    }

    private void OnDestroy()
    {
        TutorialEventManager.OnFirstInput -= ShowCounterTutorial;
        /*TutorialEventManager.OnFirstInteract -= ShowTutorial;
        TutorialEventManager.OnTurretMade -= ShowTutorial;
        TutorialEventManager.OnBulletEmpty -= ShowTutorial;*/
    }

    // 튜토리얼 UI를 보여주고 게임을 일시정지합니다.
    public void ShowManualTutorial()
    {
        // 게임 일시정지
        Time.timeScale = 0;
        
        Debug.Log("Tutorial Show");
        // 튜토리얼 UI 활성화 및 메시지 표시
        tutorialUIPanel[0].SetActive(true);
        flag = true;
    }
    
    public void ShowCounterTutorial()
    {
        // 게임 일시정지
        Time.timeScale = 0;
        
        Debug.Log("Tutorial Show");
        // 튜토리얼 UI 활성화 및 메시지 표시
        tutorialUIPanel[1].SetActive(true);
        flag = true;
    }

    // UI의 버튼 등에서 호출하여 튜토리얼을 닫고 게임을 재개합니다.
    public void CloseTutorial()
    {
        // 튜토리얼 UI 비활성화
        tutorialUIPanel[steps].SetActive(false);
        steps++;
        flag = false;
        // 게임 재개
        Time.timeScale = 1;
        
        if (steps >= tutorialUIPanel.Length)
        {
            PlayerPrefs.SetInt("TutorialCompleted", 1);
        }
    }
}