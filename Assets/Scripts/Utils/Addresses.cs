using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Addresses
{
    public static class Events 
    {
        public static class Game
        {
            public const string STATE_CHANGED = "events/game/state_changed";
            public const string COMMAND_COMPLETE = "events/game/command_complete";
            public const string ENEMY_SPAWN = "events/game/enemy_spawn";
            public const string BOSS_END = "events/game/boss_end";
            public const string WAVE_START = "events/game/wave_start";
            public const string WAVE_CLEAR = "events/game/wave_clear";
            public const string GAME_START = "events/game/game_start";
            public const string GAME_OVER = "events/game/game_over";
            public const string GAME_CLEAR = "events/game/game_clear";
        }
        
        public static class Counter
        {
            public const string CRAFT_COMPLETE = "events/counter/craft_complete";
            public const string PROCESS_COMPLETE = "events/counter/process_complete";
        }
        
        public static class Barrier
        {
            public const string BARRIER_DAMAGED = "events/barrier/damaged";
            public const string BARRIER_DESTROYED = "events/barrier/destroyed";
        }
    }
    
    public static class Data
    {
        public static class Player
        {
            public const string STATS = "data/player/stats";
        }
        
        public static class Chapter
        {
            public const string CHAPTER_LIST = "data/chapters/chapter_list";
            public const string CHAPTER_1 = "data/chapters/chapter_1";
            public const string CHAPTER_2 = "data/chapters/chapter_2";
            public const string CHAPTER_3 = "data/chapters/chapter_3";
        }
        
        public static class Wave
        {
            public const string WAVE_1 = "data/waves/wave_1";
            public const string WAVE_2 = "data/waves/wave_2";
            // ...
        }
        
        public static class Enemy
        {
            public const string BASIC = "data/enemies/basic";
            public const string FAST = "data/enemies/fast";
            public const string TANK = "data/enemies/tank";
        }
        
        public static class Turret
        {
            public const string BASIC = "data/turrets/basic";
            public const string MISSILE = "data/turrets/missile";
            public const string LASER = "data/turrets/laser";
        }

        public static class Recipe
        {
            public const string CRAFT = "data/recipes/Craft";
            public const string PROCESS = "data/recipes/Process";
        }

        public static class Barrier
        {
            public const string STAT = "data/barrier/stat";
        }

        public static class UI
        {
            public const string COMMON = "data/ui/common";
            public const string CHAPTER_RECIPE = "data/ui/chapter_recipe";
        }

        public static class FX
        {
            public const string VFXLOADER = "data/fx/vfxloader";
            public const string SFXLOADER = "data/fx/sfxloader";
        }

        public static class Quest
        {
            public const string QUESTLIST = "data/quest/questlist";
        }
        
        public static class Drops
        {
            public const string DROPITEMS = "data/drops/dropitems";
        }

        public static class Kits
        {
            public const string REPAIR = "data/kits/repairkit";
        }
    }
    
    public static class Prefabs
    {
        public static class Counters
        {
            public const string CLEAR = "prefabs/counters/clear_counter";
            public const string CRAFT = "prefabs/counters/craft_counter";
            public const string PROCESS = "prefabs/counters/process_counter";
            public const string MATERIAL = "prefabs/counters/material_counter";
        }
        
        public static class Game
        {
            public const string BASIC = "prefabs/turrets/basic_turret";
            public const string MISSILE = "prefabs/turrets/missile_turret";
            public const string LASER = "prefabs/turrets/laser_turret";
            public const string LOOTBOT = "prefabs/turrets/loot_bot";
        }
        
        public static class Effects
        {
            public const string EXPLOSION = "prefabs/effects/explosion";
            public const string SPARK = "prefabs/effects/spark";
        }
        
        public static class Enemy
        {
            public const string THROW_SWORD = "prefabs/enemy/throw_sword";
            public const string SPELL_FIELD = "prefabs/enemy/spell_field";
        }

        public static class Turret
        {
            public const string TURRET_GUN = "prefabs/turret/turret_gun";
            public const string TURRET_MISSILE = "prefabs/turret/turret_missile";
            public const string TURRET_MORTAR = "prefabs/turret/turret_mortar";
            public const string TURRET_MISSILEDOUBLE = "prefabs/turret/turret_missiledouble";
            
        }
    }
}