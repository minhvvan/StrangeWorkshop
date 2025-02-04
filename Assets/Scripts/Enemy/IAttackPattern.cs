using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAttackPattern
{
    public abstract void RunPattern(Collider other, float damage);
}

public class MeleeAttack : IAttackPattern
{
    public override void RunPattern(Collider other, float damage)
    {
        other.GetComponent<Barrier>().TakeDamage(damage);
    }
}

public class RangeAttack : IAttackPattern
{
    public override void RunPattern(Collider other, float damage)
    {
        other.GetComponent<Barrier>().TakeDamage(damage);
    }
}

public class OtherAttack : IAttackPattern
{
    public override void RunPattern(Collider other, float damage)
    {
        other.GetComponent<Barrier>().TakeDamage(damage);
    }
}

public static class PatternHandler
{
    private static readonly Dictionary<EnemyType, Func<IAttackPattern>> Patterns =
        new Dictionary<EnemyType, Func<IAttackPattern>>
        {
            {(EnemyType)0,() => new MeleeAttack()},
            {(EnemyType)1,() => new OtherAttack()},
            {(EnemyType)2,() => new RangeAttack()},
            {(EnemyType)3,() => new OtherAttack()}
        };
    
    public static IAttackPattern CreatePattern(EnemyType etype)
    {
        if (Patterns.TryGetValue(etype, out Func<IAttackPattern> pattern))
        {
            return pattern();
        }
        else
        {
            Debug.Log("Cannot find pattern");
            return null;
        }
    }
}