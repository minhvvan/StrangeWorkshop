using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropItemDatas", menuName = "SO/Drops/DropItemDatas")]
public class DropItemDatasSO : ScriptableObject
{
    [Header("DropItems")]
    public List<DropItemSO> droppedItems = new();
}