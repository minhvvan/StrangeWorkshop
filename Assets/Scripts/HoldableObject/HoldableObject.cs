using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class HoldableObject : MonoBehaviour, IInteractable
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

        if (TryGetComponent(out Collider col) && TryGetComponent(out Rigidbody rig))
        {
            col.isTrigger = true;
            rig.isKinematic = true;
        }

        transform.parent = null;
        transform.localScale = Vector3.one;
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

    public void Interact(IInteractAgent agent = null)
    {
        if (agent != null && agent.GetGameObject().TryGetComponent(out IHoldableObjectParent parent))
        {
            if (parent.HasHoldableObject()) return;
            SetHoldableObjectParent(parent);
        }
    }

    public void InteractAlternate(IInteractAgent agent = null)
    {
        if (agent != null && agent.GetGameObject().TryGetComponent(out IHoldableObjectParent parent))
        {
            if (parent == null)
            {
                Debug.LogError("parent is null");
                return;
            }
            
            if (TryGetComponent(out Rigidbody rig) && TryGetComponent(out Collider col))
            {
                parent.SetHoldableObject(null);
                transform.parent = null;
                col.isTrigger = false;
                rig.isKinematic = false;
                rig.AddForce(parent.GetGameObject().transform.forward * 1000);
            }
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
