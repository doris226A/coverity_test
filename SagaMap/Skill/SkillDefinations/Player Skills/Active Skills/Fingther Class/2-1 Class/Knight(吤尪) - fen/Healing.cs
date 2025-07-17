using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Knight
{
    public class Healing : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            if (pc.Status.Additions.ContainsKey("Spell"))
            {
                return -7;
            }
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            long HP_ADD = (long)(dActor.MaxHP * 0.08f * level);
            if (dActor.type == ActorType.MOB)
            {
                ActorMob m = (ActorMob)dActor;
                if (m.Status.undead) HP_ADD = -HP_ADD;
            }
            if (dActor.Buff.Undead)
            {
                HP_ADD = -HP_ADD;
            }
            SkillHandler.Instance.FixAttack(sActor, dActor, args, SagaLib.Elements.Holy, -HP_ADD);
        }
        #endregion
    }
}
