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
        if (_isWork && !_recipe.IsUnityNull())
        {
            _currentTime += Time.deltaTime;
            progressBar.UpdateProgressBar(_currentTime);
            if (_currentTime >= _recipe.craftNumberOfTimes)
            {
                _currentTime = 0;
                _isWork = false;
                craftCounter.ClearHoldableObject();
                var spawnHoldableObject = HoldableObject.SpawnHoldableObject(_recipe.output, craftCounter);
                var defaultScale = spawnHoldableObject.transform.localScale;
                spawnHoldableObject.transform.localScale = Vector3.zero;
                spawnHoldableObject.transform.DOScale(defaultScale, 1f);
                craftCounter.OnCraftCompleteAction?.Invoke(_recipe.output);
                _currentIndex = 0;
                progressBar.ResetBar();
                progressBar.gameObject.SetActive(false);
            }
        }
    }
    
    public override void Interact(IHoldableObjectParent plqyer)
    {
        // TODO: UI창 띄어서 레시피 받기
        
    }

    public override void InteractAlternate(IHoldableObjectParent player)
    {
        _recipe = craftCounter.GetCurrentCraftRecipeSO();

        if (_recipe != null)
        {
            _isWork = true;
            progressBar.gameObject.SetActive(true);
            progressBar.SetBar(_recipe.craftNumberOfTimes);   
        }
        
        // var currentCraftRecipeSO = craftCounter.GetCurrentCraftRecipeSO();
        //
        // if (!currentCraftRecipeSO.IsUnityNull())
        // {
        //     var craftIndex = craftCounter.GetCraftIndex();
        //     progressBar.gameObject.SetActive(true);
        //     progressBar.SetBar(craftIndex);
        //     if (craftIndex > _currentIndex && cooltime)
        //     {
        //         _currentIndex++;
        //         progressBar.UpdateProgressBar(_currentIndex);
        //         CoolTime();
        //         
        //         //UI 
        //     }
        //
        //     if (craftIndex <= _currentIndex)
        //     {
        //         craftCounter.ClearHoldableObject();
        //         var spawnHoldableObject = HoldableObject.SpawnHoldableObject(currentCraftRecipeSO.output, craftCounter);
        //         var defaultScale = spawnHoldableObject.transform.localScale;
        //         spawnHoldableObject.transform.localScale = Vector3.zero;
        //         spawnHoldableObject.transform.DOScale(defaultScale, 1f);
        //         craftCounter.OnCraftCompleteAction?.Invoke(currentCraftRecipeSO.output);
        //         _currentIndex = 0;
        //         progressBar.ResetBar();
        //         progressBar.gameObject.SetActive(false);
        //     }
        //     //VFXManager.Instance.TriggerVFX(VFXType.CRAFTCOUNTERWORKING, transform.position);
        // }
    }
    
    async void CoolTime()
    {
        cooltime = false;
        await UniTask.WaitForSeconds(0.3f);
        cooltime = true;
    }
}
