using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text recipeName;
    
    private Button _button;
    private CraftRecipeSO _recipe;
    
    public Action<CraftRecipeSO> OnClick;
    
    void Awake()
    {
        _button = GetComponent<Button>();
    }
    
    public void SetInitialize(CraftRecipeSO recipe)
    {
        _recipe = recipe;
        icon.sprite = recipe.craftRecipeIcon;
        recipeName.text = recipe.craftRecipeName;
        _button.onClick.AddListener(() => { OnClick?.Invoke(_recipe); });
    }

    public void OnSelect(BaseEventData eventData)
    {
        UIManager.Instance.GetUI<CraftSelectUIController>(UIType.RecipeSelectUI).ClearDescriptionPanel();
        UIManager.Instance.GetUI<CraftSelectUIController>(UIType.RecipeSelectUI).SetDescriptionPanel(_recipe);
    }

    public void OnDeselect(BaseEventData eventData)
    {
    }
}
