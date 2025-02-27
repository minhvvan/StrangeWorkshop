using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public enum UIType 
{
    None,
    MinimapUI,
    WaveUI,
    RecipeUI,
    ChapterUI,
    Max,
}

public class UIManager : Singleton<UIManager>
{
    private List<UIMapping> _gameUIs = new();
    private Dictionary<UIType, IGameUI> _activeUIs = new();

    private Canvas _mainCanvas;
    private Camera _mainCamera;

    private const string CanvasTag = "MainCanvas";
    
    public bool IsInitialized { get; private set; }

    private async void Start()
    {
        await Initialize();

        if (GameObject.FindWithTag(CanvasTag) != null)
        {
            _mainCanvas = GameObject.FindWithTag(CanvasTag).GetComponent<Canvas>();
        }
        
        _mainCamera = Camera.main;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameObject.FindWithTag(CanvasTag) != null)
        {
            _mainCanvas = GameObject.FindWithTag(CanvasTag).GetComponent<Canvas>();
        }
        
        foreach (var ui in _activeUIs.Values)
        {
            ui.Initialize();
        }
    }

    public async UniTask Initialize()
    {
        var result = await DataManager.Instance.LoadDataAsync<CommonUISO>(Addresses.Data.UI.COMMON);

        foreach (var commonUI in result.commonUIs)
        {
            _gameUIs.Add(commonUI);
        }

        IsInitialized = true;
    }

    public RectTransform AttachUI(RectTransform uiPrefab, Vector3 position)
    {
        if (_mainCamera.IsUnityNull()) return null;

        var result = Instantiate(uiPrefab, _mainCanvas.transform);
        
        Vector2 screenPoint = _mainCamera.WorldToScreenPoint(position);
        result.position = screenPoint;

        return result;
    }

    public T AttachUI<T>(UIType type) where T : Component, IGameUI
    {
        var ui = GetUI<T>(type);
        ui.ShowUI();

        return ui;
    }
    
    public void DetachUI<T>(UIType type) where T : Component, IGameUI
    {
        var ui = GetUI<T>(type);
        ui.HideUI();
    }
    
    public T GetUI<T>(UIType type) where T : Component, IGameUI
    {
        if (_activeUIs.TryGetValue(type, out var ui))
        {
            return ui as T;
        }

        return CreateUI<T>(type);
    }

    private T CreateUI<T>(UIType type) where T : Component, IGameUI
    {
        var ui = _gameUIs.Find(ui => ui.type == type);
        if (ui == null || ui.prefab == null) return null;
        
        var instance = Instantiate(ui.prefab, _mainCanvas.transform).GetComponent<T>();
        return (_activeUIs[type] = instance) as T;
    }
}
