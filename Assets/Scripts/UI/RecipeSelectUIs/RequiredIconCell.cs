using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequiredIconCell : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text countText;

    private int count = 1;
    
    public void SetInitialize(HoldableObjectSO recipe)
    {
        icon.sprite = recipe.objectSprite;
        countText.gameObject.SetActive(false);
    }

    public void AddCount()
    {
        count++;
        countText.text = "x" + count;
        countText.gameObject.SetActive(true);
    }
}
