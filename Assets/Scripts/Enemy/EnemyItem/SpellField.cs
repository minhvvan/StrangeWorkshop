using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellField : MonoBehaviour
{
    public SampleCharacterController sampleCharacterController;
    public Transform activeVfx;
    
    //public float force = 100f;
    public bool isInside = false;

    private void Awake()
    {
        activeVfx = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == "Activation");
        activeVfx.gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            isInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            isInside = false;
        }
    }
    
    // public void OnShockWave()
    // {
    //     if (sampleCharacterController != null && isInside)
    //     {
    //         sampleCharacterController.rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    //     }
    // }
}
