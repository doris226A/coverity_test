using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaLib;
using SagaDB.Item;
using SagaDB.Actor;

namespace SagaMap.PC
{
    public partial class StatusFactory : Singleton<StatusFactory>
    {
        public StatusFactory()
        {

        }
        public void CalcStatus(ActorPC pc)
        {
            //bool blocked = ClientManager.Blocked;
            //if (!blocked)
            //    ClientManager.EnterCriticalArea();
            CalcEquipBonus(pc);
            CalcAnotherPaperBonus(pc);
            //CalcPlayerTitleBouns(pc);
            if (pc.TamaireRental != null && pc.TamaireRental.CurrentLender != 0)//ECOKEY 心PE
            {
                CalcTamaireBonus(pc);
                if (pc.Mode != PlayerMode.NORMAL)//ECOKEY 心PE 騎士團限制
                {
                    pc.Status.aspd_tamaire = 0;
                    pc.Status.cspd_tamaire = 0;
                    pc.Status.hp_tamaire = 0;
                    pc.Status.mp_tamaire = 0;
                    pc.Status.sp_tamaire = 0;
                }
            }
            CalcRange(pc);
            CalcStatsRev(pc);
            CalcPayV(pc);
            CalcHPMPSP(pc);
            CalcStats(pc);
            pc.Inventory.CalcPayloadVolume();
            //if (blocked)
            //    ClientManager.LeaveCriticalArea();    
        }

        public void CalcStatusOnSkillEffect(ActorPC pc)
        {
            CalcHPMPSP(pc);
            CalcStats(pc);
        }

        public void CalcRange(ActorPC pc)
        {
            Dictionary<EnumEquipSlot, Item> equips;

            if (pc.Form == DEM_FORM.NORMAL_FORM)
                equips = pc.Inventory.Equipments;
            else
                equips = pc.Inventory.Parts;

            if (equips.ContainsKey(EnumEquipSlot.RIGHT_HAND))
            {
                Item item = equips[EnumEquipSlot.RIGHT_HAND];
                //ECOKEY 射程延長
                uint level = 0;
                if (pc.Status.Additions.ContainsKey("OverRange"))
                {
                    //level = (pc.Status.Additions["OverRange"] as SagaMap.Skill.Additions.Global.DefaultBuff).Variable["OverRange_skill"];
                    level = (uint)pc.TInt["OverRangeSkill"];
                }
                pc.Range = item.BaseData.range + level;

            }
            else
            {
                if (equips.ContainsKey(EnumEquipSlot.LEFT_HAND))
                {
                    Item item = equips[EnumEquipSlot.LEFT_HAND];
                    pc.Range = item.BaseData.range;
                }
                else
                    pc.Range = 1;
            }

        }
        /* public void CalcRange(ActorPC pc)
         {
             Dictionary<EnumEquipSlot, Item> equips;

             if (pc.Form == DEM_FORM.NORMAL_FORM)
                 equips = pc.Inventory.Equipments;
             else
                 equips = pc.Inventory.Parts;

             if (equips.ContainsKey(EnumEquipSlot.RIGHT_HAND))
             {
                 Item item = equips[EnumEquipSlot.RIGHT_HAND];
                 pc.Range = item.BaseData.range;
             }
             else
             {
                 if (equips.ContainsKey(EnumEquipSlot.LEFT_HAND))
                 {
                     Item item = equips[EnumEquipSlot.LEFT_HAND];
                     pc.Range = item.BaseData.range;
                 }
                 else
                     pc.Range = 1;
             }
         }*/

        ushort checkPositive(double num)
        {
            if (num > 0)
                return (ushort)num;
            return 0;
        }
        ushort checkHighVitBonus(ActorPC pc)
        {
            int vitcount = pc.Vit + pc.Status.vit_item + pc.Status.vit_chip + pc.Status.vit_rev + pc.Status.vit_mario + pc.Status.vit_skill + pc.Status.vit_iris;
            if (vitcount >= 120 && vitcount < 150)
                return 2;
            else if (vitcount >= 150)
                return 5;
            else
                return 0;
        }

        ushort checkHighIntBonus(ActorPC pc)
        {
            int intcount = pc.Int + pc.Status.int_item + pc.Status.int_chip + pc.Status.int_rev + pc.Status.int_mario + pc.Status.int_skill + pc.Status.int_iris;
            if (intcount >= 120 && intcount < 150)
                return 2;
            else if (intcount >= 150)
                return 5;
            else
                return 0;
        }

        /// <summary>
        /// 计算素质属性能力
        /// </summary>
        /// <param name="pc"></param>
        private void CalcStats(ActorPC pc)
        {
            //获取玩家基础能力
            ushort pcstr = (ushort)checkPositive((pc.Str + pc.Status.str_item + pc.Status.str_chip + pc.Status.str_rev + pc.Status.str_mario + pc.Status.str_skill + pc.Status.str_iris + pc.Status.str_another));
            ushort pcdex = (ushort)checkPositive((pc.Dex + pc.Status.dex_item + pc.Status.dex_chip + pc.Status.dex_rev + pc.Status.dex_mario + pc.Status.dex_skill + pc.Status.dex_iris + pc.Status.dex_another));
            ushort pcint = (ushort)checkPositive((pc.Int + pc.Status.int_item + pc.Status.int_chip + pc.Status.int_rev + pc.Status.int_mario + pc.Status.int_skill + pc.Status.int_iris + pc.Status.int_another));
            ushort pcvit = (ushort)checkPositive((pc.Vit + pc.Status.vit_item + pc.Status.vit_chip + pc.Status.vit_rev + pc.Status.vit_mario + pc.Status.vit_skill + pc.Status.vit_iris + pc.Status.vit_another));
            ushort pcagi = (ushort)checkPositive((pc.Agi + pc.Status.agi_item + pc.Status.agi_chip + pc.Status.agi_rev + pc.Status.agi_mario + pc.Status.agi_skill + pc.Status.agi_iris + pc.Status.agi_another));
            ushort pcmag = (ushort)checkPositive((pc.Mag + pc.Status.mag_item + pc.Status.mag_chip + pc.Status.mag_rev + pc.Status.mag_mario + pc.Status.mag_skill + pc.Status.mag_iris + pc.Status.mag_another));

            if (pc.Status.Additions.ContainsKey("ModeChange"))
            {
                #region 物魔互换模块(基础属性和buff装备分离部分)
                pcstr = (ushort)checkPositive((pc.Str + pc.Status.mag_item + pc.Status.mag_chip + pc.Status.str_rev + pc.Status.mag_mario + pc.Status.mag_skill + pc.Status.mag_iris + pc.Status.mag_another));
                pcdex = (ushort)checkPositive((pc.Dex + pc.Status.agi_item + pc.Status.agi_chip + pc.Status.dex_rev + pc.Status.agi_mario + pc.Status.agi_skill + pc.Status.agi_iris + pc.Status.agi_another));
                pcint = (ushort)checkPositive((pc.Int + pc.Status.int_item + pc.Status.int_chip + pc.Status.int_rev + pc.Status.int_mario + pc.Status.int_skill + pc.Status.int_iris + pc.Status.int_another));
                pcvit = (ushort)checkPositive((pc.Vit + pc.Status.vit_item + pc.Status.vit_chip + pc.Status.vit_rev + pc.Status.vit_mario + pc.Status.vit_skill + pc.Status.vit_iris + pc.Status.vit_another));
                pcagi = (ushort)checkPositive((pc.Agi + pc.Status.dex_item + pc.Status.dex_chip + pc.Status.agi_rev + pc.Status.dex_mario + pc.Status.dex_skill + pc.Status.dex_iris + pc.Status.dex_another));
                pcmag = (ushort)checkPositive((pc.Mag + pc.Status.str_item + pc.Status.str_chip + pc.Status.mag_rev + pc.Status.str_mario + pc.Status.str_skill + pc.Status.str_iris + pc.Status.str_another));
                #endregion
            }

            if (pc.Pet != null && pc.Pet.Ride)
            {
                //ECOKEY 坐騎技能加成
                ushort minatk = 0;
                ushort maxatk = 0;
                ushort minmatk = 0;
                ushort maxmatk = 0;
                ushort def = 0;
                ushort mdef = 0;
                ushort hit_melee = 0;
                ushort hit_ranged = 0;
                ushort avoid_melee = 0;
                ushort avoid_ranged = 0;
                ushort hit_critical = 0;
                ushort avoid_critical = 0;
                if (pc.Status.str_skill != 0)
                {
                    minatk = (ushort)Math.Floor((double)pc.Status.str_skill + Math.Pow(Math.Floor((double)(pc.Status.str_skill / 9)), 2));
                    maxatk = (ushort)(pc.Status.str_skill + Math.Pow(Math.Floor((double)((float)(pc.Status.str_skill + 14) / 5.0f)), 2));
                }
                if (pc.Status.mag_skill != 0)
                {
                    minmatk = (ushort)Math.Floor((double)pc.Status.mag_skill + Math.Pow(Math.Floor((double)((float)(pc.Status.mag_skill + 9) / 8)), 2) * (1.0f + Math.Floor((double)(pc.Status.int_skill * 1.2f)) / 320.0f));
                    maxmatk = (ushort)(pc.Status.mag_skill + Math.Pow(Math.Floor((double)((float)(pc.Status.mag_skill + 17) / 6.0f)), 2));
                }
                if (pc.Status.vit_skill != 0)
                {
                    def = (ushort)Math.Min(checkPositive((int)(pc.Status.vit_skill / 3) + (int)((float)pc.Status.vit_skill / 4.5f)), Configuration.Instance.BasePhysicDef);
                    mdef = (ushort)Math.Min(checkPositive(((int)(pc.Status.int_skill / 3) + (int)((float)pc.Status.vit_skill / 4.0f))), Configuration.Instance.BaseMagicDef);
                }
                if (pc.Status.dex_skill != 0)
                {
                    hit_melee = (ushort)(pc.Status.dex_skill + (short)Math.Floor((double)((float)pc.Status.dex_skill / 10.0f)) * 11);
                    hit_ranged = (ushort)(pc.Status.int_skill + (short)Math.Floor((double)((float)pc.Status.int_skill / 10.0f)) * 11);
                    hit_critical = (ushort)(Math.Max(Math.Floor((double)((pc.Status.dex_skill + 1) / 8)), 1));
                }
                if (pc.Status.agi_skill != 0)
                {
                    avoid_melee = (ushort)(pc.Status.agi_skill + Math.Pow(Math.Floor((float)(pc.Status.agi_skill + 18) / 9.0f), 2));
                    avoid_ranged = (ushort)(Math.Floor((float)pc.Status.int_skill * 5.0f / 3.0f) + pc.Status.agi_skill);

                    avoid_critical = (ushort)((float)(pc.Status.agi_skill + 8) / 6.0f);
                }

                //ECOKEY 機器人
                if (pc.Partner != null)
                {
                    //ECOKEY 騎寵能力取代
                    pc.Status.min_atk1 = (ushort)checkPositive(pc.Partner.Status.min_atk1 + pc.Status.min_atk1_skill + minatk);
                    pc.Status.min_atk2 = (ushort)checkPositive(pc.Partner.Status.min_atk2 + pc.Status.min_atk2_skill);
                    pc.Status.min_atk3 = (ushort)checkPositive(pc.Partner.Status.min_atk3 + pc.Status.min_atk3_skill);
                    pc.Status.max_atk1 = (ushort)checkPositive(pc.Partner.Status.max_atk1 + pc.Status.max_atk1_skill + maxatk);
                    pc.Status.max_atk2 = (ushort)checkPositive(pc.Partner.Status.max_atk2 + pc.Status.max_atk2_skill);
                    pc.Status.max_atk3 = (ushort)checkPositive(pc.Partner.Status.max_atk3 + pc.Status.max_atk3_skill);
                    pc.Status.min_matk = (ushort)checkPositive(pc.Partner.Status.min_matk + pc.Status.min_matk_skill + minmatk);
                    pc.Status.max_matk = (ushort)checkPositive(pc.Partner.Status.max_matk + pc.Status.max_matk_skill + maxmatk);
                    pc.Status.def = (ushort)checkPositive(pc.Partner.Status.def + pc.Status.def_skill + def);
                    pc.Status.def_add = (short)checkPositive(pc.Partner.Status.def_add + pc.Status.def_add_skill);
                    pc.Status.mdef = (ushort)checkPositive(pc.Partner.Status.mdef + pc.Status.mdef_skill + mdef);
                    pc.Status.mdef_add = (short)checkPositive(pc.Partner.Status.mdef_add + pc.Status.mdef_add_skill);
                    //pc.Status.aspd = (short)checkPositive(pc.Partner.Status.aspd + pc.Status.aspd_skill);
                    //pc.Status.cspd = (short)checkPositive(pc.Partner.Status.cspd + pc.Status.cspd_skill);
                    pc.Status.hit_melee = (ushort)checkPositive(pc.Partner.Status.hit_melee + pc.Status.hit_melee_skill + hit_melee);
                    pc.Status.hit_ranged = (ushort)checkPositive(pc.Partner.Status.hit_ranged + pc.Status.hit_ranged_skill + hit_ranged);
                    pc.Status.avoid_melee = (ushort)checkPositive(pc.Partner.Status.avoid_melee + pc.Status.avoid_melee_skill + avoid_melee);
                    pc.Status.avoid_ranged = (ushort)checkPositive(pc.Partner.Status.avoid_ranged + pc.Status.avoid_ranged_skill + avoid_ranged);
                    pc.Status.hit_critical = (ushort)checkPositive(pc.Partner.Status.hit_critical + pc.Status.hit_critical_skill + hit_critical);
                    pc.Status.avoid_critical = (ushort)checkPositive(pc.Partner.Status.avoid_critical + pc.Status.avoid_critical_skill + avoid_critical);
                }
                else
                {
                    //ECOKEY 機器人
                    pc.Status.min_atk1 = (ushort)checkPositive(pc.Pet.Status.min_atk1 + pc.Status.min_atk1_skill + minatk);
                    pc.Status.min_atk2 = (ushort)checkPositive(pc.Pet.Status.min_atk2 + pc.Status.min_atk2_skill);
                    pc.Status.min_atk3 = (ushort)checkPositive(pc.Pet.Status.min_atk3 + pc.Status.min_atk3_skill);
                    pc.Status.max_atk1 = (ushort)checkPositive(pc.Pet.Status.max_atk1 + pc.Status.max_atk1_skill + maxatk);
                    pc.Status.max_atk2 = (ushort)checkPositive(pc.Pet.Status.max_atk2 + pc.Status.max_atk2_skill);
                    pc.Status.max_atk3 = (ushort)checkPositive(pc.Pet.Status.max_atk3 + pc.Status.max_atk3_skill);
                    pc.Status.min_matk = (ushort)checkPositive(pc.Pet.Status.min_matk + pc.Status.min_matk_skill + minmatk);
                    pc.Status.max_matk = (ushort)checkPositive(pc.Pet.Status.max_matk + pc.Status.max_matk_skill + maxmatk);
                    pc.Status.def = (ushort)checkPositive(pc.Pet.Status.def + pc.Status.def_skill + def);
                    pc.Status.def_add = (short)checkPositive(pc.Pet.Status.def_add + pc.Status.def_add_skill);
                    pc.Status.mdef = (ushort)checkPositive(pc.Pet.Status.mdef + pc.Status.mdef_skill + mdef);
                    pc.Status.mdef_add = (short)checkPositive(pc.Pet.Status.mdef_add + pc.Status.mdef_add_skill);
                    //pc.Status.aspd = (short)checkPositive(pc.Pet.Status.aspd + pc.Status.aspd_skill);
                    //pc.Status.cspd = (short)checkPositive(pc.Pet.Status.cspd + pc.Status.cspd_skill);
                    pc.Status.hit_melee = (ushort)checkPositive(pc.Pet.Status.hit_melee + pc.Status.hit_melee_skill + hit_melee);
                    pc.Status.hit_ranged = (ushort)checkPositive(pc.Pet.Status.hit_ranged + pc.Status.hit_ranged_skill + hit_ranged);
                    pc.Status.avoid_melee = (ushort)checkPositive(pc.Pet.Status.avoid_melee + pc.Status.avoid_melee_skill + avoid_melee);
                    pc.Status.avoid_ranged = (ushort)checkPositive(pc.Pet.Status.avoid_ranged + pc.Status.avoid_ranged_skill + avoid_ranged);
                    pc.Status.hit_critical = (ushort)checkPositive(pc.Pet.Status.hit_critical + pc.Status.hit_critical_skill + hit_critical);
                    pc.Status.avoid_critical = (ushort)checkPositive(pc.Pet.Status.avoid_critical + pc.Status.avoid_critical_skill + avoid_critical);

                    pc.Status.hp_recover = pc.Pet.Status.hp_recover;
                    pc.Status.mp_recover = pc.Pet.Status.mp_recover;
                }
            }
            //New
            //ECOKEY 強制GM固定能力值 GM銅
            else if (pc.Inventory.Equipments.ContainsKey(EnumEquipSlot.CHEST_ACCE) && pc.Inventory.Equipments[EnumEquipSlot.CHEST_ACCE].ItemID == 90000041)
            {
                pc.MaxHP = 100;
                pc.Status.min_atk1 = 30000;
                pc.Status.min_atk2 = 30000;
                pc.Status.min_atk3 = 30000;
                pc.Status.max_atk1 = 30000;
                pc.Status.max_atk2 = 30000;
                pc.Status.max_atk3 = 30000;
                pc.Status.min_matk = 30000;
                pc.Status.max_matk = 30000;
                pc.Status.def = 30000;
                pc.Status.def_add = 30000;
                pc.Status.mdef = 30000;
                pc.Status.mdef_add = 30000;
                pc.Status.aspd = 800;
                pc.Status.cspd = 800;
                pc.Status.hit_melee = 30000;
                pc.Status.hit_ranged = 30000;
                pc.Status.avoid_melee = 0;
                pc.Status.avoid_ranged = 0;
                pc.Status.hit_critical = 30000;
                pc.Status.avoid_critical = 0;
                pc.Speed = 600;

            }
            else
            {
                if (pc.Status.Additions.ContainsKey("ModeChange"))
                {
                    #region 物魔互换模块(攻击力计算部分)
                    //攻击力计算
                    ushort minmatk = (ushort)Math.Floor((double)pcstr + Math.Pow(Math.Floor((double)(pcstr / 9)), 2));
                    ushort minatk = (ushort)Math.Floor((double)pcmag + Math.Pow(Math.Floor((double)((float)(pcmag + 9) / 8)), 2) * (1.0f + Math.Floor((double)(pcint * 1.2f)) / 320.0f));
                    ushort maxmatk = (ushort)(pcstr + Math.Pow(Math.Floor((double)((float)(pcstr + 14) / 5.0f)), 2));
                    ushort maxatk = (ushort)(pcmag + Math.Pow(Math.Floor((double)((float)(pcmag + 17) / 6.0f)), 2));

                    minatk = (ushort)((float)minatk * CalcATKRate(pc));

                    int weapon_atk1_add = 0, weapon_atk2_add = 0, weapon_atk3_add = 0, weapon_matk_add = 0;
                    float rate = pc.Status.weapon_add_rate_iris;
                    weapon_atk1_add += (int)(pc.Status.atk1_item * (float)(rate / 100.0f));
                    weapon_atk2_add += (int)(pc.Status.atk2_item * (float)(rate / 100.0f));
                    weapon_atk3_add += (int)(pc.Status.atk3_item * (float)(rate / 100.0f));
                    weapon_matk_add += (int)(pc.Status.matk_item * (float)(rate / 100.0f));
                    //pc.Status.min_atk_bs = (ushort)checkPositive((float)((pc.Str + pc.Status.str_item + pc.Status.str_rev) * 2f + (pc.Dex + pc.Status.dex_item + pc.Status.dex_rev) * 1f));
                    pc.Status.min_atk_bs = minatk;

                    pc.Status.min_atk1 = (ushort)checkPositive((minatk + pc.Status.atk1_item + pc.Status.min_atk1_mario) * (float)(pc.Status.min_atk1_rate_iris / 100.0f) * (float)(pc.Status.min_atk1_rate_skill / 100.0f) + pc.Status.min_atk1_skill + pc.Status.min_atk1_iris + pc.Status.weapon_add_iris + weapon_atk1_add + pc.Status.min_atk1_another);
                    pc.Status.min_atk2 = (ushort)checkPositive((minatk + pc.Status.atk2_item + pc.Status.min_atk2_mario) * (float)(pc.Status.min_atk2_rate_iris / 100.0f) * (float)(pc.Status.min_atk2_rate_skill / 100.0f) + pc.Status.min_atk2_skill + pc.Status.min_atk2_iris + pc.Status.weapon_add_iris + weapon_atk2_add + pc.Status.min_atk2_another);
                    pc.Status.min_atk3 = (ushort)checkPositive((minatk + pc.Status.atk3_item + pc.Status.min_atk3_mario) * (float)(pc.Status.min_atk3_rate_iris / 100.0f) * (float)(pc.Status.min_atk3_rate_skill / 100.0f) + pc.Status.min_atk3_skill + pc.Status.min_atk3_iris + pc.Status.weapon_add_iris + weapon_atk3_add + pc.Status.min_atk3_another);


                    //pc.Status.max_atk_bs = (ushort)checkPositive((float)((pc.Str + pc.Status.str_item + pc.Status.str_rev) * 5f + (pc.Dex + pc.Status.dex_item + pc.Status.dex_rev) * 1f));
                    pc.Status.max_atk_bs = maxatk;

                    pc.Status.max_atk1 = (ushort)checkPositive((maxatk + pc.Status.atk1_item + pc.Status.max_atk1_mario) * (float)(pc.Status.max_atk1_rate_iris / 100.0f) * (float)(pc.Status.max_atk1_rate_skill / 100.0f) + pc.Status.max_atk1_skill + pc.Status.max_atk1_iris + pc.Status.weapon_add_iris + weapon_atk1_add + pc.Status.max_atk1_another);
                    pc.Status.max_atk2 = (ushort)checkPositive((maxatk + pc.Status.atk2_item + pc.Status.max_atk2_mario) * (float)(pc.Status.max_atk2_rate_iris / 100.0f) * (float)(pc.Status.max_atk2_rate_skill / 100.0f) + pc.Status.max_atk2_skill + pc.Status.max_atk2_iris + pc.Status.weapon_add_iris + weapon_atk2_add + pc.Status.max_atk2_another);
                    pc.Status.max_atk3 = (ushort)checkPositive((maxatk + pc.Status.atk3_item + pc.Status.max_atk3_mario) * (float)(pc.Status.max_atk3_rate_iris / 100.0f) * (float)(pc.Status.max_atk3_rate_skill / 100.0f) + pc.Status.max_atk3_skill + pc.Status.max_atk3_iris + pc.Status.weapon_add_iris + weapon_atk3_add + pc.Status.max_atk3_another);


                    //pc.Status.max_matk_bs = (ushort)checkPositive((float)((pc.Mag + pc.Status.mag_item + pc.Status.mag_rev) * 2f + (pc.Int + pc.Status.int_item + pc.Status.int_rev) * 1f));
                    pc.Status.min_matk_bs = minmatk;

                    pc.Status.min_matk = (ushort)checkPositive((minmatk + pc.Status.matk_item + pc.Status.min_matk_mario) * (float)(pc.Status.min_matk_rate_iris / 100.0f) * (float)(pc.Status.min_matk_rate_skill / 100.0f) + pc.Status.min_matk_skill + pc.Status.min_matk_iris + pc.Status.weapon_add_iris + weapon_matk_add + pc.Status.min_matk_another);




                    //pc.Status.max_matk_bs = (ushort)checkPositive((float)((pc.Mag + pc.Status.mag_item + pc.Status.mag_rev) * 5f + (pc.Int + pc.Status.int_item + pc.Status.int_rev) * 1f));
                    pc.Status.max_matk_bs = maxmatk;
                    pc.Status.max_matk = (ushort)checkPositive((maxmatk + pc.Status.matk_item + pc.Status.max_matk_mario) * (float)(pc.Status.min_matk_rate_iris / 100.0f) * (float)(pc.Status.min_matk_rate_skill / 100.0f) + pc.Status.max_matk_skill + pc.Status.max_matk_iris + pc.Status.weapon_add_iris + weapon_matk_add + pc.Status.max_matk_another);
                    #endregion
                }
                else
                {
                    //攻击力计算
                    ushort minatk = (ushort)Math.Floor((double)pcstr + Math.Pow(Math.Floor((double)(pcstr / 9)), 2));
                    minatk = (ushort)((float)minatk * CalcATKRate(pc));

                    int weapon_atk1_add = 0, weapon_atk2_add = 0, weapon_atk3_add = 0, weapon_matk_add = 0;
                    float rate = pc.Status.weapon_add_rate_iris;
                    weapon_atk1_add += (int)(pc.Status.atk1_item * (float)(rate / 100.0f));
                    weapon_atk2_add += (int)(pc.Status.atk2_item * (float)(rate / 100.0f));
                    weapon_atk3_add += (int)(pc.Status.atk3_item * (float)(rate / 100.0f));
                    weapon_matk_add += (int)(pc.Status.matk_item * (float)(rate / 100.0f));
                    //pc.Status.min_atk_bs = (ushort)checkPositive((float)((pc.Str + pc.Status.str_item + pc.Status.str_rev) * 2f + (pc.Dex + pc.Status.dex_item + pc.Status.dex_rev) * 1f));
                    pc.Status.min_atk_bs = minatk;

                    pc.Status.min_atk1 = (ushort)checkPositive((minatk + pc.Status.atk1_item + pc.Status.min_atk1_mario) * (float)(pc.Status.min_atk1_rate_iris / 100.0f) * (float)(pc.Status.min_atk1_rate_skill / 100.0f) + pc.Status.min_atk1_skill + pc.Status.min_atk1_iris + pc.Status.weapon_add_iris + weapon_atk1_add + pc.Status.min_atk1_another);
                    pc.Status.min_atk2 = (ushort)checkPositive((minatk + pc.Status.atk2_item + pc.Status.min_atk1_mario) * (float)(pc.Status.min_atk2_rate_iris / 100.0f) * (float)(pc.Status.min_atk2_rate_skill / 100.0f) + pc.Status.min_atk2_skill + pc.Status.min_atk2_iris + pc.Status.weapon_add_iris + weapon_atk2_add + pc.Status.min_atk2_another);
                    pc.Status.min_atk3 = (ushort)checkPositive((minatk + pc.Status.atk3_item + pc.Status.min_atk1_mario) * (float)(pc.Status.min_atk3_rate_iris / 100.0f) * (float)(pc.Status.min_atk3_rate_skill / 100.0f) + pc.Status.min_atk3_skill + pc.Status.min_atk3_iris + pc.Status.weapon_add_iris + weapon_atk3_add + pc.Status.min_atk3_another);
                    //pc.Status.min_atk2 = (ushort)checkPositive((minatk + pc.Status.atk2_item + pc.Status.min_atk2_mario + pc.Status.min_atk2_skill + pc.Status.min_atk2_iris + pc.Status.weapon_add_iris + weapon_atk2_add) * (float)(pc.Status.min_atk2_rate_iris / 100.0f) * (float)(pc.Status.min_atk2_rate_skill / 100.0f));
                    //pc.Status.min_atk3 = (ushort)checkPositive((minatk + pc.Status.atk3_item + pc.Status.min_atk3_mario + pc.Status.min_atk3_skill + pc.Status.min_atk3_iris + pc.Status.weapon_add_iris + weapon_atk3_add) * (float)(pc.Status.min_atk3_rate_iris / 100.0f) * (float)(pc.Status.min_atk3_rate_skill / 100.0f));

                    ushort maxatk = (ushort)(pcstr + Math.Pow(Math.Floor((double)((float)(pcstr + 14) / 5.0f)), 2));
                    //pc.Status.max_atk_bs = (ushort)checkPositive((float)((pc.Str + pc.Status.str_item + pc.Status.str_rev) * 5f + (pc.Dex + pc.Status.dex_item + pc.Status.dex_rev) * 1f));
                    pc.Status.max_atk_bs = maxatk;

                    pc.Status.max_atk1 = (ushort)checkPositive((maxatk + pc.Status.atk1_item + pc.Status.max_atk1_mario) * (float)(pc.Status.max_atk1_rate_iris / 100.0f) * (float)(pc.Status.max_atk1_rate_skill / 100.0f) + pc.Status.max_atk1_skill + pc.Status.max_atk1_iris + pc.Status.weapon_add_iris + weapon_atk1_add + pc.Status.max_atk1_another);
                    pc.Status.max_atk2 = (ushort)checkPositive((maxatk + pc.Status.atk2_item + pc.Status.max_atk1_mario) * (float)(pc.Status.max_atk2_rate_iris / 100.0f) * (float)(pc.Status.max_atk2_rate_skill / 100.0f) + pc.Status.max_atk2_skill + pc.Status.max_atk2_iris + pc.Status.weapon_add_iris + weapon_atk2_add + pc.Status.max_atk2_another);
                    pc.Status.max_atk3 = (ushort)checkPositive((maxatk + pc.Status.atk3_item + pc.Status.max_atk1_mario) * (float)(pc.Status.max_atk3_rate_iris / 100.0f) * (float)(pc.Status.max_atk3_rate_skill / 100.0f) + pc.Status.max_atk3_skill + pc.Status.max_atk3_iris + pc.Status.weapon_add_iris + weapon_atk3_add + pc.Status.max_atk3_another);
                    //pc.Status.max_atk1 = (ushort)checkPositive((maxatk + pc.Status.atk1_item + pc.Status.max_atk1_mario + pc.Status.max_atk1_skill + pc.Status.max_atk1_iris + pc.Status.weapon_add_iris + weapon_atk1_add) * (float)(pc.Status.max_atk1_rate_iris / 100.0f) * (float)(pc.Status.max_atk1_rate_skill / 100.0f));
                    //pc.Status.max_atk2 = (ushort)checkPositive((maxatk + pc.Status.atk2_item + pc.Status.max_atk2_mario + pc.Status.max_atk2_skill + pc.Status.max_atk2_iris + pc.Status.weapon_add_iris + weapon_atk2_add) * (float)(pc.Status.max_atk2_rate_iris / 100.0f) * (float)(pc.Status.max_atk2_rate_skill / 100.0f));
                    //pc.Status.max_atk3 = (ushort)checkPositive((maxatk + pc.Status.atk3_item + pc.Status.max_atk3_mario + pc.Status.max_atk3_skill + pc.Status.max_atk3_iris + pc.Status.weapon_add_iris + weapon_atk3_add) * (float)(pc.Status.max_atk3_rate_iris / 100.0f) * (float)(pc.Status.max_atk3_rate_skill / 100.0f));

                    ushort minmatk = (ushort)Math.Floor((double)pcmag + Math.Pow(Math.Floor((double)((float)(pcmag + 9) / 8)), 2) * (1.0f + Math.Floor((double)(pcint * 1.2f)) / 320.0f));

                    //pc.Status.max_matk_bs = (ushort)checkPositive((float)((pc.Mag + pc.Status.mag_item + pc.Status.mag_rev) * 2f + (pc.Int + pc.Status.int_item + pc.Status.int_rev) * 1f));
                    pc.Status.min_matk_bs = minmatk;

                    pc.Status.min_matk = (ushort)checkPositive((minmatk + pc.Status.matk_item + pc.Status.min_matk_mario) * (float)(pc.Status.min_matk_rate_iris / 100.0f) * (float)(pc.Status.min_matk_rate_skill / 100.0f) + pc.Status.min_matk_skill + pc.Status.min_matk_iris + pc.Status.weapon_add_iris + weapon_matk_add + pc.Status.min_matk_another);

                    ushort maxmatk = (ushort)(pcmag + Math.Pow(Math.Floor((double)((float)(pcmag + 17) / 6.0f)), 2));

                    //pc.Status.max_matk_bs = (ushort)checkPositive((float)((pc.Mag + pc.Status.mag_item + pc.Status.mag_rev) * 5f + (pc.Int + pc.Status.int_item + pc.Status.int_rev) * 1f));
                    pc.Status.max_matk_bs = maxmatk;

                    pc.Status.max_matk = (ushort)checkPositive((maxmatk + pc.Status.matk_item + pc.Status.max_matk_mario) * (float)(pc.Status.min_matk_rate_iris / 100.0f) * (float)(pc.Status.min_matk_rate_skill / 100.0f) + pc.Status.max_matk_skill + pc.Status.max_matk_iris + pc.Status.weapon_add_iris + weapon_matk_add + pc.Status.max_matk_another);
                }
                #region 最小攻击力大于最大攻击力的修正部分
                if (pc.Status.min_atk1 > pc.Status.max_atk1)
                    pc.Status.min_atk1 = pc.Status.max_atk1;
                if (pc.Status.min_atk2 > pc.Status.max_atk2)
                    pc.Status.min_atk2 = pc.Status.max_atk2;
                if (pc.Status.min_atk3 > pc.Status.max_atk3)
                    pc.Status.min_atk3 = pc.Status.max_atk3;
                if (pc.Status.min_matk > pc.Status.max_matk)
                    pc.Status.min_matk = pc.Status.max_matk;
                #endregion
                //命中计算
                ushort hit_melee = (ushort)(pcdex + (short)Math.Floor((double)((float)pcdex / 10.0f)) * 11 + pc.Level + 3);
                pc.Status.hit_melee = (ushort)checkPositive((hit_melee + pc.Status.hit_melee_item + pc.Status.hit_melee_skill + pc.Status.hit_melee_iris + pc.Status.hit_melee_mario + pc.Status.hit_melee_another) * (float)(pc.Status.hit_melee_rate_iris / 100.0f));

                ushort hit_ranged = (ushort)(pcint + (short)Math.Floor((double)((float)pcint / 10.0f)) * 11 + pc.Level + 3);
                pc.Status.hit_ranged = (ushort)checkPositive((hit_ranged + pc.Status.hit_ranged_item + pc.Status.hit_ranged_skill + pc.Status.hit_ranged_iris + pc.Status.hit_ranged_mario + pc.Status.hit_ranged_another) * (float)(pc.Status.hit_ranged_rate_iris / 100.0f));

                //防御计算
                short add_def = 0, add_mdef = 0;
                if (pc.Status.equip_def_add_rate > 0)//Iris卡片中的防具防御力百分比提升值判定
                {
                    add_def += (short)(pc.Status.def_add_item * ((pc.Status.equip_def_add_rate) / 100.0f));
                    add_mdef += (short)(pc.Status.mdef_add_item * ((pc.Status.equip_def_add_rate) / 100.0f));
                }
                if ((pc.Skills.ContainsKey(106) || pc.DualJobSkill.Exists(x => x.ID == 106)))//骑士1转Job34-防御力上昇
                {
                    var duallv = 0;
                    if (pc.DualJobSkill.Exists(x => x.ID == 106))
                        duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 106).Level;
                    var mainlv = 0;

                    if (pc.Skills.ContainsKey(106))
                        mainlv = pc.Skills[106].Level;
                    float rate = new float[] { 0, 0.02f, 0.05f, 0.07f, 0.09f, 0.12f }[Math.Max(duallv, mainlv)];
                    add_def += (short)(pc.Status.def_add_item * (rate / 100.0f));
                    add_mdef += (short)(pc.Status.mdef_add_item * (rate / 100.0f));
                }

                //ECOKEY 防禦計算更改
                ushort def = (ushort)Math.Min(checkPositive((int)(pcvit / 3) + (int)((float)pcvit / 4.5f)), Configuration.Instance.BasePhysicDef);
                pc.Status.def_bs = def;
                pc.Status.def = (ushort)checkPositive(def + pc.Status.def_skill + checkHighVitBonus(pc) + pc.Status.def_item + pc.Status.def_iris + pc.Status.def_mario);
                //pc.Status.def = (ushort)Math.Min(checkPositive((int)(pcvit / 3) + (int)((float)pcvit / 4.5f)), Configuration.Instance.BasePhysicDef);
                //pc.Status.def_bs = pc.Status.def;
                //pc.Status.def += (ushort)checkPositive(pc.Status.def_skill + checkHighVitBonus(pc) + pc.Status.def_item + pc.Status.def_iris + pc.Status.def_mario);

                pc.Status.def_add = (short)checkPositive(pc.Status.def_add_mario + pc.Status.def_add_skill + pc.Status.def_add_item + pc.Status.def_add_iris + pc.Status.def_add_another);
                pc.Status.def_add += add_def;

                //ECOKEY 魔法防禦計算更改
                ushort mdef = (ushort)Math.Min(checkPositive(((int)(pcint / 3) + (int)((float)pcvit / 4.0f))), Configuration.Instance.BaseMagicDef);
                pc.Status.mdef_bs = mdef;
                pc.Status.mdef = (ushort)checkPositive(mdef + pc.Status.mdef_skill + checkHighIntBonus(pc) + pc.Status.mdef_item + pc.Status.mdef_iris + pc.Status.mdef_mario);
                //pc.Status.mdef = (ushort)Math.Min(checkPositive(((int)(pcint / 3) + (int)((float)pcvit / 4.0f))), Configuration.Instance.BaseMagicDef);
                //pc.Status.mdef_bs = pc.Status.mdef;
                //pc.Status.mdef += (ushort)checkPositive(pc.Status.mdef_skill + checkHighIntBonus(pc) + pc.Status.mdef_item + pc.Status.mdef_iris + pc.Status.mdef_mario);

                pc.Status.mdef_add = (short)checkPositive(pc.Status.mdef_add_mario + pc.Status.mdef_add_skill + pc.Status.mdef_add_item + pc.Status.mdef_add_iris + pc.Status.mdef_add_another);
                pc.Status.mdef_add += add_mdef;
                if (pc.Buff.Aegis3RD)//ECOKEY GU宙斯盾
                {
                    ushort ori_d = pc.Status.def;
                    short ori_d_add = pc.Status.def_add;
                    pc.Status.def = pc.Status.mdef;
                    pc.Status.def_add = pc.Status.mdef_add;
                    pc.Status.mdef = ori_d;
                    pc.Status.mdef_add = ori_d_add;
                }
                //闪避计算
                ushort avoid_melee = (ushort)(pcagi + Math.Pow(Math.Floor((float)(pcagi + 18) / 9.0f), 2) + Math.Floor((float)pc.Level / 3.0f) - 1);
                pc.Status.avoid_melee = (ushort)checkPositive((avoid_melee + pc.Status.avoid_melee_item + pc.Status.avoid_melee_skill + pc.Status.avoid_melee_mario + pc.Status.avoid_melee_another) * (pc.Status.avoid_melee_rate_iris / 100.0f));

                ushort avoid_ranged = (ushort)(Math.Floor((float)pcint * 5.0f / 3.0f) + pcagi + Math.Floor((float)pc.Level / 3.0f) + 3);
                pc.Status.avoid_ranged = checkPositive((avoid_ranged + pc.Status.avoid_ranged_item + pc.Status.avoid_ranged_skill + pc.Status.avoid_ranged_mario + pc.Status.avoid_ranged_another) * (pc.Status.avoid_ranged_rate_iris / 100.0f));

                //会心计算
                pc.Status.hit_critical = (ushort)(Math.Max(Math.Floor((double)((pcdex + 1) / 8)) + pc.Status.cri_mario, 1));
                pc.Status.avoid_critical = (ushort)((float)(pcagi + 8) / 6.0f + pc.Status.cri_avoid_mario);
                //pc.Status.hit_magic = (ushort)((pc.Int + pc.Status.int_item + pc.Status.int_chip + pc.Status.int_rev + pc.Status.int_mario + pc.Status.int_skill) * 0.2f);
                //calculate the possession spirit status
                if (!pc.Buff.三转凭依者封印)//ECOKEY SO憑依者封印
                {
                    foreach (ActorPC i in pc.PossesionedActors)
                    {
                        if (i == null || i.Status == null || i == pc)//二哈更改測試
                            continue;
                        pc.Status.min_atk1 = checkPositive(pc.Status.min_atk1 + i.Status.min_atk1_possession);
                        pc.Status.min_atk2 = checkPositive(pc.Status.min_atk2 + i.Status.min_atk2_possession);
                        pc.Status.min_atk3 = checkPositive(pc.Status.min_atk3 + i.Status.min_atk3_possession);
                        pc.Status.max_atk1 = checkPositive(pc.Status.max_atk1 + i.Status.max_atk1_possession);
                        pc.Status.max_atk2 = checkPositive(pc.Status.max_atk2 + i.Status.max_atk2_possession);
                        pc.Status.max_atk3 = checkPositive(pc.Status.max_atk3 + i.Status.max_atk3_possession);
                        pc.Status.min_matk = checkPositive(pc.Status.min_matk + i.Status.min_matk_possession);
                        pc.Status.max_matk = checkPositive(pc.Status.max_matk + i.Status.max_matk_possession);
                        pc.Status.hit_melee = checkPositive(pc.Status.hit_melee + i.Status.hit_melee_possession);
                        pc.Status.hit_ranged = checkPositive(pc.Status.hit_ranged + i.Status.hit_ranged_possession);
                        pc.Status.avoid_melee = checkPositive(pc.Status.avoid_melee + i.Status.avoid_melee_possession);
                        pc.Status.avoid_ranged = checkPositive(pc.Status.avoid_ranged + i.Status.avoid_ranged_possession);
                        pc.Status.def = checkPositive(pc.Status.def + i.Status.def_possession);
                        pc.Status.def_add = (short)checkPositive(pc.Status.def_add + i.Status.def_add_possession);
                        pc.Status.mdef = checkPositive(pc.Status.mdef + i.Status.mdef_possession);
                        pc.Status.mdef_add = (short)checkPositive(pc.Status.mdef_add + i.Status.mdef_add_possession);
                    }
                    //ECOKEY 寵物憑依素質
                    if (pc.PossesionedPartner.Count != 0)
                    {
                        pc.Status.min_atk1 = checkPositive(pc.Status.min_atk1 + pc.Status.min_atk1_petpy);
                        pc.Status.min_atk2 = checkPositive(pc.Status.min_atk2 + pc.Status.min_atk2_petpy);
                        pc.Status.min_atk3 = checkPositive(pc.Status.min_atk3 + pc.Status.min_atk3_petpy);
                        pc.Status.max_atk1 = checkPositive(pc.Status.max_atk1 + pc.Status.max_atk1_petpy);
                        pc.Status.max_atk2 = checkPositive(pc.Status.max_atk2 + pc.Status.max_atk2_petpy);
                        pc.Status.max_atk3 = checkPositive(pc.Status.max_atk3 + pc.Status.max_atk3_petpy);
                        pc.Status.min_matk = checkPositive(pc.Status.min_matk + pc.Status.min_matk_petpy);
                        pc.Status.max_matk = checkPositive(pc.Status.max_matk + pc.Status.max_matk_petpy);
                        pc.Status.hit_melee = checkPositive(pc.Status.hit_melee + pc.Status.hit_melee_petpy);
                        pc.Status.hit_ranged = checkPositive(pc.Status.hit_ranged + pc.Status.hit_ranged_petpy);
                        pc.Status.avoid_melee = checkPositive(pc.Status.avoid_melee + pc.Status.avoid_melee_petpy);
                        pc.Status.avoid_ranged = checkPositive(pc.Status.avoid_ranged + pc.Status.avoid_ranged_petpy);
                        pc.Status.def_add = (short)checkPositive(pc.Status.def_add + pc.Status.def_add_petpy);
                        pc.Status.mdef_add = (short)checkPositive(pc.Status.mdef_add + pc.Status.mdef_add_petpy);
                    }
                }
                //ECOKEY 心PE素質
                if (pc.TamaireRental != null && pc.TamaireRental.CurrentLender != 0 && pc.Mode == PlayerMode.NORMAL)
                {
                    pc.Status.min_atk1 = checkPositive(pc.Status.min_atk1 + pc.Status.min_atk1_tamaire);
                    pc.Status.min_atk2 = checkPositive(pc.Status.min_atk2 + pc.Status.min_atk2_tamaire);
                    pc.Status.min_atk3 = checkPositive(pc.Status.min_atk3 + pc.Status.min_atk3_tamaire);
                    pc.Status.max_atk1 = checkPositive(pc.Status.max_atk1 + pc.Status.max_atk1_tamaire);
                    pc.Status.max_atk2 = checkPositive(pc.Status.max_atk2 + pc.Status.max_atk2_tamaire);
                    pc.Status.max_atk3 = checkPositive(pc.Status.max_atk3 + pc.Status.max_atk3_tamaire);
                    pc.Status.min_matk = checkPositive(pc.Status.min_matk + pc.Status.min_matk_tamaire);
                    pc.Status.max_matk = checkPositive(pc.Status.max_matk + pc.Status.max_matk_tamaire);
                    pc.Status.hit_melee = checkPositive(pc.Status.hit_melee + pc.Status.hit_melee_tamaire);
                    pc.Status.hit_ranged = checkPositive(pc.Status.hit_ranged + pc.Status.hit_ranged_tamaire);
                    pc.Status.avoid_melee = checkPositive(pc.Status.avoid_melee + pc.Status.avoid_melee_tamaire);
                    pc.Status.avoid_ranged = checkPositive(pc.Status.avoid_ranged + pc.Status.avoid_ranged_tamaire);
                    pc.Status.def_add = (short)checkPositive(pc.Status.def_add + pc.Status.def_add_tamaire);
                    pc.Status.mdef_add = (short)checkPositive(pc.Status.mdef_add + pc.Status.mdef_add_tamaire);
                }
                //ECOKEY 寵物憑依素質
                if (pc.PossesionedPartner.Count != 0)
                {
                    pc.Status.min_atk1 = checkPositive(pc.Status.min_atk1 + pc.Status.min_atk1_petpy);
                    pc.Status.min_atk2 = checkPositive(pc.Status.min_atk2 + pc.Status.min_atk2_petpy);
                    pc.Status.min_atk3 = checkPositive(pc.Status.min_atk3 + pc.Status.min_atk3_petpy);
                    pc.Status.max_atk1 = checkPositive(pc.Status.max_atk1 + pc.Status.max_atk1_petpy);
                    pc.Status.max_atk2 = checkPositive(pc.Status.max_atk2 + pc.Status.max_atk2_petpy);
                    pc.Status.max_atk3 = checkPositive(pc.Status.max_atk3 + pc.Status.max_atk3_petpy);
                    pc.Status.min_matk = checkPositive(pc.Status.min_matk + pc.Status.min_matk_petpy);
                    pc.Status.max_matk = checkPositive(pc.Status.max_matk + pc.Status.max_matk_petpy);
                    pc.Status.hit_melee = checkPositive(pc.Status.hit_melee + pc.Status.hit_melee_petpy);
                    pc.Status.hit_ranged = checkPositive(pc.Status.hit_ranged + pc.Status.hit_ranged_petpy);
                    pc.Status.avoid_melee = checkPositive(pc.Status.avoid_melee + pc.Status.avoid_melee_petpy);
                    pc.Status.avoid_ranged = checkPositive(pc.Status.avoid_ranged + pc.Status.avoid_ranged_petpy);
                    pc.Status.def_add = (short)checkPositive(pc.Status.def_add + pc.Status.def_add_petpy);
                    pc.Status.mdef_add = (short)checkPositive(pc.Status.mdef_add + pc.Status.mdef_add_petpy);
                }

                pc.Status.def = (ushort)Math.Min(pc.Status.def, Configuration.Instance.MaxPhysicDef);
                pc.Status.mdef = (ushort)Math.Min(pc.Status.mdef, Configuration.Instance.MaxMagicDef);

            }

            //ECOKEY 新增心PE攻速
            //攻速计算
            if (pc.Status.Additions.ContainsKey("ModeChange"))
                pc.Status.aspd = (short)(pcdex * 3 + Math.Floor(Math.Pow((short)((float)(pcdex + 63) / 9.0f), 2)) + 129 + pc.Status.aspd_skill + pc.Status.aspd_mario + pc.Status.aspd_another + pc.Status.aspd_tamaire);
            else
                pc.Status.aspd = (short)(pcagi * 3 + Math.Floor(Math.Pow((short)((float)(pcagi + 63) / 9.0f), 2)) + 129 + pc.Status.aspd_skill + pc.Status.aspd_mario + pc.Status.aspd_another + pc.Status.aspd_tamaire);
            //ECOKEY 新增心PE唱速
            //唱速计算
            if (pc.Status.Additions.ContainsKey("ModeChange"))
                pc.Status.cspd = (short)(pcagi * 3 + Math.Floor(Math.Pow((short)((float)(pcagi + 63) / 9.0f), 2)) + 129 + pc.Status.cspd_skill + pc.Status.cspd_mario + pc.Status.cspd_another + pc.Status.cspd_tamaire);
            else
                pc.Status.cspd = (short)(pcdex * 3 + Math.Floor(Math.Pow((short)((float)(pcdex + 63) / 9.0f), 2)) + 129 + pc.Status.cspd_skill + pc.Status.cspd_mario + pc.Status.cspd_another + pc.Status.cspd_tamaire);


            //ECOKEY 騎寵速度
            if (pc.Pet != null && pc.Pet.Ride)
            {
                //ECOKEY 機器人
                if (pc.Partner != null)
                {
                    pc.Speed = (ushort)(pc.Partner.Speed + pc.Status.speed_skill);//ECOKEY 騎寵速度修改
                }
                else
                {
                    pc.Speed = (ushort)(pc.Pet.Speed + pc.Status.speed_skill);//ECOKEY 騎寵速度修改
                }
            }
            else
            {
                //移动速度
                pc.Speed = (ushort)(Configuration.Instance.Speed + pc.Status.speed_item + pc.Status.speed_skill + pc.Status.speed_mario);
            }

            //移动速度
            //pc.Speed = (ushort)(Configuration.Instance.Speed + pc.Status.speed_item + pc.Status.speed_skill + pc.Status.speed_mario);//ECOKEY 為了讓騎寵速度有效，這句不需要，可刪
            if (pc.Status.Additions.ContainsKey("Gladiator") ||//剑斗士
                pc.Status.Additions.ContainsKey("イレイザー") ||//肃清者
                pc.Status.Additions.ContainsKey("SoulTaker"))//噬魂者)
            {
                pc.Speed = 310;
            }
            //爪子和双枪的攻速惩罚
            if (pc.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
            {
                Item item = pc.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND];
                if (item.BaseData.itemType == ItemType.DUALGUN || item.BaseData.itemType == ItemType.CLAW)
                    pc.Status.aspd = (short)((float)(pc.Status.aspd) * 0.70f);
            }

            //修正dex爆表的情况下 cspd变负数导致咏唱时间超标的问题
            pc.Status.aspd = Math.Max((short)1, pc.Status.aspd);
            pc.Status.cspd = Math.Max((short)1, pc.Status.cspd);

            pc.Status.avoid_melee = Math.Min((ushort)500, pc.Status.avoid_melee);
            pc.Status.avoid_ranged = Math.Min((ushort)500, pc.Status.avoid_ranged);

            pc.Status.hit_melee = Math.Min((ushort)500, pc.Status.hit_melee);
            pc.Status.hit_ranged = Math.Min((ushort)500, pc.Status.hit_ranged);

            pc.Status.aspd = Math.Min((short)800, pc.Status.aspd);

            pc.Status.cspd = Math.Min((short)((pc.Status.speedenchantcspdbonus > 0 || pc.Status.communioncspdbonus > 0) ? 850 : 800), (short)(pc.Status.cspd + pc.Status.speedenchantcspdbonus + pc.Status.communioncspdbonus));
            //ECOKEY 新增玩家恢復率
            pc.Status.hp_recover_bs = (short)(100 + (pcvit / 2));
            pc.Status.mp_recover_bs = (short)(100 + (pcmag / 2));
            pc.Status.sp_recover_bs = (short)(100 + ((pcvit + pcint) / 6));
            pc.Status.hp_recover = (short)checkPositive(pc.Status.hp_recover_bs + pc.Status.hp_recover_iris + pc.Status.hp_recover_item + pc.Status.hp_recover_skill);
            pc.Status.mp_recover = (short)checkPositive(pc.Status.mp_recover_bs + pc.Status.mp_recover_iris + pc.Status.mp_recover_item + pc.Status.mp_recover_skill);
            pc.Status.sp_recover = (short)checkPositive(pc.Status.sp_recover_bs + pc.Status.sp_recover_iris + pc.Status.sp_recover_item + pc.Status.sp_recover_skill);

        }

        public ushort RequiredBonusPoint(ushort current)
        {
            return (ushort)Math.Ceiling((float)((float)(current + 1) / 6.0f));
        }

        public ushort GetTotalBonusPointForStats(ushort start, ushort stat)
        {
            int points = 0;
            for (ushort i = start; i < stat; i++)
            {
                points += RequiredBonusPoint(i);
            }
            return (ushort)points;
        }

        float CalcATKRate(ActorPC pc)
        {
            bool ifRanged = false;
            if (pc.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
            {
                Item item = pc.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND];
                if (item.BaseData.itemType == ItemType.BOW || item.BaseData.itemType == ItemType.GUN || item.BaseData.itemType == ItemType.DUALGUN ||
                    item.BaseData.itemType == ItemType.RIFLE || item.BaseData.itemType == ItemType.THROW)
                    ifRanged = true;
            }
            if (!ifRanged)
            {
                return 1.0f + (float)(((pc.Dex + pc.Status.dex_item + pc.Status.dex_rev + pc.Status.dex_mario + pc.Status.dex_skill) * 1.5) / 160);
            }
            else
            {
                return 1.0f + (float)(((pc.Int + pc.Status.int_item + pc.Status.int_rev + pc.Status.int_mario + pc.Status.int_skill) * 1.5) / 160);
            }
        }

        public void CalcPayV(ActorPC pc)
        {
            CalcPayl(pc);
            CalcVolume(pc);
        }

        void CalcVolume(ActorPC pc)
        {
            //CAPA = floor[ (floor[DEX/5] + floor[INT/10] + 200)×職業係数×スキルパッキングによる倍率 ]
            uint VOLU = (uint)((((Math.Max((pc.Dex + pc.Status.dex_item + pc.Status.dex_chip + pc.Status.dex_rev + pc.Status.dex_skill), 0) / 5.0f) +
                (Math.Max((pc.Int + pc.Status.int_item + pc.Status.int_chip + pc.Status.int_rev + pc.Status.int_skill), 0) / 10.0f)) + 200)
                * VolumeJobFactor(pc)
                * Configuration.Instance.VolumeRate * 10);

            if (pc.Status.volume_iris > 0)
            {
                VOLU += (uint)(VOLU * (float)((pc.Status.volume_iris) / 100.0f));
            }
            if (pc.Status.volume_add_iris > 0)
            {
                VOLU += (uint)pc.Status.volume_add_iris;
            }
            //ECOKEY 心憑依新增
            if (pc.Status.volume_add_tamaire > 0)
            {
                VOLU += (uint)pc.Status.volume_add_tamaire;
            }
            VOLU += (uint)pc.Status.CAPA_skill;//ECOKEY RO三轉
            pc.Inventory.MaxVolume[ContainerType.BODY] = VOLU; if (pc.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND) && pc.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.volumeUp > 0)
                pc.Inventory.MaxVolume[ContainerType.BODY] = (uint)(pc.Inventory.MaxVolume[ContainerType.BODY] + pc.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.volumeUp);
            if (pc.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND) && pc.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.volumeUp > 0)
                pc.Inventory.MaxVolume[ContainerType.BODY] = (uint)(pc.Inventory.MaxVolume[ContainerType.BODY] + pc.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.volumeUp);

            if (pc.Inventory.Equipments.ContainsKey(EnumEquipSlot.BACK) && pc.Inventory.Equipments[EnumEquipSlot.BACK].BaseData.volumeUp > 0)
                pc.Inventory.MaxVolume[ContainerType.BODY] = (uint)(pc.Inventory.MaxVolume[ContainerType.BODY] + pc.Inventory.Equipments[EnumEquipSlot.BACK].BaseData.volumeUp);
            if (pc.Inventory.Equipments.ContainsKey(EnumEquipSlot.PET))
                pc.Inventory.MaxVolume[ContainerType.BODY] = (uint)(pc.Inventory.MaxVolume[ContainerType.BODY] + pc.Inventory.Equipments[EnumEquipSlot.PET].BaseData.volumeUp);
            if (pc.Account.GMLevel > 50)
                pc.Inventory.MaxVolume[ContainerType.BODY] += 1000;

            if (pc.TimerItems.ContainsKey("ADDON_PAYL_CAPA"))//ECOKEY 泰迪的秘傳收納術
            {
                pc.Inventory.MaxVolume[ContainerType.BODY] = (uint)(pc.Inventory.MaxVolume[ContainerType.BODY] + pc.Inventory.MaxVolume[ContainerType.BODY] * 0.5);
            }

            //ECOKEY 根性
            if (pc.Buff.Spirit3RD)
            {
                uint tmp = (uint)(pc.Inventory.MaxVolume[ContainerType.BODY] * 0.3f);
                pc.Inventory.MaxVolume[ContainerType.BODY] += tmp;
            }
        }

        void CalcPayl(ActorPC pc)
        {
            //PAYL = floor[(X + 400)×種族係数×職業係数]
            //ただし X = floor[STR×2 / 3] + floor[VIT / 3]
            //旧公式废除
            //pc.Inventory.MaxPayload[ContainerType.BODY] = (uint)((float)((pc.Str + pc.Status.str_item + pc.Status.str_chip + pc.Status.str_rev + pc.Status.str_skill) * 3 + (2 * pc.JobLevel1 + 4 * pc.JobLevel2X + 4 * pc.JobLevel2T + 6 * pc.JobLevel3 + 350)) *
            //    PayLoadRaceFactor(pc.Race) * Configuration.Instance.PayloadRate * PayLoadJobFactor(pc) * 10);
            uint PCPAYL = (uint)((float)((((Math.Max((pc.Str + pc.Status.str_item + pc.Status.str_chip + pc.Status.str_rev + pc.Status.str_skill), 0) * 2.0f / 3.0f) +
                (Math.Max((pc.Vit + pc.Status.vit_item + pc.Status.vit_chip + pc.Status.vit_rev + pc.Status.vit_skill), 0) / 3.0f)) + 400) *
                Configuration.Instance.PayloadRate *
                PayLoadRaceFactor(pc.Race) * PayLoadJobFactor(pc)) * 10);
            if (pc.Status.payl_iris > 0)
            {
                PCPAYL += (uint)(PCPAYL * (float)((pc.Status.payl_iris) / 100.0f));
            }
            if (pc.Status.payl_add_iris > 0)
            {
                PCPAYL += (uint)pc.Status.payl_add_iris;
            }
            //ECOKEY 心憑依新增
            if (pc.Status.payl_add_tamaire > 0)
            {
                PCPAYL += (uint)pc.Status.payl_add_tamaire;
            }
            PCPAYL += (uint)pc.Status.PAYL_skill;//ECOKEY HA三轉
            pc.Inventory.MaxPayload[ContainerType.BODY] = PCPAYL;
            if (pc.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND) && pc.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.weightUp > 0)
                pc.Inventory.MaxPayload[ContainerType.BODY] = (uint)(pc.Inventory.MaxPayload[ContainerType.BODY] + pc.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.weightUp);
            if (pc.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND) && pc.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.weightUp > 0)
                pc.Inventory.MaxPayload[ContainerType.BODY] = (uint)(pc.Inventory.MaxPayload[ContainerType.BODY] + pc.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.weightUp);
            if (pc.Inventory.Equipments.ContainsKey(EnumEquipSlot.BACK) && pc.Inventory.Equipments[EnumEquipSlot.BACK].BaseData.weightUp > 0)
                pc.Inventory.MaxPayload[ContainerType.BODY] = (uint)(pc.Inventory.MaxPayload[ContainerType.BODY] + pc.Inventory.Equipments[EnumEquipSlot.BACK].BaseData.weightUp);
            if (pc.Inventory.Equipments.ContainsKey(EnumEquipSlot.PET))
                pc.Inventory.MaxPayload[ContainerType.BODY] = (uint)(pc.Inventory.MaxPayload[ContainerType.BODY] + pc.Inventory.Equipments[EnumEquipSlot.PET].BaseData.weightUp);
            if (pc.Account.GMLevel > 50)
                pc.Inventory.MaxPayload[ContainerType.BODY] += 1000;
            if (pc.TimerItems.ContainsKey("ADDON_PAYL_CAPA"))//ECOKEY 泰迪的秘傳收納術
            {
                pc.Inventory.MaxPayload[ContainerType.BODY] = (uint)(pc.Inventory.MaxPayload[ContainerType.BODY] + pc.Inventory.MaxPayload[ContainerType.BODY] * 0.5);
            }
            if (pc.Buff.Spirit3RD)
            {
                uint tmp = (uint)(pc.Inventory.MaxPayload[ContainerType.BODY] * 0.3f);
                pc.Inventory.MaxPayload[ContainerType.BODY] += tmp;
            }
        }

        float PayLoadRaceFactor(PC_RACE race)
        {
            switch (race)
            {
                case PC_RACE.EMIL:
                    return 1.3f;
                case PC_RACE.TITANIA:
                    return 0.9f;
                case PC_RACE.DOMINION:
                    return 1.1f;
                default:
                    return 1;
            }
        }

        float PayLoadJobFactor(ActorPC pc)
        {
            PC_JOB job;
            if (pc.JobJoint == PC_JOB.NONE)
                job = pc.Job;
            else
                job = pc.JobJoint;
            switch (job)
            {
                //□初期：ノービス
                case PC_JOB.NOVICE:
                    return 0.7f;
                //□F系：ファイター
                //┣■ソードマン
                //┃┣ ブレイドマスター
                //┃┗ バウンティハンター
                //┣■フェンサー
                //┃┣ ナイト
                //┃┗ ダークストーカー
                //┣■スカウト
                //┃┣ アサシン
                //┃┗ コマンド
                //┣■アーチャー
                //┃┣ ストライカー
                //┃┗ ガンナー
                case PC_JOB.SWORDMAN:
                case PC_JOB.BLADEMASTER:
                case PC_JOB.BOUNTYHUNTER:
                case PC_JOB.GLADIATOR:
                case PC_JOB.FENCER:
                case PC_JOB.KNIGHT:
                case PC_JOB.DARKSTALKER:
                case PC_JOB.GUARDIAN:
                case PC_JOB.SCOUT:
                case PC_JOB.ASSASSIN:
                case PC_JOB.COMMAND:
                case PC_JOB.ERASER:
                case PC_JOB.ARCHER:
                case PC_JOB.STRIKER:
                case PC_JOB.GUNNER:
                case PC_JOB.HAWKEYE:
                    return 1.0f;
                //□SU系：スペルユーザー
                //┣■ウィザード
                //┃┣ ソーサラー
                //┃┗ セージ
                //┣■シャーマン
                //┃┣ エレメンタラー
                //┃┗ エンチャンター
                //┣■ウァテス
                //┃┣ ドルイド
                //┃┗ バード
                //┣■ウォーロック
                //┃┣ カバリスト
                //┃┗ ネクロマンサー
                case PC_JOB.WIZARD:
                case PC_JOB.SORCERER:
                case PC_JOB.SAGE:
                case PC_JOB.FORCEMASTER:
                case PC_JOB.SHAMAN:
                case PC_JOB.ELEMENTER:
                case PC_JOB.ENCHANTER:
                case PC_JOB.ASTRALIST:
                case PC_JOB.VATES:
                case PC_JOB.DRUID:
                case PC_JOB.BARD:
                case PC_JOB.CARDINAL:
                case PC_JOB.WARLOCK:
                case PC_JOB.CABALIST:
                case PC_JOB.NECROMANCER:
                case PC_JOB.SOULTAKER:
                    return 0.8f;
                //□BP系：バックパッカー
                //┣■タタラベ
                //┃┣ ブラックスミス
                //┃┗ マシンナリー
                //┣■ファーマー
                //┃┣ アルケミスト
                //┃┗ マリオネスト
                //┣■レンジャー
                //┃┣ エクスプローラー
                //┃┗ トレジャーハンター
                //┃■マーチャント
                //┃┣ トレーダー
                //┃┗ ギャンブラー
                case PC_JOB.BLACKSMITH:
                case PC_JOB.MACHINERY:
                case PC_JOB.MAESTRO:
                case PC_JOB.ALCHEMIST:
                case PC_JOB.HARVEST:
                case PC_JOB.RANGER:
                case PC_JOB.EXPLORER:
                case PC_JOB.TREASUREHUNTER:
                case PC_JOB.STRIDER:
                case PC_JOB.MERCHANT:
                case PC_JOB.TRADER:
                case PC_JOB.GAMBLER:
                case PC_JOB.ROYALDEALER:
                    return 1.3f;
                default:
                    return 1;
            }
        }

        float VolumeJobFactor(ActorPC pc)
        {
            PC_JOB job;
            if (pc.JobJoint == PC_JOB.NONE)
                job = pc.Job;
            else
                job = pc.JobJoint;
            switch (job)
            {
                //□初期：ノービス
                case PC_JOB.NOVICE:
                    return 0.85f;
                //□F系：ファイター
                //┣■ソードマン
                //┃┣ ブレイドマスター
                //┃┗ バウンティハンター
                //┣■フェンサー
                //┃┣ ナイト
                //┃┗ ダークストーカー
                //┣■スカウト
                //┃┣ アサシン
                //┃┗ コマンド
                //┣■アーチャー
                //┃┣ ストライカー
                //┃┗ ガンナー
                case PC_JOB.SWORDMAN:
                case PC_JOB.BLADEMASTER:
                case PC_JOB.BOUNTYHUNTER:
                case PC_JOB.GLADIATOR:
                case PC_JOB.FENCER:
                case PC_JOB.KNIGHT:
                case PC_JOB.DARKSTALKER:
                case PC_JOB.GUARDIAN:
                case PC_JOB.SCOUT:
                case PC_JOB.ASSASSIN:
                case PC_JOB.COMMAND:
                case PC_JOB.ERASER:
                case PC_JOB.ARCHER:
                case PC_JOB.STRIKER:
                case PC_JOB.GUNNER:
                case PC_JOB.HAWKEYE:
                    return 1.0f;
                //□SU系：スペルユーザー
                //┣■ウィザード
                //┃┣ ソーサラー
                //┃┗ セージ
                //┣■シャーマン
                //┃┣ エレメンタラー
                //┃┗ エンチャンター
                //┣■ウァテス
                //┃┣ ドルイド
                //┃┗ バード
                //┣■ウォーロック
                //┃┣ カバリスト
                //┃┗ ネクロマンサー
                case PC_JOB.WIZARD:
                case PC_JOB.SORCERER:
                case PC_JOB.SAGE:
                case PC_JOB.FORCEMASTER:
                case PC_JOB.SHAMAN:
                case PC_JOB.ELEMENTER:
                case PC_JOB.ENCHANTER:
                case PC_JOB.ASTRALIST:
                case PC_JOB.VATES:
                case PC_JOB.DRUID:
                case PC_JOB.BARD:
                case PC_JOB.CARDINAL:
                case PC_JOB.WARLOCK:
                case PC_JOB.CABALIST:
                case PC_JOB.NECROMANCER:
                case PC_JOB.SOULTAKER:
                    return 1.0f;
                //□BP系：バックパッカー
                //┣■タタラベ
                //┃┣ ブラックスミス
                //┃┗ マシンナリー
                //┣■ファーマー
                //┃┣ アルケミスト
                //┃┗ マリオネスト
                //┣■レンジャー
                //┃┣ エクスプローラー
                //┃┗ トレジャーハンター
                //┃■マーチャント
                //┃┣ トレーダー
                //┃┗ ギャンブラー
                case PC_JOB.BLACKSMITH:
                case PC_JOB.MACHINERY:
                case PC_JOB.MAESTRO:
                case PC_JOB.ALCHEMIST:
                case PC_JOB.HARVEST:
                case PC_JOB.RANGER:
                case PC_JOB.EXPLORER:
                case PC_JOB.TREASUREHUNTER:
                case PC_JOB.STRIDER:
                case PC_JOB.MERCHANT:
                case PC_JOB.TRADER:
                case PC_JOB.GAMBLER:
                case PC_JOB.ROYALDEALER:
                    return 1.13f;
                default:
                    return 1;
            }
        }

        public void CalcHPMPSP(ActorPC pc)
        {
            pc.MaxHP = (uint)(CalcMaxHP(pc));
            pc.MaxMP = (uint)(CalcMaxMP(pc));
            pc.MaxSP = (uint)(CalcMaxSP(pc));
            pc.MaxEP = (uint)(CalcMaxEP(pc));
            if (pc.HP > pc.MaxHP) pc.HP = pc.MaxHP;
            if (pc.MP > pc.MaxMP) pc.MP = pc.MaxMP;
            if (pc.SP > pc.MaxSP) pc.SP = pc.MaxSP;
            if (pc.EP > pc.MaxEP) pc.EP = pc.MaxEP;
        }

        uint CalcMaxEP(ActorPC pc)//ECOKEY DEM限時道具
        {
            uint ep = 0;
            if (pc.Ring == null)
                ep = 30;
            else
                ep = Math.Min((uint)(30 + pc.Ring.MemberCount * 2), 110);
            if (pc.TimerItems.ContainsKey("EP_DOUBLE")) ep *= 2;
            return ep;
            /*if (pc.Ring == null)
                return 30;
            else
                return Math.Min((uint)(30 + pc.Ring.MemberCount * 2), 110);*/
            //return 100;
        }
        uint CalcMaxHP(ActorPC pc)
        {
            short possession = 0;
            byte lv = pc.Level;
            //ECOKEY 修復過圖時騎寵HP錯亂問題
            if (pc.Inventory.Equipments.ContainsKey(EnumEquipSlot.PET) && (pc.Inventory.Equipments[EnumEquipSlot.PET].BaseData.itemType == ItemType.RIDE_PARTNER || pc.Inventory.Equipments[EnumEquipSlot.PET].BaseData.itemType == ItemType.RIDE_PET_ROBOT))
            {
                if (pc.Pet != null)
                {
                    if (pc.Pet.Ride)
                    {
                        //ECOKEY 機器人（0310補上這裡！！）
                        if (pc.Partner != null)
                        {
                            if (pc.Partner.MaxHP != 0)
                            {
                                //ECOKEY 坐騎技能加成
                                uint parhp_skill = (uint)(pc.Status.vit_skill * 3 + Math.Pow((int)Math.Floor((float)pc.Status.vit_skill / 5.0f), 2));
                                pc.TInt["PET_HP"] = (int)(pc.Partner.MaxHP + (uint)pc.Status.hp_skill + parhp_skill);//ECOKEY 記錄騎寵HP
                                return pc.Partner.MaxHP + (uint)pc.Status.hp_skill + parhp_skill;
                            }
                        }
                        else
                        {
                            pc.TInt["PET_HP"] = (int)pc.Pet.MaxHP;//ECOKEY 記錄騎寵HP
                            if (pc.Pet.MaxHP != 0)
                                return pc.Pet.MaxHP;
                        }
                    }
                }
                if (pc.TInt["PET_HP"] != 0)
                {
                    return (uint)pc.TInt["PET_HP"];
                }
            }

            foreach (ActorPC i in pc.PossesionedActors)
            {
                if (i == pc) continue;
                if (i.Status == null)
                    continue;
                possession += i.Status.hp_possession;
            }

            ushort pcvit = (ushort)checkPositive((pc.Vit + pc.Status.vit_item + pc.Status.vit_chip + pc.Status.vit_rev + pc.Status.vit_mario + pc.Status.vit_skill + pc.Status.vit_iris + pc.Status.vit_another));

            uint basehp = (uint)(Math.Floor((pcvit * 3 + Math.Pow((int)Math.Floor((float)pcvit / 5.0f), 2) + lv * 2 + Math.Pow((int)Math.Floor((float)lv / 5.0f), 2) + 50)) * HPJobFactor(pc));
            basehp = (uint)Math.Max(1, basehp + pc.Status.hp_item + pc.Status.hp_mario + possession + pc.Status.hp_skill + pc.Status.hp_iris + pc.Status.hp_another + pc.Status.hp_tamaire + pc.Status.hp_petpy);//ECOKEY 新增心PE HP //ECOKEY 新增寵物PE HP
            var itembonus = (float)(pc.Status.hp_rate_item / 100.0f) - 1.0f;
            var irisbonus = (float)(pc.Status.hp_rate_iris / 100.0f) - 1.0f;
            basehp = (uint)((float)basehp * (float)(1.0f + itembonus + irisbonus));
            return Math.Min(basehp, 70000);
        }
        uint CalcMaxMP(ActorPC pc)
        {
            short possession = 0;
            byte lv = 0;
            Map map = Manager.MapManager.Instance.GetMap(pc.MapID);
            lv = pc.Level;
            //ECOKEY 騎寵MP不給玩家
            /*if (pc.Pet != null)
            {
                if (pc.Pet.Ride)
                {
                    if (pc.Partner.MaxMP != 0)
                        return pc.Partner.MaxMP;
                }
            }*/
            foreach (ActorPC i in pc.PossesionedActors)
            {
                if (i == pc) continue;
                if (i.Status == null)
                    continue;
                possession += i.Status.mp_possession;
            }

            ushort pcmag = 0;
            uint basemp = 0;
            if (pc.Status.Additions.ContainsKey("ModeChange"))
            {
                pcmag = (ushort)checkPositive((pc.Str + pc.Status.mag_item + pc.Status.mag_chip + pc.Status.str_rev + pc.Status.mag_mario + pc.Status.mag_skill + pc.Status.mag_iris + pc.Status.mag_another));
            }
            else
            {
                pcmag = (ushort)checkPositive((pc.Mag + pc.Status.mag_item + pc.Status.mag_chip + pc.Status.mag_rev + pc.Status.mag_mario + pc.Status.mag_skill + pc.Status.mag_iris + pc.Status.mag_another));
            }
            basemp = (uint)Math.Floor((float)(pcmag * 3 + lv + Math.Pow((float)Math.Floor((float)lv / 9.0f), 2) + 30) * MPJobFactor(pc));
            basemp = (uint)Math.Max(1, basemp + pc.Status.mp_item + pc.Status.mp_mario + possession + pc.Status.mp_skill + pc.Status.mp_iris + pc.Status.mp_tamaire + pc.Status.mp_petpy);//ECOKEY 新增心PE MP //ECOKEY 新增寵物PE MP
            var itembonus = (float)(pc.Status.mp_rate_item / 100.0f) - 1.0f;
            var irisbonus = (float)(pc.Status.mp_rate_iris / 100.0f) - 1.0f;
            basemp = (uint)((float)basemp * (float)(1.0f + itembonus + irisbonus));
            return Math.Min(basemp, 40000);
        }
        uint CalcMaxSP(ActorPC pc)
        {
            short possession = 0;
            byte lv = 0;
            Map map = Manager.MapManager.Instance.GetMap(pc.MapID);
            lv = pc.Level;
            //ECOKEY 騎寵SP不給玩家
            /*if (pc.Pet != null)
            {
                if (pc.Pet.Ride)
                {
                    if (pc.Partner.MaxSP != 0)
                        return pc.Partner.MaxSP;
                }
            }*/
            foreach (ActorPC i in pc.PossesionedActors)
            {
                //过滤已下线的活魂
                /* if (i == null) continue; ??
                 if (i == pc) continue;
                 possession += i.Status.sp_possession;*/
                if (i == pc) continue;
                if (i.Status != null) //(i.Status == null)
                    possession += i.Status.sp_possession;
            }

            ushort pcint = (ushort)checkPositive((pc.Int + pc.Status.int_item + pc.Status.int_chip + pc.Status.int_rev + pc.Status.int_mario + pc.Status.int_skill + pc.Status.int_iris + pc.Status.int_another));
            ushort pcvit = (ushort)checkPositive((pc.Vit + pc.Status.vit_item + pc.Status.vit_chip + pc.Status.vit_rev + pc.Status.vit_mario + pc.Status.vit_skill + pc.Status.vit_iris + pc.Status.vit_another));

            //最大SP = floor[ (INT + VIT + LV + floor[LV/9]^2 + 20)×SP係数 ] + スキルによる補正 + 装備による補正
            uint basesp = (uint)Math.Floor((float)(pcint + pcvit + lv + Math.Pow((int)Math.Floor((float)lv / 9.0f), 2) + 20) * SPJobFactor(pc));
            basesp = (uint)Math.Max(1, basesp + pc.Status.sp_item + pc.Status.sp_mario + possession + pc.Status.sp_skill + pc.Status.sp_iris + pc.Status.sp_another + pc.Status.sp_tamaire + pc.Status.sp_petpy);//ECOKEY 新增心PE SP //ECOKEY 新增寵物PE SP
            var itembonus = (float)(pc.Status.sp_rate_item / 100.0f) - 1.0f;
            var irisbonus = (float)(pc.Status.sp_rate_iris / 100.0f) - 1.0f;
            basesp = (uint)((float)basesp * (float)(1.0f + itembonus + irisbonus));
            return Math.Min(basesp, 40000);
        }

        float HPJobFactor(ActorPC pc)
        {
            PC_JOB job;
            if (pc.JobJoint == PC_JOB.NONE)
                job = pc.Job;
            else
                job = pc.JobJoint;

            switch (job)
            {
                //1次職
                case PC_JOB.NOVICE:
                    return 1.00f;

                case PC_JOB.SWORDMAN:
                    return 1.80f;

                case PC_JOB.FENCER:
                    return 1.65f;

                case PC_JOB.SCOUT:
                    return 1.45f;

                case PC_JOB.ARCHER:
                    return 1.35f;

                case PC_JOB.WIZARD:
                    return 1.10f;

                case PC_JOB.SHAMAN:
                    return 1.05f;

                case PC_JOB.VATES:
                    return 1.15f;

                case PC_JOB.WARLOCK:
                    return 1.30f;

                case PC_JOB.RANGER:
                    return 1.25f;

                case PC_JOB.MERCHANT:
                    return 1.20f;

                case PC_JOB.TATARABE:
                    return 1.50f;

                case PC_JOB.FARMASIST:
                    return 1.40f;

                //2次職エキスパート
                case PC_JOB.BLADEMASTER:
                    return 3.05f;

                case PC_JOB.KNIGHT:
                    return 3.30f;

                case PC_JOB.ASSASSIN:
                    return 2.45f;//(2.20-2.45)

                case PC_JOB.STRIKER:
                    return 2.30f;//(2.07-2.25)

                case PC_JOB.SORCERER:
                    return 1.85f;

                case PC_JOB.ELEMENTER:
                    return 1.80f;

                case PC_JOB.DRUID:
                    return 1.95f;

                case PC_JOB.CABALIST:
                    return 2.60f;

                case PC_JOB.BREEDER:
                case PC_JOB.GARDNER:
                case PC_JOB.BLACKSMITH:
                case PC_JOB.MARIONEST:
                    return 3;

                case PC_JOB.ALCHEMIST:
                    return 2.50f;

                case PC_JOB.EXPLORER:
                    return 2.80f;

                case PC_JOB.TRADER:
                    return 2.40f;

                //2次職テクニカル
                case PC_JOB.BOUNTYHUNTER:
                    return 2.90f; //?

                case PC_JOB.DARKSTALKER:
                    return 3;

                case PC_JOB.COMMAND:
                    return 2.50f;

                case PC_JOB.GUNNER:
                    return 2.15f;

                case PC_JOB.SAGE:
                    return 1.95f;

                case PC_JOB.ENCHANTER:
                    return 1.85f;//(1.62-1.85)

                case PC_JOB.BARD:
                    return 2.15f;//(2.00-2.15)

                case PC_JOB.NECROMANCER:
                    return 2.30f;

                case PC_JOB.MACHINERY:
                    return 2.60f;

                case PC_JOB.TREASUREHUNTER:
                    return 2.30f;

                case PC_JOB.GAMBLER:
                    return 2.40f;

                //3转职业补正
                case PC_JOB.GLADIATOR:
                    return 5.17f;
                case PC_JOB.GUARDIAN:
                    return 6.6f;
                case PC_JOB.ERASER:
                    return 4.31f;
                case PC_JOB.HAWKEYE:
                    return 3.92f;
                case PC_JOB.FORCEMASTER:
                    return 3.46f;
                case PC_JOB.ASTRALIST:
                    return 3.26f;
                case PC_JOB.CARDINAL:
                    return 4.02f;
                case PC_JOB.SOULTAKER:
                    return 5.20f;
                case PC_JOB.MAESTRO:
                    return 6f;
                case PC_JOB.HARVEST:
                    return 5f;
                case PC_JOB.STRIDER:
                    return 5.6f;
                case PC_JOB.ROYALDEALER:
                    return 4.8f;
                case PC_JOB.JOKER:
                    return 3f;

                default:
                    return 1;
            }
        }

        float MPJobFactor(ActorPC pc)
        {
            PC_JOB job;
            if (pc.JobJoint == PC_JOB.NONE)
                job = pc.Job;
            else
                job = pc.JobJoint;

            switch (job)
            {
                //1次職
                case PC_JOB.NOVICE:
                    return 1.00f;

                case PC_JOB.SWORDMAN:
                    return 1.05f;

                case PC_JOB.FENCER:
                    return 1.05f;

                case PC_JOB.SCOUT:
                    return 1.05f;

                case PC_JOB.ARCHER:
                    return 1.10f;

                case PC_JOB.WIZARD:
                    return 1.20f;

                case PC_JOB.SHAMAN:
                    return 1.25f;//?

                case PC_JOB.VATES:
                    return 1.15f;

                case PC_JOB.WARLOCK:
                    return 1.15f;

                case PC_JOB.RANGER:
                    return 1.05f;

                case PC_JOB.MERCHANT:
                    return 1.10f;

                case PC_JOB.FARMASIST:
                    return 1.10f;

                case PC_JOB.TATARABE:
                    return 1.10f;

                //2次職エキスパート
                case PC_JOB.BLADEMASTER:
                    return 1.25f;

                case PC_JOB.KNIGHT:
                    return 1.30f;

                case PC_JOB.ASSASSIN:
                    return 1.30f;

                case PC_JOB.STRIKER:
                    return 1.40f;

                case PC_JOB.SORCERER:
                    return 2.35f;

                case PC_JOB.ELEMENTER:
                    return 2.40f;

                case PC_JOB.DRUID:
                    return 2.20f;

                case PC_JOB.CABALIST:
                    return 2;

                case PC_JOB.BREEDER:
                case PC_JOB.GARDNER:
                case PC_JOB.BLACKSMITH:
                case PC_JOB.MARIONEST:
                    return 1.20f;

                case PC_JOB.ALCHEMIST:
                    return 1.50f;

                case PC_JOB.EXPLORER:
                    return 1.30f;

                case PC_JOB.TRADER:
                    return 1.30f;

                //2次職テクニカル
                case PC_JOB.BOUNTYHUNTER:
                    return 1.25f; //?

                case PC_JOB.DARKSTALKER:
                    return 1.25f;

                case PC_JOB.COMMAND:
                    return 1.25f;//?

                case PC_JOB.GUNNER:
                    return 1.25f;

                case PC_JOB.SAGE:
                    return 2.30f;

                case PC_JOB.ENCHANTER:
                    return 2.35f;

                case PC_JOB.BARD:
                    return 2.10f;

                case PC_JOB.NECROMANCER:
                    return 2.30f;

                case PC_JOB.MACHINERY:
                    return 1.50f;

                case PC_JOB.TREASUREHUNTER:
                    return 1.50f;

                case PC_JOB.GAMBLER:
                    return 1.90f;

                //3转职业补正
                case PC_JOB.GLADIATOR:
                    return 1.49f;
                case PC_JOB.GUARDIAN:
                    return 1.61f;
                case PC_JOB.ERASER:
                    return 1.61f;
                case PC_JOB.HAWKEYE:
                    return 1.78f;
                case PC_JOB.FORCEMASTER:
                    return 4.60f;
                case PC_JOB.ASTRALIST:
                    return 4.61f;
                case PC_JOB.CARDINAL:
                    return 4.21f;
                case PC_JOB.SOULTAKER:
                    return 4.60f;
                case PC_JOB.MAESTRO:
                    return 2.14f;
                case PC_JOB.HARVEST:
                    return 4.20f;
                case PC_JOB.STRIDER:
                    return 2.05f;
                case PC_JOB.ROYALDEALER:
                    return 3.28f;
                case PC_JOB.JOKER:
                    return 3f;

                default:
                    return 1;
            }
        }

        float SPJobFactor(ActorPC pc)
        {
            PC_JOB job;
            if (pc.JobJoint == PC_JOB.NONE)
                job = pc.Job;
            else
                job = pc.JobJoint;
            switch (job)
            {
                //1次職
                case PC_JOB.NOVICE:
                    return 1.00f;

                case PC_JOB.SWORDMAN:
                    return 1.10f;

                case PC_JOB.FENCER:
                    return 1.15f;

                case PC_JOB.SCOUT:
                    return 1.20f;

                case PC_JOB.ARCHER:
                    return 1.15f;

                case PC_JOB.WIZARD:
                    return 1.05f;

                case PC_JOB.SHAMAN:
                    return 1.10f;

                case PC_JOB.VATES:
                    return 1.10f;

                case PC_JOB.WARLOCK:
                    return 1.10f;

                case PC_JOB.RANGER:
                    return 1.15f;

                case PC_JOB.MERCHANT:
                    return 1.10f;

                case PC_JOB.FARMASIST:
                    return 1.10f;

                case PC_JOB.TATARABE:
                    return 1.15f;

                //2次職エキスパート
                case PC_JOB.BLADEMASTER:
                    return 1.75f;

                case PC_JOB.KNIGHT:
                    return 1.50f;

                case PC_JOB.ASSASSIN:
                    return 1.70f;

                case PC_JOB.STRIKER:
                    return 1.60f;

                case PC_JOB.SORCERER:
                    return 1.25f;

                case PC_JOB.ELEMENTER:
                    return 1.20f;

                case PC_JOB.DRUID:
                    return 1.35f;

                case PC_JOB.CABALIST:
                    return 1.40f;

                case PC_JOB.BREEDER:
                case PC_JOB.GARDNER:
                case PC_JOB.BLACKSMITH:
                case PC_JOB.MARIONEST:
                    return 1.60f;

                case PC_JOB.ALCHEMIST:
                    return 1.80f;

                case PC_JOB.EXPLORER:
                    return 1.85f;

                case PC_JOB.TRADER:
                    return 1.90f;

                //2次職テクニカル
                case PC_JOB.BOUNTYHUNTER:
                    return 1.80f; //?

                case PC_JOB.DARKSTALKER:
                    return 1.70f;

                case PC_JOB.COMMAND:
                    return 1.80f;//?

                case PC_JOB.GUNNER:
                    return 2.30f;//?

                case PC_JOB.SAGE:
                    return 1.25f;//?

                case PC_JOB.ENCHANTER:
                    return 1.25f;

                case PC_JOB.BARD:
                    return 1.25f;

                case PC_JOB.NECROMANCER:
                    return 1.35f;//?

                case PC_JOB.MACHINERY:
                    return 1.90f;

                case PC_JOB.TREASUREHUNTER:
                    return 2.10f;

                case PC_JOB.GAMBLER:
                    return 1.70f;

                //3转职业补正
                case PC_JOB.GLADIATOR:
                    return 2.95f;
                case PC_JOB.GUARDIAN:
                    return 2.82f;
                case PC_JOB.ERASER:
                    return 3.85f;
                case PC_JOB.HAWKEYE:
                    return 4.6f;
                case PC_JOB.FORCEMASTER:
                    return 1.49f;
                case PC_JOB.ASTRALIST:
                    return 1.56f;
                case PC_JOB.CARDINAL:
                    return 1.66f;
                case PC_JOB.SOULTAKER:
                    return 2.05f;
                case PC_JOB.MAESTRO:
                    return 3.14f;
                case PC_JOB.HARVEST:
                    return 2.82f;
                case PC_JOB.STRIDER:
                    return 4.01f;
                case PC_JOB.ROYALDEALER:
                    return 3.28f;
                case PC_JOB.JOKER:
                    return 3f;

                default:
                    return 1;
            }
        }

        void CalcStatsRev(ActorPC pc)
        {
            if (pc.JobJoint == PC_JOB.NONE)
            {
                byte joblv1 = 0;
                byte joblv2x = 0;
                byte joblv2t = 0;
                byte joblv3 = 0;
                Map map = Manager.MapManager.Instance.GetMap(pc.MapID);
                joblv1 = pc.JobLevel1;
                joblv2x = pc.JobLevel2X;
                joblv2t = pc.JobLevel2T;
                joblv3 = pc.JobLevel3;

                switch (pc.Job)
                {
                    case PC_JOB.NOVICE:
                        pc.Status.str_rev = (ushort)(joblv1 * 0.07f);
                        pc.Status.dex_rev = (ushort)(joblv1 * 0.07f);
                        pc.Status.int_rev = (ushort)(joblv1 * 0.07f);
                        pc.Status.vit_rev = (ushort)(joblv1 * 0.07f);
                        pc.Status.agi_rev = (ushort)(joblv1 * 0.07f);
                        pc.Status.mag_rev = (ushort)(joblv1 * 0.07f);
                        break;
                    case PC_JOB.SWORDMAN:
                        pc.Status.str_rev = (ushort)(joblv1 * 0.261f);
                        pc.Status.dex_rev = (ushort)(joblv1 * 0.15f);
                        pc.Status.int_rev = (ushort)(joblv1 * 0.03f);
                        pc.Status.vit_rev = (ushort)(joblv1 * 0.15f);
                        pc.Status.agi_rev = (ushort)(joblv1 * 0.19f);
                        pc.Status.mag_rev = (ushort)(joblv1 * 0.021f);
                        break;
                    case PC_JOB.FENCER:
                        pc.Status.str_rev = (ushort)(joblv1 * 0.21f);
                        pc.Status.dex_rev = (ushort)(joblv1 * 0.18f);
                        pc.Status.int_rev = (ushort)(joblv1 * 0.05f);
                        pc.Status.vit_rev = (ushort)(joblv1 * 0.18f);
                        pc.Status.agi_rev = (ushort)(joblv1 * 0.161f);
                        pc.Status.mag_rev = (ushort)(joblv1 * 0.021f);
                        break;
                    case PC_JOB.SCOUT:
                        pc.Status.str_rev = (ushort)(joblv1 * 0.20f);
                        pc.Status.dex_rev = (ushort)(joblv1 * 0.20f);
                        pc.Status.int_rev = (ushort)(joblv1 * 0.05f);
                        pc.Status.vit_rev = (ushort)(joblv1 * 0.07f);
                        pc.Status.agi_rev = (ushort)(joblv1 * 0.261f);
                        pc.Status.mag_rev = (ushort)(joblv1 * 0.021f);
                        break;
                    case PC_JOB.ARCHER:
                        pc.Status.str_rev = (ushort)(joblv1 * 0.15f);
                        pc.Status.dex_rev = (ushort)(joblv1 * 0.121f);
                        pc.Status.int_rev = (ushort)(joblv1 * 0.261f);
                        pc.Status.vit_rev = (ushort)(joblv1 * 0.10f);
                        pc.Status.agi_rev = (ushort)(joblv1 * 0.15f);
                        pc.Status.mag_rev = (ushort)(joblv1 * 0.02f);
                        break;
                    case PC_JOB.WIZARD:
                        pc.Status.str_rev = (ushort)(joblv1 * 0.021f);
                        pc.Status.dex_rev = (ushort)(joblv1 * 0.10f);
                        pc.Status.int_rev = (ushort)(joblv1 * 0.21f);
                        pc.Status.vit_rev = (ushort)(joblv1 * 0.07f);
                        pc.Status.agi_rev = (ushort)(joblv1 * 0.10f);
                        pc.Status.mag_rev = (ushort)(joblv1 * 0.30f);
                        break;
                    case PC_JOB.SHAMAN:
                        pc.Status.str_rev = (ushort)(joblv1 * 0.021f);
                        pc.Status.dex_rev = (ushort)(joblv1 * 0.13f);
                        pc.Status.int_rev = (ushort)(joblv1 * 0.18f);
                        pc.Status.vit_rev = (ushort)(joblv1 * 0.061f);
                        pc.Status.agi_rev = (ushort)(joblv1 * 0.13f);
                        pc.Status.mag_rev = (ushort)(joblv1 * 0.28f);
                        break;
                    case PC_JOB.VATES:
                        pc.Status.str_rev = (ushort)(joblv1 * 0.05f);
                        pc.Status.dex_rev = (ushort)(joblv1 * 0.11f);
                        pc.Status.int_rev = (ushort)(joblv1 * 0.25f);
                        pc.Status.vit_rev = (ushort)(joblv1 * 0.021f);
                        pc.Status.agi_rev = (ushort)(joblv1 * 0.11f);
                        pc.Status.mag_rev = (ushort)(joblv1 * 0.261f);
                        break;
                    case PC_JOB.WARLOCK:
                        pc.Status.str_rev = (ushort)(joblv1 * 0.14f);
                        pc.Status.dex_rev = (ushort)(joblv1 * 0.15f);
                        pc.Status.int_rev = (ushort)(joblv1 * 0.13f);
                        pc.Status.vit_rev = (ushort)(joblv1 * 0.081f);
                        pc.Status.agi_rev = (ushort)(joblv1 * 0.121f);
                        pc.Status.mag_rev = (ushort)(joblv1 * 0.18f);
                        break;
                    case PC_JOB.RANGER:
                        pc.Status.str_rev = (ushort)(joblv1 * 0.10f);
                        pc.Status.dex_rev = (ushort)(joblv1 * 0.241f);
                        pc.Status.int_rev = (ushort)(joblv1 * 0.13f);
                        pc.Status.vit_rev = (ushort)(joblv1 * 0.081f);
                        pc.Status.agi_rev = (ushort)(joblv1 * 0.221f);
                        pc.Status.mag_rev = (ushort)(joblv1 * 0.03f);
                        break;
                    case PC_JOB.MERCHANT:
                        pc.Status.str_rev = (ushort)(joblv1 * 0.121f);
                        pc.Status.dex_rev = (ushort)(joblv1 * 0.21f);
                        pc.Status.int_rev = (ushort)(joblv1 * 0.25f);
                        pc.Status.vit_rev = (ushort)(joblv1 * 0.05f);
                        pc.Status.agi_rev = (ushort)(joblv1 * 0.15f);
                        pc.Status.mag_rev = (ushort)(joblv1 * 0.021f);
                        break;
                    case PC_JOB.TATARABE:
                        pc.Status.str_rev = (ushort)(joblv1 * 0.14f);
                        pc.Status.dex_rev = (ushort)(joblv1 * 0.241f);
                        pc.Status.int_rev = (ushort)(joblv1 * 0.061f);
                        pc.Status.vit_rev = (ushort)(joblv1 * 0.18f);
                        pc.Status.agi_rev = (ushort)(joblv1 * 0.14f);
                        pc.Status.mag_rev = (ushort)(joblv1 * 0.021f);
                        break;
                    case PC_JOB.FARMASIST:
                        pc.Status.str_rev = (ushort)(joblv1 * 0.14f);
                        pc.Status.dex_rev = (ushort)(joblv1 * 0.18f);
                        pc.Status.int_rev = (ushort)(joblv1 * 0.161f);
                        pc.Status.vit_rev = (ushort)(joblv1 * 0.20f);
                        pc.Status.agi_rev = (ushort)(joblv1 * 0.081f);
                        pc.Status.mag_rev = (ushort)(joblv1 * 0.021f);
                        break;
                    // 2次エキスパート職補正値 = FLOOR((JobLv + 30) * 補正率)
                    case PC_JOB.BLADEMASTER:
                        pc.Status.str_rev = (ushort)((joblv2x + 30) * 0.30f);
                        pc.Status.dex_rev = (ushort)((joblv2x + 30) * 0.24f);
                        pc.Status.int_rev = (ushort)((joblv2x + 30) * 0.04f);
                        pc.Status.vit_rev = (ushort)((joblv2x + 30) * 0.20f);
                        pc.Status.agi_rev = (ushort)((joblv2x + 30) * 0.20f);
                        pc.Status.mag_rev = (ushort)((joblv2x + 30) * 0.02f);
                        break;
                    case PC_JOB.KNIGHT:
                        pc.Status.str_rev = (ushort)((joblv2x + 30) * 0.17f);
                        pc.Status.dex_rev = (ushort)((joblv2x + 30) * 0.20f);
                        pc.Status.int_rev = (ushort)((joblv2x + 30) * 0.08f);
                        pc.Status.vit_rev = (ushort)((joblv2x + 30) * 0.32f);
                        pc.Status.agi_rev = (ushort)((joblv2x + 30) * 0.14f);
                        pc.Status.mag_rev = (ushort)((joblv2x + 30) * 0.09f);
                        break;
                    case PC_JOB.ASSASSIN:
                        pc.Status.str_rev = (ushort)((joblv2x + 30) * 0.21f);
                        pc.Status.dex_rev = (ushort)((joblv2x + 30) * 0.22f);
                        pc.Status.int_rev = (ushort)((joblv2x + 30) * 0.14f);
                        pc.Status.vit_rev = (ushort)((joblv2x + 30) * 0.11f);
                        pc.Status.agi_rev = (ushort)((joblv2x + 30) * 0.30f);
                        pc.Status.mag_rev = (ushort)((joblv2x + 30) * 0.02f);
                        break;
                    case PC_JOB.STRIKER:
                        pc.Status.str_rev = (ushort)((joblv2x + 30) * 0.21f);
                        pc.Status.dex_rev = (ushort)((joblv2x + 30) * 0.14f);
                        pc.Status.int_rev = (ushort)((joblv2x + 30) * 0.30f);
                        pc.Status.vit_rev = (ushort)((joblv2x + 30) * 0.12f);
                        pc.Status.agi_rev = (ushort)((joblv2x + 30) * 0.21f);
                        pc.Status.mag_rev = (ushort)((joblv2x + 30) * 0.02f);
                        break;
                    case PC_JOB.SORCERER:
                        pc.Status.str_rev = (ushort)((joblv2x + 30) * 0.03f);
                        pc.Status.dex_rev = (ushort)((joblv2x + 30) * 0.16f);
                        pc.Status.int_rev = (ushort)((joblv2x + 30) * 0.24f);
                        pc.Status.vit_rev = (ushort)((joblv2x + 30) * 0.15f);
                        pc.Status.agi_rev = (ushort)((joblv2x + 30) * 0.12f);
                        pc.Status.mag_rev = (ushort)((joblv2x + 30) * 0.30f);
                        break;
                    case PC_JOB.ELEMENTER:
                        pc.Status.str_rev = (ushort)((joblv2x + 30) * 0.03f);
                        pc.Status.dex_rev = (ushort)((joblv2x + 30) * 0.25f);
                        pc.Status.int_rev = (ushort)((joblv2x + 30) * 0.20f);
                        pc.Status.vit_rev = (ushort)((joblv2x + 30) * 0.08f);
                        pc.Status.agi_rev = (ushort)((joblv2x + 30) * 0.15f);
                        pc.Status.mag_rev = (ushort)((joblv2x + 30) * 0.29f);
                        break;
                    case PC_JOB.DRUID:
                        pc.Status.str_rev = (ushort)((joblv2x + 30) * 0.12f);
                        pc.Status.dex_rev = (ushort)((joblv2x + 30) * 0.16f);
                        pc.Status.int_rev = (ushort)((joblv2x + 30) * 0.26f);
                        pc.Status.vit_rev = (ushort)((joblv2x + 30) * 0.06f);
                        pc.Status.agi_rev = (ushort)((joblv2x + 30) * 0.12f);
                        pc.Status.mag_rev = (ushort)((joblv2x + 30) * 0.28f);
                        break;
                    case PC_JOB.CABALIST:
                        pc.Status.str_rev = (ushort)((joblv2x + 30) * 0.21f);
                        pc.Status.dex_rev = (ushort)((joblv2x + 30) * 0.17f);
                        pc.Status.int_rev = (ushort)((joblv2x + 30) * 0.14f);
                        pc.Status.vit_rev = (ushort)((joblv2x + 30) * 0.14f);
                        pc.Status.agi_rev = (ushort)((joblv2x + 30) * 0.09f);
                        pc.Status.mag_rev = (ushort)((joblv2x + 30) * 0.25f);
                        break;
                    case PC_JOB.BLACKSMITH:
                        pc.Status.str_rev = (ushort)((joblv2x + 30) * 0.19f);
                        pc.Status.dex_rev = (ushort)((joblv2x + 30) * 0.28f);
                        pc.Status.int_rev = (ushort)((joblv2x + 30) * 0.12f);
                        pc.Status.vit_rev = (ushort)((joblv2x + 30) * 0.23f);
                        pc.Status.agi_rev = (ushort)((joblv2x + 30) * 0.06f);
                        pc.Status.mag_rev = (ushort)((joblv2x + 30) * 0.12f);
                        break;
                    case PC_JOB.ALCHEMIST:
                        pc.Status.str_rev = (ushort)((joblv2x + 30) * 0.16f);
                        pc.Status.dex_rev = (ushort)((joblv2x + 30) * 0.25f);
                        pc.Status.int_rev = (ushort)((joblv2x + 30) * 0.24f);
                        pc.Status.vit_rev = (ushort)((joblv2x + 30) * 0.21f);
                        pc.Status.agi_rev = (ushort)((joblv2x + 30) * 0.08f);
                        pc.Status.mag_rev = (ushort)((joblv2x + 30) * 0.06f);
                        break;
                    case PC_JOB.EXPLORER:
                        pc.Status.str_rev = (ushort)((joblv2x + 30) * 0.14f);
                        pc.Status.dex_rev = (ushort)((joblv2x + 30) * 0.27f);
                        pc.Status.int_rev = (ushort)((joblv2x + 30) * 0.16f);
                        pc.Status.vit_rev = (ushort)((joblv2x + 30) * 0.12f);
                        pc.Status.agi_rev = (ushort)((joblv2x + 30) * 0.26f);
                        pc.Status.mag_rev = (ushort)((joblv2x + 30) * 0.05f);
                        break;
                    case PC_JOB.TRADER:
                        pc.Status.str_rev = (ushort)((joblv2x + 30) * 0.15f);
                        pc.Status.dex_rev = (ushort)((joblv2x + 30) * 0.26f);
                        pc.Status.int_rev = (ushort)((joblv2x + 30) * 0.30f);
                        pc.Status.vit_rev = (ushort)((joblv2x + 30) * 0.06f);
                        pc.Status.agi_rev = (ushort)((joblv2x + 30) * 0.20f);
                        pc.Status.mag_rev = (ushort)((joblv2x + 30) * 0.03f);
                        break;
                    //2次テクニカル職
                    case PC_JOB.MARIONEST:
                        pc.Status.str_rev = (ushort)((joblv2t + 30) * 0.14f);
                        pc.Status.dex_rev = (ushort)((joblv2t + 30) * 0.28f);
                        pc.Status.int_rev = (ushort)((joblv2t + 30) * 0.22f);
                        pc.Status.vit_rev = (ushort)((joblv2t + 30) * 0.08f);
                        pc.Status.agi_rev = (ushort)((joblv2t + 30) * 0.21f);
                        pc.Status.mag_rev = (ushort)((joblv2t + 30) * 0.18f);
                        break;
                    case PC_JOB.BOUNTYHUNTER:
                        pc.Status.str_rev = (ushort)((joblv2t + 30) * 0.23f);
                        pc.Status.dex_rev = (ushort)((joblv2t + 30) * 0.23f);
                        pc.Status.int_rev = (ushort)((joblv2t + 30) * 0.12f);
                        pc.Status.vit_rev = (ushort)((joblv2t + 30) * 0.15f);
                        pc.Status.agi_rev = (ushort)((joblv2t + 30) * 0.25f);
                        pc.Status.mag_rev = (ushort)((joblv2t + 30) * 0.02f);
                        break;
                    case PC_JOB.DARKSTALKER:
                        pc.Status.str_rev = (ushort)((joblv2t + 30) * 0.18f);
                        pc.Status.dex_rev = (ushort)((joblv2t + 30) * 0.23f);
                        pc.Status.int_rev = (ushort)((joblv2t + 30) * 0.08f);
                        pc.Status.vit_rev = (ushort)((joblv2t + 30) * 0.24f);
                        pc.Status.agi_rev = (ushort)((joblv2t + 30) * 0.17f);
                        pc.Status.mag_rev = (ushort)((joblv2t + 30) * 0.10f);
                        break;
                    case PC_JOB.COMMAND:
                        pc.Status.str_rev = (ushort)((joblv2t + 30) * 0.22f);
                        pc.Status.dex_rev = (ushort)((joblv2t + 30) * 0.24f);
                        pc.Status.int_rev = (ushort)((joblv2t + 30) * 0.16f);
                        pc.Status.vit_rev = (ushort)((joblv2t + 30) * 0.12f);
                        pc.Status.agi_rev = (ushort)((joblv2t + 30) * 0.23f);
                        pc.Status.mag_rev = (ushort)((joblv2t + 30) * 0.03f);
                        break;
                    case PC_JOB.GUNNER:
                        pc.Status.str_rev = (ushort)((joblv2t + 30) * 0.20f);
                        pc.Status.dex_rev = (ushort)((joblv2t + 30) * 0.16f);
                        pc.Status.int_rev = (ushort)((joblv2t + 30) * 0.30f);
                        pc.Status.vit_rev = (ushort)((joblv2t + 30) * 0.12f);
                        pc.Status.agi_rev = (ushort)((joblv2t + 30) * 0.16f);
                        pc.Status.mag_rev = (ushort)((joblv2t + 30) * 0.06f);
                        break;
                    case PC_JOB.SAGE:
                        pc.Status.str_rev = (ushort)((joblv2t + 30) * 0.06f);
                        pc.Status.dex_rev = (ushort)((joblv2t + 30) * 0.24f);
                        pc.Status.int_rev = (ushort)((joblv2t + 30) * 0.24f);
                        pc.Status.vit_rev = (ushort)((joblv2t + 30) * 0.06f);
                        pc.Status.agi_rev = (ushort)((joblv2t + 30) * 0.14f);
                        pc.Status.mag_rev = (ushort)((joblv2t + 30) * 0.26f);
                        break;
                    case PC_JOB.ENCHANTER:
                        pc.Status.str_rev = (ushort)((joblv2t + 30) * 0.05f);
                        pc.Status.dex_rev = (ushort)((joblv2t + 30) * 0.27f);
                        pc.Status.int_rev = (ushort)((joblv2t + 30) * 0.25f);
                        pc.Status.vit_rev = (ushort)((joblv2t + 30) * 0.06f);
                        pc.Status.agi_rev = (ushort)((joblv2t + 30) * 0.10f);
                        pc.Status.mag_rev = (ushort)((joblv2t + 30) * 0.27f);
                        break;
                    case PC_JOB.BARD:
                        pc.Status.str_rev = (ushort)((joblv2t + 30) * 0.08f);
                        pc.Status.dex_rev = (ushort)((joblv2t + 30) * 0.20f);
                        pc.Status.int_rev = (ushort)((joblv2t + 30) * 0.22f);
                        pc.Status.vit_rev = (ushort)((joblv2t + 30) * 0.10f);
                        pc.Status.agi_rev = (ushort)((joblv2t + 30) * 0.20f);
                        pc.Status.mag_rev = (ushort)((joblv2t + 30) * 0.20f);
                        break;
                    case PC_JOB.NECROMANCER:
                        pc.Status.str_rev = (ushort)((joblv2t + 30) * 0.14f);
                        pc.Status.dex_rev = (ushort)((joblv2t + 30) * 0.13f);
                        pc.Status.int_rev = (ushort)((joblv2t + 30) * 0.18f);
                        pc.Status.vit_rev = (ushort)((joblv2t + 30) * 0.06f);
                        pc.Status.agi_rev = (ushort)((joblv2t + 30) * 0.22f);
                        pc.Status.mag_rev = (ushort)((joblv2t + 30) * 0.27f);
                        break;
                    case PC_JOB.MACHINERY:
                        pc.Status.str_rev = (ushort)((joblv2t + 30) * 0.16f);
                        pc.Status.dex_rev = (ushort)((joblv2t + 30) * 0.23f);
                        pc.Status.int_rev = (ushort)((joblv2t + 30) * 0.21f);
                        pc.Status.vit_rev = (ushort)((joblv2t + 30) * 0.16f);
                        pc.Status.agi_rev = (ushort)((joblv2t + 30) * 0.20f);
                        pc.Status.mag_rev = (ushort)((joblv2t + 30) * 0.04f);
                        break; ;
                    case PC_JOB.TREASUREHUNTER:
                        pc.Status.str_rev = (ushort)((joblv2t + 30) * 0.08f);
                        pc.Status.dex_rev = (ushort)((joblv2t + 30) * 0.16f);
                        pc.Status.int_rev = (ushort)((joblv2t + 30) * 0.16f);
                        pc.Status.vit_rev = (ushort)((joblv2t + 30) * 0.25f);
                        pc.Status.agi_rev = (ushort)((joblv2t + 30) * 0.27f);
                        pc.Status.mag_rev = (ushort)((joblv2t + 30) * 0.08f);
                        break;
                    case PC_JOB.GAMBLER:
                        pc.Status.str_rev = (ushort)((joblv2t + 30) * 0.10f);
                        pc.Status.dex_rev = (ushort)((joblv2t + 30) * 0.20f);
                        pc.Status.int_rev = (ushort)((joblv2t + 30) * 0.21f);
                        pc.Status.vit_rev = (ushort)((joblv2t + 30) * 0.14f);
                        pc.Status.agi_rev = (ushort)((joblv2t + 30) * 0.26f);
                        pc.Status.mag_rev = (ushort)((joblv2t + 30) * 0.09f);
                        break;
                    //3转职业属性补正
                    case PC_JOB.GLADIATOR:
                        pc.Status.str_rev = (ushort)((joblv3 + 30) * 0.36f);
                        pc.Status.dex_rev = (ushort)((joblv3 + 30) * 0.27f);
                        pc.Status.int_rev = (ushort)((joblv3 + 30) * 0.121f);
                        pc.Status.vit_rev = (ushort)((joblv3 + 30) * 0.23f);
                        pc.Status.agi_rev = (ushort)((joblv3 + 30) * 0.3f);
                        pc.Status.mag_rev = (ushort)((joblv3 + 30) * 0.0204f);
                        break;
                    case PC_JOB.GUARDIAN:
                        pc.Status.str_rev = (ushort)((joblv3 + 30) * 0.27f);
                        pc.Status.dex_rev = (ushort)((joblv3 + 30) * 0.26f);
                        pc.Status.int_rev = (ushort)((joblv3 + 30) * 0.08f);
                        pc.Status.vit_rev = (ushort)((joblv3 + 30) * 0.38f);
                        pc.Status.agi_rev = (ushort)((joblv3 + 30) * 0.21f);
                        pc.Status.mag_rev = (ushort)((joblv3 + 30) * 0.10f);
                        break;
                    case PC_JOB.ERASER:
                        pc.Status.str_rev = (ushort)((joblv3 + 30) * 0.33f);
                        pc.Status.dex_rev = (ushort)((joblv3 + 30) * 0.28f);
                        pc.Status.int_rev = (ushort)((joblv3 + 30) * 0.16f);
                        pc.Status.vit_rev = (ushort)((joblv3 + 30) * 0.14f);
                        pc.Status.agi_rev = (ushort)((joblv3 + 30) * 0.36f);
                        pc.Status.mag_rev = (ushort)((joblv3 + 30) * 0.03f);
                        break;
                    case PC_JOB.HAWKEYE:
                        pc.Status.str_rev = (ushort)((joblv3 + 30) * 0.30f);
                        pc.Status.dex_rev = (ushort)((joblv3 + 30) * 0.16f);
                        pc.Status.int_rev = (ushort)((joblv3 + 30) * 0.37f);
                        pc.Status.vit_rev = (ushort)((joblv3 + 30) * 0.13f);
                        pc.Status.agi_rev = (ushort)((joblv3 + 30) * 0.28f);
                        pc.Status.mag_rev = (ushort)((joblv3 + 30) * 0.06f);
                        break;
                    case PC_JOB.FORCEMASTER:
                        pc.Status.str_rev = (ushort)((joblv3 + 30) * 0.06f);
                        pc.Status.dex_rev = (ushort)((joblv3 + 30) * 0.30f);
                        pc.Status.int_rev = (ushort)((joblv3 + 30) * 0.27f);
                        pc.Status.vit_rev = (ushort)((joblv3 + 30) * 0.15f);
                        pc.Status.agi_rev = (ushort)((joblv3 + 30) * 0.16f);
                        pc.Status.mag_rev = (ushort)((joblv3 + 30) * 0.36f);
                        break;
                    case PC_JOB.ASTRALIST:
                        pc.Status.str_rev = (ushort)((joblv3 + 30) * 0.05f);
                        pc.Status.dex_rev = (ushort)((joblv3 + 30) * 0.33f);
                        pc.Status.int_rev = (ushort)((joblv3 + 30) * 0.28f);
                        pc.Status.vit_rev = (ushort)((joblv3 + 30) * 0.12f);
                        pc.Status.agi_rev = (ushort)((joblv3 + 30) * 0.18f);
                        pc.Status.mag_rev = (ushort)((joblv3 + 30) * 0.34f);
                        break;
                    case PC_JOB.CARDINAL:
                        pc.Status.str_rev = (ushort)((joblv3 + 30) * 0.12f);
                        pc.Status.dex_rev = (ushort)((joblv3 + 30) * 0.28f);
                        pc.Status.int_rev = (ushort)((joblv3 + 30) * 0.26f);
                        pc.Status.vit_rev = (ushort)((joblv3 + 30) * 0.12f);
                        pc.Status.agi_rev = (ushort)((joblv3 + 30) * 0.20f);
                        pc.Status.mag_rev = (ushort)((joblv3 + 30) * 0.32f);
                        break;
                    case PC_JOB.SOULTAKER:
                        pc.Status.str_rev = (ushort)((joblv3 + 30) * 0.23f);
                        pc.Status.dex_rev = (ushort)((joblv3 + 30) * 0.19f);
                        pc.Status.int_rev = (ushort)((joblv3 + 30) * 0.21f);
                        pc.Status.vit_rev = (ushort)((joblv3 + 30) * 0.15f);
                        pc.Status.agi_rev = (ushort)((joblv3 + 30) * 0.22f);
                        pc.Status.mag_rev = (ushort)((joblv3 + 30) * 0.30f);
                        break;
                    case PC_JOB.MAESTRO:
                        pc.Status.str_rev = (ushort)((joblv3 + 30) * 0.25f);
                        pc.Status.dex_rev = (ushort)((joblv3 + 30) * 0.29f);
                        pc.Status.int_rev = (ushort)((joblv3 + 30) * 0.21f);
                        pc.Status.vit_rev = (ushort)((joblv3 + 30) * 0.23f);
                        pc.Status.agi_rev = (ushort)((joblv3 + 30) * 0.20f);
                        pc.Status.mag_rev = (ushort)((joblv3 + 30) * 0.12f);
                        break;
                    case PC_JOB.HARVEST:
                        pc.Status.str_rev = (ushort)((joblv3 + 30) * 0.18f);
                        pc.Status.dex_rev = (ushort)((joblv3 + 30) * 0.25f);
                        pc.Status.int_rev = (ushort)((joblv3 + 30) * 0.24f);
                        pc.Status.vit_rev = (ushort)((joblv3 + 30) * 0.22f);
                        pc.Status.agi_rev = (ushort)((joblv3 + 30) * 0.21f);
                        pc.Status.mag_rev = (ushort)((joblv3 + 30) * 0.20f);
                        break;
                    case PC_JOB.STRIDER:
                        pc.Status.str_rev = (ushort)((joblv3 + 30) * 0.21f);
                        pc.Status.dex_rev = (ushort)((joblv3 + 30) * 0.33f);
                        pc.Status.int_rev = (ushort)((joblv3 + 30) * 0.16f);
                        pc.Status.vit_rev = (ushort)((joblv3 + 30) * 0.25f);
                        pc.Status.agi_rev = (ushort)((joblv3 + 30) * 0.27f);
                        pc.Status.mag_rev = (ushort)((joblv3 + 30) * 0.08f);
                        break;
                    case PC_JOB.ROYALDEALER:
                        pc.Status.str_rev = (ushort)((joblv3 + 30) * 0.23f);
                        pc.Status.dex_rev = (ushort)((joblv3 + 30) * 0.26f);
                        pc.Status.int_rev = (ushort)((joblv3 + 30) * 0.30f);
                        pc.Status.vit_rev = (ushort)((joblv3 + 30) * 0.15f);
                        pc.Status.agi_rev = (ushort)((joblv3 + 30) * 0.27f);
                        pc.Status.mag_rev = (ushort)((joblv3 + 30) * 0.09f);
                        break;
                    case PC_JOB.JOKER:
                        pc.Status.str_rev = (ushort)((joblv3 + 30) * 0.20f);
                        pc.Status.dex_rev = (ushort)((joblv3 + 30) * 0.20f);
                        pc.Status.int_rev = (ushort)((joblv3 + 30) * 0.20f);
                        pc.Status.vit_rev = (ushort)((joblv3 + 30) * 0.20f);
                        pc.Status.agi_rev = (ushort)((joblv3 + 30) * 0.20f);
                        pc.Status.mag_rev = (ushort)((joblv3 + 30) * 0.20f);
                        break;
                }
            }
            else
            {
                switch (pc.JobJoint)
                {
                    case PC_JOB.BREEDER:
                        pc.Status.str_rev = (ushort)(3 + (pc.JointJobLevel + 3) * 0.143);
                        pc.Status.dex_rev = (ushort)(6 + (pc.JointJobLevel + 1) * 0.25);
                        pc.Status.int_rev = (ushort)(1 + (pc.JointJobLevel) * 0.04);
                        pc.Status.vit_rev = (ushort)(6 + (pc.JointJobLevel + 1) * 0.25);
                        pc.Status.agi_rev = (ushort)(7 + (pc.JointJobLevel) * 0.28);
                        pc.Status.mag_rev = (ushort)(1 + (pc.JointJobLevel) * 0.04);
                        break;
                    case PC_JOB.GARDNER:
                        pc.Status.str_rev = (ushort)(3 + (pc.JointJobLevel + 2) * 0.125);
                        pc.Status.dex_rev = (ushort)(6 + (pc.JointJobLevel + 1) * 0.25);
                        pc.Status.int_rev = (ushort)(6 + (pc.JointJobLevel + 1) * 0.25);
                        pc.Status.vit_rev = (ushort)(5 + (pc.JointJobLevel + 2) * 0.25);
                        pc.Status.agi_rev = (ushort)(3 + (pc.JointJobLevel - 1) * 0.125);
                        pc.Status.mag_rev = (ushort)((pc.JointJobLevel) * 0.04);
                        break;
                }
            }
        }
    }
}
