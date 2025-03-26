using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequiredIconCell : MonoBehaviour
{
    [SerializeField] Image icon;
    
    public void SetInitialize(HoldableObjectSO recipe)
    {
        icon.sprite = recipe.objectSprite;
    }
}
