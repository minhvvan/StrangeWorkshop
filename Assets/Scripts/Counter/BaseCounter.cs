using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCounter : MonoBehaviour, IHoldableObjectParent
{
    [SerializeField] private Transform counterTopPoint;
    
    private List<HoldableObject> _holdableObject = new();
    
    // 상호작용, 키보드 e, 재료를 옮길 때 사용
    public virtual void Interact(SampleCharacterController player)
    {
    }
    
    // 상호작용, 키도브 f, 가공 및 작업할 때 사용
    public virtual void InteractAlternate(SampleCharacterController player)
    {
    }
    
    // 배치 포인트 반환
    public Transform GetHoldableObjectFollowTransform()
    {
        return counterTopPoint;
    }

    // turretObject set
    public void SetHoldableObject(HoldableObject holdableObject)
    {
        _holdableObject.Add(holdableObject);
    }

    public void GiveHoldableObject(IHoldableObjectParent parent)
    {
        if(!_holdableObject[^1].SetHoldableObjectParent(parent)) return;
        _holdableObject.Remove(_holdableObject[^1]);
    }

    // turretObject 반환
    public HoldableObject GetHoldableObject()
    {
        return _holdableObject[^1];
    }

    // turretObject clear
    public void ClearHoldableObject()
    {
        foreach (var holdableObject in _holdableObject)
        {
            Destroy(holdableObject.gameObject);
        }
        _holdableObject.Clear();
    }

    // turretObject 확인
    public bool HasHoldableObject()
    {
        return _holdableObject.Count > 0;
    }

    public bool CanSetHoldableObject()
    {
        return true;
    }

    protected List<HoldableObject> GetHoldableObjectList()
    {
        return _holdableObject;
    }
}
