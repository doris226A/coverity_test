using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    ///  小丑-一般武器
    /// </summary>
    public class Joker_Normal : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;

        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            args.argType = SkillArg.ArgType.Attack;
            float factor = new float[] { 0, 2.3f, 2.6f, 2.9f, 3.2f, 3.5f }[level];

            int[] attackTimes = { 0, 5, 5, 6, 6, 8 };

            List<Actor> dest = new List<Actor>();
            for (int i = 0; i < attackTimes[level]; i++)
                dest.Add(dActor);
            args.delayRate = 4.5f;
            SkillHandler.Instance.PhysicalAttack(sActor, dest, args, sActor.WeaponElement, factor);
        }
        #endregion
    }
}
