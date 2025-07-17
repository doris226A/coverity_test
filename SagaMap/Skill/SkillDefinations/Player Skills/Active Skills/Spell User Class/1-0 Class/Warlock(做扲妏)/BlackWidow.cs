﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Warlock
{
    public class BlackWidow : ISkill
    {
        #region ISkill Members
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
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        { 
            float factor = 0;
            factor = 1.0f + 0.2f * level;
            if (dActor.Darks != 1)
            {
                dActor.Darks = 1;
            }
            SkillHandler.Instance.MagicAttack(sActor, dActor, args, SagaLib.Elements.Dark, factor);
        }

        #endregion
    }
}
