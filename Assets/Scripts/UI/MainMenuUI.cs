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
    [SerializeField] private MainMenuButton _gameStartButton;
    [SerializeField] private MainMenuButton _optionButton;
    [SerializeField] private MainMenuButton _gameExitButton;
    
    [SerializeField] private RectTransform _defaultPanel;
    [SerializeField] private RectTransform _optionPanel;
    [SerializeField] private CanvasGroup _mainMenuCanvasGroup;

    [SerializeField] private CharacterSkinController _mainMenuCharacterController;
    
    void Start()
    {
        //게임 시작 버튼이 기본으로 선택
        _gameStartButton.Select();

        _gameStartButton.buttonIndex = 0;
        _optionButton.buttonIndex = 1;
        _gameExitButton.buttonIndex = 2;
        
        _gameStartButton.onSelected += OnMenuButtonSelected;
        _optionButton.onSelected += OnMenuButtonSelected;
        _gameExitButton.onSelected += OnMenuButtonSelected;
        
        _gameStartButton.onSubmited += OnClickGameStart;
        _optionButton.onSubmited += OnClickOption;
        _gameExitButton.onSubmited += OnClickGameExit;
        
        _optionPanel.GetComponent<OptionPanel>().onExitClick += () =>
        {
            Vector2 originalPos = _optionPanel.anchoredPosition;
            UIAnimationUtility.SlideOutRight(_optionPanel, callback: ()=>{_optionPanel.anchoredPosition = originalPos;});
            UIAnimationUtility.SlideInLeft(_defaultPanel);
        };
    }

    private void OnMenuButtonSelected(int buttonIndex)
    {
        _mainMenuCharacterController.ChangeState(buttonIndex);
    }

    private void OnClickGameStart()
    {
        UIAnimationUtility.FadeOut(_mainMenuCanvasGroup);
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
        //TODO: Popup 띄우기 
        GameManager.Instance.QuitGame();
    }

    private void OnDisable()
    {
        DOTween.Complete(_defaultPanel);
        DOTween.Kill(_defaultPanel);
    }
}