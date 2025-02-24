using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class ChapterPlanet : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationAxis;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private GameObject _planet;
    [SerializeField] private int _chapterIndex;
    
    private ChapterCamera _camera;
    private bool _locked;

    private void Awake()
    {
        _camera = FindObjectOfType<ChapterCamera>();
    }

    private void Start()
    {
        _locked = _chapterIndex > SaveData.LastUnlockedChapter;
    }

    private void Update()
    {
        _planet.transform.Rotate(_rotationAxis, _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        _camera.FocusIn(gameObject);
        var chapterUI = UIManager.Instance.AttachUI<ChapterUI>(UIType.ChapterUI);
        chapterUI.ChapterIndex = _chapterIndex;
        chapterUI.SetLocked(_locked);
        
        _planet.transform.DOScale(1.2f, .2f).SetEase(Ease.OutBounce);
    }

    private void OnTriggerExit(Collider other)
    {
        _camera.FocusOut();
        UIManager.Instance.DetachUI<ChapterUI>(UIType.ChapterUI);
        _planet.transform.DOScale(1f, .2f).SetEase(Ease.InQuint);
    }
}
