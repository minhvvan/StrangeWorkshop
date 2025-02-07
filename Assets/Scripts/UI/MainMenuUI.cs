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

    [SerializeField] private MainMenuSpaceShip _spaceShip;
    
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
            UIAnimationUtility.SlideOutRight(_optionPanel);
            UIAnimationUtility.SlideInLeft(_defaultPanel);
        };
    }

    private async UniTask OnClickGameStart()
    {
        await _spaceShip.OnClickedGameStart();
        UIAnimationUtility.SlideOutDown(_defaultPanel);
        GameManager.Instance.StartGame();
    }
    
    private void OnClickOption()
    {
        UIAnimationUtility.SlideOutLeft(_defaultPanel);
        _optionPanel.anchoredPosition = Vector2.zero;
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
        DOTween.Complete(_spaceShip.GetComponent<RectTransform>());
        
        DOTween.Kill(_defaultPanel);
        DOTween.Kill(_spaceShip.GetComponent<RectTransform>());
    }
}