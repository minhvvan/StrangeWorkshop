using System;
using UnityEngine;
using UnityEngine.UI;

public class LoseUIController : BasePopupUI
{
    [SerializeField] Button _restartButton;
    [SerializeField] Button _gotoChapterButton;

    void Awake()
    {
        Initialize();
        _gotoChapterButton.onClick.AddListener(OnClickGotoChapter);
        _restartButton.onClick.AddListener(OnClickRestart);
    }


    void OnClickGotoChapter()
    {
        GameManager.Instance.StartGame();
    }

    void OnClickRestart()
    {
        GameManager.Instance.RestartGame();
    }
}