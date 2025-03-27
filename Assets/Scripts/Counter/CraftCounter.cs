using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CraftCounter : BaseCounter
{
    private CraftRecipeSO _currentCraftRecipeSO;
    private int _craftIndex;
    private int _currentIndex;
    private bool cooltime = true;
    
    private InGameUIController _inGameUIController;

    private float roatateSpeed = 100f;
    
    [Header("Events")]
    public Action<List<CraftRecipeSO>, List<string>> OnObjectsChangedAction;
    public Action<HoldableObjectSO> OnCraftCompleteAction;

    async void Awake()
    {
        await UniTask.WaitUntil(()=>UIManager.Instance.IsInitialized);
        _inGameUIController = UIManager.Instance.GetUI<InGameUIController>(UIType.InGameUI);
        _inGameUIController.RegisterGameUI(this);
    }

    private void Update()
    {
        if (HasHoldableObject())
        {
            foreach (var obj in GetHoldableObjectList())
            {
                Vector3 directionToCenter = (GetHoldableObjectFollowTransform().position - obj.transform.position).normalized;
                Vector3 rotationAxis = Vector3.Cross(directionToCenter, Vector3.forward).y > 0 ? Vector3.Cross(directionToCenter, Vector3.forward).normalized : -Vector3.Cross(directionToCenter, Vector3.forward).normalized; ;

                // 🌎 중심점을 기준으로 회전 (올바른 공전 궤도 유지)
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
                // DeepCopy로 연산에 필요한 List생성 후 계산
                List<HoldableObject> CompareList = new(GetHoldableObjectList())
                {
                    parent.GetHoldableObject()
                };
            
                // 플레이어의 재료를 놓을 때 만들 수 있는 레시피가 있는 검사
                List<CraftRecipeSO> recipeCandidates = RecipeManager.Instance.FindCraftRecipeCandidate(CompareList);
                if (recipeCandidates.Count <= 0)
                {
                    return;
                }
            
                parent.GiveHoldableObject(this);
                GetHoldableObject().gameObject.transform.position += new Vector3(Random.Range(0.5f, 4f), Random.Range(0.5f, 4f), Random.Range(0.5f, 4f));
                // 현재 만들 수 있는 레시피가 있으면 저장
                _currentCraftRecipeSO = RecipeManager.Instance.FindCanCraftRecipe(GetHoldableObjectList());
                SetCurrentCraftIndex();
            
                var objectList = GetHoldableObjectList().Select(x => x.GetHoldableObjectSO().objectName).ToList();
                OnObjectsChangedAction?.Invoke(recipeCandidates, objectList);
            }
            else
            {
                if (HasHoldableObject())
                {
                    GiveHoldableObject(parent);
                    _currentCraftRecipeSO = RecipeManager.Instance.FindCanCraftRecipe(GetHoldableObjectList());
                    SetCurrentCraftIndex();
                    TakeOffPlayerGlove(parent);
                
                    var objectList = GetHoldableObjectList().Select(x => x.GetHoldableObjectSO().objectName).ToList();
                    OnObjectsChangedAction?.Invoke(RecipeManager.Instance.FindCraftRecipeCandidate(GetHoldableObjectList()), objectList);
                }
            }
        }
    }
    
    

    private void SetCurrentCraftIndex()
    {
        if (!_currentCraftRecipeSO.IsUnityNull())
        {
            _craftIndex = _currentCraftRecipeSO.craftNumberOfTimes;
            _currentIndex = 0;
            // _progressBar.ResetBar();
            // _progressBar.gameObject.SetActive(true);
            // _progressBar.SetBar(_craftIndex);
        }
        else
        {
            // _progressBar.gameObject.SetActive(false);
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
    
    async void CoolTime()
    {
        cooltime = false;
        await UniTask.WaitForSeconds(0.3f);
        cooltime = true;
    }
}
