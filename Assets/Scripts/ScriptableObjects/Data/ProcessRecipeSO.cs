using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Counter/ProcessRecipeSO")]
public class ProcessRecipeSO : ScriptableObject
{
    public HoldableObjectSO input;
    public HoldableObjectSO output;
    public float processTime;
}
