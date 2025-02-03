using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class HoldableObject : MonoBehaviour
{
    [SerializeField] private HoldableObjectSO holdableObjectSo;
    
    //HoldableObjectSO 반환
    public HoldableObjectSO GetHoldableObjectSO()
    {
        return holdableObjectSo;
    }

    // parent를 옮겨 이동하는 함수
    public virtual bool SetHoldableObjectParent(IHoldableObjectParent parent)
    {
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

    public virtual bool Acceptable(HoldableObject holdableObject)
    {
        return false;
    }
}
