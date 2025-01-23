using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class HoldableObject : MonoBehaviour
{
    [SerializeField] private HoldableObjectSO holdableObjectSo;

    //HoldableObjectSO 반환
    public HoldableObjectSO GetHoldableObjectSO()
    {
        return holdableObjectSo;
    }

    // parent를 옮겨 이동하는 함수
    public bool SetHoldableObjectParent(IHoldableObjectParent parent)
    {
        // 현재 HoldableObject가 완성품이면 옮길 수 있는 상태인지 검사(Player가 장갑을 꼈는지)
        if (GetHoldableObjectSO().objectType == HoldableObjectType.CraftProduct)
        {
            if (!parent.CanSetHoldableObject())
            {
                return false;
            }
        }
        
        parent.SetHoldableObject(this);
        
        transform.parent = parent.GetHoldableObjectFollowTransform();
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        return true;
    }

    //HoldableObject Spawn함수
    public static HoldableObject SpawnHoldableObject(HoldableObjectSO holdableObjectSo, IHoldableObjectParent holdableObjectParent)
    {
        GameObject holdableObjectTransform = Instantiate(holdableObjectSo.prefab);
        HoldableObject holdableObject = holdableObjectTransform.GetComponent<HoldableObject>();
        holdableObject.SetHoldableObjectParent(holdableObjectParent);

        return holdableObject;
    }
    
}
