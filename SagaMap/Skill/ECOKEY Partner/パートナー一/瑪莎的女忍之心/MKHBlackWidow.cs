using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Warlock
{
    public class MKHBlackWidow : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 0;
            args.type = ATTACK_TYPE.BLOW;

            factor = 3f;
            List<Actor> actors = new List<Actor>();
            {
                actors.Add(dActor);  //MagicAttack
                SkillHandler.Instance.PhysicalAttack(sActor, actors, args, SagaLib.Elements.Neutral, 0, factor);
            }
        }

        #endregion
    }
}
