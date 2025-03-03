using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SwitchLanguage
{
    private static readonly Dictionary<string, string> translations = new Dictionary<string, string>()
    {
        { "Gear", "기어" },
        { "Bolt", "볼트" },
        { "Carbon", "카본" },
        { "Muffler", "머플러" },
        { "Ore", "광석" },
        { "Crystal", "크리스탈" },
        { "Bullet", "총알" },
        { "Engine", "엔진" },
        { "Trash", "쓰레기" },
        { "GunTurret", "기본터렛" },
        { "MissileTurret", "미사일\n터렛" },
        { "DoubleMissileTurret", "더블미사일\n터렛" },
        { "MortarTurret", "박격포\n터렛" },
        { "Turret", "터렛" },
        { "PartMaterial", "부품" },
        { "Upgrade", "강화" },
    };

    public static string Translate(string key)
    {
        if (translations.TryGetValue(key, out string translation))
        {
            return translation;
        }
        return key;
    }
}
