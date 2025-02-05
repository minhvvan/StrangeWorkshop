using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

public class VFXManager : SingletonDontDestroy<VFXManager>
{
    Dictionary<string, Queue<GameObject>> vfxPools = new Dictionary<string, Queue<GameObject>>();
    Dictionary<string, VFXDataSO> vfxDataSOs = new Dictionary<string, VFXDataSO>();

    void Awake()
    {
        LoadVFXObjects("TestScene");
    }

    async void LoadVFXObjects(string sceneName)
    {
        // SceneName에서 필요한 vfx들을 pooling 해놓는다
        // 해당 Scene에서 필요한 vfx의 종류는 VFXLoadSO scriptableObject에서 관리한다.
        VFXLoadSO vfxLoadSO = null;
        switch (sceneName)
        {
            case "TestScene":
                vfxLoadSO = await DataManager.Instance.LoadDataAsync<VFXLoadSO>(Addresses.Data.VFX.TESTSCENE);
                break;
            default:
                Debug.LogError($"{sceneName} not found!");
                break;
        }
        foreach (var vfxDataSO in vfxLoadSO.vfxDataSOs)
        {
            Instance.RegisterVFX(vfxDataSO);
        }
    }
    
    private void RegisterVFX(VFXDataSO vfxDataSO)
    {
        if (!vfxPools.ContainsKey(vfxDataSO.vfxName))
        {
            vfxPools[vfxDataSO.vfxName] = new Queue<GameObject>();
            vfxDataSOs[vfxDataSO.vfxName] = vfxDataSO;

            for (int i = 0; i < vfxDataSO.poolSize; i++)
            {
                GameObject vfxObject = Instantiate(vfxDataSO.vfxPrefab, transform);
                vfxObject.SetActive(false);
                vfxPools[vfxDataSO.vfxName].Enqueue(vfxObject);
            }
        }
    }

    public GameObject GetVFX(string vfxName, Vector3 position, Quaternion rotation)
    {
        GameObject vfxObject = DequeueVFX(vfxName);
        
        vfxObject.transform.SetPositionAndRotation(position, rotation);
        vfxObject.SetActive(true);

        ReturnVFX(vfxName, vfxObject, vfxDataSOs[vfxName].duration);
        return vfxObject;
    }
    
    public GameObject GetVFX(string vfxName, Transform parent)
    {
        GameObject vfxObject = DequeueVFX(vfxName);
        
        vfxObject.transform.SetPositionAndRotation(parent.position, parent.rotation);
        vfxObject.transform.SetParent(parent);
        vfxObject.SetActive(true);

        ReturnVFX(vfxName, vfxObject, vfxDataSOs[vfxName].duration);
        return vfxObject;
    }

    private GameObject DequeueVFX(string vfxName)
    {
        if (!vfxPools.ContainsKey(vfxName))
        {
            Debug.LogError($"{vfxName} doesn't exist in VFXPools!");
            return null;
        }
        
        if (vfxPools[vfxName].Count <= 0)
        {
            return Instantiate(vfxDataSOs[vfxName].vfxPrefab);
        }
        else
        {
            return vfxPools[vfxName].Dequeue();
        }
    }

    private async UniTask ReturnVFX(string vfxName, GameObject vfxObject, float duration)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        vfxObject.transform.SetParent(Instance.transform);
        vfxObject.SetActive(false);
        vfxPools[vfxName].Enqueue(vfxObject);
    }
}