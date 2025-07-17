using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Enchanter
{
    /// <summary>
    /// 重力刃 (グラヴィティフォール)
    /// </summary>
    public class BudGravityFall : ISkill
    {
        bool MobUse;
        public BudGravityFall()
        {
            this.MobUse = false;
        }
        public BudGravityFall(bool MobUse)
        {
            this.MobUse = MobUse;
        }
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            if (MobUse)
            {
                level = 5;
            }

            int rate = 50;
            int lifetime = 40000;

            List<Actor> realAffected = new List<Actor>();
            if (SkillHandler.Instance.CheckValidAttackTarget(sActor, dActor))
            {
                if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, SkillHandler.DefaultAdditions.Sleep, rate))
                {
                    Additions.Global.Sleep skill = new SagaMap.Skill.Additions.Global.Sleep(args.skill, dActor, lifetime);
                    SkillHandler.ApplyAddition(dActor, skill);
                }
                realAffected.Add(dActor);
            }
        }
        #endregion
    }
}
