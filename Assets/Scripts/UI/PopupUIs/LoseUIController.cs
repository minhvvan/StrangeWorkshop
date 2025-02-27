using System;
using UnityEngine;
using UnityEngine.UI;

public class LoseUIContorller : BasePopupUI
{
    [SerializeField] Button _restartButton;
    [SerializeField] Button _gotoChapterButton;

    void Awake()
    {
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