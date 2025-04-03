using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KitType
{
    NONE,
    REPAIR
}

[CreateAssetMenu(fileName = "KitInfo", menuName = "SO/Kit/KitInfo")]
public class KitInfoSO : ScriptableObject
{
    [Header("Kit Info"), Tooltip("Kit 종류를 설정합니다.")]
    public KitType kitType;
    
    [Header("Kit Stat"), Tooltip("maxLevel을 설정하고, 레벨별 성장값을 설정해주세요.")]
    public int maxLevel;
    public int[] maxCosts;
    public float[] kitValues;
    
    [Header("Option"), Tooltip("선택적 옵션) 설정하면 cost가 해당 값만큼씩 차감됩니다.")] 
    public int modifyCost;
    
    //에디터에 실시간 반영시켜주는 함수
    private void OnValidate()
    {
        //음수 방지
        maxLevel = Mathf.Max(0, maxLevel);
        ResizeArray();
    }

    //배열 크기 조정
    private void ResizeArray()
    {
        int[] newArray = new int[maxLevel];
        float[] newValues = new float[maxLevel];
        for (int i = 0; i < Mathf.Min(maxCosts.Length, newArray.Length); i++)
        {
            newArray[i] = maxCosts[i]; // 기존 값 유지
            newValues[i] = kitValues[i]; // 기존 값 유지
        }
        maxCosts = newArray;
        kitValues = newValues;
    }
}
