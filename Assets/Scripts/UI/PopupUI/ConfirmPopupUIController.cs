using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPopupUIController : BasePopupUI
{
    public enum ConfirmPopupType
    {
        BLUE,
        RED,
        GREEN,
        YELLOW,
        GREY,
    }

    [SerializeField] SerializedDictionary<ConfirmPopupType, Sprite> _confirmPopupTypeTextures;

    [SerializeField] Image _backgroundImage;
    [SerializeField] Button _confirmButton;
    [SerializeField] Button _cancelButton;

    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] TextMeshProUGUI _messageText;

    private System.Action _onConfirm;
    private System.Action _onCancel;

    void Awake()
    {
        Initialize();
        _confirmButton.onClick.AddListener(OnClickConfirm);
        _cancelButton.onClick.AddListener(OnClickCancel);
    }

    void OnClickConfirm()
    {
        _onConfirm?.Invoke();
        HideUI();
    }

    void OnClickCancel()
    {
        _onCancel?.Invoke();
        HideUI();
    }

    public void ShowUI(string title, string message, System.Action onConfirm, System.Action onCancel, ConfirmPopupType type)
    {
        _titleText.text = title;
        _messageText.text = message;
        _onConfirm = onConfirm;
        _onCancel = onCancel;

        if (_confirmPopupTypeTextures.ContainsKey(type))
        {
            _backgroundImage.sprite = _confirmPopupTypeTextures[type];
        }
        
        ShowUI();
    }
}
