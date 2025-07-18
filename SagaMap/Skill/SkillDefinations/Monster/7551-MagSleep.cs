﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;


namespace SagaMap.Skill.SkillDefinations.Monster
{
    public class MagSleep : ISkill
    {
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 0.8f;
            int lifetime = 3000;
            int rate = 20;

            if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, SkillHandler.DefaultAdditions.Frosen, rate))
            {
                Additions.Global.Freeze skill = new SagaMap.Skill.Additions.Global.Freeze(args.skill, dActor, lifetime);
                SkillHandler.ApplyAddition(dActor, skill);
            }
            SkillHandler.Instance.MagicAttack(sActor, dActor, args, SagaLib.Elements.Neutral, factor);
            //if (SagaLib.Global.Random.Next(0, 99) < rate)
            //{

            //    Additions.Global.Freeze skill = new SagaMap.Skill.Additions.Global.Freeze(args.skill, dActor, lifetime);
            //    SkillHandler.ApplyAddition(dActor, skill);
            //}
        }
    }
}
