using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class KnightSword : MonoBehaviour
{
    public enum ThrowType
    {
        RangeAttack,
        BossPattern
    }
    
    public ThrowType throwType;
    
    public void OnTriggerEnter(Collider other)
    {
        //if (other.name == "Player")
        if (throwType == ThrowType.BossPattern)
        {
            if (other.name == "chest_low")
            {
                //var player = GetComponentInParent<SampleCharacterController>();
                var player = FindObjectOfType<SampleCharacterController>();
            
                //임시 강제제어. 
                float originSpeed = 15f;
                player.walkSpeed = originSpeed * 0.6f; //40% 둔화
                DOVirtual.DelayedCall(2f, () => player.walkSpeed = originSpeed);
            }
        }
        else if (throwType == ThrowType.RangeAttack)
        {
            Destroy(gameObject);
        }
    }

    public void OnAction(Action action)
    {
        action?.Invoke();
    }

    private void OnDestroy()
    {
        
    }
}
