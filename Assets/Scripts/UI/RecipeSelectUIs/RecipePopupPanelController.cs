using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipePopupPanelController : BasePopupUI
{
    private RectTransform _root;

    [SerializeField] private GameObject requiredIconCell;
    [SerializeField] private RectTransform requiredIconParent;
    
    void Start()
    {
        _root = transform as RectTransform;
    }


    public void ShowUI(CraftRecipeSO recipe)
    {
        foreach (var input in recipe.inputs)
        {
            GameObject inputCell = Instantiate(requiredIconCell, requiredIconParent);
            inputCell.GetComponent<RequiredIconCell>().SetInitialize(input);
        }
        
        ShowUI();
    }
    
    public void ShowUI()
    {
        UIAnimationUtility.SlideInLeft(_root);
    }

    public void HideUI()
    {
        Vector2 originalPos = _root.anchoredPosition;
        UIAnimationUtility.SlideOutLeft(_root, callback:()=>{_root.anchoredPosition = originalPos;});
    }

    public void Initialize()
    {
    }

    public void CleanUp()
    {
    }
}
