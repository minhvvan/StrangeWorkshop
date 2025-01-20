using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Collider Enter를 통한 데미지 전달 확인용 오브젝트.
public class EnemyInteracter : MonoBehaviour
{
    private float damage = 1f;
    void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<IDamageable>();
        if (obj != null)
        {
            obj.TakeDamage(damage);
        }
    }
}
