using System;
using UnityEngine;
using UnityEngine.UI;

public class ResultUIContorller : BasePopupUI
{
    [SerializeField] Button _gotoChapterButton;

    void Awake()
    {
        _gotoChapterButton.onClick.AddListener(OnClickGotoChapter);
    }

    void OnClickGotoChapter()
    {
        GameManager.Instance.StartGame();
    }
}