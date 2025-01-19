using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCustom : MonoBehaviour
{
    public Blackboard_Projectile projectileData;

    public void InitProjectile(Transform target, float damage)
    {
        InitComponent();
        projectileData.target = target;
        projectileData.damage = damage;
        InitBulletData();
    }
    
    private void InitComponent()
    {
        projectileData = gameObject.GetComponent<Blackboard_Projectile>();
    }
    private void InitBulletData()
    {
        if (projectileData.bulletType == BulletType.MORTAR)
        {
            projectileData.trajectory = new Trajectory_Parabola(this, transform.position, projectileData.target.position);
        }
        else if (projectileData.bulletType == BulletType.GUN)
        {
            projectileData.trajectory = new Trajectory_Straight(this, transform.position, projectileData.target.position);
        }
    }
    
    private void Update()
    {
        // 타겟 없으면 폭발
        if (projectileData.target == null)
        {
            Explosion();
            return;
        }

        // 땅에 닿으면 폭발
        if (transform.position.y < -0.2F)
        {
            Explosion();
        }

        // 어떤 이유에 의해 불발되면 일정 시간 뒤 터지도록 설정
        projectileData.boomTimer -= Time.deltaTime;
        if (projectileData.boomTimer < 0)
        {
            Explosion();
        }
        
        projectileData.trajectory.MoveToTarget();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Vector3 dir = other.transform.position - transform.position;
            //Vector3 knockBackPos = other.transform.position * (-dir.normalized * knockBack);
            Vector3 knockBackPos = other.transform.position + (dir.normalized * projectileData.knockBack);
            knockBackPos.y = 1;
            other.transform.position = knockBackPos;
            Explosion();
            GiveDamage(other);
        }
    }

    public void Explosion()
    {
        Instantiate(projectileData.explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    
    private void GiveDamage(Collider other)
    {
        other.GetComponent<SampleEnemy>().GetDamage(projectileData.damage);
    }
}
