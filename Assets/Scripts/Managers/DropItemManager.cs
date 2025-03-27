using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropItemManager : Singleton<DropItemManager>
{
    private Dictionary<ItemName, Queue<GameObject>> itemPools = new();
    private Dictionary<ItemName, DropItemSO> itemDatas = new();
    
    public async UniTask Initialize()
    {
        await LoadDropItems();
    }

    private async UniTask LoadDropItems()
    {
        var dropItemDataSO = await DataManager.Instance.LoadDataAsync<DropItemDatasSO>
            (Addresses.Data.Drops.DROPITEMS);

        foreach (var dropitemSO in dropItemDataSO.droppedItems)
        {
            RegisterItem(dropitemSO);
        }
    }

    private void RegisterItem(DropItemSO dropItemSO)
    {
        if (!itemPools.ContainsKey(dropItemSO.itemName))
        {
            itemPools[dropItemSO.itemName] = new Queue<GameObject>();
            itemDatas[dropItemSO.itemName] = dropItemSO;
            
            for (int i = 0; i < dropItemSO.poolSize; i++)
            {
                GameObject itemObject = Instantiate(dropItemSO.item, transform);
                itemObject.SetActive(false);
                itemPools[dropItemSO.itemName].Enqueue(itemObject);
            }
        }
    }

    private GameObject DequeueItem(ItemName itemName)
    {
        if (!itemPools.ContainsKey(itemName))
        {
            Debug.LogError($"{itemName} doesn't exist in ItemPools!");
            return null;
        }
        
        if (itemPools[itemName].Count <= 0)
        {
            return Instantiate(itemDatas[itemName].item);
        }
        else
        {
            return itemPools[itemName].Dequeue();
        }
    }
    
    public async UniTask ReturnItem(ItemName itemName, GameObject itemObject, float? duration = null)
    {
        var itemInfo = itemObject.GetComponent<DropItem>().itemInfo;
        duration ??= 0;
        
        //비동기 중복 방지
        itemInfo.Cts?.Cancel();
        itemInfo.Cts = new CancellationTokenSource();
        await UniTask.Delay((int)(1000*duration.Value),cancellationToken: itemInfo.Cts.Token);
        
        DOTween.Kill(itemObject);
        itemObject.transform.SetParent(Instance.transform);
        itemObject.SetActive(false);
        itemPools[itemName].Enqueue(itemObject);
    }
    
    //오브젝트를 내보낼 때 기본 값 세팅용
    private GameObject SetDropItem(ItemName itemName, GameObject itemObject, Vector3 position, 
        Vector3? size = null, [CanBeNull] Transform parent = null, float? duration = null)
    {
        var itemObjectInfo = itemObject.GetComponent<DropItem>();
        itemObjectInfo.itemInfo.itemName = itemName;
        itemObjectInfo.itemInfo.itemType = itemDatas[itemName].itemType;
        itemObjectInfo.itemInfo.itemValue = itemDatas[itemName].itemValue;
        itemObjectInfo.itemInfo.Cts = new CancellationTokenSource();
        itemObjectInfo.Initialize();
        
        itemObject.transform.localScale = size ?? itemDatas[itemName].size;
        itemObject.transform.SetParent(parent ?? transform);
        itemObject.transform.position = position;
        itemObject.transform.DORotate(new Vector3(0,360,0), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        itemObject.SetActive(true);
        
        _ = ReturnItem(itemName, itemObject, duration ?? itemDatas[itemName].duration);
        
        return itemObject;
    }
    
    //이름이 같은 아이템을 생성합니다.
    public GameObject GetDropItemByName(ItemName itemName, Vector3 position, 
        Vector3? size = null, [CanBeNull] Transform parent = null, float? duration = null)
    {
        if (itemDatas.Count == 0) return null;
        var itemObject = DequeueItem(itemName);
        itemObject = SetDropItem(itemName, itemObject, position, size, parent, duration);
        
        return itemObject;
    }

    //아이템 중 하나를 랜덤하게 생성합니다.
    public GameObject GetRandomDropItems(Vector3 position, 
        Vector3? size = null, [CanBeNull] Transform parent = null, float? duration = null)
    {
        if (itemDatas.Count == 0) return null;
        
        int index;
        do index = Random.Range(0, itemDatas.Count);
        while (index == 0);
        
        var itemObject = DequeueItem((ItemName)index);
        itemObject = SetDropItem((ItemName)index, itemObject, position, size, parent, duration);
        
        return itemObject;
    }

    //같은 타입의 아이템 중 하나를 랜덤하게 생성합니다.
    public GameObject GetRandomDropItemByType(ItemType itemType, Vector3 position, 
        Vector3? size = null, [CanBeNull] Transform parent = null, float? duration = null)
    {
        if (itemDatas.Count == 0) return null;

        var filteredItems = 
            (from item in itemDatas 
                where item.Value.itemType == itemType select item.Key).ToList();
        
        int index;
        do index = Random.Range(0, filteredItems.Count);
        while (index == 0);
        
        var itemObject = DequeueItem(filteredItems[index]);
        itemObject = SetDropItem(filteredItems[index], itemObject, position, size, parent, duration);
        
        return itemObject;
    }

    
}
