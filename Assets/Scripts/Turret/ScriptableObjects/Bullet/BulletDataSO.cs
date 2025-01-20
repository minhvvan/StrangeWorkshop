using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletStats", menuName = "SO/Bullet/Stats")]
public class BulletDataSO : ScriptableObject
{
    public TurretType turretType;
    public BulletType bulletType;
    
    // bullet stat
    public float timeToTarget;
    public float turnSpeed;
    public float knockBack;
    public float boomTimer;
}
