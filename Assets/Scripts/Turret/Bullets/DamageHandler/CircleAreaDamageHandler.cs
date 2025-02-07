using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAreaDamageHandler : IDamageHandler
{
    public Bullet bullet { get; set; }

    public CircleAreaDamageHandler(Bullet bullet)
    {
        this.bullet = bullet;
    }
    
    public void GiveDamage(Enemy targetEnemy, Enemy collideEnemy)
    {
        Transform target = bullet.transform;
        int layerMask = LayerMask.GetMask("Enemy");
        Collider[] enemies = Physics.OverlapSphere(target.position, bullet.bulletData.damageArea, layerMask);
        foreach (Collider c in enemies)
        {
            c.GetComponent<Enemy>().TakeDamage(bullet.bulletData.damage);
        }
    }
}
