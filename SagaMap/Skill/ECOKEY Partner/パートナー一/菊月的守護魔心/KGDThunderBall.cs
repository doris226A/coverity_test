using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Shaman
{
    public class KGDThunderBall : ISkill
    {
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
                return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 1.15f;

            SkillHandler.Instance.MagicAttack(sActor, dActor, args, SagaLib.Elements.Wind, factor);
        }

    }
}
