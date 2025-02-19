using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveData
{
    private const string CURRENT_CHAPTER = "Current_Chapter";

    public static int CurrentChapter
    {
        get => PlayerPrefs.GetInt(CURRENT_CHAPTER, 1);
        set 
        { 
            PlayerPrefs.SetInt(CURRENT_CHAPTER, value);
            PlayerPrefs.Save();
        }
    }
}
