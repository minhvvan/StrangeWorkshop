using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CraftCounter : BaseCounter
{
    private CraftRecipeSO _currentCraftRecipeSO;
    private int _craftIndex;
    private int _currentIndex;
    
    private InGameUIController _inGameUIController;

    private float roatateSpeed = 100f;
    
    private RecipeSelectEvent _recipeSelectEvent;
    
    [Header("Events")]
    public Action<List<CraftRecipeSO>, List<string>> OnObjectsChangedAction;
    public Action<HoldableObjectSO> OnCraftCompleteAction;

    async void Awake()
    {
        await UniTask.WaitUntil(()=>UIManager.Instance.IsInitialized);
        _inGameUIController = UIManager.Instance.GetUI<InGameUIController>(UIType.InGameUI);
        _inGameUIController.RegisterGameUI(this);

        _recipeSelectEvent = await DataManager.Instance.LoadDataAsync<RecipeSelectEvent>(Addresses.Events.Counter.RECIPE_SELECTED);
        _recipeSelectEvent.AddListener(SetRecipe);
    }

    private void Update()
    {
        if (HasHoldableObject())
        {
            foreach (var obj in GetHoldableObjectList())
            {
                Vector3 directionToCenter = (GetHoldableObjectFollowTransform().position - obj.transform.position).normalized;
                Vector3 rotationAxis = Vector3.Cross(directionToCenter, Vector3.forward).y > 0 ? Vector3.Cross(directionToCenter, Vector3.forward).normalized : -Vector3.Cross(directionToCenter, Vector3.forward).normalized; ;

                // 중심점을 기준으로 회전 (올바른 공전 궤도 유지)
                obj.transform.RotateAround(GetHoldableObjectFollowTransform().position, rotationAxis, roatateSpeed * Time.deltaTime);    
            }
            
        }
    }

    public override void Interact(IInteractAgent agent = null)
    {
        if (agent != null && agent.GetGameObject().TryGetComponent(out IHoldableObjectParent parent))
        {
            // 플레이어가 물체를 들고 있으면
            if (parent.HasHoldableObject())
            {
                // 레시피를 가지고 있지 않으면 return
                if(_currentCraftRecipeSO.IsUnityNull())
                    return;
                
                // DeepCopy로 연산에 필요한 List생성 후 계산
                List<HoldableObject> CompareList = new(GetHoldableObjectList())
                {
                    parent.GetHoldableObject()
                };
            
                // 플레이어의 재료를 놓을 때 만들 수 있는 레시피가 있는 검사
                //List<CraftRecipeSO> recipeCandidates = RecipeManager.Instance.FindCraftRecipeCandidate(CompareList);
                if (!RecipeManager.Instance.WillMake(CompareList, _currentCraftRecipeSO))
                    return;
                parent.GiveHoldableObject(this);
                //빙글빙글 돌게 만드는 로직 주석처리 - 애니메이션 효과 후 위치를 정해주기 위하여 - 해당 기능은 코드 정리 후 추가 해야할 듯 싶음
                //GetHoldableObject().gameObject.transform.position += new Vector3(Random.Range(0.5f, 4f), Random.Range(0.5f, 4f), Random.Range(0.5f, 4f));

                var objectList = GetHoldableObjectList().Select(x => x.GetHoldableObjectSO().objectName).ToList();
                //OnObjectsChangedAction?.Invoke(recipeCandidates, objectList);
            }
            else
            {
                if (HasHoldableObject())
                {
                    GiveHoldableObject(parent);
                    TakeOffPlayerGlove(parent);
                
                    var objectList = GetHoldableObjectList().Select(x => x.GetHoldableObjectSO().objectName).ToList();
                    //OnObjectsChangedAction?.Invoke(RecipeManager.Instance.FindCraftRecipeCandidate(GetHoldableObjectList()), objectList);
                }
            }
        }
    }

    public float GetCraftIndex()
    {
        return _craftIndex;
    }

    public CraftRecipeSO GetCurrentCraftRecipeSO()
    {
        return _currentCraftRecipeSO;
    }
    
    // async void CoolTime()
    // {
    //     cooltime = false;
    //     await UniTask.WaitForSeconds(0.3f);
    //     cooltime = true;
    // }

    private void SetRecipe(CraftRecipeSO recipe)
    {
        _currentCraftRecipeSO = recipe;
        Debug.Log(_currentCraftRecipeSO);
    }
}
