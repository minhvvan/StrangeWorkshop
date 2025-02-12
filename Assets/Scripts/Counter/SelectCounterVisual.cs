using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Outline))]
public class SelectCounterVisual : MonoBehaviour
{
    public enum SelectType
    {
        Prefab,
        Outline
    }
    
    [SerializeField] SelectType selectType;
    [SerializeField] private GameObject[] visualGameObjectArray;
    private Outline outline;
    
    

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
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
                outline.enabled = true;
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
                outline.enabled = false;
                break;
        }
    }
}
