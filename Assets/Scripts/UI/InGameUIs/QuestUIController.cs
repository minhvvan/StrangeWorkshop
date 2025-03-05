using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUIController : MonoBehaviour, IGameUI
{
    private RectTransform _root;
    
    [SerializeField] private TMP_Text _missionText1;
    [SerializeField] private TMP_Text _missionText2;

    void Awake()
    {
        _root = GetComponent<RectTransform>();
    }
    public void ShowUI()
    {
        UIAnimationUtility.SlideInLeft(_root);
    }

    public void HideUI()
    {
        Vector2 originalPos = _root.anchoredPosition;
        UIAnimationUtility.SlideOutLeft(_root, callback: ()=>{_root.anchoredPosition = originalPos;});
    }

    public void Initialize()
    {
    }

    public void CleanUp()
    {
    }

    public void SetQuestUIController()
    {
        
    }
}
