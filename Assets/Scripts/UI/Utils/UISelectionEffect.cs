using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEditor.ShaderGraph;

public class UISelectionEffect : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleFactor = 1.2f;
    public float transitionTime = 0.1f;

    Vector3 originalScale;
    Coroutine scaleCoroutine;
    Coroutine colorCoroutine;

    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;
    

    bool isSelected = false;
    
    TextMeshProUGUI text;
    Image backgroundImage;

    void Awake()
    {
        originalScale = transform.localScale;
        text = GetComponentInChildren<TextMeshProUGUI>();
        if(text == null)
        {
            backgroundImage = GetComponent<Image>();
        }
    }
    

    void OnDisable()
    {
        transform.localScale = originalScale;
        isSelected = false;
        
        transform.localScale = originalScale;
        if (text != null)
        {
            text.color = normalColor;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        SetSelectedScale();
        SetSelectedColor();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        SetUnselectedScale();
        SetUnselectedColor();
    }

    IEnumerator AnimateScale(Vector3 targetScale)
    {
        float elapsedTime = 0;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < transitionTime)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / transitionTime);
            elapsedTime += Time.unscaledDeltaTime; 
            yield return null;
        }

        transform.localScale = targetScale;
        scaleCoroutine = null;
    }

    IEnumerator AnimateColor(Color targetColor)
    {
        float elapsedTime = 0;
        Color startColor = text != null ? text.color : backgroundImage.color;

        while (elapsedTime < transitionTime)
        {
            if (text != null)
            {
                text.color = Color.Lerp(startColor, targetColor, elapsedTime / transitionTime);
            }
            if(backgroundImage != null)
            {
                backgroundImage.color = Color.Lerp(startColor, targetColor, elapsedTime / transitionTime);
            }
            elapsedTime += Time.unscaledDeltaTime; 
            yield return null;
        }

        if(text != null)
            text.color = targetColor;
        if(backgroundImage != null)
            backgroundImage.color = targetColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            SetSelectedColor();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetUnselectedColor();
    }


    void SetUnselectedColor()
    {
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        colorCoroutine = StartCoroutine(AnimateColor(isSelected ? selectedColor : normalColor));
    }

    void SetSelectedColor()
    {
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        colorCoroutine = StartCoroutine(AnimateColor(selectedColor));
    }

    void SetUnselectedScale()
    {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(AnimateScale(originalScale));
    }

    void SetSelectedScale()
    {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(AnimateScale(originalScale * scaleFactor));
    }
}