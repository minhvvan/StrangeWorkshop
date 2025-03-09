using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public int questID;
    public QuestType questType;
    public string description;
    
    public IQuestObjective questObjective;

    public Quest(QuestDataSO questDataSO)
    {
        this.questID = questDataSO.id;
        this.questType = questDataSO.questType;
        this.description = questDataSO.description;
        
        
        if (questDataSO.questType == QuestType.ProtectBarrier)
        {
            // questObjective = 
        }
    }
}
