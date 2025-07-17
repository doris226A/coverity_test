
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaDB.Item;
using SagaMap.PC;
using SagaMap.Network.Client;
namespace SagaMap.Skill.SkillDefinations.Blacksmith
{
    /// <summary>
    /// 根性（根性）
    /// </summary>
    public class Balls : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int lifetime = 180000;
            DefaultBuff skill = new DefaultBuff(args.skill, sActor, "Balls", lifetime);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(sActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.Spirit3RD = true;
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                StatusFactory.Instance.CalcStatus(pc);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                MapClient.FromActorPC((ActorPC)pc).SendStatus();
            }


        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            actor.Buff.Spirit3RD = false;
            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                StatusFactory.Instance.CalcStatus(pc);
                Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
                MapClient.FromActorPC((ActorPC)pc).SendStatus();
            }
        }
        #endregion
    }
}
