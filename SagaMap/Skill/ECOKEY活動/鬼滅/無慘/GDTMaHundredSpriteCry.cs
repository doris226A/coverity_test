using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.BladeMaster
{
    /// <summary>
    ///  百鬼哭（百鬼哭）
    /// </summary>
    public class GDTMaHundredSpriteCry : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
                return 0;

        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
         
            float factor = 2.5f; //1.7f

            int attackTimes = 10; //3
          
            args.argType = SkillArg.ArgType.Attack;
            args.type = ATTACK_TYPE.SLASH;
            List<Actor> dest = new List<Actor>();
            for (int i = 0; i < attackTimes; i++)
                dest.Add(dActor);
            args.delayRate = 4.5f;
            SkillHandler.Instance.PhysicalAttack(sActor, dest, args, sActor.WeaponElement, factor);
        }
        #endregion
    }
}
