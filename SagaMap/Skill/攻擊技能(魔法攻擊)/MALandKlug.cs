using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Shaman
{
    public class MALandKlug : ISkill
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
                float factor = 1.15f;
                SkillHandler.Instance.MagicAttack(sActor, dActor, args, SagaLib.Elements.Earth, factor);   
                Skill.Additions.Global.Stone skill = new Additions.Global.Stone(args.skill, dActor, 2500);
                SkillHandler.ApplyAddition(dActor, skill);
        }

        #endregion
    }
}
