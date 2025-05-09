using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MaterialCounter : BaseCounter
{
    [SerializeField] private HoldableObjectSO holdableObjectSO;
    
    [Header("Price UI")]
    [SerializeField] private RectTransform priceTagRect;
    [SerializeField] private TextMeshProUGUI priceText;

    private readonly float _rotateSpeed = 100f;

    void Start()
    {
        HoldableObject.SpawnHoldableObject(holdableObjectSO, this, GetHoldableObjectFollowTransform());
        priceText.text = $"${holdableObjectSO.price.ToString()}";
    }
    
    private void Update()
    {
        if (HasHoldableObject())
        {
            GetHoldableObject().transform.Rotate(0, _rotateSpeed * Time.deltaTime, 0, Space.World);
        }
    }

    public override void Interact(IInteractAgent agent = null)
    {
    	int price = holdableObjectSO.price;
        if (agent != null && agent.GetGameObject().TryGetComponent(out IHoldableObjectParent parent))
        {
            if (!parent.HasHoldableObject() && HasHoldableObject() && InGameDataManager.Instance.Purchasable(price))
            {
                GiveHoldableObject(parent);
                StartCoroutine(SpawnHoldableObject());
                TakeOffPlayerGlove(parent);
            }
        }
    }

    IEnumerator SpawnHoldableObject()
    {
        yield return new WaitForSeconds(3f);
        HoldableObject.SpawnHoldableObject(holdableObjectSO, this, GetHoldableObjectFollowTransform());
    }

    public void ActivatePriceTag()
    {
        UIAnimationUtility.PopupShow(priceTagRect, duration:0.1f);
    }

    public void DeactivatePriceTag()
    {
        UIAnimationUtility.PopupHide(priceTagRect, duration:0.1f);
    }
}
