using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "BGMData", menuName = "SO/Sound/BGM")]
public class BGMDataSO : ScriptableObject
{
    public SerializedDictionary<string, AudioClip> BGMs;

    public bool TryGetAudioClip(string sceneName, out AudioClip audioClip)
    {
        return BGMs.TryGetValue(sceneName, out audioClip);
    }
}