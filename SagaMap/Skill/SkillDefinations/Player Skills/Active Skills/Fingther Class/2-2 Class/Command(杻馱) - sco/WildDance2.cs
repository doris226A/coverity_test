using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Command
{
    /// <summary>
    /// 狂放舞蹈（ワイルドダンス）
    /// </summary>
    public class WildDance2 : ISkill
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
            //float[] factors = { 0f, 1.2f, 1.6f, 2.0f, 2.4f, 2.8f };
            float[] factors = { 0f, 1.0f, 1.5f, 2.5f, 3.5f, 4.0f };
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


            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2415, level, 1000));
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2416, level, 1000));
            args.autoCast.Add(SkillHandler.Instance.CreateAutoCastInfo(2417, level, 1000));

            //if (actorPC.Skills2.ContainsKey(MartialArtDamUp_SkillID))
            //{
            //    if (actorPC.Skills2[MartialArtDamUp_SkillID].Level == 3)
            //    {
            //        factor = 0.52f + 0.52f * level;
            //    }
            //}
            //if (actorPC.SkillsReserve.ContainsKey(MartialArtDamUp_SkillID))
            //{
            //    if (actorPC.SkillsReserve[MartialArtDamUp_SkillID].Level == 3)
            //    {
            //        factor = 0.52f + 0.52f * level;
            //    }
            //}
            /*args.argType = SkillArg.ArgType.Attack;
            args.type = ATTACK_TYPE.BLOW;
            List<Actor> dest = new List<Actor>();
            for (int i = 0; i < 4; i++)
            {
                dest.Add(dActor);
            }
            args.delayRate = 4f;
            SkillHandler.Instance.PhysicalAttack(sActor, dest, args, sActor.WeaponElement, factor);*/
        }
        #endregion
    }
}
