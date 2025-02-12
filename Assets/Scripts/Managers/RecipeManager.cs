using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;

public class RecipeManager : Singleton<RecipeManager>
{
    [SerializeField] private CraftRecipeCollectionSO craftRecipeCollection;
    [SerializeField] private ProcessRecipeCollectionSO processRecipeCollection;

    public bool IsInitialized { get; private set; }

    private async void Start()
    {
        await Initialize();
    }

    public async UniTask Initialize()
    {
        craftRecipeCollection = await DataManager.Instance.LoadDataAsync<CraftRecipeCollectionSO>(Addresses.Data.Recipe.CRAFT);
        processRecipeCollection = await DataManager.Instance.LoadDataAsync<ProcessRecipeCollectionSO>(Addresses.Data.Recipe.PROCESS);
        IsInitialized = true;
    }

    public void Reset()
    {
        // 초기화 관련 데이터 리셋
        IsInitialized = false;
    }

    //ProcessRecipe 검사
    public ProcessRecipeSO FindProcessRecipe(HoldableObject holdableObject)
    {
        foreach (var recipe in processRecipeCollection.recipes)
        {
            if (recipe.input == holdableObject.GetHoldableObjectSO())
            {
                return recipe;
            }
        }
        return null;
    }
    
    // Can CraftRecipe 검사
    public CraftRecipeSO FindCanCraftRecipe(List<HoldableObject> inputs)
    {
        CraftRecipeSO result = null;
        var inputList = inputs.Select(x => x.GetHoldableObjectSO()).ToList();
        foreach (var craftRecipe in craftRecipeCollection.recipes)
        {
            if (CanMake(inputList, craftRecipe))
            {
                result = craftRecipe;
            }
        }
        return result;
    }
    
    // WillMake CraftRecipe 검사
    public CraftRecipeSO FindCraftRecipeCandidate(List<HoldableObject> inputs)
    {
        var inputList = inputs.Select(x => x.GetHoldableObjectSO()).ToList();
        foreach (var craftrecipe in craftRecipeCollection.recipes)
        {
            if (WillMake(inputList, craftrecipe))
            {
                return craftrecipe;
            }
        }
        return null;
    }
    
    private bool WillMake(List<HoldableObjectSO> inputs, CraftRecipeSO craftRecipe)
    {
        ISet<int> inputSet = new HashSet<int>();
        foreach (HoldableObjectSO input in inputs)
        {
            int inputIndex = FindNotInSet(craftRecipe.inputs, input, inputSet);
            if (inputIndex < 0)
            {
                return false;
            }
            inputSet.Add(inputIndex);
        }
        return true;
    }

    private bool CanMake(List<HoldableObjectSO> inputs, CraftRecipeSO craftRecipe)
    {
        ISet<int> inputSet = new HashSet<int>();
        foreach (HoldableObjectSO recipeInput in craftRecipe.inputs)
        {
            int inputIndex = FindNotInSet(inputs, recipeInput, inputSet);
            if (inputIndex < 0)
            {
                return false;
            }
            inputSet.Add(inputIndex);
        }
        if(inputSet.Count != inputs.Count)
            return false;
            
        return true;
    }

    private int FindNotInSet(List<HoldableObjectSO> soList, HoldableObjectSO so, ISet<int> exclusionSet)
    {
        for (int index = 0; index < soList.Count; ++index)
        {
            if (exclusionSet.Contains(index))
            {
                continue;
            }
            else if (soList[index] != so)
            {
                continue;
            }
            return index;
        }
        return -1;
    }
}
