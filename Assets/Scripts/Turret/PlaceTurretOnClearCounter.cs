using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class PlaceTurretOnClearCounter : MonoBehaviour
{
    // editor 상에서 터렛 위치 편집을 쉽도록 하는 class
    // play중에는 작동하지 않는다
    private CancellationTokenSource cancelToken;
    public async UniTask UpdateClosestCounter()
    {            
        ClearCounter[] clearCounters = GameObject.FindObjectsOfType<ClearCounter>();
        cancelToken = new CancellationTokenSource();
        float time = 0f;

        while (!cancelToken.IsCancellationRequested)
        {
            foreach (ClearCounter counter in clearCounters)
            {
                Transform parentTransform = counter.GetHoldableObjectFollowTransform();
                float distance = Vector3.Distance(parentTransform.position, transform.position);
                if (distance < 3f)
                {
                    transform.SetParent(null);
                    transform.localScale = Vector3.one;
                    transform.SetParent(parentTransform);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }
            }
            await UniTask.Yield();
        }
    }

    public void Disabled()
    {
        cancelToken.Cancel();
    }
}
