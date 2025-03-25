using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class InGameDataManager : SingletonDontDestroy<InGameDataManager>
{
    InGameDataController inGameDataController;

    public void InitializeChapter()
    {
        inGameDataController = new InGameDataController();
        inGameDataController.StartChapter();
    }

    public void ModifyInGameData(InGameDataType dataType, object value, [CanBeNull] object subType = null)
    {
        inGameDataController.ModifyInGameData(dataType, value, subType);
    }

    public void PauseGame()
    {
        inGameDataController.PauseGame();
    }

    public void ResumeGame()
    {
        inGameDataController.ResumeGame();
    }

    public void SaveInGameData()
    {
        
    }
}
