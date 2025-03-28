using UnityEngine;

//카테고리별 분류용
public enum ItemType
{
    NONE,
    GOLD,
    MODULE
}

//새 아이템 추가하기 전, 이곳에 이름을 넣어주세요.
public enum ItemName
{
    NONE,
    GOLD_PENNY,
    GOLD_PACK,
    MODULE_NORMAL,
    MOUDLE_COMMON,
    MOUDLE_RARE
}

[CreateAssetMenu(fileName = "DropItem", menuName = "SO/Drops/DropItem")]
public class DropItemSO : ScriptableObject
{
    [Header("ItemInfo")]
    public ItemType itemType;
    public ItemName itemName;
    public float itemValue;
    public string description;
    public GameObject item;
    public Vector3 size;
    public float duration;
    public int poolSize;
}