using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewSpawnData", menuName = "SO/SpawnData")]

//적 스폰에 관한 데이터를 저장하는 SO
public class SpawnDataSO : ScriptableObject
{
    //생설될 위치 리스트.
    public List<Vector3> spawnPoints;
    
    //생성될 Point의 index
    public int spawnIndex;
    
    //생성할 적 Prefab List의 index, 캐싱된 Prefab종류 중 골라서 생성할때 쓴다.
    public int selectPrefabIndex;
    
    //이 수만큼 생성 하겠다.
    public int spawnAmount;
    
    //생성 후 따라갈 타켓 번호.
    public TargetCode targetCode;
}


/* EnemyID 와 EnemyPrefab GameObject에 관한 고민
 SpawnDataSO 에서 EnemyID까지 지정을 해준다면
 List<GameObject>를 통헤 할당해준 Prefab데이터에서 골라 생성하는게 가능해지는데.
 이 List<GameObject>를 EnemyDataSO에서 관리할지 SpawnDataSO에서 관리할지 고민.
 */