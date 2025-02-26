using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class RecipeUIController : MonoBehaviour, IGameUI
{
    // string: holdableobject의 objectName
    private Dictionary<string, RectTransform> _recipeUIs; // 모든 recipe UI
    private Dictionary<string, RecipePartController> _recipePartControllers; // recipePartController 캐싱
    private Dictionary<string, RectTransform> _activatedRecipeUIs; // 활성화된 recipe UI
    private List<CraftRecipeSO> _recommendedRecipes;
    private ChapterRecipeSO _chapterRecipes;

    private RectTransform _root;
    
    [SerializeField] private CraftRecipeSO _bulletRecipe;
    [SerializeField] private CraftRecipeSO _upgradeRecipe;

    private int currentLevel = 0;
    
    private readonly int _maxRecipeUINum = 2;
    private readonly float _xPos = -28f;
    private readonly float _topYPos = 85f;
    private readonly float _interval = 30f;

    public async UniTask Initialize(CraftRecipeCollectionSO recipeCollectionSO)
    {
        // 일단 비활성화 후 uianimation에서 활성화 -> 깜빡임 제거
        gameObject.SetActive(false);
        _root = this.GetComponent<RectTransform>();
        _recipeUIs = new Dictionary<string, RectTransform>();
        _recipePartControllers = new Dictionary<string, RecipePartController>();
        _activatedRecipeUIs = new Dictionary<string, RectTransform>();
        _recommendedRecipes = new List<CraftRecipeSO>();
        _chapterRecipes =
            await DataManager.Instance.LoadDataAsync<ChapterRecipeSO>(Addresses.Data.UI.CHAPTER_RECIPE);
        
        // 모든 UI prefab pooling
        foreach (CraftRecipeSO recipe in recipeCollectionSO.recipes)
        {
            RectTransform recipeUI = GameObject.Instantiate(recipe.craftRecipeUI, transform);
            recipeUI.GameObject().SetActive(false);
            _recipeUIs[recipe.output.objectName] = recipeUI;
            _recipePartControllers[recipe.output.objectName] = recipeUI.GetComponent<RecipePartController>();
        }
        
        ShowUI();
        UpdateUI(null, null);
    }
    
    public void ShowUI()
    {
        UIAnimationUtility.SlideInRight(_root);
    }
    
    public void HideUI()
    {
        Vector2 originalPos = _root.anchoredPosition;
        UIAnimationUtility.SlideOutRight(_root, callback: () => _root.anchoredPosition = originalPos);
    }

    public void Initialize()
    {
        
    }

    public void UpdateUI(List<CraftRecipeSO> recipes, List<string> partNames)
    {
        /*
         * partNames: craftcounter 위에 올라간 재료들 이름
         * 추천 레시피 ui 표시
         * 호출 시점
         * 1. craft counter에 물건 들기/놓기
         * 2. craft counter에서 물건 완성
         */
        RecommendRecipe(recipes);
        
        List<string> recommendedRecipeNames = _recommendedRecipes.Select(a => a.output.objectName).ToList();

        // 추천에서 벗어난 설계도 비활성화
        var recipeNames = _activatedRecipeUIs.Keys.ToList();
        foreach (var recipeName in recipeNames)
        {
            _recipePartControllers[recipeName].DeactivateColor();
            if (!recommendedRecipeNames.Contains(recipeName))
            {
                UIAnimationUtility.SlideOutRight(_activatedRecipeUIs[recipeName]);
                _activatedRecipeUIs.Remove(recipeName);
            }
        }
        
        float currentYPos = _topYPos;
        for (int i = 0; i < _recommendedRecipes.Count; i++)
        {
            string recipeName = _recommendedRecipes[i].output.objectName;
            RectTransform recipeUI = _recipeUIs[recipeName];
            
            // 이미 활성화 되어있던 레시피일 때
            if (_activatedRecipeUIs.ContainsKey(recipeName))
            {
                Vector3 newPosition = new Vector3(recipeUI.localPosition.x, currentYPos, recipeUI.localPosition.z);
                UIAnimationUtility.MoveSmoothly(recipeUI, newPosition);
            }
            // 활성화되지 않은 레시피일 때
            else
            {
                recipeUI.localPosition = new Vector3(_xPos, currentYPos, recipeUI.localPosition.z);
                UIAnimationUtility.SlideInRight(recipeUI);
                _activatedRecipeUIs[recipeName] = recipeUI;
            }
            
            // 레시피 UI 높이 조정
            if (i <= _recommendedRecipes.Count - 2)
            {
                RectTransform nextRecipeUI = _recipeUIs[_recommendedRecipes[i + 1].output.objectName];
                currentYPos -= (recipeUI.sizeDelta.y * recipeUI.lossyScale.y / 2 + 
                                nextRecipeUI.sizeDelta.y * nextRecipeUI.lossyScale.y / 2 +
                                _interval);
            }
        }
        
        if (partNames != null && partNames.Count > 0)
        {
            foreach (string objectName in _activatedRecipeUIs.Keys)
            {
                _recipePartControllers[objectName].ActivateColor(partNames);
            }
        }
    }
    
    public void CraftComplete(HoldableObjectSO outputSO)
    {
        string outputName = outputSO.objectName;
        RectTransform outputRecipeUI = _recipeUIs[outputSO.objectName];
        
        Image recipeUIImage = outputRecipeUI.GetComponent<Image>();
        Color originalColor = recipeUIImage.color;
        Vector3 originalScale = outputRecipeUI.localScale;
        
        
        // 완성 애니메이션
        outputRecipeUI.DOKill();
        var sequence = DOTween.Sequence();
        sequence.SetLink(outputRecipeUI.gameObject);
        sequence.Append(recipeUIImage.DOColor(Color.green, 0f))
            .AppendInterval(0.2f)
            .Append(outputRecipeUI.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack))
            .AppendCallback(() =>
            {
                outputRecipeUI.gameObject.SetActive(false);
                outputRecipeUI.localScale = originalScale;
                recipeUIImage.color = originalColor;
                _activatedRecipeUIs.Remove(outputName);
                _recipePartControllers[outputName].DeactivateColor();
                UpdateUI(null, null);
            });
    }
    
    private void RecommendRecipe(List<CraftRecipeSO> recipes)
    {
        /*
         * 1.craft counter에 아무것도 없을 시
         *      해당 레벨의 추천 레시피
         * 2.craft counter에 무언가 올라가 있을 때
         *      만들 수 있는 레시피
         * 3. 특수 재료를 얻었을 때
         *      특수 재료가 들어간 레시피
         * 4. 총알과 업그레이드 모듈 레시피는 항상 띄우기
         * 5. 특정 키를 누르면 모든 레시피 띄우기
         */
        _recommendedRecipes.Clear();

        // craftcounter에 아무것도 없을 때
        if (recipes == null || recipes.Count <= 0)
        {
            foreach (CraftRecipeSO recipe in _chapterRecipes.chapterRecipes[currentLevel].recommendedRecipes)
            {
                _recommendedRecipes.Add(recipe);
            }
        }
        // craftcounter에 뭔가 올라갔을 때
        else
        {
            for (int i = 0; i < recipes.Count; i++)
            {
                // bullet과 upgrade는 무조건 일단 빼둠
                if (recipes[i].output.objectType == HoldableObjectType.Bullet ||
                    recipes[i].output.objectType == HoldableObjectType.Upgrade) continue;
                
                _recommendedRecipes.Add(recipes[i]);
                
                // 터렛 추천 레시피 개수가 3개가 넘지 않도록 조정
                if (_recommendedRecipes.Count >= _maxRecipeUINum)
                {
                    break;
                }
            }
        }
        // _recommendedRecipes.Add(_bulletRecipe);
        // _recommendedRecipes.Add(_upgradeRecipe);
    }
    
    public void CleanUp()
    {
    }
}
