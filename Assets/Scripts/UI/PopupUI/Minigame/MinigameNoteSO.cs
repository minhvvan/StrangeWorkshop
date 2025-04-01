using UnityEngine;

[CreateAssetMenu(fileName = "MinigameNote", menuName = "SO/Minigame/Notes", order = 1)]
public class MinigameNoteSO : ScriptableObject
{
    public string note;
    public Sprite sprite;


    [Header("MeterTiming")]
    public float interactionRange = 0.05f;

    //미사용
    [Header("CommandRush")]
    public KeyCode mappingKeyCode; 
}