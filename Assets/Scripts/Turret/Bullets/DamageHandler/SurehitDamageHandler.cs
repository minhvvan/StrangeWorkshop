using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurehitDamageHandler : IDamageHandler
{
    public Bullet bullet{get;set;}

    public SurehitDamageHandler(Bullet bullet)
    {
        this.bullet = bullet;
    }
    
    public void GiveDamage(Enemy targetEnemy, Enemy collideEnemy)
    {
        if (targetEnemy != null)
        {
            targetEnemy.TakeDamage(bullet.bulletData.damage);
        }
    }
}