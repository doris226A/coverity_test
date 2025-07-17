
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaMap.Network.Client;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
namespace SagaMap.Skill.SkillDefinations.Global
{
    /// <summary>
    /// 魔法復位
    /// </summary>
    public class NekoPurple : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            DefaultBuff skill = new DefaultBuff(args.skill, dActor, "NekoPurple", 600000);
            skill.OnAdditionStart += this.StartEventHandler;
            skill.OnAdditionEnd += this.EndEventHandler;
            SkillHandler.ApplyAddition(dActor, skill);
        }
        void StartEventHandler(Actor actor, DefaultBuff skill)
        {
            MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("魔法復位已進入狀態");
        }
        void EndEventHandler(Actor actor, DefaultBuff skill)
        {
            MapClient.FromActorPC((ActorPC)actor).SendSystemMessage("魔法復位已解除狀態");
        }
        #endregion
    }
}
