// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class TurretFactory
// {
//     private string _turretPrefabsPath = "Prefabs/Turrets/";
//     private List<TurretDataSO> _turretDataSOs = new List<TurretDataSO>();
//
//     public TurretFactory(List<TurretDataSO> turretDataSOs)
//     {
//         _turretDataSOs = turretDataSOs;
//     }
//     
//     public Turret CreateTurret(TurretType turretType, Transform spawnPoint)
//     {
//         // turret type에 따른 turret을 spawnpoint에 생성한다.
//         GameObject turretPrefab = Resources.Load(_turretPrefabsPath + $"{turretType}") as GameObject;
//
//         if (turretPrefab == null)
//         {
//             Debug.LogError($"Invalid turret type: {turretType}");
//         }
//         
//         GameObject turretInstance = Object.Instantiate(turretPrefab, spawnPoint.position, spawnPoint.rotation);
//         Turret turret = turretInstance.GetComponent<Turret>();
//         
//         // blackboard 세팅
//         Blackboard_Turret blackboardTurret = turretInstance.GetComponent<Blackboard_Turret>();
//         TurretDataSO turretDataSO = GetTurretData(turretType);
//         if(turretDataSO == null) Debug.LogError($"Invalid turret type: {turretType}");
//         blackboardTurret.Initialize(turretDataSO);
//         
//         // 터렛별 발사 방식 설정
//         switch (turretType)
//         {
//             case TurretType.GUN:
//                 turret.turretActions.SetShootingStrategy(new SingleShootingStrategy(turret));
//                 break;
//             case TurretType.MISSILE:
//                 turret.turretActions.SetShootingStrategy(new SingleShootingStrategy(turret));
//                 break;
//             case TurretType.MORTAR:
//                 turret.turretActions.SetShootingStrategy(new SingleShootingStrategy(turret));
//                 break;
//             default:
//                 break;
//         }
//         return turret;
//     }
//
//     private TurretDataSO GetTurretData(TurretType turretType)
//     {
//         // turrettype에 맞는 SO 검색
//         foreach (TurretDataSO turretDataSO in _turretDataSOs)
//         {
//             if (turretDataSO.turretType == turretType)
//             {
//                 return turretDataSO;
//             }
//         }
//         return null;
//     }
// }
