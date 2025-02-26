    using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _gameStartButton;
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _gameExitButton;
    
    [SerializeField] private RectTransform _defaultPanel;
    [SerializeField] private RectTransform _optionPanel;
    [SerializeField] private CanvasGroup _mainMenuCanvasGroup;

    [SerializeField] private MainMenuSpaceShip _mainMenuSpaceShip;
    
    void Start()
    {
        _gameStartButton.onClick.AddListener(() =>
        {
            OnClickGameStart().Forget();
        });
        _optionButton.onClick.AddListener(OnClickOption);
        _gameExitButton.onClick.AddListener(OnClickGameExit);

        _optionPanel.GetComponent<OptionPanel>().onExitClick += () =>
        {
            Vector2 originalPos = _optionPanel.anchoredPosition;
            UIAnimationUtility.SlideOutRight(_optionPanel, callback: ()=>{_optionPanel.anchoredPosition = originalPos;});
            UIAnimationUtility.SlideInLeft(_defaultPanel);
        };
    }

    private async UniTask OnClickGameStart()
    {
        UIAnimationUtility.FadeOut(_mainMenuCanvasGroup);
        await _mainMenuSpaceShip.OnClickedGameStart();
        GameManager.Instance.StartGame();
    }
    
    private void OnClickOption()
    {
        Vector2 originalPos = _defaultPanel.anchoredPosition;
        UIAnimationUtility.SlideOutLeft(_defaultPanel, callback: () => _defaultPanel.anchoredPosition = originalPos);
        UIAnimationUtility.SlideInRight(_optionPanel);
    }
    
    private void OnClickGameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDisable()
    {
        DOTween.Complete(_defaultPanel);
        DOTween.Kill(_defaultPanel);
    }
}