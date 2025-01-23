using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoldableObjectParent
{
    // HoldableObject가 위치할 transform return
    public Transform GetHoldableObjectFollowTransform();

    // Parent에 holdableObject set
    public void SetHoldableObject(HoldableObject holdableObject);

    // HoldableObject를 옮기는 함수
    public void GiveHoldableObject(IHoldableObjectParent parent);
    
    // HoldableObject get
    public HoldableObject GetHoldableObject();

    // HoldableObject Clear
    public void ClearHoldableObject();

    // HoldableObject를 가지는지 검사
    public bool HasHoldableObject();
    
    // HoldableObject를 둘 수 있는 상태인지 검사(완성품일 때 검사하기 위해 호출됨)
    public bool CanSetHoldableObject();
}
