using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaLib;
using static SagaMap.Skill.SkillHandler;

namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    ///  小丑-遠距離
    /// </summary>
    public class Joker_Long : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;

        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            args.argType = SkillArg.ArgType.Attack;
            float factor = new float[] { 0, 2.25f, 3.5f, 4.5f, 5.5f, 6.5f }[level];

            int[] attackTimes = { 0, 5, 5, 6, 6, 8 };
            //public int PhysicalAttack(Actor sActor, List<Actor> dActor, SkillArg arg, DefType defType, Elements element, int index, float ATKBonus, bool setAtk, float SuckBlood, bool doublehate, int shitbonus, int scribonus)
            List<Actor> dest = new List<Actor>();
            dest.Add(dActor);
            dest.Add(dActor);
            args.delayRate = 4.5f;
            SkillHandler.Instance.PhysicalAttack(sActor, dest, args, DefType.Def, sActor.WeaponElement, 0, factor, false, 0, false, 0, 100);
        }
        #endregion
    }
}
