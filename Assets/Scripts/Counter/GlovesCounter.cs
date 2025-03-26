using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlovesCounter : BaseCounter
{
    [SerializeField] Transform glovePrefab;

    private Transform glove;
    public override void Interact(IInteractAgent agent = null)
    {
        if (agent != null && agent.GetGameObject().TryGetComponent(out IHoldableObjectParent parent))
        {
            SampleCharacterController player = parent as SampleCharacterController;
            if (player != null && !player.HasHoldableObject())
            {
                player.WearGlove(glove);
                glove.localPosition = Vector3.zero;
                glove.localRotation = Quaternion.identity;
                StartCoroutine(SpawnGlove());
            }
        }
    }

    private void Start()
    {
        SetGlove();
    }

    private void SetGlove()
    {
        glove = Instantiate(glovePrefab, GetHoldableObjectFollowTransform());
        glove.localPosition = Vector3.zero;
        glove.localRotation = Quaternion.identity;
    }

    IEnumerator SpawnGlove()
    {
        yield return new WaitForSeconds(1f);
        SetGlove();
    }
}
