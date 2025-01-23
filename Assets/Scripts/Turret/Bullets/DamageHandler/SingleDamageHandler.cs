using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleDamageHandler : IDamageHandler
{
    public Bullet bullet{get;set;}

    public SingleDamageHandler(Bullet bullet)
    {
        this.bullet = bullet;
    }
    

    public void GiveDamage(Enemy enemy)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(bullet.bulletData.damage);
        }
    }
}