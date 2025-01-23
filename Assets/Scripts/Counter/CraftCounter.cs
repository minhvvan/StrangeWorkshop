using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CraftCounter : BaseCounter
{
    private CraftRecipeSO _currentCraftRecipeSO;
    private int _currentCraftIndex = int.MaxValue;
    private bool cooltime = true;
    public override void Interact(SampleCharacterController player)
    {
        // 플레이어가 물체를 들고 있으면
        if (player.HasHoldableObject())
        {
            // DeepCopy로 연산에 필요한 List생성 후 계산
            List<HoldableObject> CompareList = new(GetHoldableObjectList())
            {
                player.GetHoldableObject()
            };
            
            // 플레이어의 재료를 놓을 때 만들 수 있는 레시피가 있는 검사
            if (!RecipeManager.Instance.FindCraftRecipeCandidate(CompareList))
            {
                return;
            }
            
            player.GiveHoldableObject(this);
            
            // 현재 만들 수 있는 레시피가 있으면 저장
            _currentCraftRecipeSO = RecipeManager.Instance.FindCanCraftRecipe(GetHoldableObjectList());
            SetCurrentCraftIndex();
        }
        else
        {
            if (HasHoldableObject())
            {
                GiveHoldableObject(player);
                _currentCraftRecipeSO = RecipeManager.Instance.FindCanCraftRecipe(GetHoldableObjectList());
                SetCurrentCraftIndex();
                player.TakeoffGlove();
            }
        }
    }
    
    // 레시피가 존재하면 상호작용시 결과 반환
    public override void InteractAlternate(SampleCharacterController player)
    {
        if (!_currentCraftRecipeSO.IsUnityNull())
        {
            if (_currentCraftIndex > 0 && cooltime)
            {
                _currentCraftIndex--;
                CoolTime();
            }
            if(_currentCraftIndex <= 0)
            {
                ClearHoldableObject();
                HoldableObject.SpawnHoldableObject(_currentCraftRecipeSO.output, this);
                _currentCraftRecipeSO = null;
                _currentCraftIndex = int.MaxValue;   
            }
        }
    }

    private void SetCurrentCraftIndex()
    {
        if (!_currentCraftRecipeSO.IsUnityNull())
            _currentCraftIndex = _currentCraftRecipeSO.craftNumberOfTimes;
    }

    async void CoolTime()
    {
        cooltime = false;
        await UniTask.WaitForSeconds(0.3f);
        cooltime = true;
    }
}
