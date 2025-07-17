using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Swordman
{
    /// <summary>
    /// 乾坤一擊
    /// </summary>
    public class TAHBlow : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
                return 0;
        }

       

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
                args.type = ATTACK_TYPE.BLOW;

            float factor = 2.0f;

                SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
        }

        #endregion
    }
}