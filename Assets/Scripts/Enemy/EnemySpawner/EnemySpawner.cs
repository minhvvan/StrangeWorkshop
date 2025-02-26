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

public enum EnemyType
{
    MeleeNormal,
    MeleeHeavy,
    LongRangeNormal,
    LongRangeHeavy,
}

/// 코드 최상단에 테스트용 코드를 활성화 하는 전처리문이 있습니다. 사용 시에는 주석처리 해주세요.
/// 적 스폰위치를 설정하고 원하는 적 데이터를 불러와 생성할 수 있는 클래스 입니다.

public class EnemySpawner : MonoBehaviour
{
    //Target Object가 값을 전달하도록 만든 생성자.
    public static EnemySpawner Instance;
    
    //생성할 적의 프리팹
    private GameObject _enemyPrefab; 
    
    //해당 웨이브에 생성된 적 카운팅
    public List<GameObject> enemyCountList = new List<GameObject>();
    
    //풀링용 프리팹 List -- 미구현 --
    //private List<GameObject> _enemyPrefabPool;
    
    //생성할 적의 정보, 스탯
    private EnemyStatus _status;
    
    //생성된 적에게 Info를 주입하기 위한 인스턴스.
    private Enemy _enemy;
    
    ///스폰 Area프리팹의 위치를 받아오는 List
    public List<SpawnArea> SpawnAreas { get; private set; }
    
    //생성될 위치 List의 index
    private int _areaID = 0; // 추후 조정이 필요. 임시로 0번에서 소환.
    //EnemySpawnInfo에서 지정할 수 있으면 즉시 반영대기.
    
    //이 수만큼 생성 하겠다.
    private int _spawnAmount; // WavaDataSO -> count로 받을 것.

    [SerializeField] private WaveDataSO testWave;
    //각 SO 생성파일 경로를 지정해주세요.
    [NonSerialized] public string enemyDataSOpath = Addresses.Data.Enemy.BASIC;
    
    private bool _waveEnd;
    private bool _isSequence;
    private int _waveNumber; //웨이브 넘버
    private float _duration; //웨이브 지속시간 -> 남은시간 보여주는 용도
    private float _spawnInterval; //재소환 간격 
    private float _preparationTime;//웨이브 간 대기시간 추후 챕터읽어올때 반영
    
    private List<EnemySpawnInfo> _spawnInfos = new(); //스폰할 적 정보, 소환할 양.
    private List<WaveDataSO> _waveDatas = new(); //챕터에게서 받을 웨이브 데이터 리스트
    // private WaveUIController _waveUIController;
    
    [SerializeField] private ChapterDataSO _chapterData;

    private CancellationTokenSource _cts;//Delay강제종료

    private InGameUIController _inGameUIController;
    
    [Header("Events")]
    public Action OnWaveClearAction;
    public Action<float> OnWaveAlertAction;
    
    private async void Awake()
    {
        //TargetObject가 Transform을 보내기위한 인스턴스 캐싱
        Instance = this;
        
        //씬에 생성된 모든 SpawnArea를 받아
        SpawnAreas = FindObjectsOfType<SpawnArea>().ToList();
        SpawnAreas.Sort((u, d) => u.areaID.CompareTo(d.areaID));
        
        var chapterDatas = await DataManager.Instance.LoadDataAsync<ChapterListSO>
            (Addresses.Data.Chapter.CHAPTER_LIST);
        
        _chapterData = await EnemyFactory.LoadChapter(chapterDatas);
        
        //WaveUIController 생성
        // await UniTask.WaitUntil(() => _waveUIController = 
        //     UIManager.Instance.GetUI<WaveUIController>(UIType.WaveUI));
        await UniTask.WaitUntil(()=>UIManager.Instance.IsInitialized);
        _inGameUIController = UIManager.Instance.GetUI<InGameUIController>(UIType.InGameUI);
        _inGameUIController.RegisterGameUI(this);
    }

    private async void Start()
    {
        await UniTask.WaitUntil(() => _chapterData != null);
        // await UniTask.WaitUntil(() => _waveUIController != null);
        ChapterSequence(_chapterData).Forget();
    }

    private void Update()
    {
#if TestMode
#endif
    }
    
    private async UniTask ChapterSequence(ChapterDataSO _chapterData)
    {
        await SetWaveList(_chapterData.waves);
        // await SetPreparationTime(_chapterData.preparationTime);
        foreach (var waveData in _waveDatas)
        {
            await StartUpWave();
            await RunWaveSequence(waveData);
            await WaveClearAlarm();
        }
    }

    private async UniTask SetWaveList(List<WaveDataSO> waveData)
    {
        if(waveData != null) _waveDatas.Clear();
        _waveDatas.AddRange(waveData);
        await UniTask.CompletedTask;
    }

    private async UniTask SetPreparationTime(float time)
    {
        _preparationTime = time;
        // _waveUIController.SetPreparationTime(_preparationTime);
        await UniTask.CompletedTask;
    }

    private async UniTask StartUpWave()
    {
        OnWaveAlertAction?.Invoke(_chapterData.preparationTime);
        // await UniTask.WaitUntil(() => _waveUIController != null);
        // _waveUIController.SetWaveUIController(WaveUIController.instance.OnWaveAlertPopup);
        await UniTask.Delay(TimeSpan.FromSeconds(_preparationTime));
    }

    /// <summary>
    /// WaveDataSO를 받으면 해당 웨이브 시퀀스를 수행하는 함수
    /// </summary>
    /// <param name="waveData">수행할 WaveDataSO를 넣어주세요</param>
    private async UniTask RunWaveSequence(WaveDataSO waveData)
    {
        _cts = new CancellationTokenSource();
        _waveEnd = false;
        
        await GetWaveData(waveData);
        
        //웨이브 지속시간 끝나면 소환 종료.
        WaveTimer(true).Forget();
        while (!_waveEnd)
        {
            //들어있는 소환정보에 따라 적 소환.
            foreach (EnemySpawnInfo spawnInfo in _spawnInfos)
            {
                await GetEnemyData(spawnInfo);
                SpawnEnemy(_areaID);
            }

            //스폰 간격 대기
            await UniTask.Delay(TimeSpan.FromSeconds(_spawnInterval),
                cancellationToken: _cts.Token);
        }

        //남은 적이 다 죽을때 까지 기다렸다가 알림
        await UniTask.WaitUntil(() => enemyCountList.Count == 0);
    }
    
    /// <summary>
    /// duration(지속시간)동안 기다렸다가 End를 전달한다.
    /// </summary>
    /// <param name="waveEnd"></param>
    private async UniTask WaveTimer(bool waveEnd)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_duration));
        _waveEnd = waveEnd;
    }

    private async UniTask WaveClearAlarm()
    {
        OnWaveClearAction?.Invoke();
        // _waveUIController.SetWaveUIController(WaveUIController.instance.OnWaveClearPopup);
        await UniTask.WaitForSeconds(3f);
    }
    
    private async UniTask GetWaveData(WaveDataSO waveData)
    {
        List<EnemySpawnInfo> spawnInfos = new();
        (_waveNumber, _duration, spawnInfos, _spawnInterval) = await EnemyFactory.LoadWaveData(waveData);
        _spawnInfos.Clear();
        _spawnInfos.AddRange(spawnInfos);
    }
    
    ///스폰할 적의 정보를 읽어오는 기능, 불러올 적 SO의 이름을 넣고 사용합니다.
    private async UniTask GetEnemyData(EnemySpawnInfo enemySpawns)
    {
        //EnemyDataSO내의 스탯, 생성할 프리팹 정보를 받아옵니다.
        (_status, _enemyPrefab, _spawnAmount, _areaID) = await EnemyFactory.LoadEnemyStatus(enemySpawns);
    }
    
    //생성 전 값 검사.
    private void CheckValue()
    {
        //스폰 포인트 비어있으면 실행 안함.
        if (SpawnAreas == null) 
        {
            Debug.Log($"Cannot Find SpawnPoint");
            return;
        }
        
        //프리팹 안들어 있으면 중지.
        if (_enemyPrefab == null)
        {
            Debug.Log($"Cannot Find EnemyPrefab");
            return;
        }
    }

    /// <summary>
    /// 적을 생성합니다.
    /// </summary>
    /// <param name="enemyPrefab">생성할 적</param>
    /// <param name="spawnArea">스폰구역 리스트</param>
    /// <param name="areaID">구역 넘버</param>
    private void CreateEnemy(GameObject enemyPrefab, List<Transform> spawnArea, int areaID)
    {
        var newEnemy = 
            Instantiate(enemyPrefab, spawnArea[areaID].position, Quaternion.identity);
        var enemy = newEnemy.GetComponent<Enemy>();
        enemy.blackboard.enemyStatus = new EnemyStatus(_status);
        enemyCountList.Add(newEnemy);
    }
    
    //단일지점 소환 / 원하는 한 지점에, 한 종류의 몬스터를 원하는 양만큼 생성한다.
    private void SpawnEnemy(int areaID)
    {
        //실행 전 자료 검사.
        CheckValue();

        List<Transform> areas = new List<Transform>();
        foreach (SpawnArea area in SpawnAreas)
        {
            areas.Add(area.transform);
        }
        
        //입력 받은 수 만큼
        for (int a = 0; a < _spawnAmount; a++) 
        {
            //적을 생성한다.
            CreateEnemy(_enemyPrefab,areas,areaID);
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

작업로그0206
EnemySpawner를 Wave데이터를 받으면 Wave시퀀스를 수행하는 기능으로 변경.
SpawnDataSO가 더이상 필요해지지 않아 제거함.
 */
