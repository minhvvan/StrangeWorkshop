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

    public CraftRecipeCollectionSO GetCraftRecipeCollection => craftRecipeCollection;
    
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
    // public CraftRecipeSO FindCanCraftRecipe(List<HoldableObject> inputs)
    // {
    //     CraftRecipeSO result = null;
    //     var inputList = inputs.Select(x => x.GetHoldableObjectSO()).ToList();
    //     foreach (var craftRecipe in craftRecipeCollection.recipes)
    //     {
    //         if (CanMake(inputList, craftRecipe))
    //         {
    //             result = craftRecipe;
    //         }
    //     }
    //     return result;
    // }
    //
    // // WillMake CraftRecipe 검사
    // public List<CraftRecipeSO> FindCraftRecipeCandidate(List<HoldableObject> inputs)
    // {
    //     // 레시피 후보군을 List로 전부 반환하도록 변경 완료
    //     // 후보군이 있는지 확인할 때는 list 길이가 0보다 큰지 체크
    //     List<CraftRecipeSO> result = new List<CraftRecipeSO>();
    //
    //     if (inputs.Count <= 0) return result; // inputs가 빈 리스트일때 예외처리
    //     
    //     var inputList = inputs.Select(x => x.GetHoldableObjectSO()).ToList();
    //     foreach (var craftrecipe in craftRecipeCollection.recipes)
    //     {
    //         if (WillMake(inputList, craftrecipe))
    //         {
    //             result.Add(craftrecipe);
    //         }
    //     }
    //
    //     return result;
    // }
    
    public bool WillMake(List<HoldableObject> inputs, CraftRecipeSO craftRecipe)
    {
        ISet<int> inputSet = new HashSet<int>();
        var inputList = inputs.Select(x => x.GetHoldableObjectSO()).ToList();
        foreach (HoldableObjectSO input in inputList)
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

    public bool CanMake(List<HoldableObject> inputs, CraftRecipeSO craftRecipe)
    {
        ISet<int> inputSet = new HashSet<int>();
        var inputList = inputs.Select(x => x.GetHoldableObjectSO()).ToList();
        foreach (HoldableObjectSO recipeInput in craftRecipe.inputs)
        {
            int inputIndex = FindNotInSet(inputList, recipeInput, inputSet);
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
