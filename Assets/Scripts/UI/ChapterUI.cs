using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChapterUI : MonoBehaviour, IGameUI
{
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
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
}
