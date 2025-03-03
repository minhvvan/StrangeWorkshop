using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EquipmentUIController : MonoBehaviour, IGameUI
{
    private RectTransform _root;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _objectnameText;
    [SerializeField] private TMP_Text _objectTypeText;
    [SerializeField] private Image _typeBackgroundImage;
    
    [SerializeField] private SerializedDictionary<HoldableObjectType, Sprite> _typeBackgrounds;
    
    void Awake()
    {
        _root = GetComponent<RectTransform>();
        UpdateEquipment(null);
        
        ShowUI();
    }
    
    public void ShowUI()
    {
        UIAnimationUtility.SlideInLeft(_root);
    }

    public void HideUI()
    {
        Vector2 originalPos = _root.anchoredPosition;
        UIAnimationUtility.SlideOutLeft(_root, callback:()=>{_root.anchoredPosition = originalPos;});
    }

    public void Initialize()
    {
    }

    public void CleanUp()
    {
    }

    public void UpdateEquipment(HoldableObject holdableObject)
    {
        if (holdableObject == null)
        {
            _iconImage.enabled = false;
            _objectnameText.enabled = false;
            _objectTypeText.enabled = false;
            _typeBackgroundImage.sprite = _typeBackgrounds[HoldableObjectType.None];
        }
        else
        {
            HoldableObjectSO holdableObjectSO = holdableObject.GetHoldableObjectSO();
            
            _iconImage.enabled = true;
            _objectnameText.enabled = true;
            _objectTypeText.enabled = true;
            
            _iconImage.sprite = holdableObjectSO.objectSprite;
            _objectnameText.text = SwitchLanguage.Translate(holdableObjectSO.objectName);
            _objectTypeText.text = SwitchLanguage.Translate(holdableObjectSO.objectType.ToString());
            _typeBackgroundImage.sprite = _typeBackgrounds[holdableObjectSO.objectType];
        }
    }
}
