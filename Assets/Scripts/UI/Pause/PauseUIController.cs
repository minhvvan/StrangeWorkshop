using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseUIController : MonoBehaviour
{
    public CanvasGroup pauseCanvasGroup;
    public float fadeDuration = 0.3f;

    bool isPaused = false;
    public Selectable firstSelected;

    
    // Post Processing
    public Volume globalVolume;
    DepthOfField dof;
    

    void Start()
    {
        pauseCanvasGroup.alpha = 0;
        pauseCanvasGroup.gameObject.SetActive(false);

        if (globalVolume.profile.TryGet(out dof))
        {
            dof.focusDistance.value = 5f;
        }
    }

    //Set GlobalVolume Post Processing
    public void SetDepthOfField(float value)
    {
        if(dof != null)
        {
            dof.focusDistance.value = value;
        }
    }

    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0;
        pauseCanvasGroup.DOKill(false);
        pauseCanvasGroup.gameObject.SetActive(true);
        pauseCanvasGroup.DOFade(1, fadeDuration).SetUpdate(true);
        SetDepthOfField(0.5f);

        SelectFirstUI();

        
    }

    public void Resume()
    {
        isPaused = false;
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