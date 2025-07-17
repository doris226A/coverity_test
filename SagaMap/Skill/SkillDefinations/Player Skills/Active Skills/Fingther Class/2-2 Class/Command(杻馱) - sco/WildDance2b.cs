using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Command
{
    /// <summary>
    /// 狂放舞蹈（ワイルドダンス）[接續技能2]
    /// </summary>
    public class WildDance2b : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float[] factors = { 0f, 1.0f, 1.5f, 2.5f, 3.5f, 4.0f };
            //float factor = 0.75f + 0.25f * level;
            float factor = factors[level];
            ActorPC actorPC = (ActorPC)sActor;
            uint MartialArtDamUp_SkillID = 125;
            if (actorPC.Skills2.ContainsKey(MartialArtDamUp_SkillID))
            {
                factor = factor + 0.1f * actorPC.Skills2[MartialArtDamUp_SkillID].Level;
            }
            if (actorPC.SkillsReserve.ContainsKey(MartialArtDamUp_SkillID))
            {
                factor = factor + 0.1f * actorPC.SkillsReserve[MartialArtDamUp_SkillID].Level;
            }
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
        }
        #endregion
    }
}
