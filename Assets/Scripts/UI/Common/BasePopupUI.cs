using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public abstract class BasePopupUI: MonoBehaviour
{
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] float fadeDuration = 0.3f;    
    [SerializeField] Selectable _firstSelected;    
    [SerializeField] Volume _globalVolume;
    DepthOfField _dof;
 
    public bool IsOpen { get ; private set ; }


    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.gameObject.SetActive(false);

        if (_globalVolume.profile.TryGet(out _dof))
        {
            _dof.focusDistance.value = 5f;
        }
    }
  
    public void Show()
    {
        IsOpen = true;
        Time.timeScale = 0;
        _canvasGroup.DOKill(false);
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.DOFade(1, fadeDuration).SetUpdate(true);
        DOTween.To(() => _dof.focusDistance.value, x => _dof.focusDistance.value = x, 1f, fadeDuration).SetUpdate(true);

        SelectFirstUI();
    }

    public void Hide()
    {
        IsOpen = false;
        Time.timeScale = 1;
        _canvasGroup.DOKill(false);
        
        DOTween.To(() => _dof.focusDistance.value, x => _dof.focusDistance.value = x, 5f, fadeDuration).SetUpdate(true).SetEase(Ease.InExpo);
        _canvasGroup.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            UnselectUI();
            _canvasGroup.gameObject.SetActive(false);
        });
    }
    
    /// <summary>
    /// Transition to hide the popup immediately
    /// </summary>
    public void HideImmediate()
    {
        IsOpen = false;
        Time.timeScale = 1;
        _canvasGroup.DOKill(false);
        _dof.focusDistance.value = 5f;
        _canvasGroup.alpha = 0;
        UnselectUI();
        _canvasGroup.gameObject.SetActive(false);
    }

    void SelectFirstUI()
    {
        if (_firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);
        }
    }

    void UnselectUI()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}