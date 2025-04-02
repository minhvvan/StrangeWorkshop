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
    private CraftRecipeSO _recipe;
    
    void Awake()
    {
        progressBar.Initialize();
        progressBar.SetColor(Color.green);
        progressBar.ResetBar();
        progressBar.gameObject.SetActive(false);
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
        if (agent != null && agent.GetGameObject().TryGetComponent(out IHoldableObjectParent parent))
        {
            var currentCraftRecipeSO = craftCounter.GetCurrentCraftRecipeSO();
        
            if (!currentCraftRecipeSO.IsUnityNull())
            {
                var craftIndex = craftCounter.GetCraftIndex();
                progressBar.gameObject.SetActive(true);
                progressBar.SetBar(craftIndex);
                if (craftIndex > _currentIndex && cooltime)
                {
                    _currentIndex++;
                    progressBar.UpdateProgressBar(_currentIndex);
                    CoolTime();
                
                    //UI 
                }

                if (craftIndex <= _currentIndex)
                {
                    craftCounter.ClearHoldableObject();
                    var spawnHoldableObject = HoldableObject.SpawnHoldableObject(currentCraftRecipeSO.output, craftCounter, GetHoldableObjectFollowTransform());
                    // var defaultScale = spawnHoldableObject.transform.localScale;
                    // spawnHoldableObject.transform.localScale = Vector3.zero;
                    // spawnHoldableObject.transform.DOScale(defaultScale, 1f);
                    //craftCounter.OnCraftCompleteAction?.Invoke(currentCraftRecipeSO.output);
                    _currentIndex = 0;
                    progressBar.ResetBar();
                    progressBar.gameObject.SetActive(false);
                }
                //VFXManager.Instance.TriggerVFX(VFXType.CRAFTCOUNTERWORKING, transform.position);
            }
        }
    }

    
    
    async void CoolTime()
    {
        cooltime = false;
        await UniTask.WaitForSeconds(0.3f);
        cooltime = true;
    }
}
