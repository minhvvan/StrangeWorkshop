using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class SequenceCommandSO : ScriptableObject
{
    public float startDelay;
    public float completionDelay;
    public bool IsInitialized { get; protected set; }

    //초기화
    public abstract UniTaskVoid Initialize();
    
    // 커맨드 실행
    public abstract void Execute(CommandCompleteEventSO completeEventSo);
}
