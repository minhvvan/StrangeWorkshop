using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftSelectUIController : MonoBehaviour, IGameUI
{
    public Action OnHide;
    
    private RectTransform _root;
    private Vector2 _startPosition;
    
    [SerializeField] private GameObject recipeButtonPrefab;
    [SerializeField] private RectTransform recipeContent;
    [SerializeField] private DescriptionPanelController descriptionPanelController;
    [SerializeField] private RectTransform craftSelectPanel;
    [SerializeField] private List<Button> typeButtons;
    [SerializeField] private Image craftSelectImage;
    [SerializeField] private TMP_Text craftSelectText;
    
    private CraftRecipeCollectionSO _craftCollectionSo;
    private Selectable _firstSelected;
    
    private Dictionary<CraftType, List<RecipeButton>> _recipeButtons = new Dictionary<CraftType, List<RecipeButton>>();
    private List<RecipeButton> _currentRecipeButtons = new List<RecipeButton>();
    private RecipeSelectEvent _recipeSelectEvent;
    
    async void Awake()
    {
        Initialize();
        
        _recipeSelectEvent = await DataManager.Instance.LoadDataAsync<RecipeSelectEvent>(Addresses.Events.Counter.RECIPE_SELECTED);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideUI();
        }
    }
    
    public void ShowUI()
    {
        var endPos = _startPosition;
        _root.anchoredPosition = new Vector2(endPos.x, -Screen.height);
        _root.gameObject.SetActive(true);
        _root.DOAnchorPos(endPos, 0.5f).SetEase(Ease.OutQuad);
        SelectFirstUI();
    }

    public void HideUI()
    {
        OnHide?.Invoke();
        UIAnimationUtility.SlideOutDown(_root);
    }

    public void Initialize()
    {
        _root = GetComponent<RectTransform>();
        _startPosition = _root.anchoredPosition;
        _craftCollectionSo = RecipeManager.Instance.GetCraftRecipeCollection;
        Button currentButton = null;
        
        // collection의 key를 가져옴
        ICollection<CraftType> keys = _craftCollectionSo.recipesCollection.Keys;

        // 모든 CraftRecipe 버튼을 만든다
        foreach (CraftType key in keys)
        {
            List<RecipeButton> recipeButtons = new List<RecipeButton>();
            foreach (var recipe in _craftCollectionSo.recipesCollection[key].recipes)
            {
                GameObject recipeButton = Instantiate(recipeButtonPrefab, recipeContent);
                recipeButton.GetComponent<RecipeButton>().SetInitialize(recipe);
                recipeButton.GetComponent<RecipeButton>().OnClick += SelectRecipe;
                recipeButtons.Add(recipeButton.GetComponent<RecipeButton>());
                Navigation navigation;
                if (currentButton != null)
                {
                    navigation = currentButton.navigation;
                    navigation.selectOnDown = recipeButton.GetComponent<Button>();
                    currentButton.navigation = navigation;
                
                    navigation = recipeButton.GetComponent<Button>().navigation;
                    navigation.selectOnUp = currentButton;
                    recipeButton.GetComponent<Button>().navigation = navigation;
                }
                currentButton = recipeButton.GetComponent<Button>();
                recipeButton.gameObject.SetActive(false);
                
                navigation = currentButton.navigation;
                
                // 상단 CraftType의 버튼 네비게이션 달아주기
                switch (key)
                {
                    case CraftType.Fix:
                        navigation.selectOnLeft = typeButtons[2];
                        navigation.selectOnRight = typeButtons[1];
                        break;
                    case CraftType.Bullet:
                        navigation.selectOnLeft = typeButtons[0];
                        navigation.selectOnRight = typeButtons[2];
                        break;
                    case CraftType.Energy:
                        navigation.selectOnLeft = typeButtons[1];
                        navigation.selectOnRight = typeButtons[0];
                        break;
                }
                currentButton.navigation = navigation;
            }    
            _recipeButtons.Add(key, recipeButtons);
        }
        
        // 초기 CraftType.Fix에 있는 것만 보여주기
        foreach (var recipeButton in _recipeButtons[CraftType.Fix])
        {
            recipeButton.gameObject.SetActive(true);
        }
        _currentRecipeButtons = _recipeButtons[CraftType.Fix];

        typeButtons[0].onClick.AddListener(() =>
        {
            SelectType(CraftType.Fix);
        });
        
        typeButtons[1].onClick.AddListener(() =>
        {
            SelectType(CraftType.Bullet);
        });
        
        typeButtons[2].onClick.AddListener(() =>
        {
           SelectType(CraftType.Energy);
        });
        
        
        _firstSelected = recipeContent.GetComponentInChildren<Selectable>();
    }

    public void CleanUp()
    {
    }

    public void SetDescriptionPanel(CraftRecipeSO recipe)
    {
        descriptionPanelController.SetInitialDescription(recipe);
    }

    public void ClearDescriptionPanel()
    {
        descriptionPanelController.ClearDescription();
    }

    public void SelectRecipe(CraftRecipeSO recipe)
    {
        craftSelectImage.sprite = recipe.craftRecipeIcon;
        craftSelectText.text = recipe.craftRecipeName;
        
        _recipeSelectEvent.Raise(recipe);
        
        HideUI();
    }
    
    // 처음 선택된 버튼
    private void SelectFirstUI()
    {
        if (_firstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);
        }
    }

    private void SelectType(CraftType type)
    {
        foreach (var button in _currentRecipeButtons)
        {
            button.gameObject.SetActive(false);
        }
        
        foreach (var recipeButton in _recipeButtons[type])
        {
            recipeButton.gameObject.SetActive(true);
        }
        _currentRecipeButtons = _recipeButtons[type];

        foreach (var button in typeButtons)
        {
            var navigation = button.navigation;
            navigation.selectOnDown = _currentRecipeButtons[0].GetComponent<Button>();
            button.navigation = navigation;
        }

        _firstSelected = _currentRecipeButtons[0].GetComponent<Selectable>();
        SelectFirstUI();
    }
}
