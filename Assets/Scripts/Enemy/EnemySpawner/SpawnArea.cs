using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// 적 스폰구역을 담당하는 오브젝트 입니다. 씬에 배치 후, areaID를 지정해주세요.
/// </summary>
/// <param name="areaID">스폰구역 넘버. 에디터에서 필수로 지정해주세요.</param>
public class SpawnArea : MonoBehaviour
{
    [Header("SpawnArea ID"), Tooltip("스폰구역 넘버, 에디터에서 중복되지 않게 지정해주세요.")]
    public int areaID;

    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.green;
        style.fontSize = 16;
        
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, Vector3.one);
        Handles.Label(
            transform.position + new Vector3(0,1f,0), 
            "Area"+areaID.ToString(), style);
    }
}
