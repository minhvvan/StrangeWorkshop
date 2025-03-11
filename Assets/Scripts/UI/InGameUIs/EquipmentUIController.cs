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
            
            // sprite에 따라 내부 이미지의 위치, 사이즈가 다름
            // 겉으로 보기에 사이즈, 위치가 일정하도록 조정
            SetImagePivot(holdableObjectSO.objectSprite);
            _iconImage.sprite = holdableObjectSO.objectSprite;
            _iconImage.SetNativeSize();
            // 영문 => 한글
            string switchedName = SwitchLanguage.Translate(holdableObjectSO.objectName);
            SetObjectNameFontSize(switchedName);
            _objectnameText.text = switchedName;
            _objectTypeText.text = SwitchLanguage.Translate(holdableObjectSO.objectType.ToString());
            _typeBackgroundImage.sprite = _typeBackgrounds[holdableObjectSO.objectType];
        }
    }

    private void SetObjectNameFontSize(string text)
    {
        int textLength = text.Length;
        if (textLength < 4)
        {
            _objectnameText.fontSize = 32;
        }
        else if (textLength < 5)
        {
            _objectnameText.fontSize = 28;
        }
        else
        {
            _objectnameText.fontSize = 24;
        }
    }

    private void SetImagePivot(Sprite sprite)
    {
        _iconImage.rectTransform.pivot = sprite.pivot/sprite.rect.size;
    }
}
