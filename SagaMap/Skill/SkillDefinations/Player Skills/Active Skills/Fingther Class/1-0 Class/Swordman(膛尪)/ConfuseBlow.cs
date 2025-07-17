﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Swordman
{
    /// <summary>
    /// 暈眩攻擊
    /// </summary>
    public class ConfuseBlow : ISkill
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
            factor = 1.4f;
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
            if ((args.flag[0] & SagaLib.AttackFlag.HP_DAMAGE) != 0)
            {
                int rate = 0;
                int lifetime = 0;
                switch (level)
                {
                    case 2:
                        rate = 10;
                        lifetime = 3000;
                        break;
                    case 3:
                        rate = 15;
                        lifetime = 4000;
                        break;
                    case 4:
                        rate = 22;
                        lifetime = 5000;
                        break;
                    case 5:
                        rate = 30;
                        lifetime = 6000;
                        break;
                }

                if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, SkillHandler.DefaultAdditions.Confuse, rate))
                {
                    Additions.Global.Confuse skill = new SagaMap.Skill.Additions.Global.Confuse(args.skill, dActor, lifetime);
                    SkillHandler.ApplyAddition(dActor, skill);
                }
            }
        }

        #endregion
    }
}
