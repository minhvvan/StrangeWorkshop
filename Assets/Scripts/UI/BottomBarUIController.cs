using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BottomBarUIController : MonoBehaviour
{
    public enum BottomBarUIType
    {
        FillBackground,
        EmptyBackground
    }

    [SerializeField] BottomBarUIType _bottomBarUIType;
    [SerializeField] RectTransform _ControlsGridLayout;
    [SerializeField] Image _backgroundImage;
    
    
    [SerializeField] SerializedDictionary<string, Sprite> _controlCollection;
    [SerializeField] TMP_FontAsset _fontAsset;

    List<GameObject> _controlList = new List<GameObject>();


    void Start()
    {
        Initialize();   
    }

    void Initialize()
    {
        foreach (var control in _controlList)
        {
            DestroyImmediate(control);
        }
        _controlList.Clear();

        SetBottomBarUIType(_bottomBarUIType);
        foreach (var controlName in _controlCollection.Keys)
        {
            AddControl(controlName);
        }
    }

    void SetBottomBarUIType(BottomBarUIType type)
    {
        _bottomBarUIType = type;

        switch (type)
        {
            case BottomBarUIType.FillBackground:
                _backgroundImage.color = new Color(255, 255, 255, 255);
                break;
            case BottomBarUIType.EmptyBackground:
                _backgroundImage.color = new Color(255, 255, 255, 0);
                break;
        }
        
    }

    // void OnValidate()
    // {         
    //     //Initialize();
    // }

    void AddControl(string controlName)
    {
        GameObject control = new GameObject(controlName + " Image");
        control.transform.SetParent(_ControlsGridLayout);
        Image controlImage = control.AddComponent<Image>();
        controlImage.rectTransform.sizeDelta = new Vector2(50, 50);
        controlImage.color = new Color(0, 0, 0, 255);
        controlImage.sprite = _controlCollection[controlName];
        _controlList.Add(control);

        //TMP
        GameObject controlText = new GameObject(controlName + " Text");
        controlText.transform.SetParent(_ControlsGridLayout);
        TextMeshProUGUI controlTextMesh = controlText.AddComponent<TextMeshProUGUI>();
        controlTextMesh.text = controlName;
        controlTextMesh.fontSize = 30;
        controlTextMesh.font = _fontAsset;
        controlTextMesh.color = new Color(0, 0, 0, 255);
        controlTextMesh.alignment = TextAlignmentOptions.Left;
        controlTextMesh.rectTransform.sizeDelta = new Vector2(100, 50);
        _controlList.Add(controlText);
    }
}
