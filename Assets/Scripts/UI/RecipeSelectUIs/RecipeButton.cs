using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeButton : MonoBehaviour
{
    public RecipePopupPanelController recipePopupPanelController;
    
    [SerializeField] Image icon;
    
    private Button _button;
    //private CraftRecipeSO _recipe;
    
    void Start()
    {
        _button = GetComponent<Button>();
    }
    
    public void SetInitialize(CraftRecipeSO recipe)
    {
        icon.sprite = recipe.craftRecipeIcon;
        
        //_button.onClick.AddListener();
    }
}
