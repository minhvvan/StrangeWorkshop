using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ChapterUI : MonoBehaviour, IGameUI
{
    [SerializeField] private Button _gameStartButton;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private Image _lockImage;
    
    private RectTransform _rectTransform;
    private int _chapterIndex;

    public int ChapterIndex
    {
        get => _chapterIndex;
        set => _titleText.text = $"챕터 {value + 1}";
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        _gameStartButton.onClick.AddListener(OnClickGameStart);
    }

    private void OnClickGameStart()
    {
        GameManager.Instance.LoadChapter(ChapterIndex);
    }

    public void ShowUI()
    {
        UIAnimationUtility.PopupShow(_rectTransform, 1f);
    }

    public void HideUI()
    {
        UIAnimationUtility.PopupHide(_rectTransform, .3f);
    }

    public void Initialize()
    {
    }

    public void CleanUp()
    {
    }

    public void SetLocked(bool locked)
    {
        if (locked)
        {
            _lockImage.gameObject.SetActive(true);
            _gameStartButton.interactable = false;
        }
        else
        {
            _lockImage.gameObject.SetActive(false);
            _gameStartButton.interactable = true;
        }
    }
}
