﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.SkillDefinations.Global;
using SagaLib;
using SagaMap;
using SagaMap.Skill.Additions.Global;


namespace SagaMap.Skill.SkillDefinations.Royaldealer
{
    class DamagePossession : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 140000 - 20000 * level;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "DamagePossession", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        #endregion
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.TInt["DamagePossession"] = skill.skill.Level * 20;
            actor.Buff.DamageReduceSpriteMark3RD = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.TInt["DamagePossession"] = 0;
            actor.Buff.DamageReduceSpriteMark3RD = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
    }
}

