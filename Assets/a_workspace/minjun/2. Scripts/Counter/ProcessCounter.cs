using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ProcessCounter : BaseCounter
{
    private ProcessRecipeSO _currentRecipe;
    
    public override void Interact(Player player)
    {
        if (!HasHoldableObject())
        {
            if (player.HasHoldableObject())
            {
                //플레이어가 가지고 있는 HoldableObject로 가공품을 만들 수 있지 검사
                _currentRecipe = RecipeManager.Instance.FindProcessRecipe(player.GetHoldableObject());
                if (_currentRecipe.IsUnityNull()) return;
                player.GiveHoldableObject(this);
            }
        }
        else
        {
            if (!player.HasHoldableObject())
            {
                GiveHoldableObject(player);
                player.TakeoffGlove();
                _currentRecipe = null;
            }
        }
    }

    // 레시피가 존재하면 상호작용시 반환
    public override void InteractAlternate(Player player)
    {
        if (!_currentRecipe.IsUnityNull())
        {
            ClearHoldableObject();
            HoldableObject.SpawnHoldableObject(_currentRecipe.output, this);
            _currentRecipe = null;
        }
    }
}
