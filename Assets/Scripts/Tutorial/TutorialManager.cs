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
        TutorialEventManager.OnFirstInteract += ShowRecipeTutorial;
        TutorialEventManager.OnTurretMade += ShowGloveTutorial;
        TutorialEventManager.OnBulletEmpty += ShowBulletTutorial;
    }

    private void OnDestroy()
    {
        
        
        
        
    }

    private int currentTutorialIndex = -1;
    // 튜토리얼 UI를 보여주고 게임을 일시정지합니다.
    public void ShowManualTutorial()
    {
        // 게임 일시정지
        Time.timeScale = 0;
        
        Debug.Log("Tutorial Show");
        currentTutorialIndex = 0;
        // 튜토리얼 UI 활성화 및 메시지 표시
        tutorialUIPanel[currentTutorialIndex].SetActive(true);
        flag = true;
        Debug.Log(steps);
    }
    
    public void ShowCounterTutorial()
    {
        // 게임 일시정지
        Time.timeScale = 0;
        currentTutorialIndex = 1;
        Debug.Log("Tutorial Show");
        // 튜토리얼 UI 활성화 및 메시지 표시
        tutorialUIPanel[currentTutorialIndex].SetActive(true);
        flag = true;
        
        TutorialEventManager.OnFirstInput -= ShowCounterTutorial;
    }

    public void ShowRecipeTutorial()
    {
        // 게임 일시정지
        Time.timeScale = 0;
        currentTutorialIndex = 2;
        Debug.Log("Tutorial Show");
        // 튜토리얼 UI 활성화 및 메시지 표시
        tutorialUIPanel[currentTutorialIndex].SetActive(true);
        flag = true;
        
        TutorialEventManager.OnFirstInteract -= ShowRecipeTutorial;
    }
    public void ShowGloveTutorial()
    {
        // 게임 일시정지
        Time.timeScale = 0;
        currentTutorialIndex = 3;
        Debug.Log("Tutorial Show");
        // 튜토리얼 UI 활성화 및 메시지 표시
        tutorialUIPanel[currentTutorialIndex].SetActive(true);
        flag = true;
        
        TutorialEventManager.OnTurretMade -= ShowGloveTutorial;
    }
    
    public void ShowBulletTutorial()
    {
        // 게임 일시정지
        Time.timeScale = 0;
        currentTutorialIndex = 4;
        Debug.Log("Tutorial Show");
        // 튜토리얼 UI 활성화 및 메시지 표시
        tutorialUIPanel[currentTutorialIndex].SetActive(true);
        flag = true;
        
        TutorialEventManager.OnBulletEmpty -= ShowBulletTutorial;
    }
    // UI의 버튼 등에서 호출하여 튜토리얼을 닫고 게임을 재개합니다.
    public void CloseTutorial()
    {
        // 튜토리얼 UI 비활성화
        tutorialUIPanel[currentTutorialIndex].SetActive(false);
        flag = false;
        // 게임 재개
        Time.timeScale = 1;
    }
}