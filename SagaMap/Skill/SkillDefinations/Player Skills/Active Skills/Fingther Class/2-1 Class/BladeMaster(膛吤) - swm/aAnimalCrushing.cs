﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.BladeMaster
{
    /// <summary>
    ///  アギト砕き
    /// </summary>
    public class aAnimalCrushing : BeheadSkill, ISkill
    {
        #region ISkill Members
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            this.Proc(sActor, dActor, args, level, SagaDB.Mob.MobType.WATER_ANIMAL);
        }
        #endregion
    }
}

