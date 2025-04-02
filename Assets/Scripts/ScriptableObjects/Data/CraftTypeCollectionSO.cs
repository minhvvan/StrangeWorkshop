using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "SO/Counter/CraftTypeCollectionSO")]
public class CraftTypeCollectionSO : ScriptableObject
{
    public List<CraftRecipeSO> recipes;
}
