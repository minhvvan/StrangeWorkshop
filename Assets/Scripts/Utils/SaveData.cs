using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveData
{
    private const string LAST_UNLOCKED_CHAPTER = "Last_Unlocked_Chapter";

    public static int LastUnlockedChapter
    {
        get => PlayerPrefs.GetInt(LAST_UNLOCKED_CHAPTER, 0);
        set 
        { 
            PlayerPrefs.SetInt(LAST_UNLOCKED_CHAPTER, value);
            PlayerPrefs.Save();
        }
    }
}