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
        if (gameObject.TryGetComponent<MaterialCounter>(out MaterialCounter materialCounter))
        {
            materialCounter.ActivatePriceTag();
        }
    }

    public void Hide()
    {
        _quickOutline.enabled = false;
        if (gameObject.TryGetComponent<MaterialCounter>(out MaterialCounter materialCounter))
        {
            materialCounter.DeactivatePriceTag();
        }
    }
}
