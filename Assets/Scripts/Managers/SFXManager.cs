using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;

public class SFXManager : SingletonDontDestroy<SFXManager>
{
    Dictionary<SFXType, Queue<AudioSource>> sfxPools = new Dictionary<SFXType, Queue<AudioSource>>();
    Dictionary<SFXType, SFXDataSO> sfxDataSOs = new Dictionary<SFXType, SFXDataSO>();
    
    public bool IsInitialized { get; private set; }

    private async void Start()
    {
        await Initialize();
        IsInitialized = true;
    }
    
    public async UniTask Initialize()
    {
        await LoadSFXSources("Loader");
    }

    async UniTask LoadSFXSources(string sceneName)
    {
        // SceneName에서 필요한 sfx들을 pooling 해놓는다
        // 해당 Scene에서 필요한 sfx의 종류는 SFXLoadSO scriptableObject에서 관리한다.
        SFXLoadSO sfxLoadSO = null;
        switch (sceneName)
        {
            case "Loader":
                sfxLoadSO = await DataManager.Instance.LoadDataAsync<SFXLoadSO>(Addresses.Data.FX.SFXLOADER);
                break;
            default:
                Debug.LogError($"{sceneName} not found!");
                break;
        }
        foreach (var sfxDataSO in sfxLoadSO.sfxDataSOs)
        {
            Instance.RegisterSFX(sfxDataSO);
        }
    }
    
    private void RegisterSFX(SFXDataSO sfxDataSO)
    {
        if (!sfxPools.ContainsKey(sfxDataSO.sfxType))
        {
            sfxPools[sfxDataSO.sfxType] = new Queue<AudioSource>();
            sfxDataSOs[sfxDataSO.sfxType] = sfxDataSO;
        }
    }

    public AudioSource PlaySFX(SFXType sfxType, Vector3 position)
    {
        if (!sfxPools.ContainsKey(sfxType))
        {
            Debug.LogError($"{sfxType} doesn't exist in SFXPools!");
            return null;
        }

        AudioSource source = null;
        if (sfxPools[sfxType].Count <= 0)
        {
            source = CreateAudioSource(sfxDataSOs[sfxType]);
        }
        else
        {
            source = sfxPools[sfxType].Dequeue();
        }

        source.transform.position = position;
        source.Play();

        if (!source.loop)
        {
            UniTask.Void(async () => await ReturnSFX(sfxType, source, source.clip.length));
        }

        return source;
    }
    
    public AudioSource PlaySFX(SFXType sfxType, Transform parent)
    {
        if (!sfxPools.ContainsKey(sfxType))
        {
            Debug.LogError($"{sfxType} doesn't exist in SFXPools!");
            return null;
        }

        AudioSource source = null;
        if (sfxPools[sfxType].Count <= 0)
        {
            source = CreateAudioSource(sfxDataSOs[sfxType]);
        }
        else
        {
            source = sfxPools[sfxType].Dequeue();
        }
        
        source.transform.position = parent.position;
        source.transform.SetParent(parent);
        source.Play();

        if (!source.loop)
        {
            UniTask.Void(async () => await ReturnSFX(sfxType, source, source.clip.length));
        }

        return source;
    }
    
    private AudioSource CreateAudioSource(SFXDataSO sfxData)
    {
        GameObject obj = new GameObject($"SFX_{sfxData.sfxType}");
        obj.transform.SetParent(transform);
        AudioSource audioSource = obj.AddComponent<AudioSource>();

        audioSource.clip = sfxData.clip;
        audioSource.volume = sfxData.volume;
        audioSource.loop = sfxData.loop;
        audioSource.playOnAwake = false;

        return audioSource;
    }
    
    public void ReturnSFX(SFXType sfxType, AudioSource audioSource)
    {
        audioSource.Stop();
        audioSource.transform.SetParent(Instance.transform);
        sfxPools[sfxType].Enqueue(audioSource);
    }

    private async UniTask ReturnSFX(SFXType sfxType, AudioSource audioSource, float length)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(length));
        audioSource.Stop();
        audioSource.transform.SetParent(Instance.transform);
        sfxPools[sfxType].Enqueue(audioSource);
    }
}

