using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TestCollider : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera vcam;

    private bool isTrigger;

    void OnTriggerEnter(Collider other)
    {
        if (isTrigger)
        {
            StartCoroutine(ZoomIn());
            isTrigger = false;
        }
        else
        {
            StartCoroutine(ZoomOut());
            isTrigger = true;
        }
    }

    IEnumerator ZoomIn()
    {
        while (vcam.m_Lens.OrthographicSize >= 44.2)
        {
            vcam.m_Lens.OrthographicSize -= 0.1f;
            yield return null;
        }
    }

    IEnumerator ZoomOut()
    {
        while (vcam.m_Lens.OrthographicSize <= 45)
        {
            vcam.m_Lens.OrthographicSize += 0.1f;
            yield return null;
        }
    }
}
