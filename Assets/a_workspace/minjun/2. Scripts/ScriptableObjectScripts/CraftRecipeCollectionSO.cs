using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "SO/Counter/CraftRecipeCollectionSO")]
public class CraftRecipeCollectionSO : ScriptableObject
{
    public List<CraftRecipeSO> recipes;
}
