
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// 各屬性守護(各属性契约)
    /// </summary>
    public class ElementLimitUp : ISkill
    {
        public Elements element;
        public ElementLimitUp(Elements e)
        {
            element = e;
        }
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            bool active = true;
            DefaultPassiveSkill skill = new DefaultPassiveSkill(args.skill, sActor,element.ToString()+ "LimitUp", active);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            if (!actor.Status.ElementContract.ContainsKey(element))
                actor.Status.ElementContract.Add(element, skill.skill.Level);
            else
                actor.Status.ElementContract[element] = Math.Max(actor.Status.ElementContract[element], skill.skill.Level);
        }
        void EndEventHandler(Actor actor, DefaultPassiveSkill skill)
        {
            if (actor.Status.ElementContract.ContainsKey(element))
                actor.Status.ElementContract.Remove(element);
        }
        #endregion
    }
}

