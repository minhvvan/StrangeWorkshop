using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _gameStartButton;
    
    void Start()
    {
        _gameStartButton.onClick.AddListener(OnClickGameStart);
    }

    private void OnClickGameStart()
    {
        GameManager.Instance.StartGame();
    }
}