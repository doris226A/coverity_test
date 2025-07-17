
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Gunner
{
    /// <summary>
    /// 射程延長（オーバーレンジ）
    /// </summary>
    public class OverRange : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 30000;
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "OverRange", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
            if (dActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)dActor;
                pc.TInt["OverRangeSkill"] = level;
                SagaMap.PC.StatusFactory.Instance.CalcRange(pc);
            }
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {

            /*if (skill.Variable.ContainsKey("OverRange_skill"))
                skill.Variable.Remove("OverRange_skill");
            skill.Variable.Add("OverRange_skill", skill.skill.Level);*/

            //射程上升量為Level
            actor.Buff.OverRange = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);

        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.OverRange = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                SagaMap.PC.StatusFactory.Instance.CalcRange(pc);
            }
        }
        #endregion
    }
}
