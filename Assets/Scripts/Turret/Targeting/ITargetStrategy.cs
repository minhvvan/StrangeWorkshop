using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetStrategy
{
    GameObject SelectTarget(Collider[] targets, GameObject turret);
}
