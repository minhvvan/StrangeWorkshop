using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverButton : Button
{
    private AudioSource _audioSource;
    
    protected override void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        transform.DOScale(1.3f, .3f);
        _audioSource.Play();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        transform.DOScale(1.0f, .3f);
    }
}
