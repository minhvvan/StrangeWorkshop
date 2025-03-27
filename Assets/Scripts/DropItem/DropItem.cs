using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class DropItem : MonoBehaviour
{
    [Header("ItemInfo")]
    public ItemInfo itemInfo;
    private ItemType _itemType;
    private ItemName _itemName;
    private float _itemValue;

    public void Initialize()
    {
        _itemType = itemInfo.itemType;
        _itemName = itemInfo.itemName;
        _itemValue = itemInfo.itemValue;
    }

    public ItemType GetItemType()
    {
        return _itemType;
    }

    public ItemName GetItemName()
    {
        return _itemName;
    }

    public float GetValue()
    {
        return _itemValue;
    }

    public void Collect()
    {
        DropItemManager.Instance.ReturnItem(_itemName, gameObject).Forget();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            Collect();
        }
    }
}