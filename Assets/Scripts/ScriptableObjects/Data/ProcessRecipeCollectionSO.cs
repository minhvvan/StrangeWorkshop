using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Counter/ProcessRecipeCollectionSO")]
public class ProcessRecipeCollectionSO : ScriptableObject
{
    public List<ProcessRecipeSO> recipes;
}
