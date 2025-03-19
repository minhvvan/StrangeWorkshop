using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "SO/Enemy/EnemyData")]
//EnemyData를 저장하는 SO
public class EnemyDataSO : ScriptableObject
{
    //사용할 모델프리팹.
    public GameObject enemyPrefab;
    
    //적 능력치
    public EnemyStatus enemyStatus;
}
