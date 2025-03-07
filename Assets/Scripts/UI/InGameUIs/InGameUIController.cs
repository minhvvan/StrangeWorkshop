using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

public class InGameUIController : MonoBehaviour, IGameUI
{
    List<IGameUI> gameUIControllers = new List<IGameUI>();
    private List<IGameUI> tabUIControllers = new List<IGameUI>();
    
    [SerializeField] private RecipeUIController _recipeUIController;
    [SerializeField] private WaveUIController _waveUIController;
    [SerializeField] private BarrierUIController _barrierUIController;
    [SerializeField] private ChapterInfoUIController _chapterInfoUIController;
    [SerializeField] private EquipmentUIController _equipmentUIController;
    [SerializeField] private QuestUIController _questUIController;
    
    public static event Action<bool> OnTabToggled; // Tab UI 토글 이벤트
    private bool isTabActive = false;

    async void Awake()
    {
        await UniTask.WaitUntil(()=>UIManager.Instance.IsInitialized);
        gameUIControllers.Add(_recipeUIController);
        gameUIControllers.Add(_waveUIController);
        gameUIControllers.Add(_barrierUIController);
        gameUIControllers.Add(_chapterInfoUIController);

        RegisterGameUI();
    }

    async public void RegisterGameUI(CraftCounter craftCounter)
    {
        await UniTask.WaitUntil(() => RecipeManager.Instance.IsInitialized);
        _recipeUIController.gameObject.SetActive(true);
        _recipeUIController.Initialize(RecipeManager.Instance.GetCraftRecipeCollection);
        craftCounter.OnObjectsChangedAction += _recipeUIController.UpdateUI;
        craftCounter.OnCraftCompleteAction += _recipeUIController.CraftComplete;
    }

    async public void RegisterGameUI(EnemySpawner enemySpawner)
    {
        _waveUIController.gameObject.SetActive(true);
        var waveClearEvent = await DataManager.Instance.LoadDataAsync<WaveClearEventSO>(Addresses.Events.Game.WAVE_CLEAR);
        waveClearEvent.AddListener(_waveUIController.OnWaveClearPopup);
        
        var waveStartEvent = await DataManager.Instance.LoadDataAsync<WaveStartEventSO>(Addresses.Events.Game.WAVE_START);
        waveStartEvent.AddListener(_waveUIController.OnWaveAlertPopup);
    }

    async public void RegisterGameUI(BarrierController barrierController)
    {
        _barrierUIController.gameObject.SetActive(true);
        _barrierUIController.SetBarrierController(barrierController);
    }

    async public void RegisterGameUI(CharacterInteraction characterInteraction)
    {
        _equipmentUIController.gameObject.SetActive(true);
        characterInteraction.OnHoldObjectAction += _equipmentUIController.UpdateEquipment;
    }
    
    async public void RegisterGameUI()
    {
        _chapterInfoUIController.gameObject.SetActive(true);
    }

    async public void RegisterGameUI(QuestManager questManager)
    {
        _questUIController.gameObject.SetActive(true);
        _questUIController.Initialize();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isTabActive = !isTabActive;
            OnTabToggled?.Invoke(isTabActive); // 이벤트 발생 (모든 구독자에게 전달)
        }
    }

    private void OpenTabUI()
    {
        foreach (var gameUI in gameUIControllers)
        {
            gameUI.HideUI();
        }
        
    }

    public void ShowUI()
    {
        // 모든 자식 UI controller들 활성화
        foreach (var gameUI in gameUIControllers)
        {
            gameUI.ShowUI();
        }
    }

    public void HideUI()
    {
        throw new NotImplementedException();
    }

    public void Initialize()
    {
    }

    public void CleanUp()
    {
        throw new NotImplementedException();
    }
}

/*
 * child UI controller들은 awake가 아니라 onenable을 사용하는게 좋을지도?
 * child UI controller는 IGameUI가 아닌 다른 interface를 만들어도 될 것 같다.
 * childUIcontroller의 showUI는 enable 될때 등장하는 animation
 * hide UI는 disable 될때 퇴장하는 animation
 * 
*/