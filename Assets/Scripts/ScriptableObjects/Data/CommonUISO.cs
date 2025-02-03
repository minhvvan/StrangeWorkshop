using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CommonUIs", menuName = "SO/UI/CommonUI")]
public class CommonUISO : ScriptableObject
{
    public List<UIMapping> commonUIs;
}

[Serializable]
public class UIMapping
{
    public UIType type;
    public RectTransform prefab;
    public Vector2 position;
}