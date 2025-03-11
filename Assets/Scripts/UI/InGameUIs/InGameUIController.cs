using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class InGameUIController : MonoBehaviour, IGameUI
{
    List<(IGameUI, GameObject)> gameUIControllers = new List<(IGameUI, GameObject)>();
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

        RegisterGameUI();
    }

    async public void RegisterGameUI(CraftCounter craftCounter)
    {
        await UniTask.WaitUntil(() => RecipeManager.Instance.IsInitialized);
        _recipeUIController.Initialize(RecipeManager.Instance.GetCraftRecipeCollection);
        craftCounter.OnObjectsChangedAction += _recipeUIController.UpdateUI;
        craftCounter.OnCraftCompleteAction += _recipeUIController.CraftComplete;
        gameUIControllers.Add((_recipeUIController, _recipeUIController.gameObject));
    }

    async public void RegisterGameUI(EnemySpawner enemySpawner)
    {
        enemySpawner.OnWaveClearAction += _waveUIController.OnWaveClearPopup;
        enemySpawner.OnWaveAlertAction += _waveUIController.OnWaveAlertPopup;
        gameUIControllers.Add((_waveUIController, _waveUIController.gameObject));
    }

    async public void RegisterGameUI(BarrierController barrierController)
    {
        _barrierUIController.SetBarrierController(barrierController);
        gameUIControllers.Add((_barrierUIController, _barrierUIController.gameObject));
    }

    async public void RegisterGameUI(CharacterInteraction characterInteraction)
    {
        characterInteraction.OnHoldObjectAction += _equipmentUIController.UpdateEquipment;
        gameUIControllers.Add((_equipmentUIController, _equipmentUIController.gameObject));
    }

    async public void RegisterGameUI(QuestManager questManager)
    {
        _questUIController.Initialize();
        QuestManager.Instance.OnQuestProgressUpdated += _questUIController.UpdateQuestProgress;
        gameUIControllers.Add((_questUIController, _questUIController.gameObject));
    }

    async public void RegisterGameUI()
    {
        gameUIControllers.Add((_chapterInfoUIController, _chapterInfoUIController.gameObject));
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
        foreach (var gameUI in gameUIControllers)
        {
            gameUI.Item2.SetActive(true);
            gameUI.Item1.ShowUI();
        }
    }

    public void HideUI()
    {
        foreach (var gameUI in gameUIControllers)
        {
            gameUI.Item1.HideUI();
        }
    }

    public void Initialize()
    {
        throw new NotImplementedException();
    }

    public void CleanUp()
    {
        throw new NotImplementedException();
    }
}