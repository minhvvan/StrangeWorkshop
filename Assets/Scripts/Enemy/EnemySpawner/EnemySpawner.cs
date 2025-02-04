#define TestMode

using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public enum EnemyType
{
    MeleeNormal,
    MeleeHeavy,
    LongRangeNormal,
    LongRangeHeavy,
}

public enum TargetCode
{
    A,
    B,
    C,
    D,
    E,
}

//코드 최상단에 테스트용 코드를 활성화 하는 전처리문이 있습니다. 사용 시에는 주석처리 해주세요.

//스크립트 하단에 기본 사용순서 있습니다.

//적 스폰위치를 설정하고 원하는 적 데이터를 불러와 생성할 수 있는 클래스 입니다.
public class EnemySpawner : MonoBehaviour
{
    //Target Object가 값을 전달하도록 만든 생성자.
    public static EnemySpawner Instance;
    
    //생성할 적의 프리팹 List
    private List<GameObject> _enemyPrefab = new();
    
    //풀링용 프리팹 List -- 미구현 --
    private List<GameObject> _enemyPrefabPool;
    
    //생성할 적의 정보, 스탯
    private EnemyStatus _status;
    
    //생성 후 따라갈 타겟. List로 받은 후 TargetCode로 지정하여 전달한다.
    private List<Transform> _target = new();
    
    //생성된 적에게 Info를 주입하기 위한 인스턴스.
    private Enemy _enemy;
    
    //스폰 포인트 List, index로 선택하게 한다.
    private List<Vector3> _spawnPoints =  new();
    
    //스폰 포인트 활성/비활성화 List
    private List<bool> _spawnCheck = new();
    
    //생성될 위치 List의 index
    private int _spawnIndex;

    //생성할 적 Prefab List의 index, 캐싱된 Prefab종류 중 골라서 생성할때 쓴다.
    private int _selectPrefabIndex;
    
    //이 수만큼 생성 하겠다.
    private int _spawnAmount;
    
    //생성 후 따라갈 타켓 넘버.
    private TargetCode _targetCode;
    
    //각 SO 생성파일 경로를 지정해주세요.
    [NonSerialized] public string enemyDataSOpath = Addresses.Data.Enemy.BASIC;
    [NonSerialized] public string spawnDataSOpath = "Assets/Data/Enemy/SpawnDataA.asset";
    
    //적 생성 사전작업
    async UniTask SetUp() 
    {
        await GetEnemyData(enemyDataSOpath);
        await GetSpawnData(spawnDataSOpath);
        ResetActivateSpawnPoints(true);  
    }

    void Awake()
    {
        //TargetObject가 Transform을 보내기위한 인스턴스 캐싱
        Instance = this;
    }
    
    //TEST
    async void Start()
    {
#if TestMode
        //적 생성 사전시퀀스.
        _ = SetUp();
#endif

    }

    private void Update()
    {
#if TestMode
        //다방면 일괄 소환
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnEnemyPointPick(_selectPrefabIndex);
        }
        
        //단일지점 소환
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnEnemyRollout(_selectPrefabIndex);
        }
#endif
    }
    /* 기본 사용 방법.
     생성할 적 데이터 불러오기.
     #1 GetEnemyData(enemyDataSOpath)로 EnemyDataSO 불러오기.
     #2 GetSpawnData(spawnDataSOpath)로 SpawnDataSO 불러오기.
        *이때, _spawnPoint의 값이 모두 제거된 후 입력된다.*
     #3 ResetActivateSpawnPoints(bool); 새로 불러온 spawnPoint List의 활성화/비활성화 여부를 일괄 초기화.

     #4 적 생성하기 - 테스트용 키 : 1, 2
        - 1 SpawnEnemyPointPick(_selectPrefabIndex); 단일지점에 생성.
        - 2 SpawnEnemyRollout(_selectPrefabIndex); 다방면 일괄 생성.
     */
    
    //적 스폰위치를 하나씩 추가합니다.
    public void AddSpawnPosition(Vector3 position)
    {
        _spawnPoints.Add(position);
    }
    
    //적 스폰위치를 한번에 추가합니다.
    public void AddAllSpawnPositions(List<Vector3> positions)
    {
        _spawnPoints.AddRange(positions);
    }

    //기존 적 스폰위치를 깔끔히 지우고 일괄 추가합니다.
    public void BatchAddSpawnPositions(List<Vector3> positions)
    {
        _spawnPoints.Clear();
        _spawnPoints.AddRange(positions);
    }
    
    //적 스폰위치를 일괄적으로 삭제합니다.
    public void ClearSpawnPositions()
    {
        _spawnPoints.Clear();
    }
    
    //적 스폰 위치 사용여부를 True또는 False로 (선택하여) 모두 초기화합니다.
    public void ResetActivateSpawnPoints(bool setBool)
    {
        if (_spawnCheck != null)
        {
            _spawnCheck.Clear();
            
            for (int i = 0; i < _spawnPoints.Count; i++)
            {
                _spawnCheck.Add(setBool);
            }
        }
    }
    
    //스폰 포인트를 개별적으로 활성/비활성화 하고 싶을 때 사용합니다.
    public void ChangeActivateSpawnPoint(int index, bool setBool)
    {
        if (index <= _spawnCheck.Count - 1)
        {
            _spawnCheck[index] = setBool;
        }
        else
        {
            Debug.Log($"This index is out of range. -> spawnCheck's index");
        }
    }
    
    //<비동기>스폰할 적의 정보를 읽어오는 기능, 불러올 적 SO의 이름을 넣고 사용합니다.
    public async UniTask GetEnemyData(string loadPath)
    {
        
        //EnemyDataSO내의 스탯, 생성할 프리팹 정보를 받아옵니다.
        GameObject prefab;
        (_status, prefab) = await EnemyFactory.LoadEnemyStatus(loadPath);
        
        //적 SO 종류가 여러개일 경우 프리팹을 리스트로 받아 둔 후 꺼내쓰기위한 Add
        _enemyPrefab.Add(prefab);
    }
    
    //Prefab List를 모두 삭제합니다.
    public void ClearEnemyPrefab()
    {
        _enemyPrefab.Clear();
    }

    //<비동기>스폰할 위치, 적의 종류, 생성할 양을 읽어오는 기능. 스폰데이터 SO의 이름을 넣고 사용합니다.
    public async UniTask GetSpawnData(string loadPath)
    {
        
        //SpawnData의 각 데이터를 튜플로 받아온다.
        List<Vector3> spawnPoints = new();
        
        (spawnPoints, _spawnIndex, _selectPrefabIndex, _spawnAmount, _targetCode)
            = await EnemyFactory.LoadSpawnData(loadPath);
        
        //기존 spawnPoint를 싹 지우고 새 데이터로 채워넣는다.
        BatchAddSpawnPositions(spawnPoints);
        
        //그냥 이어서 추가한다.
        //AddAllSpawnPositions(spawnPoints);
        //AddSpawnPosition(spawnPoints);
    }

    //Target 트랜스폼을 List에 추가합니다.
    public void AddTarget(Transform target)
    {
        _target.Add(target);
    }

    //Target 트랜스폼 List를 지웁니다.
    public void ClearTarget()
    {
        _target.Clear();
    }
    
    //생성 전 값 검사.
    public void CheckValue()
    {
        //스폰 포인트 비어있으면 실행 안함.
        if (_spawnPoints == null) 
        {
            Debug.Log($"Cannot Find SpawnPoint");
            return;
        }
        
        //각 포인트 활성/비활성화 체크리스트 비어있으면 실행 안함.
        if (_spawnCheck == null) 
        {
            Debug.Log($"Cannot Find SpawnCheck");
            return;
        }
        
        //프리팹 안들어 있으면 중지.
        if (_enemyPrefab == null)
        {
            Debug.Log($"Cannot Find EnemyPrefab");
            return;
        }
        
        //타겟 안들어 있으면 중지.
        if (_target == null)
        {
            Debug.Log($"Cannot Find Targets.");
            return;
        }
    }
    
    //값을 받아 적을 생성합니다. **인자를 별도로 검사하는 구문이 없습니다. 단독으로 사용하는걸 권장하지 않습니다.**
    public void CreateEnemy(int prefabIndex, List<Vector3> spawnPoints, int spawnIndex, Transform targets)
    {
        var newEnemy = 
            Instantiate(_enemyPrefab[prefabIndex], spawnPoints[spawnIndex], Quaternion.identity);
        var enemy = newEnemy.GetComponent<Enemy>();
        enemy.blackboard.enemyStatus = new EnemyStatus(_status);
        enemy.blackboard.SetTarget(targets);
    }
    
    //단일지점 소환 / 원하는 한 지점에, 한 종류의 몬스터를 원하는 양만큼 생성한다.
    public void SpawnEnemyPointPick(int prefabIndex)
    {
        //실행 전 자료 검사.
        CheckValue();
        
        //생성 후 최초 추적할 타겟 지정
        Transform targets = _target[(int)_targetCode];

        
        //입력 받은 수 만큼
        for (int a = 0; a < _spawnAmount; a++) 
        {
            //적을 생성한다.
            CreateEnemy(prefabIndex,_spawnPoints,_spawnIndex,targets);
        }
    }
    
    //다방면 일괄 소환 / 단일 개체를 여기저기에 일제히 물량으로 뽑고싶을 때 씀.
    //모든 위치, 생성여부등을 정한 후 특정 프리팹만 대량 생산 할때 사용하는 생성 함수.
    public void SpawnEnemyRollout(int prefabIndex)
    {
        //실행 전 자료 검사.
        CheckValue();
        
        //활성화 된 스폰 포인트만 받기 위한 리스트
        List<Vector3> spawnPos = new();
        
        //생성 후 최초 추적할 타겟 지정
        Transform targets = _target[(int)_targetCode];
        

        //_spawnCheck index세는 용도
        int bind = 0;
        
        foreach (var spawnPoints in _spawnPoints)
        {
            //이 스폰포인트는 true인지 확인
            if (_spawnCheck[bind])
            {
                //spawnPos에 스폰지점 추가.
                spawnPos.Add(spawnPoints);
            }
            bind++;
        }
        
        
        //할당된 스폰 포인트 횟수만큼 반복.
        for (int i = 0; i < _spawnPoints.Count; i++)
        {
            //입력 받은 수 만큼
            for (int a = 0; a < _spawnAmount; a++)
            {
                //적을 생성한다.
                CreateEnemy(prefabIndex, spawnPos, i, targets);
            }
        }
        
    }
}
/*
작업로그0116
EnemySpawner사용 시 주의사항)
반드시 GetSpawnEnemyStatus()가 완전히 완료된 후에
SpawnEnemy()를 실행해야함.
GetSpawnEnemyStatus가 비동기로 진행되기 때문에
그냥 주르륵 이어서 시키면 null을 받아와버림
->동기함수에서 비동기를 같이 실행해서 생긴 문제

spawnPoint List로 관리. 생성지점은 List의 index로 가능.

작업로그0117
현재 SpawnEnemy는 사전에 모든값을 미리 설정 후 일괄 생성하는 방식임.
objectPool 적용 시에는 로직이 바뀌거나 Pooling으로 생성하는 함수를 하나 더 만드는 식으로 바꿀 예정.

작업로그0118
스크립트 구조 개편, 주석 추가.

작업로그0121
주석 하나 수정.

[경고 표시 제거 작업]
ChangeActivateSpawnPoint의 null체크 필요없어서 제거. (bool은 항상 true or false 값 가진다고 함.
 */
