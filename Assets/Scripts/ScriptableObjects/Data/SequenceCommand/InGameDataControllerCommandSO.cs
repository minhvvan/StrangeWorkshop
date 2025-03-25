using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InGameDataControllerCommandSO", menuName = "SO/Chapter/InGameDataController")]
public class InGameDataControllerCommandSO : SequenceCommandSO
{
    public override void Initialize()
    {
    }

    public override void Execute(CommandCompleteEventSO completeEventSo)
    {
        InGameDataManager.Instance.InitializeChapter();
        completeEventSo.Raise();
    }
}
