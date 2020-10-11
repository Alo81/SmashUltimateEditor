﻿using SmashUltimateEditor.DataTables;
using SmashUltimateEditor.Helpers;
using SmashUltimateEditor.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static SmashUltimateEditor.Enums;
using static SmashUltimateEditor.Extensions;

namespace SmashUltimateEditor
{

    public class Fighter : DataTbl
    {
        internal static string XML_NAME = "fighter_data_tbl";

        public void Cleanup(ref Random rnd, bool isMain, bool isLoseEscort, List<string> fighters, bool isBoss = false, string unlockableFighter = null)
        {
            // Post Randomize fighter modifiers
            var realBoss = BossTypeCheck(isBoss, ref rnd);
            EntryCheck(isMain, isLoseEscort, realBoss);
            HealthCheck(isLoseEscort, ref rnd);
            FighterCheck(fighters, realBoss, unlockableFighter, ref rnd);
            BossCheck(isBoss&(!realBoss));
        }

        public void FighterCheck(List<string> options, bool realBoss, string unlockableFighter, ref Random rnd)
        {
            // If unlockable fighter, set as that.  If real boss, get a boss.  Otherwise, get a regular character we'll bossify.  
            fighter_kind = unlockableFighter 
                ?? 
                (realBoss? Defs.BOSSES[rnd.Next(Defs.BOSSES.Count)] : options[rnd.Next(options.Count)]);
        }

        public void StockCheck(int fighterCount)
        {
            // If there are a lot, makes multiple lives tedius
            if (fighterCount > Defs.FIGHTER_COUNT_STOCK_CUTOFF)
            {
                stock = 1;
            }
        }

        public void HealthCheck(bool isLoseEscort, ref Random rnd)
        {
            // Check if init HP is lower than init damage (?)
            if (hp < init_damage)
            {
                var hold = init_damage;
                init_damage = hp;
                hp = hold;
            }

            if(isLoseEscort && (hp - init_damage) < Defs.ALLY_LOW_HP_CUTOFF)
            {
                // Losing from low health escorts sucks.  Either add HP, or add Stocks. 
                if(RandomizerHelper.ChancePass(50, ref rnd))
                {
                    hp += Defs.ALLY_LOW_HP_MOD;
                }
                else
                {
                    stock += Defs.ALLY_LOW_STOCK_MOD;
                }
            }
        }

        public void EntryCheck(bool isMain, bool isLoseEscort, bool realBoss)
        {
            if (isLoseEscort)
                entry_type = "friend_type";
            else if (realBoss)
            {
                entry_type = "boss_type";
                first_appear = true;
            }
            // One of our fake bosses.  
            else if (isMain || entry_type == "boss_type")
            {
                entry_type = "main_type";
                first_appear = true;
            }
        }
        public void FirstAppearCheck(bool isMain)
        {
            if (isMain)
                first_appear = true;
        }

        public bool BossTypeCheck(bool isBoss, ref Random rnd)
        {
            if (isBoss)
            {
                return RandomizerHelper.ChancePass(Defs.BOSS_CHECK, ref rnd);
            }
            else
                return false;
        }

        public void BossCheck(bool isBoss)
        {
            if (isBoss)
            {
                scale = scale > 1 ? scale * Defs.BOSS_SCALE_MOD : scale + Defs.BOSS_SCALE_MOD;
                attack = (short)(attack * Defs.BOSS_ATTACK_MOD);
                defense = (short)(defense * Defs.BOSS_DEFENSE_MOD);
                hp = hp < Defs.BOSS_HP_CUTOFF ? (ushort)(hp * Defs.BOSS_LOW_HP_MOD) : (ushort)(hp + Defs.BOSS_HIGH_HP_MOD);
                cpu_lv = (byte)(cpu_lv + Defs.BOSS_CPU_LVL_ADD > Defs.CPU_LV_MAX ? Defs.CPU_LV_MAX : cpu_lv + Defs.BOSS_CPU_LVL_ADD);
            }
        }

        public static void SetMiiFighterSpecials(ref TabPage subPage, Enums.mii_brawler_mod mod)
        {
            foreach(var combo in subPage.Controls.OfType<ComboBox>())
            {
                if (Defs.MII_MOVES.Contains(combo.Name))
                {
                    List<string> labels = new List<string>();
                    for(int i = 0; i < 4; i++)
                    {
                        labels.Add(EnumUtil<Enums.mii_sp_hi_opt>.GetByValue(i * (int)mod));
                    }
                    // Save value off.  Change datasource.  Set the value.  
                    var value = combo.Text;
                    combo.DataSource = labels;
                    if (combo.Items.Contains(value ?? ""))
                    {
                        combo.SelectedItem = value;
                    }
                    return;
                }
            }
        }

        public Fighter Copy()
        {
            Fighter newCopy = new Fighter();
            foreach(PropertyInfo property in GetType().GetProperties())
            {
                newCopy.SetValueFromName(property.Name, this.GetPropertyValueFromName(property.Name));
            }

            return newCopy;
        }
        public static TabPage BuildEmptyPage(DataTbls dataTbls)
        {
            Type type = typeof(Fighter);
            TabPage topLevelPage = UiHelper.GetEmptyTabPage(dataTbls.pageCount);
            topLevelPage.Name = Top_Level_Page.Fighters.ToString();

            List<Point> points = new List<Point>();
            TabControl subControl = new TabControl();

            subControl.Anchor = (((((AnchorStyles.Top | AnchorStyles.Bottom)
            | AnchorStyles.Left)
            | AnchorStyles.Right)));

            topLevelPage.Controls.Add(subControl);

            TabPage page;
            Point currentPos;
            LabelBox lb;

            for (int i = 0; i < Enum.GetNames(typeof(Fighter_Page)).Length; i++)
            {
                subControl.TabPages.Add(UiHelper.GetEmptyTabPage(i));
                subControl.TabPages[i].Name = subControl.TabPages[i].Text = ((Fighter_Page)i).ToString();
                points.Add(new Point(0, 0));
            }

            // First pass, add button to first page.  
            page = subControl.TabPages[0];
            currentPos = points[0];

            Button b = UiHelper.GetEmptyRemoveFighterButton(UiHelper.IncrementPoint(ref currentPos, page.Controls.Count));

            /* We need to set the button name for real though.  */
            dataTbls.SetRemoveFighterButtonMethod(ref b);
            page.Controls.Add(b);
            Label spacer = new Label() { Location = UiHelper.IncrementPoint(ref currentPos, page.Controls.Count) };
            page.Controls.Add(spacer);

            points[0] = currentPos;

            // GetBattleIndex
            foreach (PropertyInfo field in type.GetProperties().OrderBy(x => x.Name))
            {
                lb = new LabelBox();
                var pageNum = field.GetCustomAttributes(true).OfType<PageAttribute>().First().Page;
                page = subControl.TabPages[pageNum];
                currentPos = points[pageNum];

                // Range values?  Use a textbox.
                if (Defs.RANGE_VALUES.Contains(field.Name.ToUpper()))
                {
                    lb.SetLabel(field.Name, UiHelper.IncrementPoint(ref currentPos, page.Controls.Count));
                    lb.SetTextBox(field.Name, UiHelper.IncrementPoint(ref currentPos, page.Controls.Count + 1));
                }
                //Else - use a combo box with preset list.  
                else
                {
                    lb.SetLabel(field.Name, UiHelper.IncrementPoint(ref currentPos, page.Controls.Count));
                    lb.SetComboBox(field.Name, dataTbls.GetOptionsFromTypeAndName(type, field.Name), UiHelper.IncrementPoint(ref currentPos, page.Controls.Count + 1));
                }

                page.Controls.Add(lb.label);
                if (lb.IsComboSet())
                {
                    page.Controls.Add(lb.combo);
                }
                else if (lb.IsTextboxSet())
                {
                    page.Controls.Add(lb.text);
                }

                points[pageNum] = currentPos;
            }

            return topLevelPage;
        }

        [Order][Page((int)Enums.Fighter_Page.Basics)]
        public string battle_id { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Basics)]
        public string entry_type { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Basics)]
        public bool first_appear { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Basics)]
        public ushort appear_rule_time { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Basics)]
        public ushort appear_rule_count { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public string fighter_kind { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Basics)]
        public byte color { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Mii)]
        public string mii_hat_id { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Mii)]
        public string mii_body_id { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Mii)]
        public byte mii_color { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Mii)]
        public string mii_voice { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Mii)]
        public byte mii_sp_n { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Mii)]
        public byte mii_sp_s { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Mii)]
        public byte mii_sp_hi { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Mii)]
        public byte mii_sp_lw { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public byte cpu_lv { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public string cpu_type { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public string cpu_sub_type { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public bool cpu_item_pick_up { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public byte stock { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Basics)]
        public bool corps { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Basics)]
        public bool _0x0f2077926c { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public ushort hp { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public ushort init_damage { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Basics)]
        public string sub_rule { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public float scale { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public float fly_rate { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Basics)]
        public bool invalid_drop { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Basics)]
        public bool enable_charge_final { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Basics)]
        public string spirit_name { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public short attack { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public short defense { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public string attr { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public string ability1 { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public string ability2 { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public string ability3 { get; set; }
        [Order][Page((int)Enums.Fighter_Page.Attributes)]
        public string ability_personal { get; set; }
    }
}
