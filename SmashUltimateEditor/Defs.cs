﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashUltimateEditor
{
    public static class Defs
    {
        public const string SPIRIT_BATTLE_DATA_XML = "battle_data_tbl";
        public const string FIGHTER_DATA_XML = "fighter_data_tbl";

        #region FighterDataTbl
        public static ushort APPEAR_RULE_TIME_MIN = 0;
        public static ushort APPEAR_RULE_TIME_MAX = 60;

        // Technically, 7 doesn't ever get used (?)
        public static ushort APPEAR_RULE_COUNT_MIN = 0;
        public static ushort APPEAR_RULE_COUNT_MAX = 8;

        public static byte CPU_LV_MIN = 1;
        public static byte CPU_LV_MAX = 100;

        public static byte STOCK_MAX = 99;
        public static byte STOCK_MIN = 1;
        
        // 0, 500
        public static ushort HP_MIN = 0;
        public static ushort HP_MAX = 999;

        // 0, 300
        public static ushort INIT_DAMAGE_MIN = 0;
        public static ushort INIT_DAMAGE_MAX = 999;

        public static float SCALE_MAX = 2.75f;
        public static float SCALE_MIN = 0.3f;

        public static float FLY_RATE_MIN = 0f;
        public static float FLY_RATE_MAX = 5f;

        public static short ATTACK_MIN = 0;
        public static short ATTACK_MAX = 10000;

        public static short DEFENSE_MIN = 0;
        public static short DEFENSE_MAX = 10000;
        #endregion

        #region BattleDataTbl
        public static ushort BATTLE_TIME_SEC_MIN = 0;
        public static ushort BATTLE_TIME_SEC_MAX = 180;

        public static ushort BASIC_INIT_DAMAGE_MIN = 0;
        public static ushort BASIC_INIT_DAMAGE_MAX = 300;

        public static ushort BASIC_INIT_HP_MIN = 0;
        public static ushort BASIC_INIT_HP_MAX = 150;

        public static byte BASIC_STOCK_MIN = 1;
        public static byte BASIC_STOCK_MAX = 99;

        public static string event1_type;
        public static string event1_label;

        public static int EVENT_START_TIME_MIN = 0;
        public static int EVENT_START_TIME_MAX = 80;

        public static int EVENT_RANGE_TIME_MIN = -1;
        public static int EVENT_RANGE_TIME_MAX = 120;

        public static byte EVENT_COUNT_MIN = 0;
        public static byte EVENT_COUNT_MAX = 200;

        public static ushort EVENT_DAMAGE_MIN = 0;
        public static ushort EVENT_DAMAGE_MAX = 150;
        
        public static uint BATTLE_POWER_MIN = 0;
        public static uint BATTLE_POWER_MAX = 20000;
        #endregion
    }
}