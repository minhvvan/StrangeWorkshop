using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//EnemySpawner에게 Target을 전달합니다.
public class TargetObject : MonoBehaviour
{
    private Transform tr;
    private void Start()
    {
        tr = transform;
        EnemySpawner.Instance.AddTarget(tr);
    }
}
