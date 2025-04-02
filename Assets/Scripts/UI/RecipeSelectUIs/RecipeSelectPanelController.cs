using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RecipeSelectPanelController : MonoBehaviour, IGameUI
{
    private RectTransform _root;
    private CraftRecipeCollectionSO _recipeCollection;
    
    [SerializeField] private RectTransform recipeSelectParent;
    [SerializeField] private GameObject selectRecipeButton;
    
    
    void Start()
    {
        _root = transform as RectTransform;
        _recipeCollection = RecipeManager.Instance.GetCraftRecipeCollection;
        
        // var recipes = RecipeManager.Instance.GetCraftRecipeCollection;
        // foreach (var recipe in recipes.recipes)
        // {
        //     var recipeButton = Instantiate(selectRecipeButton, recipeSelectParent);
        //     recipeButton.GetComponent<RecipeButton>().SetInitialize(recipe);
        // }
    }
    
    public void ShowUI()
    {
        // var recipes = RecipeManager.Instance.GetCraftRecipeCollection;
        // foreach (var recipe in recipes.recipes)
        // {
        //     var recipeButton = Instantiate(selectRecipeButton, recipeSelectParent);
        //     recipeButton.GetComponent<RecipeButton>().SetInitialize(recipe);
        // }
        UIAnimationUtility.SlideInDown(_root);
    }

    public void HideUI()
    {
        UIAnimationUtility.SlideOutDown(_root);
    }

    public void Initialize()
    {
    }

    public void CleanUp()
    {
        // TODO: Button 없애기
    }
}
