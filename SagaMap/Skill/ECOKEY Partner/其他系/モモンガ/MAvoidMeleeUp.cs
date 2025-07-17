using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Scout
{
    /// <summary>
    /// 敏捷的動作
    /// </summary>
    public class MAvoidMeleeUp : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {

            int lifetime = 50000;
            Actor realactor = SkillHandler.Instance.GetPossesionedActor((ActorPC)sActor);
            DefaultBuff skill = new DefaultBuff(args.skill, realactor, "AvoidBurst", lifetime);
            skill.OnAdditionStart += skill_OnAdditionStart;
            skill.OnAdditionEnd += skill_OnAdditionEnd;
            SkillHandler.ApplyAddition(realactor, skill);
        }
        void skill_OnAdditionStart(Actor actor, DefaultBuff skill)
        {
            short savo = 6;
            if (skill.Variable.ContainsKey("AvoidBurst"))
                skill.Variable.Remove("AvoidBurst");
            skill.Variable.Add("AvoidBurst", savo);
            actor.Status.avoid_melee_skill += savo;
            actor.Buff.ShortDodgeUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void skill_OnAdditionEnd(Actor actor, DefaultBuff skill)
        {
            actor.Status.avoid_melee_skill -= (short)skill.Variable["AvoidBurst"];
            actor.Buff.ShortDodgeUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }



        #endregion
    }
}
