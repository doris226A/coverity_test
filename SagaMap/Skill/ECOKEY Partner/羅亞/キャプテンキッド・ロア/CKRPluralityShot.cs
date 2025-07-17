using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Gunner
{
    /// <summary>
    /// 散彈射擊（バラージショット）
    /// </summary>
    public class CKRPluralityShot : ISkill
    {
        #region ISkill Members
        int[] numdownmindouble = new int[] { 0, 3, 3, 4, 4, 5 };
        int[] numdownmaxdouble = new int[] { 0, 3, 4, 5, 6, 7 };
        int numdown = 0;
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            numdown = SagaLib.Global.Random.Next(numdownmindouble[args.skill.Level], numdownmaxdouble[args.skill.Level]);
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 1.2f;
            args.argType = SkillArg.ArgType.Attack;
            List<Actor> target = new List<Actor>();
            for (int i = 0; i < numdown; i++)
            {
                target.Add(dActor);
            }
            args.delayRate = 4.5f;
            SkillHandler.Instance.PhysicalAttack(sActor, target, args, sActor.WeaponElement, factor);
        }
        #endregion
    }
}
