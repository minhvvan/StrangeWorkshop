using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleEnemy : MonoBehaviour
{
    public float health = 100f;

    public void GetDamage(float damage)
    {
        health -= damage;
        Debug.Log(health);
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
