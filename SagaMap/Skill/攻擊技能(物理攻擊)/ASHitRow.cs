using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Knight
{
    /// <summary>
    /// 胳膊卸力（アームスラスト）
    /// </summary>
    public class ASHitRow : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 1000;
            float factor = 1.8f;
            SkillHandler.Instance.PhysicalAttack(sActor, dActor, args, sActor.WeaponElement, factor);
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "HitRow", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            float rate = 0.5f;
            int atk1 = (int)(actor.Status.min_atk1 * rate);
            int atk2 = (int)(actor.Status.min_atk2 * rate);
            int atk3 = (int)(actor.Status.min_atk3 * rate);

            if (skill.Variable.ContainsKey("HitRow_atk1_down"))
                skill.Variable.Remove("HitRow_atk1_down");
            skill.Variable.Add("HitRow_atk1_down", atk1);
            actor.Status.min_atk1_skill -= (short)atk1;

            if (skill.Variable.ContainsKey("HitRow_atk2_down"))
                skill.Variable.Remove("HitRow_atk2_down");
            skill.Variable.Add("HitRow_atk2_down", atk2);
            actor.Status.min_atk2_skill -= (short)atk2;

            if (skill.Variable.ContainsKey("HitRow_atk3_down"))
                skill.Variable.Remove("HitRow_atk3_down");
            skill.Variable.Add("HitRow_atk3_down", atk3);
            actor.Status.min_atk3_skill -= (short)atk3;
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.min_atk1_skill += (short)skill.Variable["HitRow_atk1_down"];
            actor.Status.min_atk2_skill += (short)skill.Variable["HitRow_atk2_down"];
            actor.Status.min_atk3_skill += (short)skill.Variable["HitRow_atk3_down"];
        }
        #endregion
    }
}
