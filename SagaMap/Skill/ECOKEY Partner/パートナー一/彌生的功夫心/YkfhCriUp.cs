using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaMap.Network.Client;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;

namespace SagaMap.Skill.SkillDefinations.Scout
{
    /// <summary>
    /// 會心一擊
    /// </summary>
    public class YkfhCriUp : ISkill
    {
        #region ISkill Members

        public int TryCast(ActorPC pc, Actor dActor, SkillArg args)
        {
            return 0;
        }

        bool CheckPossible(Actor sActor)
        {
            return true;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            int life = 10000;
            args.dActor = 0;
                DefaultBuff skill = new DefaultBuff(args.skill, dActor, "CriUp", life);
                skill.OnAdditionStart += this.StartEventHandler;
                skill.OnAdditionEnd += this.EndEventHandler;
                SkillHandler.ApplyAddition(dActor, skill);
            }

        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
                short rate = (short)(skill.skill.Level * 10);
            if (skill.Variable.ContainsKey("CriUp"))
                skill.Variable.Remove("CriUp");
            skill.Variable.Add("CriUp", rate);
            actor.Status.cri_skill += rate;

            actor.Buff.CriticalRateUp = true;
            //if (actor.type == ActorPC)
           // MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("會心一擊率提高已進入狀態");
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            int value = skill.Variable["CriUp"];
            actor.Status.cri_skill -= (short)value;

           actor.Buff.CriticalRateUp = false;
          //  MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("會心一擊率提高狀態已解除");
            Manager.MapManager.Instance.GetMap(actor.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, actor, true);
        }

        #endregion
    }
}
