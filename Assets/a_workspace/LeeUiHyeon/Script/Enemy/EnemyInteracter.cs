using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInteracter : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<IDamageable>();
        if (obj != null)
        {
            obj.TakeDamage(1f);
        }
    }
    
}
