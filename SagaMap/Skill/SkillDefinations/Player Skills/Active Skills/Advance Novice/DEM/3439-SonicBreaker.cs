using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// ソニックブレイカー
    /// </summary>
    public class SonicBreaker : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            level = 3;//锁死在3级，等待DEM正常化
            float factor = new float[] { 0, 18.0f, 23.5f, 28.0f }[level];
            args.type = ATTACK_TYPE.STAB;
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
        }

        #endregion
    }
}
