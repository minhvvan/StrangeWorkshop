using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Counter/CraftRecipeSO")]
public class CraftRecipeSO : ScriptableObject
{
    public List<HoldableObjectSO> inputs;
    public HoldableObjectSO output;
    public int craftNumberOfTimes;
    public RectTransform craftRecipeUI;
}
