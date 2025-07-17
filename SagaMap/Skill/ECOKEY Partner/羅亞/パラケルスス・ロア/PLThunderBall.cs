using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Shaman
{
    public class PLThunderBall : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
           /* if (SkillHandler.Instance.CheckValidAttackTarget(pc, dActor))
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

            float factor = 5.0f;

            SkillHandler.Instance.MagicAttack(sActor, dActor, args, SagaLib.Elements.Wind, factor);
            SkillHandler.Instance.MagicAttack(sActor, dActor, args, SagaLib.Elements.Fire, factor);
        }

        #endregion
    }
}
