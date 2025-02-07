using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageHandler
{
    public Bullet bullet{get;set;}
    public void GiveDamage(Enemy targetEnemy, Enemy collidEnemy);
}