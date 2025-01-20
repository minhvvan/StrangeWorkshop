using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryData", menuName = "SO/Story/StoryData")]
public class StoryDataSO : ScriptableObject
{
    [Header("Story Info")]
    public string storyId;
    [TextArea(3, 5)]
    public string description;
}