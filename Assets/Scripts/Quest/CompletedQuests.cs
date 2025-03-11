using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CompletedQuests
{
    public List<int> ids;

    public CompletedQuests()
    {
        ids = new List<int>();
    }
    
    public CompletedQuests(List<int> ids)
    {
        this.ids = ids;
    }
}
