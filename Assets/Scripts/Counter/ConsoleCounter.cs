using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class ConsoleCounter : BaseCounter
{
    [SerializeField] private CraftCounter craftCounter;
    [SerializeField] private ProgressBar progressBar;

    private bool cooltime = true;
    private int _currentIndex;
    private float _currentTime;
    private bool _isWork = false;
    private CraftRecipeSO _recipe;
    
    
    void Awake()
    {
        progressBar.Initialize();
        progressBar.SetColor(Color.green);
        progressBar.ResetBar();
        progressBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (_isWork)
        {
            _currentTime += Time.deltaTime;
            progressBar.UpdateProgressBar(_currentTime);
            if (_currentTime >= _recipe.craftNumberOfTimes)
            {
                _currentTime = 0;
                _isWork = false;
                craftCounter.ClearHoldableObject();
                var spawnHoldableObject = HoldableObject.SpawnHoldableObject(_recipe.output, craftCounter, GetHoldableObjectFollowTransform());
                //craftCounter.OnCraftCompleteAction?.Invoke(_recipe.output);
                _currentIndex = 0;
                progressBar.ResetBar();
                progressBar.gameObject.SetActive(false);
            }
        }
    }
    
    public override void Interact(IInteractAgent agent = null)
    {
        if (agent != null && agent.GetGameObject().TryGetComponent(out IHoldableObjectParent parent))
        {
            if(agent is SampleCharacterController player)
            {
                player.SetState(player.interactState);
                UIManager.Instance.GetUI<CraftSelectUIController>(UIType.RecipeSelectUI).ShowUI();
                UIManager.Instance.GetUI<CraftSelectUIController>(UIType.RecipeSelectUI).OnHide += () => {player.SetState(player.idleState);};
            }

        }
    }

    public override void InteractAlternate(IInteractAgent agent = null)
    {
        _recipe = craftCounter.GetCurrentCraftRecipeSO();

        if (!_recipe.IsUnityNull() && RecipeManager.Instance.CanMake(craftCounter.GetHoldableObjectList(), _recipe))
        {
            _isWork = true;
            progressBar.gameObject.SetActive(true);
            progressBar.SetBar(_recipe.craftNumberOfTimes);   
        }
    }

    
    
    async void CoolTime()
    {
        cooltime = false;
        await UniTask.WaitForSeconds(0.3f);
        cooltime = true;
    }
}
