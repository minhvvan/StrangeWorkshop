using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "SO/Enemy/EnemyData")]
//EnemyData를 저장하는 SO
public class EnemyDataSO : ScriptableObject
{
    //타입
    public EnemyType enemytype;
    
    //사용할 모델프리팹.
    public GameObject enemyPrefab;
    
    //애니메이터
    public float animFirstDelay;

    //적 이름
    public string enemyName;
    
    //적 능력치
    public float hp;
    public float armor;
    
    public float attackDamage;
    public float attackRange;
    public float attackSpeed;

    public float moveSpeed;
}
