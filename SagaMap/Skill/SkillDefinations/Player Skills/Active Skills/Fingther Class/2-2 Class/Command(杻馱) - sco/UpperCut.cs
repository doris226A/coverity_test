using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Command
{
    /// <summary>
    /// 上段切（アッパーカット）
    /// </summary>
    public class UpperCut : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float[] factors = { 0f, 1.2f, 1.6f, 2.0f, 2.4f, 2.8f };
            uint MartialArtDamUp_SkillID = 125;
            ActorPC actorPC = (ActorPC)sActor;
            float factor = factors[level];
            if (actorPC.Skills2.ContainsKey(MartialArtDamUp_SkillID))
            {
                factor = factor + 0.1f * actorPC.Skills2[MartialArtDamUp_SkillID].Level;
            }
            if (actorPC.SkillsReserve.ContainsKey(MartialArtDamUp_SkillID))
            {
                factor = factor + 0.1f * actorPC.SkillsReserve[MartialArtDamUp_SkillID].Level;
            }

            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
            SkillHandler.Instance.PushBack(sActor, dActor, 3);
            int rate = 15 + 5 * level;
            if (SagaLib.Global.Random.Next(0, 99) < rate)
            {
                Additions.Global.Stun skill1 = new SagaMap.Skill.Additions.Global.Stun(args.skill, dActor, 1500);
                SkillHandler.ApplyAddition(dActor, skill1);
            }
        }
        #endregion
    }
}
