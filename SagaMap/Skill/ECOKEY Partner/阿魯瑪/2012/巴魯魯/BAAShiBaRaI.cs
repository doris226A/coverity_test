﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.BountyHunter
{
    /// <summary>
    /// 絆腿（足払い）
    /// </summary>
    public class BAAShiBaRaI : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
            /*if (SkillHandler.Instance.CheckValidAttackTarget(sActor, dActor))
            {
                return 0;
            }
            else
            {
                return -14;
            }*/

        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 0.9f + 0.1f * level;
            /*  uint MartialArtDamUp_SkillID = 125;
              ActorPC actorPC = (ActorPC)sActor;
              if (actorPC.Skills2.ContainsKey(MartialArtDamUp_SkillID))
              {
                  if (actorPC.Skills2[MartialArtDamUp_SkillID].Level == 3)
                  {
                      factor = 1.32f + 0.1f * level;
                  }
              }
              if (actorPC.SkillsReserve.ContainsKey(MartialArtDamUp_SkillID))
              {
                  if (actorPC.SkillsReserve[MartialArtDamUp_SkillID].Level == 3)
                  {
                      factor = 1.32f + 0.1f * level;
                  }
              }*/
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
            int[] rate = { 0, 5, 10, 15, 20, 25, 30 };
            if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, SkillHandler.DefaultAdditions.Stiff, rate[level]))
            {
                Additions.Global.Stiff skill = new SagaMap.Skill.Additions.Global.Stiff(args.skill, dActor, 10000);
                SkillHandler.ApplyAddition(dActor, skill);
            }
        }
        #endregion
    }
}
