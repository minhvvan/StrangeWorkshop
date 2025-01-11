using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard_Turret : MonoBehaviour
{
    [NonSerialized] public float damage;
    [NonSerialized] public float attackRange;
    [NonSerialized] public float fireRate;
    
    [NonSerialized] public int remainingBulletsNum = 50;
    [NonSerialized] public bool isOnCounter = true;
    [NonSerialized] public bool isCrashed = false;
    [NonSerialized] public GameObject target = null;
}
