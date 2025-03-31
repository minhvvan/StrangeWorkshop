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

    public void ModifyInGameData(InGameDataType dataType, object value, object subType = null)
    {
        inGameDataController.ModifyInGameData(dataType, value, subType);
    }

    public object GetInGameData(InGameDataType dataType)
    {
        return inGameDataController.GetInGameData(dataType);
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

    public bool Purchasable(int gold)
    {
        return inGameDataController.Purchasable(gold);
    }

    public bool UseGold(int gold)
    {
        return inGameDataController.UseGold(gold);
    }

    public void EarnGold(int gold)
    {
        inGameDataController.EarnGold(gold);
    }
}
