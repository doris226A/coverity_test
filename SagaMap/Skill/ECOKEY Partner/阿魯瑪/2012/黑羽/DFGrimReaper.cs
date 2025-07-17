using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Cabalist
{
    /// <summary>
    /// グリムリーパー
    /// </summary>
    public class DFGrimReaper : ISkill
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

            factor = 1.2f;
            List<Actor> actors = new List<Actor>();
            {
                actors.Add(dActor);
                SkillHandler.Instance.PhysicalAttack(sActor, actors, args, SkillHandler.DefType.Def, SagaLib.Elements.Dark, 0, factor, false, 0.1f, false);
            }

        }

        #endregion
    }
}
