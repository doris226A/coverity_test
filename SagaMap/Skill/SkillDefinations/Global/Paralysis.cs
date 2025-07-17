using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaDB.Skill;

namespace SagaMap.Skill.Additions.Global
{
    public class Paralysis : DefaultBuff
    {
        public Paralysis(SagaDB.Skill.Skill skill, Actor actor, int lifetime)
            : base(skill, actor, "Paralysis", (int)(lifetime * (1f - actor.AbnormalStatus[SagaLib.AbnormalStatus.Paralyse] / 255)), 100)
        {
            if (!SkillHandler.Instance.isBossMob(actor))
            {
                this.OnAdditionStart += this.StartEvent;
                this.OnAdditionEnd += this.EndEvent;
            }
        }

        void StartEvent(Actor actor, DefaultBuff skill)
        {
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            actor.Buff.Paralysis = true;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            if (skill.Variable.ContainsKey("ParalysisFrosenElement"))
                skill.Variable.Remove("ParalysisFrosenElement");
            skill.Variable.Add("ParalysisFrosenElement", 100 - actor.Elements[SagaLib.Elements.Wind]);
            actor.Elements[SagaLib.Elements.Wind] = 100;
            SkillHandler.Instance.CancelSkillCast(actor);
        }

        void EndEvent(Actor actor, DefaultBuff skill)
        {
            Map map = Manager.MapManager.Instance.GetMap(actor.MapID);
            actor.Buff.Paralysis = false;
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
            actor.Elements[SagaLib.Elements.Wind] -= skill.Variable["ParalysisFrosenElement"];
        }
    }
}
