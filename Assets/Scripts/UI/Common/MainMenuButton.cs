using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButton : Button
{
    public int buttonIndex;
    public Action<int> onSelected;
    public Action onSubmited;
    
    private TMP_Text _text;

    private new void Start()
    {
        _text = gameObject.GetComponentInChildren<TMP_Text>();
    }
    
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        image.color = Color.white;
        _text.color = Color.black;
        onSelected?.Invoke(buttonIndex);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        image.color = Color.clear;
        _text.color = Color.white;
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        onSubmited?.Invoke();
    }
}
