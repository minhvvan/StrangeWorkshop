using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class KnightSword : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        //if (other.name == "Player")
        if (other.name == "chest_low")
        {
            //var player = GetComponentInParent<SampleCharacterController>();
            var player = FindObjectOfType<SampleCharacterController>();
            player.ClearHoldableObject();
        }
    }
}
