using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(QuickOutline))]
public class SelectCounterVisual : MonoBehaviour
{
    public enum SelectType
    {
        Prefab,
        Outline
    }
    
    [SerializeField] SelectType selectType;
    [SerializeField] private GameObject[] visualGameObjectArray;
    private QuickOutline _quickOutline;
    
    

    void Start()
    {
        _quickOutline = GetComponent<QuickOutline>();
        _quickOutline.enabled = false;
    }
    
    public void Show()
    {
        switch (selectType)
        {
            case SelectType.Prefab:
                foreach (var visualGameObject in visualGameObjectArray)
                {
                    visualGameObject.SetActive(true);
                }
                break;
            case SelectType.Outline:
                _quickOutline.enabled = true;
                break;
        }
    }

    public void Hide()
    {
        switch (selectType)
        {
            case SelectType.Prefab:
                foreach (var visualGameObject in visualGameObjectArray)
                {
                    visualGameObject.SetActive(false);
                }
                break;
            case SelectType.Outline:
                _quickOutline.enabled = false;
                break;
        }
    }
}
