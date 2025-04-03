using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HoldableObjectType
{
    None,
    PartMaterial,
    Bullet,
    Turret,
    Upgrade,
    Kit
}

public enum PartMaterialType
{
    None,
    Carbon,
    Gear,
    Ore,
    Bolt
}

[CreateAssetMenu(menuName = "SO/Counter/HoldableObjectSO")]
public class HoldableObjectSO : ScriptableObject
{ 
    public GameObject prefab;
    public string objectName;
    public HoldableObjectType objectType;
    public Sprite objectSprite;
    public int price;
}
