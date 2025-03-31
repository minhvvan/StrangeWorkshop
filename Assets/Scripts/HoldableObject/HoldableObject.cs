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
        transform.localScale = Vector3.zero;
        StartCoroutine(ScaleZeroToOne(0.2f)); // 0.2초 동안 스케일을 0에서 1로 변경
        
        return true;
    }

    public virtual bool SetHoldableObjectParentWithAnimation(IHoldableObjectParent parent)
    {
        // 부모 객체 설정
        parent.SetHoldableObject(this);

        if (TryGetComponent(out Collider col) && TryGetComponent(out Rigidbody rig))
        {
            col.isTrigger = true; 
            rig.isKinematic = false; // 물리를 적용하여 힘을 줄 수 있도록 설정

            // 튕겨오르는 효과 추가
            float bounceForce = 20f;
            float bounceTorque = 10f;

            rig.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
            rig.AddTorque(Random.insideUnitSphere * bounceTorque, ForceMode.Impulse);

            // 일정 시간 후 손으로 이동하도록 설정
            StartCoroutine(MoveToHand(parent, rig, col));
        }

        return true;
    }

    IEnumerator ScaleZeroToOne(float duration)
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.one;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }


    IEnumerator MoveToHand(IHoldableObjectParent parent, Rigidbody rig, Collider col)
    {
        yield return new WaitForSeconds(0.2f);

        rig.isKinematic = true;
        col.isTrigger = true;

        Transform target = parent.GetHoldableObjectFollowTransform();
        transform.parent = target;

        //float moveSpeed = 5f;
        float elapsedTime = 0f;
        float duration = 0.2f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, target.position, elapsedTime / duration);
            transform.rotation = Quaternion.Lerp(startRotation, target.rotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    //HoldableObject Spawn함수
    public static HoldableObject SpawnHoldableObject(HoldableObjectSO holdableObjectSo, IHoldableObjectParent toHoldableObjectParent, Transform fromTransform = null)
    {
        GameObject holdableObjectTransform = Instantiate(holdableObjectSo.prefab, fromTransform);
        holdableObjectTransform.transform.localPosition = Vector3.zero;
        holdableObjectTransform.transform.localRotation = Quaternion.identity;
        holdableObjectTransform.transform.localScale = Vector3.one;

        HoldableObject holdableObject = holdableObjectTransform.GetComponent<HoldableObject>();

        //holdableObject.SetHoldableObjectParentWithAnimation(holdableObjectParent);
        if(fromTransform == null)
        {
            holdableObject.SetHoldableObjectParent(toHoldableObjectParent);
        }
        else
        {
            holdableObject.SetHoldableObjectParentWithAnimation(toHoldableObjectParent);
        }

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
