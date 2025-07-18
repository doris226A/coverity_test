﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Enchanter
{
    /// <summary>
    /// 寒氣勢力（ウォーターオーラ）
    /// </summary>
    public class SoulOfWater : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (dActor.Status.Additions.ContainsKey("DevineBarrier"))
            {
                return -12;
            }
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 60000 * level;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "SoulOfWater", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            short LMDef = 0, RMDef = 0;
            int level = skill.skill.Level;
            switch (level)
            {
                case 1:
                    LMDef = 5;
                    RMDef = 20;
                    break;
                case 2:
                    LMDef = 10;
                    RMDef = 30;
                    break;
                case 3:
                    LMDef = 20;
                    RMDef = 40;
                    break;
            }
            SkillHandler.RemoveAddition(actor, "MagicShield");

            //左魔防
            if (skill.Variable.ContainsKey("SoulOfWater_mdef"))
                skill.Variable.Remove("SoulOfWater_mdef");
            skill.Variable.Add("SoulOfWater_mdef", LMDef);
            actor.Status.mdef_skill += (short)LMDef;

            //右魔防
            if (skill.Variable.ContainsKey("SoulOfWater_mdef_add"))
                skill.Variable.Remove("SoulOfWater_mdef_add");
            skill.Variable.Add("SoulOfWater_mdef_add", RMDef);
            actor.Status.mdef_add_skill += (short)RMDef;

            actor.Buff.MagicDefUp = true;
            actor.Buff.MagicDefRateUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEffect(actor, 5232);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            //左魔防
            actor.Status.mdef_skill -= (short)skill.Variable["SoulOfWater_mdef"];

            //右魔防
            actor.Status.mdef_add_skill -= (short)skill.Variable["SoulOfWater_mdef_add"];


            actor.Buff.MagicDefUp = false;
            actor.Buff.MagicDefRateUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
