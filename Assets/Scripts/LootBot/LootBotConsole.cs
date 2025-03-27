using System;
using Managers;
using UnityEngine;

public class LootBotConsole : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform lootBotSpawnPosition;
    
    private LootBotInputHandler _currentLootBotInputHandler;
    private GameObject _lootBotPrefab;
    private GameObject _lootBot;
    
    private async void Awake()
    {
        _lootBotPrefab = await DataManager.Instance.LoadPrefabAsync(Addresses.Prefabs.Game.LOOTBOT);
       
    }

    private void OnLootBotInteractAlternate()
    {
        Debug.Log("Return to Player");
        _currentLootBotInputHandler.OnInteract -= OnLootBotInteract;
        _currentLootBotInputHandler.OnInteractAlternate -= OnLootBotInteractAlternate;
        
        CameraManager.Instance.ResetFollowTarget();
        InputManager.Instance.ReturnToPlayerControl();
    }

    private void OnLootBotInteract()
    {
        Debug.Log("LootBot Interact");
    }

    public void Interact(IInteractAgent agent = null)
    {
        _lootBot = Instantiate(_lootBotPrefab, lootBotSpawnPosition.position, lootBotSpawnPosition.rotation);

        if (_lootBot.TryGetComponent(out LootBotInputHandler inputHandler))
        {
            _currentLootBotInputHandler = inputHandler;
            
            _currentLootBotInputHandler.OnInteract += OnLootBotInteract;
            _currentLootBotInputHandler.OnInteractAlternate += OnLootBotInteractAlternate;
            
            CameraManager.Instance.SetFollowTarget(_lootBot);
            InputManager.Instance.SwitchControl(_currentLootBotInputHandler);
        }
    }

    public void InteractAlternate(IInteractAgent agent = null)
    {
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
