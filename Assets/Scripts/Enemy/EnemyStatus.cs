using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//적의 능력치 조정, 관리하는 클래스.
[Serializable] 
public class EnemyStatus
{
    //타입
    public EnemyType enemytype;

    //애니메이터
    public Animator animator;
    public float animSpeed = 0;

    //적 이름
    public string enemyName;

    //적 능력치
    public float maxHp = 0;
    public float hp = 0;
    public float armor = 0;

    public float attackDamage = 0;
    public float attackRange = 0;
    public float attackSpeed = 0;

    public float moveSpeed = 0;

    public EnemyStatus()
    {
    }

    public EnemyStatus(EnemyStatus status)
    {
        enemytype = status.enemytype;
        animator = status.animator;
        animSpeed = status.animSpeed;
        enemyName = status.enemyName;
        hp = status.hp;
        armor = status.armor;
        attackDamage = status.attackDamage;
        attackRange = status.attackRange;
        attackSpeed = status.attackSpeed;
        moveSpeed = status.moveSpeed;
    }
}
