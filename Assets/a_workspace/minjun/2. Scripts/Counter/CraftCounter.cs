using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class CraftCounter : BaseCounter
{
    private CraftRecipeSO currentCraftRecipeSO;
    
    public override void Interact(Player player)
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
            currentCraftRecipeSO = RecipeManager.Instance.FindCanCraftRecipe(GetHoldableObjectList());
        }
        else
        {
            if (HasHoldableObject())
            {
                GiveHoldableObject(player);
                currentCraftRecipeSO = RecipeManager.Instance.FindCanCraftRecipe(GetHoldableObjectList());
                player.TakeoffGlove();
            }
        }
    }
    
    // 레시피가 존재하면 상호작용시 결과 반환
    public override void InteractAlternate(Player player)
    {
        if (!currentCraftRecipeSO.IsUnityNull())
        {
            ClearHoldableObject();
            HoldableObject.SpawnHoldableObject(currentCraftRecipeSO.output, this);
            currentCraftRecipeSO = null;
        }
    }
}
