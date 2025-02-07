using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    [NonSerialized] public Blackboard_Bullet bulletData;
    
    private Enemy _targetEnemy;

    public void InitBullet(Transform target, float damage)
    {
        // 여기서 target은 총알이 발사된 시점에 적이 있던 위치다
        // 적이 이동하면 target에 적이 없을수도 있다
        bulletData = gameObject.GetComponent<Blackboard_Bullet>();
        bulletData.Initialize(target, damage);
        _targetEnemy = target.gameObject.GetComponent<Enemy>();
    }
    
    private void Update()
    {
        // 타겟 없으면 폭발
        if (bulletData.target == null)
        {
            Explosion(null, null);
            return;
        }

        // 땅에 닿으면 폭발
        if (transform.position.y < -0.2F)
        {
            Explosion(_targetEnemy, null);
        }

        // 어떤 이유에 의해 불발되면 일정 시간 뒤 터지도록 설정
        bulletData.boomTimer -= Time.deltaTime;
        if (bulletData.boomTimer < 0)
        {
            Explosion(_targetEnemy, null);
        }
        
        bulletData.trajectory.MoveToTarget();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Explosion(_targetEnemy, other.gameObject.GetComponent<Enemy>());
        }
    }

    public void Explosion(Enemy targetEnemy, Enemy collideEnemy)
    {
        // // 넉백 시스템
        // Vector3 dir = other.transform.position - transform.position;
        // //Vector3 knockBackPos = other.transform.position * (-dir.normalized * knockBack);
        // Vector3 knockBackPos = other.transform.position + (dir.normalized * bulletData.knockBack);
        // knockBackPos.y = 1;
        // other.transform.position = knockBackPos;
        
        bulletData.damageHandler.GiveDamage(targetEnemy, collideEnemy);

        switch (bulletData.bulletType)
        {
            case BulletType.GUN:
                VFXManager.Instance.TriggerVFX(VFXType.GUNBULLETEXPLOSION, transform.position, transform.rotation);
                break;
            case BulletType.MISSILE:
                VFXManager.Instance.TriggerVFX(VFXType.MISSILEBULLETEXPLOSION, transform.position, transform.rotation);
                break;
            case BulletType.MORTAR:
                VFXManager.Instance.TriggerVFX(VFXType.MORTARBULLETEXPLOSION, transform.position, transform.rotation);
                break;
        }
        Destroy(gameObject);
    }
}
