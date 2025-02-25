using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseUIController : MonoBehaviour
{
    [SerializeField] public CanvasGroup pauseCanvasGroup;
    [SerializeField] public float fadeDuration = 0.3f;

    bool _isPaused = false;
    [SerializeField] public Selectable firstSelected;
    
    [SerializeField] public Volume globalVolume;
    DepthOfField _dof;
    

    void Start()
    {
        pauseCanvasGroup.alpha = 0;
        pauseCanvasGroup.gameObject.SetActive(false);

        if (globalVolume.profile.TryGet(out _dof))
        {
            _dof.focusDistance.value = 5f;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void SetDepthOfField(float value)
    {
        if(_dof != null)
        {
            _dof.focusDistance.value = value;
        }
    }

    public void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0;
        pauseCanvasGroup.DOKill(false);
        pauseCanvasGroup.gameObject.SetActive(true);
        pauseCanvasGroup.DOFade(1, fadeDuration).SetUpdate(true);
        SetDepthOfField(0.5f);

        SelectFirstUI();        
    }

    public void Resume()
    {
        _isPaused = false;
        Time.timeScale = 1;
        pauseCanvasGroup.DOKill(false);

        SetDepthOfField(5f);
        pauseCanvasGroup.DOFade(0, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            
            UnselectUI();
            pauseCanvasGroup.gameObject.SetActive(false);
        });
    }

    void SelectFirstUI()
    {
        if (firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
        }
    }

    void UnselectUI()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}