using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaLib;
using SagaDB.Item;
using SagaDB.Actor;
using SagaDB.Partner;

namespace SagaMap.Partner
{
    public partial class StatusFactory : Singleton<StatusFactory>
    {
        public StatusFactory()
        {
        }

        public void CalcPartnerStatus(ActorPartner partner)
        {
            ClearPartnerEquipBouns(partner);
            CalcPartnerEquipBonus(partner);
            CalcPartnerRange(partner);
            CalcPartnerHPMPSP(partner);
            CalcPartnerStats(partner);
        }

        /// <summary>
        /// 计算普通攻击距离
        /// </summary>
        /// <param name="pc"></param>
        public void CalcPartnerRange(ActorPartner partner)
        {
            Dictionary<EnumPartnerEquipSlot, Item> equips = partner.equipments;
            if (equips.ContainsKey(EnumPartnerEquipSlot.WEAPON))
            {
                Item item = equips[EnumPartnerEquipSlot.WEAPON];
                partner.Range = item.BaseData.range;
            }
            else
            {
                partner.Range = (uint)(partner.BaseData.range);
            }
        }

        ushort checkPositive(double num)
        {
            if (num > 0)
                return (ushort)num;
            return 0;
        }

        //ECOKEY 等差計算修正
        private float CalcPartnerAndPlayerLevelDiff(byte partnerlv, byte playerlv)
        {
            if (partnerlv > playerlv)
            {
                var lvdelte = Math.Abs(partnerlv - playerlv);
                if (lvdelte >= 0 && lvdelte <= 30)
                    return 1.0f;
                else if (lvdelte > 30 && lvdelte <= 60)
                    return 0.8f;
                else if (lvdelte > 60 && lvdelte <= 90)
                    return 0.6f;
                else if (lvdelte > 90 && lvdelte <= 100)
                    return 0.4f;
                else if (lvdelte > 100)
                    return 0.2f;
                else
                    return 0.1f;
            }
            return 1.0f;
        }

        /*    /// <summary>
            /// 计算素质属性能力
            /// </summary>
            /// <param name="partner">Partner对象</param>
            public void CalcPartnerStats(ActorPartner partner)
            {
                var lv = (byte)(partner.Level / 10 * 10);
                partner.Status.min_atk_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.atk_min_fn - partner.BaseData.atk_min_in) / 109 * partner.Level)) + (partner.BaseData.atk_min_in + partner.perk0 * 1) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].atk_min : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.max_atk_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.atk_max_fn - partner.BaseData.atk_max_in) / 109 * partner.Level)) + (partner.BaseData.atk_max_in + partner.perk0 * 1) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].atk_max : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.min_matk_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.matk_min_fn - partner.BaseData.matk_min_in) / 109 * partner.Level)) + (partner.BaseData.matk_min_in + partner.perk1 * 1) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].matk_min : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.max_matk_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.matk_max_fn - partner.BaseData.matk_max_in) / 109 * partner.Level)) + (partner.BaseData.matk_max_in + partner.perk1 * 1) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].matk_max : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.def_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.def_fn - partner.BaseData.def_in) / 109 * partner.Level)) + (partner.BaseData.def_in + partner.perk2 / 10) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].def : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.def_add_bs = (short)checkPositive(Math.Ceiling((float)((partner.BaseData.def_add_fn - partner.BaseData.def_add_in) / 109 * partner.Level)) + (partner.BaseData.def_add_in + partner.perk2 * 1) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].def_add : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.mdef_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.mdef_fn - partner.BaseData.mdef_in) / 109 * partner.Level)) + (partner.BaseData.mdef_in + partner.perk3 / 10) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].mdef : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.mdef_add_bs = (short)checkPositive(Math.Ceiling((float)((partner.BaseData.mdef_add_fn - partner.BaseData.mdef_add_in) / 109 * partner.Level)) + (partner.BaseData.mdef_add_in + partner.perk3 * 1) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].mdef_add : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));

                if (partner.Status.def_bs > 40)
                    partner.Status.def_bs = 40;
                if (partner.Status.mdef_bs > 40)
                    partner.Status.mdef_bs = 40;

                partner.Status.hit_melee_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.hit_melee_fn - partner.BaseData.hit_melee_in) / 109 * partner.Level)) + (partner.BaseData.hit_melee_in + partner.perk4 * 3) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].hit_melee : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.hit_ranged_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.hit_ranged_fn - partner.BaseData.hit_ranged_in) / 109 * partner.Level)) + (partner.BaseData.hit_ranged_in + partner.perk4 * 3) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].hit_ranged : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.hit_magic_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.hit_magic_fn - partner.BaseData.hit_magic_in) / 109 * partner.Level)) + (partner.BaseData.hit_magic_in + partner.perk4 * 3) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].hit_magic : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.hit_critical_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.hit_critical_fn - partner.BaseData.hit_critical_in) / 109 * partner.Level)) + (partner.BaseData.hit_critical_in + partner.perk4 * 3) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].hit_critical : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.avoid_melee_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.avoid_melee_fn - partner.BaseData.avoid_melee_in) / 109 * partner.Level)) + (partner.BaseData.avoid_melee_in + partner.perk5 * 3) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].avoid_melee : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.avoid_ranged_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.avoid_ranged_fn - partner.BaseData.avoid_ranged_in) / 109 * partner.Level)) + (partner.BaseData.avoid_ranged_in + partner.perk5 * 3) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].avoid_ranged : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.avoid_magic_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.avoid_magic_fn - partner.BaseData.avoid_magic_in) / 109 * partner.Level)) + (partner.BaseData.avoid_magic_in + partner.perk5 * 3) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].avoid_magic : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.avoid_critical_bs = (ushort)checkPositive(Math.Ceiling((float)((partner.BaseData.avoid_critical_fn - partner.BaseData.avoid_critical_in) / 109 * partner.Level)) + (partner.BaseData.avoid_critical_in + partner.perk5 * 3) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].avoid_critical : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.aspd_bs = (short)checkPositive(Math.Ceiling((float)((partner.BaseData.aspd_fn - partner.BaseData.aspd_in) / 109 * partner.Level)) + (partner.BaseData.aspd_in + partner.perk5 * 2) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].aspd : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
                partner.Status.cspd_bs = (short)checkPositive(Math.Ceiling((float)((partner.BaseData.cspd_fn - partner.BaseData.cspd_in) / 109 * partner.Level)) + (partner.BaseData.cspd_in + partner.perk3 * 2) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].cspd : 0) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));

                partner.Status.hp_recover_bs = (short)checkPositive(Math.Ceiling((float)((partner.BaseData.hp_rec_fn - partner.BaseData.hp_rec_in) / 109 * partner.Level)) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].hp_rec : 0));
                partner.Status.mp_recover_bs = (short)checkPositive(Math.Ceiling((float)((partner.BaseData.mp_rec_fn - partner.BaseData.mp_rec_in) / 109 * partner.Level)) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].mp_rec : 0));
                partner.Status.sp_recover_bs = (short)checkPositive(Math.Ceiling((float)((partner.BaseData.sp_rec_fn - partner.BaseData.sp_rec_in) / 109 * partner.Level)) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].sp_rec : 0));

                partner.Status.min_atk1 = (ushort)checkPositive((partner.Status.min_atk_bs + partner.Status.atk1_item + partner.Status.min_atk1_skill) * (partner.Status.atk1_rate_item / 100f) * (partner.Status.min_atk1_rate_skill / 100f) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.min_atk2 = (ushort)checkPositive((partner.Status.min_atk_bs + partner.Status.atk2_item + partner.Status.min_atk2_skill) * (partner.Status.atk2_rate_item / 100f) * (partner.Status.min_atk2_rate_skill / 100f) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.min_atk3 = (ushort)checkPositive((partner.Status.min_atk_bs + partner.Status.atk3_item + partner.Status.min_atk3_skill) * (partner.Status.atk3_rate_item / 100f) * (partner.Status.min_atk3_rate_skill / 100f) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.max_atk1 = (ushort)checkPositive((partner.Status.max_atk_bs + partner.Status.atk1_item + partner.Status.max_atk1_skill) * (partner.Status.atk1_rate_item / 100f) * (partner.Status.max_atk1_rate_skill / 100f) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.max_atk2 = (ushort)checkPositive((partner.Status.max_atk_bs + partner.Status.atk2_item + partner.Status.max_atk2_skill) * (partner.Status.atk2_rate_item / 100f) * (partner.Status.max_atk2_rate_skill / 100f) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.max_atk3 = (ushort)checkPositive((partner.Status.max_atk_bs + partner.Status.atk3_item + partner.Status.max_atk3_skill) * (partner.Status.atk3_rate_item / 100f) * (partner.Status.max_atk3_rate_skill / 100f) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.min_matk = (ushort)checkPositive((partner.Status.min_matk_bs + partner.Status.matk_item + partner.Status.min_matk_skill) * (partner.Status.matk_rate_item / 100f) * (partner.Status.min_matk_rate_skill / 100f) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.max_matk = (ushort)checkPositive((partner.Status.max_matk_bs + partner.Status.matk_item + partner.Status.max_matk_skill) * (partner.Status.matk_rate_item / 100f) * (partner.Status.max_matk_rate_skill / 100f) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.def = (ushort)checkPositive((partner.Status.def_bs + partner.Status.def_skill) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.def_add = (short)checkPositive((partner.Status.def_add_bs + partner.Status.def_item + partner.Status.def_add_item + partner.Status.def_add_skill) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.mdef = (ushort)checkPositive((partner.Status.mdef_bs + partner.Status.mdef_skill) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.mdef_add = (short)checkPositive((partner.Status.mdef_add_bs + partner.Status.mdef_item + partner.Status.mdef_add_item + partner.Status.mdef_add_skill) * FoodBouns(partner) * ReliabilityBouns(partner));
                if (partner.Status.def > 90)
                    partner.Status.def = 90;
                if (partner.Status.mdef > 90)
                    partner.Status.mdef = 90;

                partner.Status.hit_melee = (ushort)checkPositive(((partner.Status.hit_melee_bs + partner.Status.hit_melee_item + partner.Status.hit_melee_skill)) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.hit_ranged = (ushort)checkPositive(((partner.Status.hit_ranged_bs + partner.Status.hit_ranged_item + partner.Status.hit_ranged_skill)) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.hit_magic = (ushort)checkPositive(((partner.Status.hit_magic_bs + partner.Status.hit_magic_item + partner.Status.hit_magic_skill)) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.hit_critical = (ushort)checkPositive((partner.Status.hit_critical_bs + partner.Status.hit_critical_item + partner.Status.hit_critical_skill) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.avoid_melee = (ushort)checkPositive(((partner.Status.avoid_melee_bs + partner.Status.avoid_melee_item + partner.Status.avoid_melee_skill)) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.avoid_ranged = (ushort)checkPositive((partner.Status.avoid_ranged_bs + partner.Status.avoid_ranged_item + partner.Status.avoid_ranged_skill) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.avoid_magic = (ushort)checkPositive((partner.Status.avoid_magic_bs + partner.Status.avoid_magic_item + partner.Status.avoid_magic_skill) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.Status.avoid_critical = (ushort)checkPositive((partner.Status.avoid_critical_bs + partner.Status.avoid_critical_item + partner.Status.avoid_critical_skill) * FoodBouns(partner) * ReliabilityBouns(partner));
                if (partner.Status.avoid_melee > 500)
                    partner.Status.avoid_melee = 500;
                if (partner.Status.avoid_ranged > 500)
                    partner.Status.avoid_ranged = 500;
                if (partner.Status.avoid_magic > 400)
                    partner.Status.avoid_magic = 400;
                partner.Status.aspd = (short)checkPositive((partner.Status.aspd_bs + partner.Status.aspd_item + partner.Status.aspd_skill) * FoodBouns(partner) * ReliabilityBouns(partner));
                if (partner.Status.aspd > 775)
                    partner.Status.aspd = 775;
                if (partner.Status.aspd < 1)
                {
                    partner.Status.aspd = 1;
                }
                partner.Status.cspd = (short)checkPositive((partner.Status.cspd_bs + partner.Status.cspd_item + partner.Status.cspd_skill) * FoodBouns(partner) * ReliabilityBouns(partner));
                if (partner.Status.cspd > 775)
                    partner.Status.cspd = 775;
                if (partner.Status.cspd < 1)
                {
                    partner.Status.cspd = 1;
                }

                //恢复力计算
                partner.Status.hp_recover = (short)checkPositive(partner.Status.hp_recover_bs + partner.Status.hp_recover_item + partner.Status.hp_recover_skill);
                partner.Status.mp_recover = (short)checkPositive(partner.Status.mp_recover_bs + partner.Status.mp_recover_item + partner.Status.mp_recover_skill);
                partner.Status.sp_recover = (short)checkPositive(partner.Status.sp_recover_bs + partner.Status.sp_recover_item + partner.Status.sp_recover_skill);
            }
            float FoodBouns(ActorPartner partner)
            {
                if (partner.reliabilityuprate <= 100)
                    return 1f;
                return partner.reliabilityuprate / 100f;
            }
            float ReliabilityBouns(ActorPartner partner)
            {
                switch (partner.reliability)
                {
                    case 0:
                        return 1f;
                    case 1:
                        return 1.1f;
                    case 2:
                        return 1.2f;
                    case 3:
                        return 1.3f;
                    case 4:
                        return 1.4f;
                    case 5:
                        return 1.5f;
                    case 6:
                        return 1.6f;
                    case 7:
                        return 1.7f;
                    case 8:
                        return 1.8f;
                    case 9:
                        return 1.9f;
                }
                return 1f;
            }
            public void CalcPartnerHPMPSP(ActorPartner partner) //ECOKEY1
            {
                byte lv = (byte)(partner.Level / 10 * 10);
                // var hp = (partner.rebirth ? partner.BaseData.AdvanceStatus[lv].hp : 0);
                //ECOKEY 修復裝備對數值的影響 + 增加hp_skill  mp_skill  sp_skill
                partner.MaxHP = (uint)((Math.Ceiling((float)(partner.BaseData.hp_fn - partner.BaseData.hp_in) / 109 * partner.Level) + (partner.rebirth ? partner.BaseData.AdvanceStatus[(byte)lv].hp : 0) + partner.Status.hp_item + partner.Status.hp_skill + partner.BaseData.hp_in + partner.perk2 * 10) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.MaxMP = (uint)((Math.Ceiling((float)(partner.BaseData.mp_fn - partner.BaseData.mp_in) / 109 * partner.Level) + (partner.rebirth ? partner.BaseData.AdvanceStatus[(byte)lv].mp : 0) + partner.Status.mp_item + partner.Status.mp_skill + partner.BaseData.mp_in + partner.perk1 * 10) * FoodBouns(partner) * ReliabilityBouns(partner));
                partner.MaxSP = (uint)((Math.Ceiling((float)(partner.BaseData.sp_fn - partner.BaseData.sp_in) / 109 * partner.Level) + (partner.rebirth ? partner.BaseData.AdvanceStatus[(byte)lv].sp : 0) + partner.Status.sp_item + partner.Status.sp_skill + partner.BaseData.sp_in + partner.perk0 * 10) * FoodBouns(partner) * ReliabilityBouns(partner));

                if (partner.HP > partner.MaxHP) partner.HP = partner.MaxHP;
                if (partner.MP > partner.MaxMP) partner.MP = partner.MaxMP;
                if (partner.SP > partner.MaxSP) partner.SP = partner.MaxSP;

            }
            /*  public void CalcPartnerHPMPSP(ActorPartner partner) //ECOKEY1
              {
                  byte lv = (byte)(partner.Level / 10 * 10);
                  // var hp = (partner.rebirth ? partner.BaseData.AdvanceStatus[lv].hp : 0);

                   partner.MaxHP = (uint)((Math.Ceiling((float)(partner.BaseData.hp_fn - partner.BaseData.hp_in) / 109 * partner.Level) + (partner.rebirth ? partner.BaseData.AdvanceStatus[(byte)lv].hp : 0) + partner.BaseData.hp_in + partner.perk2 * 10) * FoodBouns(partner) * ReliabilityBouns(partner));
                   partner.MaxMP = (uint)((Math.Ceiling((float)(partner.BaseData.mp_fn - partner.BaseData.mp_in) / 109 * partner.Level) + (partner.rebirth ? partner.BaseData.AdvanceStatus[(byte)lv].mp : 0) + partner.BaseData.mp_in + partner.perk1 * 10) * FoodBouns(partner) * ReliabilityBouns(partner));
                   partner.MaxSP = (uint)((Math.Ceiling((float)(partner.BaseData.sp_fn - partner.BaseData.sp_in) / 109 * partner.Level) + (partner.rebirth ? partner.BaseData.AdvanceStatus[(byte)lv].sp : 0) + partner.BaseData.sp_in + partner.perk0 * 10) * FoodBouns(partner) * ReliabilityBouns(partner));

                  if (partner.HP > partner.MaxHP) partner.HP = partner.MaxHP;
                  if (partner.MP > partner.MaxMP) partner.MP = partner.MaxMP;
                  if (partner.SP > partner.MaxSP) partner.SP = partner.MaxSP;

                        }*/


        /// <summary>
        /// ECOKEY2版按照wiki計算素質屬性能力
        /// </summary>
        /// <param name="partner">Partner对象</param>
        public void CalcPartnerStats(ActorPartner partner)
        {
            PassiveSkill(partner);//ECOKEY 被動技能計算
            ushort perk0_ATKmin = (ushort)(partner.perk0 * 3 + Math.Pow(Math.Floor((float)(partner.perk0 / 8)), 2));
            ushort perk0_ATKmax = (ushort)(partner.perk0 * 5 + Math.Floor((float)((partner.perk0 + 10) / 10)) * Math.Floor((float)((partner.perk0 + 15) / 10)) - 1);
            ushort perk1_MATKmin = (ushort)(partner.perk1 * 2 + Math.Pow(Math.Floor((float)((partner.perk1 + 4) / 8)), 2));
            ushort perk1_MATKmax = (ushort)(partner.perk1 * 2 + Math.Pow(Math.Floor((float)((partner.perk1 + 5) / 7)), 2));
            ushort perk2_DEFadd = (ushort)(partner.perk2 * 2);
            ushort perk2_DEF = (ushort)(Math.Floor((float)partner.perk2 / 2) - Math.Floor(Math.Pow(Math.Floor((float)(partner.perk2 / 2)), 2) / 100) + Math.Floor((float)(partner.perk2 / 100)));
            ushort perk3_MDEFadd = (ushort)(partner.perk3 * 2 - Math.Floor((float)((partner.perk3 + 10) * 2 / 21)));
            ushort perk3_MDEF = (ushort)(Math.Floor((float)partner.perk3 / 4) - Math.Floor(Math.Pow(Math.Floor((float)(partner.perk3 / 4)), 2) / 100) + Math.Floor((float)(partner.perk3 / 100)));
            ushort perk3_CSPD = (ushort)(partner.perk3 * 6 - Math.Floor(Math.Pow(partner.perk3, 2) / 100));
            ushort perk4_HITmelee = (ushort)(partner.perk4 * 1 + Math.Floor((float)(partner.perk4 / 10)) * Math.Floor((float)(partner.perk4 / 10 + 1)));
            ushort perk4_HITranged = (ushort)(partner.perk4 * 1 + Math.Pow(Math.Floor((float)(partner.perk4 / 10)), 2));
            ushort perk5_AVOIDmelee = (ushort)(partner.perk5 * 1 + Math.Floor((float)(partner.perk5 / 10)) * Math.Floor((float)(partner.perk5 / 10 + 1)));
            ushort perk5_AVOIDranged = (ushort)(partner.perk5 * 1 + Math.Pow(Math.Floor((float)(partner.perk5 / 10)), 2));
            ushort perk5_ASPD = (ushort)(partner.perk5 * 6 - Math.Floor(Math.Pow(partner.perk5, 2) / 100));

            var lv = (byte)(partner.Level / 10 * 10);
            partner.Status.min_atk_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.atk_min_fn - partner.BaseData.atk_min_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.atk_min_in + perk0_ATKmin) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].atk_min : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.max_atk_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.atk_max_fn - partner.BaseData.atk_max_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.atk_max_in + perk0_ATKmax) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].atk_max : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.min_matk_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.matk_min_fn - partner.BaseData.matk_min_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.matk_min_in + perk1_MATKmin) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].matk_min : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.max_matk_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.matk_max_fn - partner.BaseData.matk_max_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.matk_max_in + perk1_MATKmax) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].matk_max : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.def_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.def_fn - partner.BaseData.def_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.def_in + perk2_DEF) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].def : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.def_add_bs = (short)checkPositive((Math.Ceiling((float)((partner.BaseData.def_add_fn - partner.BaseData.def_add_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.def_add_in + perk2_DEFadd) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].def_add : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.mdef_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.mdef_fn - partner.BaseData.mdef_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.mdef_in + perk3_MDEF) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].mdef : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.mdef_add_bs = (short)checkPositive((Math.Ceiling((float)((partner.BaseData.mdef_add_fn - partner.BaseData.mdef_add_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.mdef_add_in + perk3_MDEFadd) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].mdef_add : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));

            partner.Status.hit_melee_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.hit_melee_fn - partner.BaseData.hit_melee_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.hit_melee_in + perk4_HITmelee) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].hit_melee : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.hit_ranged_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.hit_ranged_fn - partner.BaseData.hit_ranged_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.hit_ranged_in + perk4_HITranged) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].hit_ranged : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.hit_magic_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.hit_magic_fn - partner.BaseData.hit_magic_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.hit_magic_in) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].hit_magic : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.hit_critical_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.hit_critical_fn - partner.BaseData.hit_critical_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.hit_critical_in) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].hit_critical : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.avoid_melee_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.avoid_melee_fn - partner.BaseData.avoid_melee_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.avoid_melee_in + perk5_AVOIDmelee) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].avoid_melee : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.avoid_ranged_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.avoid_ranged_fn - partner.BaseData.avoid_ranged_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.avoid_ranged_in + perk5_AVOIDranged) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].avoid_ranged : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.avoid_magic_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.avoid_magic_fn - partner.BaseData.avoid_magic_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.avoid_magic_in) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].avoid_magic : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.avoid_critical_bs = (ushort)checkPositive((Math.Ceiling((float)((partner.BaseData.avoid_critical_fn - partner.BaseData.avoid_critical_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.avoid_critical_in) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].avoid_critical : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.aspd_bs = (short)checkPositive((Math.Ceiling((float)((partner.BaseData.aspd_fn - partner.BaseData.aspd_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.aspd_in + perk5_ASPD) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].aspd : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));
            partner.Status.cspd_bs = (short)checkPositive((Math.Ceiling((float)((partner.BaseData.cspd_fn - partner.BaseData.cspd_in) / 109.0f * (partner.Level - 1))) + (partner.BaseData.cspd_in + perk3_CSPD) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].cspd : 0)) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level));

            //ECOKEY 寵物恢復率修正
            partner.Status.hp_recover_bs = (short)checkPositive(100 + Math.Ceiling((float)((partner.BaseData.hp_rec_fn - partner.BaseData.hp_rec_in) / 109.0f * (partner.Level - 1))) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].hp_rec : 0));
            partner.Status.mp_recover_bs = (short)checkPositive(100 + Math.Ceiling((float)((partner.BaseData.mp_rec_fn - partner.BaseData.mp_rec_in) / 109.0f * (partner.Level - 1))) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].mp_rec : 0));
            partner.Status.sp_recover_bs = (short)checkPositive(100 + Math.Ceiling((float)((partner.BaseData.sp_rec_fn - partner.BaseData.sp_rec_in) / 109.0f * (partner.Level - 1))) + (ushort)(partner.rebirth ? partner.BaseData.AdvanceStatus[lv].sp_rec : 0));
            //基準上限
            if (partner.Status.def_bs > 40)
                partner.Status.def_bs = 40;
            if (partner.Status.mdef_bs > 40)
                partner.Status.mdef_bs = 40;
            if (partner.Status.hit_melee_bs > 500)
                partner.Status.hit_melee_bs = 500;
            if (partner.Status.hit_ranged_bs > 500)
                partner.Status.hit_ranged_bs = 500;
            if (partner.Status.avoid_melee_bs > 500)
                partner.Status.avoid_melee_bs = 500;
            if (partner.Status.avoid_ranged_bs > 500)
                partner.Status.avoid_ranged_bs = 500;

            //成長カンスト値（信賴度上限）
            partner.Status.hit_melee_bs += (ushort)ReliabilityBouns_ADD(partner, "HITmelee");
            partner.Status.hit_ranged_bs += (ushort)ReliabilityBouns_ADD(partner, "HITranged");
            partner.Status.avoid_melee_bs += (ushort)ReliabilityBouns_ADD(partner, "AVOIDmelee");
            partner.Status.avoid_ranged_bs += (ushort)ReliabilityBouns_ADD(partner, "AVOIDranged");
            if (partner.Status.hit_melee_bs > 700)
                partner.Status.hit_melee_bs = 700;
            if (partner.Status.hit_ranged_bs > 750)
                partner.Status.hit_ranged_bs = 750;
            if (partner.Status.avoid_melee_bs > 600)
                partner.Status.avoid_melee_bs = 600;
            if (partner.Status.avoid_ranged_bs > 600)
                partner.Status.avoid_ranged_bs = 600;

            //補正込み上限（其他補正上限）
            partner.Status.min_atk1 = (ushort)checkPositive((partner.Status.min_atk_bs + ReliabilityBouns_ADD(partner, "ATKmin") + partner.Status.atk1_item + partner.Status.min_atk1_skill) * (partner.Status.atk1_rate_item / 100f) * (partner.Status.min_atk1_rate_skill / 100f) * FoodBouns(partner));
            partner.Status.min_atk2 = (ushort)checkPositive((partner.Status.min_atk_bs + ReliabilityBouns_ADD(partner, "ATKmin") + partner.Status.atk2_item + partner.Status.min_atk2_skill) * (partner.Status.atk2_rate_item / 100f) * (partner.Status.min_atk2_rate_skill / 100f) * FoodBouns(partner));
            partner.Status.min_atk3 = (ushort)checkPositive((partner.Status.min_atk_bs + ReliabilityBouns_ADD(partner, "ATKmin") + partner.Status.atk3_item + partner.Status.min_atk3_skill) * (partner.Status.atk3_rate_item / 100f) * (partner.Status.min_atk3_rate_skill / 100f) * FoodBouns(partner));
            partner.Status.max_atk1 = (ushort)checkPositive((partner.Status.max_atk_bs + ReliabilityBouns_ADD(partner, "ATKmax") + partner.Status.atk1_item + partner.Status.max_atk1_skill) * (partner.Status.atk1_rate_item / 100f) * (partner.Status.max_atk1_rate_skill / 100f) * FoodBouns(partner));
            partner.Status.max_atk2 = (ushort)checkPositive((partner.Status.max_atk_bs + ReliabilityBouns_ADD(partner, "ATKmax") + partner.Status.atk2_item + partner.Status.max_atk2_skill) * (partner.Status.atk2_rate_item / 100f) * (partner.Status.max_atk2_rate_skill / 100f) * FoodBouns(partner));
            partner.Status.max_atk3 = (ushort)checkPositive((partner.Status.max_atk_bs + ReliabilityBouns_ADD(partner, "ATKmax") + partner.Status.atk3_item + partner.Status.max_atk3_skill) * (partner.Status.atk3_rate_item / 100f) * (partner.Status.max_atk3_rate_skill / 100f) * FoodBouns(partner));
            partner.Status.min_matk = (ushort)checkPositive((partner.Status.min_matk_bs + ReliabilityBouns_ADD(partner, "MATKmin") + partner.Status.matk_item + partner.Status.min_matk_skill) * (partner.Status.matk_rate_item / 100f) * (partner.Status.min_matk_rate_skill / 100f) * FoodBouns(partner));
            partner.Status.max_matk = (ushort)checkPositive((partner.Status.max_matk_bs + ReliabilityBouns_ADD(partner, "MATKmax") + partner.Status.matk_item + partner.Status.max_matk_skill) * (partner.Status.matk_rate_item / 100f) * (partner.Status.max_matk_rate_skill / 100f) * FoodBouns(partner));
            //防禦計算修正（下面四列def def_add mdef mdef_add）
            partner.Status.def = (ushort)checkPositive((partner.Status.def_bs + partner.Status.def_add_item + partner.Status.def_skill) * FoodBouns(partner));
            partner.Status.def_add = (short)checkPositive((partner.Status.def_add_bs + ReliabilityBouns_ADD(partner, "DEF") + partner.Status.def_item + partner.Status.def_add_skill) * FoodBouns(partner));
            partner.Status.mdef = (ushort)checkPositive((partner.Status.mdef_bs + partner.Status.mdef_add_item + partner.Status.mdef_skill) * FoodBouns(partner));
            partner.Status.mdef_add = (short)checkPositive((partner.Status.mdef_add_bs + ReliabilityBouns_ADD(partner, "MDEF") + partner.Status.mdef_item + partner.Status.mdef_add_skill) * FoodBouns(partner));
            if (partner.Status.def > 90)
                partner.Status.def = 90;
            if (partner.Status.mdef > 90)
                partner.Status.mdef = 90;

            partner.Status.hit_melee = (ushort)checkPositive(((partner.Status.hit_melee_bs + partner.Status.hit_melee_item + partner.Status.hit_melee_skill)) * FoodBouns(partner));
            partner.Status.hit_ranged = (ushort)checkPositive(((partner.Status.hit_ranged_bs + partner.Status.hit_ranged_item + partner.Status.hit_ranged_skill)) * FoodBouns(partner));
            partner.Status.hit_magic = (ushort)checkPositive(((partner.Status.hit_magic_bs + partner.Status.hit_magic_item + partner.Status.hit_magic_skill)) * FoodBouns(partner));
            partner.Status.hit_critical = (ushort)checkPositive((partner.Status.hit_critical_bs + partner.Status.hit_critical_item + partner.Status.hit_critical_skill) * FoodBouns(partner));
            partner.Status.avoid_melee = (ushort)checkPositive(((partner.Status.avoid_melee_bs + partner.Status.avoid_melee_item + partner.Status.avoid_melee_skill)) * FoodBouns(partner));
            partner.Status.avoid_ranged = (ushort)checkPositive((partner.Status.avoid_ranged_bs + partner.Status.avoid_ranged_item + partner.Status.avoid_ranged_skill) * FoodBouns(partner));
            partner.Status.avoid_magic = (ushort)checkPositive((partner.Status.avoid_magic_bs + partner.Status.avoid_magic_item + partner.Status.avoid_magic_skill) * FoodBouns(partner));
            partner.Status.avoid_critical = (ushort)checkPositive((partner.Status.avoid_critical_bs + partner.Status.avoid_critical_item + partner.Status.avoid_critical_skill) * FoodBouns(partner));
            if (partner.Status.avoid_melee > 500)
                partner.Status.avoid_melee = 500;
            if (partner.Status.avoid_ranged > 500)
                partner.Status.avoid_ranged = 500;
            if (partner.Status.avoid_magic > 400)
                partner.Status.avoid_magic = 400;
            partner.Status.aspd = (short)checkPositive((partner.Status.aspd_bs + partner.Status.aspd_item + partner.Status.aspd_skill) * FoodBouns(partner));
            if (partner.Status.aspd > 775)
                partner.Status.aspd = 775;
            if (partner.Status.aspd < 1)
            {
                partner.Status.aspd = 1;
            }
            partner.Status.cspd = (short)checkPositive((partner.Status.cspd_bs + partner.Status.cspd_item + partner.Status.cspd_skill) * FoodBouns(partner));
            if (partner.Status.cspd > 775)
                partner.Status.cspd = 775;
            if (partner.Status.cspd < 1)
            {
                partner.Status.cspd = 1;
            }

            //恢复力计算
            partner.Status.hp_recover = (short)checkPositive(partner.Status.hp_recover_bs + partner.Status.hp_recover_item + partner.Status.hp_recover_skill);
            partner.Status.mp_recover = (short)checkPositive(partner.Status.mp_recover_bs + ReliabilityBouns_ADD(partner, "MRCV") + partner.Status.mp_recover_item + partner.Status.mp_recover_skill);
            partner.Status.sp_recover = (short)checkPositive(partner.Status.sp_recover_bs + ReliabilityBouns_ADD(partner, "SRCV") + partner.Status.sp_recover_item + partner.Status.sp_recover_skill);
        }
        float FoodBouns(ActorPartner partner)
        {
            if (partner.reliabilityuprate <= 100)
                return 1f;
            return partner.reliabilityuprate / 100f;
        }
        float ReliabilityBouns(ActorPartner partner)
        {
            switch (partner.reliability)
            {
                case 0:
                    return 1f;
                case 1:
                    return 1.1f;
                case 2:
                    return 1.2f;
                case 3:
                    return 1.3f;
                case 4:
                    return 1.4f;
                case 5:
                    return 1.5f;
                case 6:
                    return 1.6f;
                case 7:
                    return 1.7f;
                case 8:
                    return 1.8f;
                case 9:
                    return 1.9f;
            }
            return 1f;
        }
        //ECOKEY2版按照wiki計算信賴度+
        int ReliabilityBouns_ADD(ActorPartner partner, string status)
        {
            switch (partner.reliability)
            {
                case 0:
                    return 0;
                case 1:
                    switch (status)
                    {
                        case "HP":
                            return 300;
                        case "MP":
                        case "SP":
                            return 50;
                        case "ATKmin":
                        case "ATKmax":
                        case "MATKmin":
                        case "MATKmax":
                            return 10;
                        case "HITmelee":
                            return 10;
                        case "HITranged":
                            return 20;
                        case "DEF":
                        case "MDEF":
                        case "AVOIDmelee":
                        case "AVOIDranged":
                            return 20;
                        case "MRCV":
                        case "SRCV":
                            return 20;
                    }
                    return 0;
                case 2:
                    switch (status)
                    {
                        case "HP":
                            return 500;
                        case "MP":
                        case "SP":
                            return 100;
                        case "ATKmin":
                        case "ATKmax":
                        case "MATKmin":
                        case "MATKmax":
                            return 20;
                        case "HITmelee":
                            return 20;
                        case "HITranged":
                            return 40;
                        case "DEF":
                        case "MDEF":
                        case "AVOIDmelee":
                        case "AVOIDranged":
                            return 30;
                        case "MRCV":
                        case "SRCV":
                            return 20;
                    }
                    return 0;
                case 3:
                    switch (status)
                    {
                        case "HP":
                            return 800;
                        case "MP":
                        case "SP":
                            return 150;
                        case "ATKmin":
                        case "ATKmax":
                        case "MATKmin":
                        case "MATKmax":
                            return 30;
                        case "HITmelee":
                            return 30;
                        case "HITranged":
                            return 60;
                        case "DEF":
                        case "MDEF":
                        case "AVOIDmelee":
                        case "AVOIDranged":
                            return 40;
                        case "MRCV":
                        case "SRCV":
                            return 40;
                    }
                    return 0;
                case 4:
                    switch (status)
                    {
                        case "HP":
                            return 1000;
                        case "MP":
                        case "SP":
                            return 200;
                        case "ATKmin":
                        case "ATKmax":
                        case "MATKmin":
                        case "MATKmax":
                            return 40;
                        case "HITmelee":
                            return 40;
                        case "HITranged":
                            return 80;
                        case "DEF":
                        case "MDEF":
                        case "AVOIDmelee":
                        case "AVOIDranged":
                            return 50;
                        case "MRCV":
                        case "SRCV":
                            return 40;
                    }
                    return 0;
                case 5:
                    switch (status)
                    {
                        case "HP":
                            return 1300;
                        case "MP":
                        case "SP":
                            return 250;
                        case "ATKmin":
                        case "ATKmax":
                        case "MATKmin":
                        case "MATKmax":
                            return 50;
                        case "HITmelee":
                            return 50;
                        case "HITranged":
                            return 100;
                        case "DEF":
                        case "MDEF":
                        case "AVOIDmelee":
                        case "AVOIDranged":
                            return 60;
                        case "MRCV":
                        case "SRCV":
                            return 60;
                    }
                    return 0;
                case 6:
                    switch (status)
                    {
                        case "HP":
                            return 1600;
                        case "MP":
                        case "SP":
                            return 300;
                        case "ATKmin":
                        case "ATKmax":
                        case "MATKmin":
                        case "MATKmax":
                            return 60;
                        case "HITmelee":
                            return 60;
                        case "HITranged":
                            return 130;
                        case "DEF":
                        case "MDEF":
                        case "AVOIDmelee":
                        case "AVOIDranged":
                            return 70;
                        case "MRCV":
                        case "SRCV":
                            return 80;
                    }
                    return 0;
                case 7:
                    switch (status)
                    {
                        case "HP":
                            return 2000;
                        case "MP":
                        case "SP":
                            return 400;
                        case "ATKmin":
                        case "ATKmax":
                        case "MATKmin":
                        case "MATKmax":
                            return 70;
                        case "HITmelee":
                            return 70;
                        case "HITranged":
                            return 150;
                        case "DEF":
                        case "MDEF":
                        case "AVOIDmelee":
                        case "AVOIDranged":
                            return 80;
                        case "MRCV":
                        case "SRCV":
                            return 90;
                    }
                    return 0;
                case 8:
                    switch (status)
                    {
                        case "HP":
                            return 2500;
                        case "MP":
                        case "SP":
                            return 600;
                        case "ATKmin":
                        case "ATKmax":
                        case "MATKmin":
                        case "MATKmax":
                            return 80;
                        case "HITmelee":
                            return 80;
                        case "HITranged":
                            return 180;
                        case "DEF":
                        case "MDEF":
                        case "AVOIDmelee":
                        case "AVOIDranged":
                            return 90;
                        case "MRCV":
                        case "SRCV":
                            return 100;
                    }
                    return 0;
                case 9:
                    switch (status)
                    {
                        case "HP":
                            return 3000;
                        case "MP":
                        case "SP":
                            return 800;
                        case "ATKmin":
                        case "ATKmax":
                        case "MATKmin":
                        case "MATKmax":
                            return 100;
                        case "HITmelee":
                            return 200;
                        case "HITranged":
                            return 250;
                        case "DEF":
                        case "MDEF":
                        case "AVOIDmelee":
                        case "AVOIDranged":
                            return 100;
                        case "MRCV":
                        case "SRCV":
                            return 100;
                    }
                    return 0;
            }
            return 0;
        }
        //ECOKEY2版按照wiki計算信賴度%
        float ReliabilityBouns_P(ActorPartner partner, string status)
        {
            switch (status)
            {
                case "HP":
                    switch (partner.reliability)
                    {
                        case 0:
                            return 1.0f;
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            return 1.05f;
                        case 7:
                            return 1.1f;
                        case 8:
                            return 1.15f;
                        case 9:
                            return 1.2f;
                    }
                    return 1f;
                case "MP":
                case "SP":
                    switch (partner.reliability)
                    {
                        case 0:
                        case 1:
                            return 1.0f;
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            return 1.2f;
                        case 7:
                        case 8:
                            return 1.4f;
                        case 9:
                            return 1.5f;
                    }
                    return 1f;
            }
            return 1.0f;
        }
        //ECOKEY2版按照wiki計算三圍
        public void CalcPartnerHPMPSP(ActorPartner partner)
        {
            byte lv = (byte)(partner.Level / 10 * 10);
            ushort perk2_HP = (ushort)(partner.perk2 * 10 + (Math.Pow(Math.Floor((float)(partner.perk2 / 5)), 2) * 3));
            ushort perk1_MP = (ushort)(partner.perk1 * 10 + Math.Pow(Math.Floor((float)(partner.perk1 / 10)), 2));
            ushort perk0_SP = (ushort)(partner.perk0 * 2 + Math.Floor((float)(partner.perk0 / 7)) * 2 + Math.Pow(Math.Floor((float)(partner.perk0 / 10)), 2));

            //ECOKEY 馴獸師技能加乘
            float breeder_skill_HP = 1;
            float breeder_skill_MPSP = 1;
            if (partner.Owner.JobJoint == PC_JOB.BREEDER && partner.Owner.JointJobLevel >= 23)
            {
                breeder_skill_HP = 1.4f;
                breeder_skill_MPSP = 1.3f;
            }
            //ECOKEY 馴獸師技能加乘，這三行也要一起改
            partner.MaxHP = (uint)(((Math.Ceiling((float)(partner.BaseData.hp_fn - partner.BaseData.hp_in) / 109 * (partner.Level - 1)) + (partner.rebirth ? partner.BaseData.AdvanceStatus[(byte)lv].hp : 0) + partner.BaseData.hp_in + perk2_HP) * breeder_skill_HP * FoodBouns(partner) * ReliabilityBouns_P(partner, "HP") + ReliabilityBouns_ADD(partner, "HP")) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level) + partner.Status.hp_item + partner.Status.hp_skill);
            partner.MaxMP = (uint)(((Math.Ceiling((float)(partner.BaseData.mp_fn - partner.BaseData.mp_in) / 109 * (partner.Level - 1)) + (partner.rebirth ? partner.BaseData.AdvanceStatus[(byte)lv].mp : 0) + partner.BaseData.mp_in + perk1_MP) * breeder_skill_MPSP * FoodBouns(partner) * ReliabilityBouns_P(partner, "MP") + ReliabilityBouns_ADD(partner, "MP")) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level) + partner.Status.mp_item + partner.Status.mp_skill);
            partner.MaxSP = (uint)(((Math.Ceiling((float)(partner.BaseData.sp_fn - partner.BaseData.sp_in) / 109 * (partner.Level - 1)) + (partner.rebirth ? partner.BaseData.AdvanceStatus[(byte)lv].sp : 0) + partner.BaseData.sp_in + perk0_SP) * breeder_skill_MPSP * FoodBouns(partner) * ReliabilityBouns_P(partner, "SP") + ReliabilityBouns_ADD(partner, "SP")) * CalcPartnerAndPlayerLevelDiff(partner.Level, partner.Owner.Level) + partner.Status.sp_item + partner.Status.sp_skill);

            if (partner.HP > partner.MaxHP) partner.HP = partner.MaxHP;
            if (partner.MP > partner.MaxMP) partner.MP = partner.MaxMP;
            if (partner.SP > partner.MaxSP) partner.SP = partner.MaxSP;

        }

        float HPSystemTypeFactor(ActorPartner partner)
        {
            switch (partner.BaseData.partnersystemid)
            {
                case 0:
                    return 0.80f;
                case 1:
                    return 1.00f;
                case 2:
                    return 1.00f;
                case 3:
                    return 1.00f;
                case 4:
                    return 1.00f;
                case 5:
                    return 1.00f;
                case 6:
                    return 1.00f;
                case 7:
                    return 1.00f;
                case 8:
                    return 1.00f;
                case 9:
                    return 1.00f;
                case 10:
                    return 1.00f;
                case 11:
                    return 1.80f;
                case 12:
                    return 1.80f;

                default:
                    return 1;
            }
        }
        //ECOKEY 被動技能計算0529再修正
        void PassiveSkill(ActorPartner partner)
        {
            for (int i = 0; i < partner.equipcubes_passiveskill.Count; i++)
            {
                ushort cubeid = partner.equipcubes_passiveskill[i];
                ActCubeData acd = PartnerFactory.Instance.actcubes_db_uniqueID[cubeid];
                if (acd.skillID == 7200)//HP
                {
                    partner.Status.hp_medicine = (short)acd.parameter1;
                }
                if (acd.skillID == 7201)//MP
                {
                    partner.Status.mp_medicine = (short)acd.parameter1;
                }
                if (acd.skillID == 7202)//SP
                {
                    partner.Status.sp_medicine = (short)acd.parameter1;
                }
                if (acd.skillID == 7222)//物理防御（％）
                {
                    partner.Status.def_add_item += (short)acd.parameter1;
                }
                if (acd.skillID == 7223)//魔法防御（％）
                {
                    partner.Status.mdef_add_item += (short)acd.parameter1;
                }
            }
        }

        float MPSystemTypeFactor(ActorPartner partner)
        {
            switch (partner.BaseData.partnersystemid)
            {
                case 0:
                    return 0.80f;
                case 1:
                    return 1.00f;
                case 2:
                    return 1.00f;
                case 3:
                    return 1.00f;
                case 4:
                    return 1.00f;
                case 5:
                    return 1.00f;
                case 6:
                    return 1.00f;
                case 7:
                    return 1.00f;
                case 8:
                    return 1.00f;
                case 9:
                    return 1.00f;
                case 10:
                    return 1.00f;
                case 11:
                    return 1.80f;
                case 12:
                    return 1.80f;

                default:
                    return 1;
            }
        }

        float SPSystemTypeFactor(ActorPartner partner)
        {
            switch (partner.BaseData.partnersystemid)
            {
                case 0:
                    return 0.80f;
                case 1:
                    return 1.00f;
                case 2:
                    return 1.00f;
                case 3:
                    return 1.00f;
                case 4:
                    return 1.00f;
                case 5:
                    return 1.00f;
                case 6:
                    return 1.00f;
                case 7:
                    return 1.00f;
                case 8:
                    return 1.00f;
                case 9:
                    return 1.00f;
                case 10:
                    return 1.00f;
                case 11:
                    return 1.80f;
                case 12:
                    return 1.80f;

                default:
                    return 1;
            }
        }
    }
}
