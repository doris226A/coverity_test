﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Monster
{
    public class PAWaterAttack : ISkill
    {
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            if (SkillHandler.Instance.CheckValidAttackTarget(pc, dActor))
            {
                return 0;
            }
            else
            {
                return -14;
            }
            // return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 1.1f;
            SkillHandler.Instance.MagicAttack(sActor, dActor, args, SagaLib.Elements.Water, factor);
        }
    }
}
