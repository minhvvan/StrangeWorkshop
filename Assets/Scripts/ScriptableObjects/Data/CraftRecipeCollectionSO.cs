using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu( menuName = "SO/Counter/CraftRecipeCollectionSO")]
public class CraftRecipeCollectionSO : ScriptableObject
{
    public SerializedDictionary<CraftType, CraftTypeCollectionSO> recipesCollection;
}

public enum CraftType
{
    Fix,
    Bullet,
    Energy
}
