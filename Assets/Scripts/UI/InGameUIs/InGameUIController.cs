using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class InGameUIController : MonoBehaviour, IGameUI
{
    Dictionary<InGameUIType, (IGameUI, GameObject)> gameUIControllers = new Dictionary<InGameUIType, (IGameUI, GameObject)>();
    private List<IGameUI> tabUIControllers = new List<IGameUI>();

    [SerializeField] private RecipeUIController _recipeUIController;
    [SerializeField] private WaveUIController _waveUIController;
    [SerializeField] private BarrierUIController _barrierUIController;
    [SerializeField] private ChapterInfoUIController _chapterInfoUIController;
    [SerializeField] private EquipmentUIController _equipmentUIController;
    [SerializeField] private QuestUIController _questUIController;

    private bool gameUIActivated = false;

    async void Awake()
    {
        await UniTask.WaitUntil(() => UIManager.Instance.IsInitialized);
        
        gameUIControllers[InGameUIType.Recipe] = (_recipeUIController, _recipeUIController.gameObject);
        gameUIControllers[InGameUIType.Wave] = (_waveUIController, _waveUIController.gameObject);
        gameUIControllers[InGameUIType.Barrier] = (_barrierUIController, _barrierUIController.gameObject);
        gameUIControllers[InGameUIType.Equipment] = (_equipmentUIController, _equipmentUIController.gameObject);
        gameUIControllers[InGameUIType.ChapterInfo] = (_chapterInfoUIController, _chapterInfoUIController.gameObject);
        gameUIControllers[InGameUIType.Quest] = (_questUIController, _questUIController.gameObject);
    }

    public async void RegisterGameUI(CraftCounter craftCounter)
    {
        await UniTask.WaitUntil(() => RecipeManager.Instance.IsInitialized);
        await _recipeUIController.Initialize(RecipeManager.Instance.GetCraftRecipeCollection);
        craftCounter.OnObjectsChangedAction += _recipeUIController.UpdateUI;
        craftCounter.OnCraftCompleteAction += _recipeUIController.CraftComplete;
    }

    public void RegisterGameUI(EnemySpawner enemySpawner)
    {
        enemySpawner.OnWaveClearAction += _waveUIController.OnWaveClearPopup;
        enemySpawner.OnWaveAlertAction += _waveUIController.OnWaveAlertPopup;
    }

    public void RegisterGameUI(BarrierController barrierController)
    {
        _barrierUIController.SetBarrierController(barrierController);
    }

    public void RegisterGameUI(CharacterInteraction characterInteraction)
    {
        characterInteraction.OnHoldObjectAction += _equipmentUIController.UpdateEquipment;
    }
    public void RegisterGameUI(CharacterInteractionAlternate characterInteraction)
    {
        characterInteraction.OnHoldObjectAction += _equipmentUIController.UpdateEquipment;
    }

    public void RegisterGameUI(QuestManager questManager)
    {
        _questUIController.Initialize();
        QuestManager.Instance.OnQuestProgressUpdated += _questUIController.UpdateQuestProgress;
    }

    public void RegisterGameUI(InGameDataController dataController)
    {
        dataController.OnGoldChanged += _chapterInfoUIController.UpdateGold;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (gameUIActivated) HideUI();
            else ShowUI();

            gameUIActivated = !gameUIActivated;
        }
    }

    public void ShowUI()
    {
        // 모든 자식 UI controller들 활성화
        foreach (var gameUIType in gameUIControllers.Keys)
        {
            gameUIControllers[gameUIType].Item2.SetActive(true);
            gameUIControllers[gameUIType].Item1.ShowUI();
        }
    }

    public void HideUI()
    {
        foreach (var gameUIType in gameUIControllers.Keys)
        {
            gameUIControllers[gameUIType].Item1.HideUI();
        }
    }

    public void Initialize()
    {
    }

    public void CleanUp()
    {
    }

    private enum InGameUIType
    {
        Wave,
        Equipment,
        ChapterInfo,
        Quest,
        Recipe,
        Barrier
    }
}