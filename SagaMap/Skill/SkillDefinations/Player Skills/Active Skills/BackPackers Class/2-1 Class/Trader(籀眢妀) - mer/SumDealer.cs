
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Network.Client;
namespace SagaMap.Skill.SkillDefinations.Trader
{
    /// <summary>
    /// 召喚商人（商人召喚）
    /// </summary>
    public class SumDealer : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            if (sActor.type == ActorType.PC)
            {
                ActorPC pc;
                pc = (ActorPC)sActor;
                Map map = Manager.MapManager.Instance.GetMap(pc.MapID);
                if (!map.Info.Healing)
                {
                    return 0;
                }
            }
            return -32;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            ActorPC pc;
            pc = (ActorPC)sActor;
            pc.TInt["SumDealerX"] = args.x;
            pc.TInt["SumDealerY"] = args.y;
            MapClient client1 = MapClient.FromActorPC((ActorPC)sActor);
            uint Event = 91000535;
            client1.EventActivate(Event);
        }
        #endregion
    }
}