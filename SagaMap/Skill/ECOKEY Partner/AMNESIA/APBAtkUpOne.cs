﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Merchant
{
    /// <summary>
    /// 應援（応援）
    /// </summary>
    public class APBAtkUpOne : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 40000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "AtkUpOne", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int MaxAtk = 6;
            int MinAtk = 12;
            int MaxMAtk = 3;
            int MinMAtk = 3;
            if (actor.type == ActorType.PC || actor.type == ActorType.PET || actor.type == ActorType.PARTNER || actor.type == ActorType.SHADOW)
            {
                //最大攻擊
                int max_atk1_add = MaxAtk;
                if (skill.Variable.ContainsKey("AtkUpOne_max_atk1"))
                    skill.Variable.Remove("AtkUpOne_max_atk1");
                skill.Variable.Add("AtkUpOne_max_atk1", max_atk1_add);
                actor.Status.max_atk1_skill += (short)max_atk1_add;

                //最大攻擊
                int max_atk2_add = MaxAtk;
                if (skill.Variable.ContainsKey("AtkUpOne_max_atk2"))
                    skill.Variable.Remove("AtkUpOne_max_atk2");
                skill.Variable.Add("AtkUpOne_max_atk2", max_atk2_add);
                actor.Status.max_atk2_skill += (short)max_atk2_add;

                //最大攻擊
                int max_atk3_add = MaxAtk;
                if (skill.Variable.ContainsKey("AtkUpOne_max_atk3"))
                    skill.Variable.Remove("AtkUpOne_max_atk3");
                skill.Variable.Add("AtkUpOne_max_atk3", max_atk3_add);
                actor.Status.max_atk3_skill += (short)max_atk3_add;

                //最小攻擊
                int min_atk1_add = MinAtk;
                if (skill.Variable.ContainsKey("AtkUpOne_min_atk1"))
                    skill.Variable.Remove("AtkUpOne_min_atk1");
                skill.Variable.Add("AtkUpOne_min_atk1", min_atk1_add);
                actor.Status.min_atk1_skill += (short)min_atk1_add;

                //最小攻擊
                int min_atk2_add = MinAtk;
                if (skill.Variable.ContainsKey("AtkUpOne_min_atk2"))
                    skill.Variable.Remove("AtkUpOne_min_atk2");
                skill.Variable.Add("AtkUpOne_min_atk2", min_atk2_add);
                actor.Status.min_atk2_skill += (short)min_atk2_add;

                //最小攻擊
                int min_atk3_add = MinAtk;
                if (skill.Variable.ContainsKey("AtkUpOne_min_atk3"))
                    skill.Variable.Remove("AtkUpOne_min_atk3");
                skill.Variable.Add("AtkUpOne_min_atk3", min_atk3_add);
                actor.Status.min_atk3_skill += (short)min_atk3_add;

                //最大魔攻
                int max_matk_add = MaxMAtk;
                if (skill.Variable.ContainsKey("AtkUpOne_max_matk"))
                    skill.Variable.Remove("AtkUpOne_max_matk");
                skill.Variable.Add("AtkUpOne_max_matk", max_matk_add);
                actor.Status.max_matk_skill += (short)max_matk_add;

                //最小魔攻
                int min_matk_add = MinMAtk;
                if (skill.Variable.ContainsKey("AtkUpOne_min_matk"))
                    skill.Variable.Remove("AtkUpOne_min_matk");
                skill.Variable.Add("AtkUpOne_min_matk", min_matk_add);
                actor.Status.min_matk_skill += (short)min_matk_add;

                actor.Buff.MinAtkUp = true;
                actor.Buff.MaxAtkUp = true;
                actor.Buff.MinMagicAtkUp = true;
                actor.Buff.MaxMagicAtkUp = true;
            }
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            if (actor.type == ActorType.PC || actor.type == ActorType.PET || actor.type == ActorType.PARTNER || actor.type == ActorType.SHADOW)
            {
                //最大攻擊
                actor.Status.max_atk1_skill -= (short)skill.Variable["AtkUpOne_max_atk1"];

                //最大攻擊
                actor.Status.max_atk2_skill -= (short)skill.Variable["AtkUpOne_max_atk2"];

                //最大攻擊
                actor.Status.max_atk3_skill -= (short)skill.Variable["AtkUpOne_max_atk3"];

                //最小攻擊
                actor.Status.min_atk1_skill -= (short)skill.Variable["AtkUpOne_min_atk1"];

                //最小攻擊
                actor.Status.min_atk2_skill -= (short)skill.Variable["AtkUpOne_min_atk2"];

                //最小攻擊
                actor.Status.min_atk3_skill -= (short)skill.Variable["AtkUpOne_min_atk3"];

                //最大魔攻
                actor.Status.max_matk_skill -= (short)skill.Variable["AtkUpOne_max_matk"];

                //最小魔攻
                actor.Status.min_matk_skill -= (short)skill.Variable["AtkUpOne_min_matk"];

                actor.Buff.MinAtkUp = false;
                actor.Buff.MaxAtkUp = false;
                actor.Buff.MinMagicAtkUp = false;
                actor.Buff.MaxMagicAtkUp = false;
            }
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
        #endregion
    }
}
