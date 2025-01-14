using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "SO/Player/Stats")]
public class PlayerDataSO : ScriptableObject
{
    //플레이어 관련 데이터
    public float moveSpeed;
    public float interactionRange;
}