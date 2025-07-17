using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.ECOKEY
{

    /// <summary>
    /// 機械回復
    /// </summary>
    public class SNFixHealing : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            long HP_ADD = (long)(dActor.MaxHP * 0.3f);
            if (dActor.Buff.Undead)
            {
                HP_ADD = -HP_ADD;
            }
            SkillHandler.Instance.FixAttack(sActor, dActor, args, SagaLib.Elements.Holy, -HP_ADD);
        }
        #endregion
    }
}
