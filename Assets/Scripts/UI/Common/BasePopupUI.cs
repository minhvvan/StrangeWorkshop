using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public abstract class BasePopupUI: MonoBehaviour, IGameUI
{
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] float fadeDuration = 0.3f;    
    [SerializeField] Selectable _firstSelected;
    Volume _globalVolume;
    DepthOfField _dof;
 
    public bool IsOpen { get ; private set ; }


    void Awake(){
        _globalVolume = GameObject.FindObjectOfType<Volume>();
    }

    public void Initialize()
    {
        if(_canvasGroup != null)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.gameObject.SetActive(false);
        }

        if (_globalVolume.profile.TryGet(out _dof))
        {
            _dof.focusDistance.value = 5f;
        }
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
        if(_canvasGroup != null)
        {
            _canvasGroup.DOKill(false);
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.DOFade(1, fadeDuration).SetUpdate(true);
        }
        if(_dof != null)
        {
            DOTween.To(() => _dof.focusDistance.value, x => _dof.focusDistance.value = x, 1f, fadeDuration).SetUpdate(true);
        }

        SelectFirstUI();        
    }

    public void HideUI()
    {
        IsOpen = false;
        Time.timeScale = 1;

        if(_canvasGroup != null)
        {
            _canvasGroup.DOKill(false);
        }
        
        if(_dof != null)
        {
            DOTween.To(() => _dof.focusDistance.value, x => _dof.focusDistance.value = x, 5f, fadeDuration).SetUpdate(true).SetEase(Ease.InExpo);
        }
        
        if(_canvasGroup != null)
        {
            _canvasGroup.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
            {
                UnselectUI();
                _canvasGroup.gameObject.SetActive(false);
            });
        }
        else
        {
            UnselectUI();
        }
    }
    
    /// <summary>
    /// Transition to hide the popup immediately
    /// </summary>
    public void HideImmediate()
    {
        IsOpen = false;
        Time.timeScale = 1;
        _dof.focusDistance.value = 5f;

        if(_canvasGroup != null)
        {
            _canvasGroup.DOKill(false);
            _canvasGroup.alpha = 0;
            _canvasGroup.gameObject.SetActive(false);    
        }        
        
        UnselectUI();
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
}