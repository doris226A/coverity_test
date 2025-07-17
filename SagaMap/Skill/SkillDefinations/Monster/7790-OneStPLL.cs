using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaLib;
using SagaMap;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Monster
{
    /// <summary>
    /// 一周年PLL專用
    /// </summary>
    public class OneStPLL : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 10000;
            switch (SagaLib.Global.Random.Next(1, 4))
            {
                case 1://增加物攻
                    DefaultBuff skillA = new DefaultBuff(args.skill, dActor, "OneStPLL_A", lifetime);
                    skillA.OnAdditionStart += this.StartEventHandlerA;
                    skillA.OnAdditionEnd += this.EndEventHandlerA;
                    SkillHandler.ApplyAddition(dActor, skillA);
                    break;
                case 2://增加魔攻
                    DefaultBuff skillB = new DefaultBuff(args.skill, dActor, "OneStPLL_B", lifetime);
                    skillB.OnAdditionStart += this.StartEventHandlerB;
                    skillB.OnAdditionEnd += this.EndEventHandlerB;
                    SkillHandler.ApplyAddition(dActor, skillB);
                    break;
                case 3://經驗值UP
                    DefaultBuff skillC = new DefaultBuff(args.skill, dActor, "OneStPLL_C", lifetime);
                    skillC.OnAdditionStart += this.StartEventHandlerC;
                    skillC.OnAdditionEnd += this.EndEventHandlerC;
                    SkillHandler.ApplyAddition(dActor, skillC);
                    break;
                case 4://三圍UP
                    DefaultBuff skillD = new DefaultBuff(args.skill, dActor, "OneStPLL_D", lifetime);
                    skillD.OnAdditionStart += this.StartEventHandlerD;
                    skillD.OnAdditionEnd += this.EndEventHandlerD;
                    SkillHandler.ApplyAddition(dActor, skillD);
                    break;
            }

        }
        void StartEventHandlerA(Actor actor, DefaultBuff skill)
        {
            int add = 100;
            //最大攻擊
            if (skill.Variable.ContainsKey("MobMAtkAtkup_max_atk1"))
                skill.Variable.Remove("MobMAtkAtkup_max_atk1");
            skill.Variable.Add("MobMAtkAtkup_max_atk1", add);
            actor.Status.max_atk1_skill += (short)add;

            //最大攻擊
            if (skill.Variable.ContainsKey("MobMAtkAtkup_max_atk2"))
                skill.Variable.Remove("MobMAtkAtkup_max_atk2");
            skill.Variable.Add("MobMAtkAtkup_max_atk2", add);
            actor.Status.max_atk2_skill += (short)add;

            //最大攻擊
            if (skill.Variable.ContainsKey("MobMAtkAtkup_max_atk3"))
                skill.Variable.Remove("MobMAtkAtkup_max_atk3");
            skill.Variable.Add("MobMAtkAtkup_max_atk3", add);
            actor.Status.max_atk3_skill += (short)add;

            //最小攻擊
            if (skill.Variable.ContainsKey("MobMAtkAtkup_min_atk1"))
                skill.Variable.Remove("MobMAtkAtkup_min_atk1");
            skill.Variable.Add("MobMAtkAtkup_min_atk1", add);
            actor.Status.min_atk1_skill += (short)add;

            //最小攻擊
            if (skill.Variable.ContainsKey("MobMAtkAtkup_min_atk2"))
                skill.Variable.Remove("MobMAtkAtkup_min_atk2");
            skill.Variable.Add("MobMAtkAtkup_min_atk2", add);
            actor.Status.min_atk2_skill += (short)add;

            //最小攻擊
            if (skill.Variable.ContainsKey("MobMAtkAtkup_min_atk3"))
                skill.Variable.Remove("MobMAtkAtkup_min_atk3");
            skill.Variable.Add("MobMAtkAtkup_min_atk3", add);
            actor.Status.min_atk3_skill += (short)add;

            //最大魔攻
            if (skill.Variable.ContainsKey("MobMAtkAtkup_max_matk"))
                skill.Variable.Remove("MobMAtkAtkup_max_matk");
            skill.Variable.Add("MobMAtkAtkup_max_matk", add);
            actor.Status.max_matk_skill += (short)add;

            //最小魔攻
            if (skill.Variable.ContainsKey("MobMAtkAtkup_min_matk"))
                skill.Variable.Remove("MobMAtkAtkup_min_matk");
            skill.Variable.Add("MobMAtkAtkup_min_matk", add);
            actor.Status.min_matk_skill += (short)add;


            actor.Buff.MinAtkUp = true;
            actor.Buff.MaxAtkUp = true;
            actor.Buff.MinMagicAtkUp = true;
            actor.Buff.MaxMagicAtkUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandlerA(Actor actor, DefaultBuff skill)
        {
            //最大攻擊
            actor.Status.max_atk1_skill -= (short)skill.Variable["MobMAtkAtkup_max_atk1"];

            //最大攻擊
            actor.Status.max_atk2_skill -= (short)skill.Variable["MobMAtkAtkup_max_atk2"];

            //最大攻擊
            actor.Status.max_atk3_skill -= (short)skill.Variable["MobMAtkAtkup_max_atk3"];

            //最小攻擊
            actor.Status.min_atk1_skill -= (short)skill.Variable["MobMAtkAtkup_min_atk1"];

            //最小攻擊
            actor.Status.min_atk2_skill -= (short)skill.Variable["MobMAtkAtkup_min_atk2"];

            //最小攻擊
            actor.Status.min_atk3_skill -= (short)skill.Variable["MobMAtkAtkup_min_atk3"];

            //最大魔攻
            actor.Status.max_matk_skill -= (short)skill.Variable["MobMAtkAtkup_max_matk"];

            //最小魔攻
            actor.Status.min_matk_skill -= (short)skill.Variable["MobMAtkAtkup_min_matk"];

            actor.Buff.MinAtkUp = false;
            actor.Buff.MaxAtkUp = false;
            actor.Buff.MinMagicAtkUp = false;
            actor.Buff.MaxMagicAtkUp = false;

            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void StartEventHandlerB(Actor actor, DefaultBuff skill)
        {
            int add = 100;
            //最大魔攻
            if (skill.Variable.ContainsKey("MobMAtkAtkup_max_matk"))
                skill.Variable.Remove("MobMAtkAtkup_max_matk");
            skill.Variable.Add("MobMAtkAtkup_max_matk", add);
            actor.Status.max_matk_skill += (short)add;
            //最小魔攻
            if (skill.Variable.ContainsKey("MobMAtkAtkup_min_matk"))
                skill.Variable.Remove("MobMAtkAtkup_min_matk");
            skill.Variable.Add("MobMAtkAtkup_min_matk", add);
            actor.Status.min_matk_skill += (short)add;
            actor.Buff.MinMagicAtkUp = true;
            actor.Buff.MaxMagicAtkUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandlerB(Actor actor, DefaultBuff skill)
        {
            //最大魔攻
            actor.Status.max_matk_skill -= (short)skill.Variable["MobMAtkAtkup_max_matk"];
            //最小魔攻
            actor.Status.min_matk_skill -= (short)skill.Variable["MobMAtkAtkup_min_matk"];
            actor.Buff.MinMagicAtkUp = false;
            actor.Buff.MaxMagicAtkUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void StartEventHandlerC(Actor actor, DefaultBuff skill)
        {
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            actor.Buff.EXPUp = true;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandlerC(Actor actor, DefaultBuff skill)
        {
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            actor.Buff.EXPUp = false;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }


        void StartEventHandlerD(Actor actor, DefaultBuff skill)
        {
            float addrate1 = 0.5f;
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


            actor.Buff.MaxHPUp = true;
            actor.Buff.MaxSPUp = true;
            actor.Buff.MaxMPUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
        void EndEventHandlerD(Actor actor, DefaultBuff skill)
        {
            actor.Status.hp_skill -= (short)skill.Variable["OverClock_HP"];
            actor.Status.sp_skill -= (short)skill.Variable["OverClock_SP"];
            actor.Status.mp_skill -= (short)skill.Variable["OverClock_MP"];

            actor.Buff.MaxHPUp = false;
            actor.Buff.MaxSPUp = false;
            actor.Buff.MaxMPUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
        #endregion
    }
}
