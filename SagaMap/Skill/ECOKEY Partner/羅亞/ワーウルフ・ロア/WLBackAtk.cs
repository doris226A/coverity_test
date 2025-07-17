
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
namespace SagaMap.Skill.SkillDefinations.Assassin
{
    /// <summary>
    /// 死神晚宴 | 背刺（バックアタック）
    /// </summary>
    public class WLBackAtk : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 2.1f;

            // 使用物理攻擊，根據背面傷害倍率計算傷害
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);

            // 如果成功攻擊到背面，並且滿足僵直的條件，應用僵直效果
            if (SkillHandler.Instance.GetIsBack(sActor, dActor) && SkillHandler.Instance.CanAdditionApply(sActor, dActor, SkillHandler.DefaultAdditions.Stiff, 95))
            {
                Skill.Additions.Global.Stiff stiff = new Additions.Global.Stiff(args.skill, dActor, 3000);
                SkillHandler.ApplyAddition(dActor, stiff);
            }
        }
        #endregion
    }
}