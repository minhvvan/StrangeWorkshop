using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

//방벽 데미지 전달 관련 예시 클래스.
public class BarrierTransfer : MonoBehaviour, IDamageable
{
    private Transform tr;
    private float hp = 3;
    private void Start()
    {
        tr = transform;
        
        //방벽에 추가해줄 것1.
        EnemyPathfinder.instance.barrierPoints.Add(tr);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            //방벽에 추가해줄 것2.
            EnemyPathfinder.instance.barrierPoints.Remove(tr);
            Destroy(gameObject);
        }
    }
}
