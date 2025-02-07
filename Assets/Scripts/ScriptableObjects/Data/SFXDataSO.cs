using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SFXData", menuName = "SO/FX/SFXData")]
public class SFXDataSO : ScriptableObject
{
    public SFXType sfxType;
    public AudioClip clip;
    public float volume;
    public bool loop;
}
