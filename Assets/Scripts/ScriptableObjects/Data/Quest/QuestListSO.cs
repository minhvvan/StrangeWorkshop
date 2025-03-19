using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestList", menuName = "SO/Quest/QuestList")]
public class QuestListSO : ScriptableObject
{
    public List<QuestDataSO> questDataSOs;
}
