﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;


namespace SagaMap.Skill.SkillDefinations.Monster
{
    public class SleepStrike : ISkill, MobISkill
    {
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void BeforeCast(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            return;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            args.type = ATTACK_TYPE.BLOW;
            float factor = 0.8f;
            int lifetime = 5000;
            int rate = 50;
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, SagaLib.Elements.Neutral, factor);
            if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, SkillHandler.DefaultAdditions.Silence, rate))
            {
                Additions.Global.Silence skill = new SagaMap.Skill.Additions.Global.Silence(args.skill, dActor, lifetime);
                SkillHandler.ApplyAddition(dActor, skill);
            }
            //if (SagaLib.Global.Random.Next(0, 99) < rate)
            //{
                
            //    Additions.Global.Silence skill = new SagaMap.Skill.Additions.Global.Silence(args.skill, dActor, lifetime);
            //    SkillHandler.ApplyAddition(dActor, skill);
            //}
        }
    }
}
