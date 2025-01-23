using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HoldableObjectType
{
    None,
    CraftProduct,
    Bullet,
    Turret,
    Max
}

[CreateAssetMenu(menuName = "SO/Counter/HoldableObjectSO")]
public class HoldableObjectSO : ScriptableObject
{ 
    public GameObject prefab;
    public string objectName;
    public HoldableObjectType objectType;
}
