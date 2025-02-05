using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "VFXData", menuName = "SO/VFX/VFXData")]
public class VFXDataSO : ScriptableObject
{
    public string vfxName;
    public GameObject vfxPrefab;
    public int poolSize;
    public float duration;
}
