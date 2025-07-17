using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaLib;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.SoulTaker
{
    class PossessorSeal : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 25000 + 5000 * level;
            if (dActor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)dActor;
                foreach (ActorPC i in pc.PossesionedActors)
                {
                    if (i == null || i.Status == null || i == pc || !i.Online)
                        continue;
                    DefaultBuff skill2 = new DefaultBuff(args.skill, i, "PossessorSeal", lifetime);
                    skill2.OnAdditionStart += this.StartEventHandler;
                    skill2.OnAdditionEnd += this.EndEventHandler;
                    SkillHandler.ApplyAddition(i, skill2);
                }
                DefaultBuff skill = new DefaultBuff(args.skill, dActor, "PossessorSeal", lifetime);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(dActor, skill);
            }
        }
        #endregion
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.三转凭依者封印 = true;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.三转凭依者封印 = false;
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }
    }
}