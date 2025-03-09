using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public abstract class BasePopupUI: MonoBehaviour, IGameUI
{
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] RectTransform _popupRect;
    [SerializeField] float _fadeDuration = 0.3f;
    [SerializeField] Selectable _firstSelected;
    [SerializeField] GameObject _bottombarUIPrefab;

    GameObject _bottombarUI;
    public Volume _globalVolume;
    public DepthOfField _dof;
    ColorAdjustments _colorAdjustments;

    [SerializeField] bool _isBlurBackground = true;
    [SerializeField] bool _isBinarizeBackground = false;
    [SerializeField] bool _isStackable = true;

    Action _onCloseEvent;
    Action _onShowEvent;
 
    public bool IsOpen { get ; private set ; }

    void Start(){
        Initialize();
    }

    public void Initialize()
    {        
        _globalVolume = FindObjectOfType<Volume>();
        _globalVolume.profile.TryGet(out _dof);
        _globalVolume.profile.TryGet(out _colorAdjustments);        
    }

    public void CleanUp()
    {
        HideImmediate();
    }

    public void ShowUI()
    {
        transform.SetAsLastSibling();
        IsOpen = true;
        Time.timeScale = 0;

        //Background effects
        CanvasActivate();
        ApplyBackgroundBlur();
        ApplyBinarizeSaturationEffect();

        SelectFirstUI();

        InitializeBottomBarUI();

        //Animations
        _onShowEvent?.Invoke();
        AnimatePopupDisplay();
        PushToStackIfStackable();        
    }

    //Focus on the UI
    public void SetFocus()
    {
        transform.SetAsLastSibling();
        IsOpen = true;
        Time.timeScale = 0;

        //Background effects
        CanvasActivate();
        ApplyBackgroundBlur();
        ApplyBinarizeSaturationEffect();

        SelectFirstUI();
    }

    void PushToStackIfStackable()
    {
        if (_isStackable)
        {
            UIManager.Instance.PushPopupUI(this);
        }
    }

    void AnimatePopupDisplay()
    {
        if (_popupRect != null)
        {
            UIAnimationUtility.PopupShow(_popupRect, 0.3f, null, true);
        }
    }

    void InitializeBottomBarUI()
    {
        if (_bottombarUIPrefab != null && _bottombarUI == null)
        {
            _bottombarUI = Instantiate(_bottombarUIPrefab, transform);
            _bottombarUI.transform.SetAsLastSibling();
        }
    }

    void ApplyBinarizeSaturationEffect()
    {
        if (_colorAdjustments != null && _isBinarizeBackground)
        {
            DOTween.To(() => _colorAdjustments.saturation.value, x => _colorAdjustments.saturation.value = x, -100f, _fadeDuration).SetUpdate(true);
        }
    }

    void ApplyBackgroundBlur()
    {
        if (_dof != null && _isBlurBackground)
        {
            DOTween.To(() => _dof.focusDistance.value, x => _dof.focusDistance.value = x, 1f, _fadeDuration).SetUpdate(true);
        }
    }

    void CanvasActivate()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.DOKill(false);
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.DOFade(1, _fadeDuration).SetUpdate(true);
        }
    }

    public void HideUI()
    {
        IsOpen = false;
        Time.timeScale = 1;
        StopCanvasGroupAnimation();
        CancelBackgroundBlur();
        CancelBinirizeSaturationEffect();
        ReleaseBottomBarUI();

        //animation
        CanvasDeactivate(() => {
            UnselectUI();
            PopStackIfStackable();
            _canvasGroup.gameObject.SetActive(false);
        });
        _onCloseEvent?.Invoke();
        AnimatePopupHide();
    }

    //최종 이벤트를 딜레이 후 발생하기 위한 action 포함
    void CanvasDeactivate(Action callback, bool isImmediate = false)
    {
        if (_canvasGroup == null) return;

        if (isImmediate)
        {
            _canvasGroup.DOKill(false);
            _canvasGroup.alpha = 0;
            callback?.Invoke();
        }
        else{
            _canvasGroup.DOFade(0, _fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                callback?.Invoke();
            });
        }
    }

    void ReleaseBottomBarUI(bool isImmediate = false)
    {
        if(_bottombarUI == null) return;

        if(isImmediate)
        {
            Destroy(_bottombarUI);
        }
        else
        {
            _bottombarUI.GetComponent<RectTransform>().DOAnchorPosY(-120, 0.3f).SetUpdate(true).OnComplete(() =>
            {
                Destroy(_bottombarUI);
            });
        }
    }

    void AnimatePopupHide(bool isImmediate = false)
    {
        if (_popupRect == null) return;

        if(isImmediate)
        {
            _popupRect.gameObject.SetActive(false);
        }
        else
        {
            UIAnimationUtility.PopupHide(_popupRect, 0.3f, null, true);
        }
    }

    void PopStackIfStackable()
    {
        if (_isStackable)
        {
            UIManager.Instance.PopPopupUI();
        }
    }

    void CancelBinirizeSaturationEffect(bool isImmediate = false)
    {
        if (_colorAdjustments == null) return;

        if(isImmediate)
        {
            _colorAdjustments.saturation.value = 0;
        }
        else
        {
            DOTween.To(() => _colorAdjustments.saturation.value, x => _colorAdjustments.saturation.value = x, 0f, _fadeDuration).SetUpdate(true);
        }
    }

    void CancelBackgroundBlur(bool isImmediate = false)
    {
        if (_dof == null) return;
        
        if(isImmediate)
        {
            _dof.focusDistance.value = 5f;
        }
        else
        {
            DOTween.To(() => _dof.focusDistance.value, x => _dof.focusDistance.value = x, 5f, _fadeDuration).SetUpdate(true).SetEase(Ease.InExpo);
        }
    }

    void StopCanvasGroupAnimation()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.DOKill(false);
        }
    }

    /// <summary>
    /// Transition to hide the popup immediately (without base class animation)
    /// </summary>
    public void HideImmediate()
    {
        IsOpen = false;
        Time.timeScale = 1;
        CancelBackgroundBlur(true);
        CancelBinirizeSaturationEffect(true);
        ReleaseBottomBarUI(true);
        CanvasDeactivate(null, true);
        
        UnselectUI();
        PopStackIfStackable();

        _onCloseEvent?.Invoke();
    }

    public void SelectFirstUI()
    {
        if (_firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);
        }
    }

    public void UnselectUI()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SetOnCloseEvent(Action action)
    {
        _onCloseEvent = action;
    }

    public void SetOnShowEvent(Action action)
    {
        _onShowEvent = action;
    }
}