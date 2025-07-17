using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Assassin
{
    public class BConClaw : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            bool active = false;
            float factor = 0.75f;
            DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor, "ConClaw", active);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
            List<Actor> target = new List<Actor>();
            for (int i = 0; i < 3; i++)
            {
                target.Add(dActor);
            }

            SkillHandler.Instance.PhysicalAttack(sActor, target, args, sActor.WeaponElement, factor);
        }
        void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            actor.Status.combo_rate_skill += 100;
            actor.Status.combo_skill = 3;

        }

        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            actor.Status.combo_rate_skill -= 100;
            actor.Status.combo_skill = 2;
        }
        #endregion
    }
}
