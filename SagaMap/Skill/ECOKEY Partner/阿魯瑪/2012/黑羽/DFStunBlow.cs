using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Swordman
{
    /// <summary>
    /// 麻痺攻擊（スタンブロウ）
    /// </summary>
    public class DFStunBlow : ISkill
    {
        #region ISkill Members
        bool MobUse;
        public DFStunBlow()
        {
            this.MobUse = false;
        }
        public DFStunBlow(bool MobUse)
        {
            this.MobUse = MobUse;
        }

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 0;
            args.type = ATTACK_TYPE.BLOW;
            factor = 1.5f;

            if (MobUse)
            {
                level = 3;
            }

            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
            if (level > 1 && ((args.flag[0] & SagaLib.AttackFlag.HP_DAMAGE) != 0))
            {
                int rate = 20;
                int lifetime = 3000;

                if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, SkillHandler.DefaultAdditions.Stun, rate))
                {
                    Additions.Global.Stun skill = new SagaMap.Skill.Additions.Global.Stun(args.skill, dActor, lifetime);
                    SkillHandler.ApplyAddition(dActor, skill);
                }
            }
        }

        #endregion
    }
}
