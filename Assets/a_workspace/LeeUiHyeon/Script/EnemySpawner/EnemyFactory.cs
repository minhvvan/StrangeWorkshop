using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;


//각 SO를 읽어 데이터를 할당하는 클래스
public static class EnemyFactory
{
    //EnemyDataSO를 읽어옵니다.
    public static async UniTask<(EnemyStatus, GameObject)> LoadEnemyStatus(string path)
    {
        EnemyStatus stats = new();
        EnemyDataSO enemyDatasSO = new();
        
        var handle = Addressables.LoadAssetAsync<EnemyDataSO>(path);
        await handle.Task;
        
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            enemyDatasSO = (handle.Result);
        }
        else
        {
            Debug.LogError($"Failed to load enemy data");
        }

        //읽어온 EnemyData SO 데이터를 EnemyStatus에 할당. 
        GameObject objPrefab;
        objPrefab = enemyDatasSO.enemyPrefab;
        
        stats.enemytype = enemyDatasSO.enemytype;
        stats.animator = enemyDatasSO.animator;
        stats.animSpeed = enemyDatasSO.animSpeed;
        stats.name = enemyDatasSO.name;
        stats.hp = enemyDatasSO.hp;
        stats.armor = enemyDatasSO.armor;
        stats.attackDamage = enemyDatasSO.attackDamage;
        stats.attackRange = enemyDatasSO.attackRange;
        stats.attackSpeed = enemyDatasSO.attackSpeed;
        stats.moveSpeed = enemyDatasSO.moveSpeed;
        
        return (stats, objPrefab);
    }

    //SpawnDataSO를 읽어옵니다.
    public static async UniTask<(List<Vector3>,int,int,int,TargetCode)> LoadSpawnData(string path)
    {
        SpawnDataSO spawnDatasSO = new();
        
        //Addressables를 사용합니다. SpawnDataSO파일에 세팅 해주세요. *그룹 별도 설정 안해둔 상태*
        var handle = Addressables.LoadAssetAsync<SpawnDataSO>(path);
        await handle.Task; ;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            spawnDatasSO = (handle.Result);
        }
        else
        {
            Debug.LogError($"Failed to load spawn data");
        }
        return (spawnDatasSO.spawnPoints,
                spawnDatasSO.spawnIndex,
                spawnDatasSO.selectPrefabIndex,
                spawnDatasSO.spawnAmount,
                spawnDatasSO.targetCode);
    }
}