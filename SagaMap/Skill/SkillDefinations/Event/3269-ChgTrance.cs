
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaDB.Skill;
namespace SagaMap.Skill.SkillDefinations.Event
{
    /// <summary>
    /// 蝙蝠變身
    /// </summary>
    public class ChgTrance : ISkill
    {
        private bool isTransformed = false;
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 300000;// * level; 
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "ChgTrance", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            ActorPC pc = (ActorPC)actor;
            if (pc.PossessionTarget != 0)
            {
                return;
            }
            else
            {
                if (isTransformed == true)
                {
                    RemoveTransformSkills(pc, skill.skill.Level);
                    SkillHandler.Instance.TranceMob(pc, 0);
                    isTransformed = false; // 设置为非变身状态
                }
                else
                {
                    ApplyTransformSkills(pc, skill.skill.Level);
                    isTransformed = true; // 设置为变身状态
                }
            }
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
          /*  ActorPC pc = (ActorPC)actor;
            if (isTransformed == true)
            {
                RemoveTransformSkills(pc, skill.skill.Level);
                SkillHandler.Instance.TranceMob(pc, 0);
                isTransformed = false; // 设置为非变身状态
            }
            else
            {
                RemoveTransformSkills(pc, skill.skill.Level);
                SkillHandler.Instance.TranceMob(pc, 0);
                isTransformed = false; // 设置为非变身状态
            }*/
        }

        void ApplyTransformSkills(ActorPC pc, int skillLevel)
        {
            switch (skillLevel)
            {
                case 1:
                    AddSkill(pc, 952, 1); // 吸血
                    AddSkill(pc, 3282, 1); // 隐身
                    SkillHandler.Instance.TranceMob(pc, 10020000); // 蝙蝠
                    break;
                case 2:
                    AddSkill(pc, 6309, 1); // 野兽震怒
                    AddSkill(pc, 6303, 1); // 咆哮
                    SkillHandler.Instance.TranceMob(pc, 10136901); // 魔法狼
                    break;
                case 3:
                    AddSkill(pc, 3279, 1); // 元灵护盾
                    AddSkill(pc, 3280, 1); // 元素护盾
                    AddSkill(pc, 7738, 1); // 魔法冲击波
                    SkillHandler.Instance.TranceMob(pc, 10410000); // 诅咒者
                    break;
            }
        }

        void RemoveTransformSkills(ActorPC pc, int skillLevel)
        {
            switch (skillLevel)
            {
                case 1:
                    DelSkill(pc, 952); // 吸血
                    DelSkill(pc, 3282); // 隐身

                    DelSkill(pc, 6309); // 野兽震怒
                    DelSkill(pc, 6303); // 咆哮
                    DelSkill(pc, 3279); // 元灵护盾
                    DelSkill(pc, 3280); // 元素护盾
                    DelSkill(pc, 7738); // 魔法冲击波
                    break;
                case 2:
                    DelSkill(pc, 6309); // 野兽震怒
                    DelSkill(pc, 6303); // 咆哮

                    DelSkill(pc, 952); // 吸血
                    DelSkill(pc, 3282); // 隐身
                    DelSkill(pc, 3279); // 元灵护盾
                    DelSkill(pc, 3280); // 元素护盾
                    DelSkill(pc, 7738); // 魔法冲击波
                    break;
                case 3:
                    DelSkill(pc, 3279); // 元灵护盾
                    DelSkill(pc, 3280); // 元素护盾
                    DelSkill(pc, 7738); // 魔法冲击波

                    DelSkill(pc, 6309); // 野兽震怒
                    DelSkill(pc, 6303); // 咆哮
                    DelSkill(pc, 952); // 吸血
                    DelSkill(pc, 3282); // 隐身
                    break;
            }
        }

        void AddSkill(ActorPC actor, uint skillID, byte level)
        {
            var skill = SkillFactory.Instance.GetSkill(skillID, level);
            if (skill != null)
            {
                skill.NoSave = true;
                actor.Skills[skill.ID] = skill;
            }
        }

        void DelSkill(ActorPC actor, uint skillID)
        {
            if (actor.Skills.ContainsKey(skillID))
            {
                actor.Skills.Remove(skillID);
            }
        }

        #endregion
    }
}
