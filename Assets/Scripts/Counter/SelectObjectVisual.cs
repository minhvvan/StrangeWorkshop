using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(QuickOutline))]
public class SelectObjectVisual : MonoBehaviour
{
    [SerializeField] private GameObject[] visualGameObjectArray;
    private QuickOutline _quickOutline;

    void Start()
    {
        _quickOutline = GetComponent<QuickOutline>();
        _quickOutline.enabled = false;
    }
    
    public void Show()
    {
        _quickOutline.enabled = true;
    }

    public void Hide()
    {
        _quickOutline.enabled = false;
    }
}
