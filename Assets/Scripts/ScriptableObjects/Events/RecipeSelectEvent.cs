using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeSelectEvent", menuName = "SO/Event/RecipeSelectEvent")]
public class RecipeSelectEvent : BaseEventSO
{
    private event Action<CraftRecipeSO> _onRecipeSelected;
    
    public void Raise(CraftRecipeSO recipe) => _onRecipeSelected?.Invoke(recipe);
    public void AddListener(Action<CraftRecipeSO>  listener) => _onRecipeSelected += listener;
    public void RemoveListener(Action<CraftRecipeSO>  listener) => _onRecipeSelected -= listener;
}
