using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.ECOKEY
{
    public class SakuraPowerUp : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(SagaDB.Actor.Actor sActor, SagaDB.Actor.Actor dActor, SkillArg args, byte level)
        {
            //int level = skill.skill.Level;
            if (sActor.type == ActorType.PC)
            {
                //int[] lifetimes = new int[] { 0, 30000, 120000, 120000 };
                //int lifetime = lifetimes[level];
                int lifetime = 360000;
                //DefaultBuff skill = new DefaultBuff(args.skill, sActor, "OverClock", lifetime, 1000);
                DefaultBuff skill = new DefaultBuff(args.skill, sActor, "OverClock", lifetime);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                //skill.OnUpdate += this.UpdateEventHandler;
                SkillHandler.ApplyAddition(sActor, skill);
                Manager.MapManager.Instance.GetMap(sActor.MapID).SendEffect(sActor, 5472);
            }

        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int level = skill.skill.Level;
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                if (pc.Race != PC_RACE.DEM)
                {
                    level = 3;
                }
            }
            float addrate1 = new float[] { 0, 1.2f, 1.5f, 2.0f }[level];
            short hpadd = (short)((actor.MaxHP - (actor.Status.hp_item + actor.Status.hp_mario + actor.Status.hp_skill + actor.Status.hp_iris + actor.Status.hp_another)) * addrate1);
            short spadd = (short)((actor.MaxSP - (actor.Status.sp_item + actor.Status.sp_mario + actor.Status.sp_skill + actor.Status.sp_iris + actor.Status.sp_another)) * addrate1);
            short mpadd = (short)((actor.MaxMP - (actor.Status.mp_item + actor.Status.mp_mario + actor.Status.mp_skill + actor.Status.mp_iris + actor.Status.mp_another)) * addrate1);
            //hpspmp
            if (skill.Variable.ContainsKey("OverClock_HP"))
                skill.Variable.Remove("OverClock_HP");
            skill.Variable.Add("OverClock_HP", hpadd);
            actor.Status.hp_skill += hpadd;

            if (skill.Variable.ContainsKey("OverClock_SP"))
                skill.Variable.Remove("OverClock_SP");
            skill.Variable.Add("OverClock_SP", spadd);
            actor.Status.sp_skill += spadd;

            if (skill.Variable.ContainsKey("OverClock_MP"))
                skill.Variable.Remove("OverClock_MP");
            skill.Variable.Add("OverClock_MP", mpadd);
            actor.Status.mp_skill += mpadd;


            short minatkadd = (short)(actor.Status.min_atk_bs * addrate1);
            short maxatkadd = (short)(actor.Status.max_atk_bs * addrate1);
            short minmatkadd = (short)(actor.Status.min_matk_bs * addrate1);
            short maxmatkadd = (short)(actor.Status.max_matk_bs * addrate1);
            //atk
            if (skill.Variable.ContainsKey("OverClock_min_atk1"))
                skill.Variable.Remove("OverClock_min_atk1");
            skill.Variable.Add("OverClock_min_atk1", minatkadd);
            actor.Status.min_atk1_skill += minatkadd;

            if (skill.Variable.ContainsKey("OverClock_min_atk2"))
                skill.Variable.Remove("OverClock_min_atk2");
            skill.Variable.Add("OverClock_min_atk2", minatkadd);
            actor.Status.min_atk2_skill += minatkadd;

            if (skill.Variable.ContainsKey("OverClock_min_atk3"))
                skill.Variable.Remove("OverClock_min_atk3");
            skill.Variable.Add("OverClock_min_atk3", minatkadd);
            actor.Status.min_atk3_skill += minatkadd;

            if (skill.Variable.ContainsKey("OverClock_max_atk1"))
                skill.Variable.Remove("OverClock_max_atk1");
            skill.Variable.Add("OverClock_max_atk1", maxatkadd);
            actor.Status.max_atk1_skill += maxatkadd;

            if (skill.Variable.ContainsKey("OverClock_max_atk2"))
                skill.Variable.Remove("OverClock_max_atk2");
            skill.Variable.Add("OverClock_max_atk2", maxatkadd);
            actor.Status.max_atk2_skill += maxatkadd;

            if (skill.Variable.ContainsKey("OverClock_max_atk3"))
                skill.Variable.Remove("OverClock_max_atk3");
            skill.Variable.Add("OverClock_max_atk3", maxatkadd);
            actor.Status.max_atk3_skill += maxatkadd;

            //matk
            if (skill.Variable.ContainsKey("OverClock_min_matk"))
                skill.Variable.Remove("OverClock_min_matk");
            skill.Variable.Add("OverClock_min_matk", minmatkadd);
            actor.Status.min_matk_skill += minmatkadd;

            if (skill.Variable.ContainsKey("OverClock_max_matk"))
                skill.Variable.Remove("OverClock_max_matk");
            skill.Variable.Add("OverClock_max_matk", maxmatkadd);
            actor.Status.max_matk_skill += maxmatkadd;

            short hitadd = new short[] { 0, 80, 120, 200 }[level];
            //hit
            if (skill.Variable.ContainsKey("OverClock_shit"))
                skill.Variable.Remove("OverClock_shit");
            skill.Variable.Add("OverClock_shit", hitadd);
            actor.Status.hit_melee_skill += hitadd;

            if (skill.Variable.ContainsKey("OverClock_lhit"))
                skill.Variable.Remove("OverClock_lhit");
            skill.Variable.Add("OverClock_lhit", hitadd);
            actor.Status.hit_ranged_skill += hitadd;

            float addrate2 = new float[] { 0, 1.1f, 1.2f, 1.3f }[level];
            //avoid
            short savoidadd = (short)(actor.Status.avoid_melee_bs * addrate2);
            short lavoidadd = (short)(actor.Status.avoid_ranged_bs * addrate2);
            if (skill.Variable.ContainsKey("OverClock_savoid"))
                skill.Variable.Remove("OverClock_savoid");
            skill.Variable.Add("OverClock_savoid", savoidadd);
            actor.Status.avoid_melee_skill += savoidadd;

            if (skill.Variable.ContainsKey("OverClock_lavoid"))
                skill.Variable.Remove("OverClock_lavoid");
            skill.Variable.Add("OverClock_lavoid", lavoidadd);
            actor.Status.avoid_ranged_skill += lavoidadd;

            short statusadd = new short[] { 0, 10, 10, 20 }[level];
            //status
            if (skill.Variable.ContainsKey("OverClock_str"))
                skill.Variable.Remove("OverClock_str");
            skill.Variable.Add("OverClock_str", statusadd);
            actor.Status.str_skill += statusadd;

            if (skill.Variable.ContainsKey("OverClock_vit"))
                skill.Variable.Remove("OverClock_vit");
            skill.Variable.Add("OverClock_vit", statusadd);
            actor.Status.vit_skill += statusadd;

            if (skill.Variable.ContainsKey("OverClock_agi"))
                skill.Variable.Remove("OverClock_agi");
            skill.Variable.Add("OverClock_agi", statusadd);
            actor.Status.agi_skill += statusadd;

            if (skill.Variable.ContainsKey("OverClock_int"))
                skill.Variable.Remove("OverClock_int");
            skill.Variable.Add("OverClock_int", statusadd);
            actor.Status.int_skill += statusadd;

            if (skill.Variable.ContainsKey("OverClock_dex"))
                skill.Variable.Remove("OverClock_dex");
            skill.Variable.Add("OverClock_dex", statusadd);
            actor.Status.dex_skill += statusadd;

            if (skill.Variable.ContainsKey("OverClock_mag"))
                skill.Variable.Remove("OverClock_mag");
            skill.Variable.Add("OverClock_mag", statusadd);
            actor.Status.mag_skill += statusadd;


            actor.Buff.MaxSPUp = true;
            actor.Buff.MaxMPUp = true;
            actor.Buff.STRUp = true;
            actor.Buff.VITUp = true;
            actor.Buff.AGIUp = true;
            actor.Buff.INTUp = true;
            actor.Buff.DEXUp = true;
            actor.Buff.MagUp = true;
            actor.Buff.MinAtkUp = true;
            actor.Buff.MaxAtkUp = true;
            actor.Buff.MinMagicAtkUp = true;
            actor.Buff.MaxMagicAtkUp = true;
            actor.Buff.ShortHitUp = true;
            actor.Buff.LongHitUp = true;
            actor.Buff.ShortDodgeUp = true;
            actor.Buff.LongDodgeUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.hp_skill -= (short)skill.Variable["OverClock_HP"];
            actor.Status.sp_skill -= (short)skill.Variable["OverClock_SP"];
            actor.Status.mp_skill -= (short)skill.Variable["OverClock_MP"];
            actor.Status.min_atk1_skill -= (short)skill.Variable["OverClock_min_atk1"];
            actor.Status.min_atk2_skill -= (short)skill.Variable["OverClock_min_atk2"];
            actor.Status.min_atk3_skill -= (short)skill.Variable["OverClock_min_atk3"];
            actor.Status.max_atk1_skill -= (short)skill.Variable["OverClock_max_atk1"];
            actor.Status.max_atk2_skill -= (short)skill.Variable["OverClock_max_atk2"];
            actor.Status.max_atk3_skill -= (short)skill.Variable["OverClock_max_atk3"];
            actor.Status.min_matk_skill -= (short)skill.Variable["OverClock_min_matk"];
            actor.Status.max_matk_skill -= (short)skill.Variable["OverClock_max_matk"];
            actor.Status.hit_melee_skill -= (short)skill.Variable["OverClock_shit"];
            actor.Status.hit_ranged_skill -= (short)skill.Variable["OverClock_lhit"];
            actor.Status.avoid_melee_skill -= (short)skill.Variable["OverClock_savoid"];
            actor.Status.avoid_ranged_skill -= (short)skill.Variable["OverClock_lavoid"];
            actor.Status.str_skill -= (short)skill.Variable["OverClock_str"];
            actor.Status.vit_skill -= (short)skill.Variable["OverClock_vit"];
            actor.Status.agi_skill -= (short)skill.Variable["OverClock_agi"];
            actor.Status.int_skill -= (short)skill.Variable["OverClock_int"];
            actor.Status.dex_skill -= (short)skill.Variable["OverClock_dex"];
            actor.Status.mag_skill -= (short)skill.Variable["OverClock_mag"];

            actor.Buff.MaxHPUp = false;
            actor.Buff.MaxSPUp = false;
            actor.Buff.MaxMPUp = false;
            actor.Buff.STRUp = false;
            actor.Buff.VITUp = false;
            actor.Buff.AGIUp = false;
            actor.Buff.INTUp = false;
            actor.Buff.DEXUp = false;
            actor.Buff.MagUp = false;
            actor.Buff.MinAtkUp = false;
            actor.Buff.MaxAtkUp = false;
            actor.Buff.MinMagicAtkUp = false;
            actor.Buff.MaxMagicAtkUp = false;
            actor.Buff.ShortHitUp = false;
            actor.Buff.LongHitUp = false;
            actor.Buff.ShortDodgeUp = false;
            actor.Buff.LongDodgeUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void UpdateEventHandler(Actor actor, DefaultBuff skill)
        {
            int hpdown = (int)(actor.MaxHP * 0.01f);
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                if (pc.Race != PC_RACE.DEM && pc.Marionette == null)
                {
                    actor.Status.Additions["OverClock"].AdditionEnd();
                    actor.Status.Additions.TryRemove("OverClock", out _);
                    return;
                }
            }
            if (actor.HP > hpdown)
            {
                actor.HP -= (ushort)hpdown;
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, actor, true);
            }
            else
            {
                actor.Status.Additions["OverClock"].AdditionEnd();
                actor.Status.Additions.TryRemove("OverClock", out _);
            }
        }
        #endregion
    }
}
