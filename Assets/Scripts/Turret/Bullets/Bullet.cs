using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour
{
    [NonSerialized] public Blackboard_Bullet bulletData;

    public void InitBullet(Transform target, float damage)
    {
        // 여기서 target은 총알이 발사된 시점에 적이 있던 위치다
        // 적이 이동하면 target에 적이 없을수도 있다
        bulletData = gameObject.GetComponent<Blackboard_Bullet>();
        bulletData.Initialize(target, damage);
    }
    
    private void Update()
    {
        // 타겟 없으면 폭발
        if (bulletData.target == null)
        {
            Explosion();
            return;
        }

        // 땅에 닿으면 폭발
        if (transform.position.y < -0.2F)
        {
            bulletData.damageHandler.GiveDamage(null);
            Explosion();
        }

        // 어떤 이유에 의해 불발되면 일정 시간 뒤 터지도록 설정
        bulletData.boomTimer -= Time.deltaTime;
        if (bulletData.boomTimer < 0)
        {
            Explosion();
        }
        
        bulletData.trajectory.MoveToTarget();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // 넉백 시스템
            Vector3 dir = other.transform.position - transform.position;
            //Vector3 knockBackPos = other.transform.position * (-dir.normalized * knockBack);
            Vector3 knockBackPos = other.transform.position + (dir.normalized * bulletData.knockBack);
            knockBackPos.y = 1;
            other.transform.position = knockBackPos;
            bulletData.damageHandler.GiveDamage(other.gameObject.GetComponent<Enemy>());
            Explosion();
        }
    }

    public void Explosion()
    {
        VFXManager.Instance.TriggerVFX("Explosion", transform.position, transform.rotation);
        // Instantiate(bulletData.explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    
    private void GiveDamage(Collider other)
    {
        other.GetComponent<Enemy>().TakeDamage(bulletData.damage);
    }
}
