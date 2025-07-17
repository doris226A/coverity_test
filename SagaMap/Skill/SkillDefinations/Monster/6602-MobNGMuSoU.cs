using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaMap.Skill.Additions.Global;
using SagaDB.Actor;

namespace SagaMap.Skill.SkillDefinations.Monster
{
    /// <summary>
    /// 參擊無雙（斬撃無双）
    /// </summary>
    public class MobNGMuSoU : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            sActor.MuSoUCount = 0;

                float factor = 3.0f;
                args.argType = SkillArg.ArgType.Attack;
                args.type = ATTACK_TYPE.SLASH;

                if (sActor.MuSoUCount == 8)
                {
                    args.skill.BaseData.nAnim1 = args.skill.BaseData.nAnim2 = 332;
                }

                Stiff skill = new Stiff(args.skill, dActor, 1000);
                SkillHandler.ApplyAddition(dActor, skill);
                List<Actor> target = new List<Actor>();
                for (int i = 0; i <= 8; i++)
                {
                    target.Add(dActor);
                }

                SkillHandler.Instance.PhysicalAttack(sActor, target, args, sActor.WeaponElement, factor);

                sActor.MuSoUCount++;
                if (sActor.MuSoUCount == 8)
                    SkillHandler.Instance.PushBack(sActor, dActor, 2);

        }
        #endregion
    }
}
