using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ReturnPort : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LootBot lootBot))
        {
            lootBot.SuccessReturn();
        }
    }
}
