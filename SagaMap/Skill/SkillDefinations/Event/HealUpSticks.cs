
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Event
{
    /// <summary>
    /// 回復训练
    /// </summary>
    public class HealUpSticks : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "HealUpSticks", 600000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            int heal = 20;

            if (skill.Variable.ContainsKey("HealUpSticks"))
                skill.Variable.Remove("HealUpSticks");
            skill.Variable.Add("HealUpSticks", heal);
            actor.Status.hp_recover_skill += (short)heal;
            actor.Status.mp_recover_skill += (short)heal;
            actor.Status.sp_recover_skill += (short)heal;

            actor.Buff.HPRegenUp = true;
            actor.Buff.MPRegenUp = true;
            actor.Buff.SPRegenUp = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Status.hp_recover_skill -= (short)skill.Variable["HealUpSticks"];
            actor.Status.mp_recover_skill -= (short)skill.Variable["HealUpSticks"];
            actor.Status.sp_recover_skill -= (short)skill.Variable["HealUpSticks"];

            actor.Buff.HPRegenUp = false;
            actor.Buff.MPRegenUp = false;
            actor.Buff.SPRegenUp = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        #endregion
    }
}
