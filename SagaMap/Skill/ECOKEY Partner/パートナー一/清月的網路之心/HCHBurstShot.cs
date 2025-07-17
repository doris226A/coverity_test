using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Gunner
{
    /// <summary>
    /// 3連發精密射擊（バーストショット）
    /// </summary>
    public class HCHBurstShot : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
                return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            float factor = 1.1f;
            int rate = 5;
            args.argType = SkillArg.ArgType.Attack;
            List<Actor> affected = new List<Actor>();
            for (int i = 0; i < 3; i++)
            {
                affected.Add(dActor);
            }
            args.delayRate = 4.5f;
            SkillHandler.Instance.PhysicalAttack(sActor, affected, args, sActor.WeaponElement, factor);
            if (SkillHandler.Instance.CanAdditionApply(sActor, dActor, "BurstShot", rate))
            {
                DefaultBuff skill = new DefaultBuff(args.skill, dActor, "BurstShot", 15000);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(dActor, skill);
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {

            //近命中
            actor.Status.hit_melee_skill = (short)(-20);

            //遠命中
            actor.Status.hit_ranged_skill = (short)(-20);


        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.hit_melee_skill -= (short)(-20);

            //遠命中
            actor.Status.hit_ranged_skill -= (short)(-20);

        }
        #endregion
    }
}
