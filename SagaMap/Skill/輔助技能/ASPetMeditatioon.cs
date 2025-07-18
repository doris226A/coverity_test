﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.BladeMaster
{
    /// <summary>
    /// 冥想（瞑想）
    /// </summary>
    public class ASPetMeditatioon : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            uint HP_ADD = (uint)((float)sActor.MaxHP * 0.02f * (float)level);
            SkillHandler.Instance.FixAttack(sActor, dActor, args, sActor.WeaponElement, -HP_ADD);
        }
        #endregion



    }
}