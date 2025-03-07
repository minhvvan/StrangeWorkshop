#define TestMode

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum EnemyType
{
    MeleeNormal,
    MeleeHeavy,
    LongRangeNormal,
    LongRangeHeavy,
    Chapter1Boss
}

/// 코드 최상단에 테스트용 코드를 활성화 하는 전처리문이 있습니다. 사용 시에는 주석처리 해주세요.
/// 적 스폰위치를 설정하고 원하는 적 데이터를 불러와 생성할 수 있는 클래스 입니다.

public class EnemySpawner : MonoBehaviour
{
    //Target Object가 값을 전달하도록 만든 생성자.
    public static EnemySpawner Instance;
    
    //해당 웨이브에 생성된 적 카운팅
    public List<GameObject> enemyCountList = new List<GameObject>();
    
    ///스폰 Area프리팹의 위치를 받아오는 List
    public List<SpawnArea> SpawnAreas { get; private set; }
    
    private InGameUIController _inGameUIController;
    
    [NonSerialized] public Action OnWaveClearAction;
    [NonSerialized] public Action<float> OnWaveAlertAction;
    
    private async void Awake()
    {
        //TargetObject가 Transform을 보내기위한 인스턴스 캐싱
        Instance = this;
        
        //씬에 생성된 모든 SpawnArea를 받아
        FindSpawnArea();
        
        await UniTask.WaitUntil(()=>UIManager.Instance.IsInitialized);
        _inGameUIController = UIManager.Instance.GetUI<InGameUIController>(UIType.InGameUI);
        _inGameUIController.RegisterGameUI(this);

        var spawnEventSO = await DataManager.Instance.LoadDataAsync<SpawnEnemyEventSO>(Addresses.Events.Game.ENEMY_SPAWN);
        spawnEventSO.AddListener(SpawnEnemyCallback);
    }

    private void SpawnEnemyCallback(List<EnemySpawnInfo> enemySpawnInfo)
    {
        foreach (EnemySpawnInfo spawnInfo in enemySpawnInfo)
        {
            SpawnEnemy(spawnInfo);
        }
    }

    //생성 전 값 검사.
    private void CheckSpawnArea()
    {
        //스폰 포인트 비어있으면 find
        if (SpawnAreas.IsUnityNull()) 
        {
            Debug.Log($"Cannot Find SpawnPoint");
            FindSpawnArea();
            return;
        }

        //스폰 포인트 중 이상이 있는게 있다면 find
        if (SpawnAreas.Any(area => area.IsUnityNull()))
        {
            SpawnAreas.Clear();
            FindSpawnArea();
        }
    }

    private void FindSpawnArea()
    {
        SpawnAreas = FindObjectsOfType<SpawnArea>().ToList();
        SpawnAreas.Sort((u, d) => u.areaID.CompareTo(d.areaID));
    }

    /// <summary>
    /// 적을 생성합니다.
    /// </summary>
    /// <param name="enemyPrefab">생성할 적</param>
    /// <param name="spawnArea">스폰구역 리스트</param>
    /// <param name="areaID">구역 넘버</param>
    private void CreateEnemy(EnemyDataSO enemyDataSO, List<Transform> spawnArea, int areaID)
    {
        // 랜덤한 각도와 반지름 계산
        float radius = 5f;
        float angle = Random.Range(0f, Mathf.PI * 2); // 0 ~ 360도
        float distance = Random.Range(0f, radius); // 원의 반지름 내에서 랜덤 거리

        Vector3 areaPos = spawnArea[areaID].position;
        Vector3 spawnPosition = new Vector3(
            areaPos.x + Mathf.Cos(angle) * distance,
            areaPos.y,
            areaPos.z + Mathf.Sin(angle) * distance);
            
        var newEnemy = Instantiate(enemyDataSO.enemyPrefab, spawnPosition, Quaternion.identity);
        var enemy = newEnemy.GetComponent<Enemy>();
        enemy.blackboard.enemyStatus = new EnemyStatus(enemyDataSO.enemyStatus);
        enemyCountList.Add(newEnemy);
    }
    
    //단일지점 소환 / 원하는 한 지점에, 한 종류의 몬스터를 원하는 양만큼 생성한다.
    private void SpawnEnemy(EnemySpawnInfo enemySpawnInfo)
    {
        //실행 전 자료 검사.
        CheckSpawnArea();

        List<Transform> areas = new List<Transform>();
        foreach (SpawnArea area in SpawnAreas)
        {
            areas.Add(area.transform);
        }
        
        //입력 받은 수 만큼
        for (int i = 0; i < enemySpawnInfo.count; i++) 
        {
            //적을 생성한다.
            CreateEnemy(enemySpawnInfo.enemyData, areas, enemySpawnInfo.areaID);
        }
    }
}