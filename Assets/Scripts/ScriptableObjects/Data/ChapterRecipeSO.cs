using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChapterRecipe
{
    public string chapterName;
    public List<CraftRecipeSO> recommendedRecipes;
}

[CreateAssetMenu(fileName = "ChapterRecipe", menuName = "SO/UI/ChapterRecipe")]
public class ChapterRecipeSO : ScriptableObject
{
    public List<ChapterRecipe> chapterRecipes;
}
