using System;
using System.Threading;

[Serializable]
public struct ItemInfo
{
    public ItemType itemType;
    public ItemName itemName;
    public float itemValue;
    public CancellationTokenSource Cts;
}