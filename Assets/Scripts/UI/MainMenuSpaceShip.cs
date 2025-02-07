using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class MainMenuSpaceShip : MonoBehaviour
{
    public async UniTask OnClickedGameStart()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Animation anim = GetComponent<Animation>();
  
        anim.enabled = false;
   
        var completion = new UniTaskCompletionSource();
        float duration = 1f;
   
        var sequence = DOTween.Sequence();
        sequence.Append(rect.DOLocalPath(new Vector3[] {
                new Vector3(-653, -185, 0),
                new Vector3(-24, -329, 0),
                new Vector3(1364, 403, 0)
            }, duration, PathType.CatmullRom))
            .Join(rect.DOScale(new Vector3(2f, 2f, 1f), duration))
            .OnComplete(() => completion.TrySetResult());
   
        await completion.Task;
    }
}
