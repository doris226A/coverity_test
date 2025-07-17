﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Blacksmith
{
    /// <summary>
    /// 提升強化成功率（力量）（強化成功率上昇（力））
    /// </summary>
    public class BoostPower : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            bool active = true;
            DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "BoostPower", active);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            int[] num = { 0, 1, 3, 5, 8, 10 };
            skill["BoostPower"] = 100 * num[skill.skill.Level];
        }
        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            int[] num = { 0, 1, 3, 5, 8, 10 };
            skill["BoostPower"] = 100 * num[skill.skill.Level];
        }
        #endregion
    }
}

