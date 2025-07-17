
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SagaDB.Actor;
using SagaMap.Network.Client;
using SagaLib;
namespace SagaMap.Skill.SkillDefinations.Shaman
{
    /// <summary>
    /// 屬性調查（センスエレメント）
    /// </summary>
    public class SenseElement : ISkill
    {
        #region ISkill Members
        public int TryCast(ActorPC sActor, Actor dActor, SkillArg args)
        {
            return 0;
        }
        public void Proc(Actor sActor, Actor dActor, SkillArg args, byte level)
        {
            ActorPC pc = MapClient.FromActorPC((ActorPC)sActor).Character;
            byte x, y;
            x = SagaLib.Global.PosX16to8(pc.X, MapClient.FromActorPC((ActorPC)sActor).map.Width);
            y = SagaLib.Global.PosY16to8(pc.Y, MapClient.FromActorPC((ActorPC)sActor).map.Height);

            MapClient.FromActorPC((ActorPC)sActor).SendSystemMessage("當前座標資訊： [" + x + "," + y + "]");
            MapClient.FromActorPC((ActorPC)sActor).SendSystemMessage("-----地圖屬性-----");
            MapClient.FromActorPC((ActorPC)sActor).SendSystemMessage("水: " + MapClient.FromActorPC((ActorPC)sActor).Map.Info.water[x, y].ToString());
            MapClient.FromActorPC((ActorPC)sActor).SendSystemMessage("火: " + MapClient.FromActorPC((ActorPC)sActor).Map.Info.fire[x, y].ToString());
            MapClient.FromActorPC((ActorPC)sActor).SendSystemMessage("地: " + MapClient.FromActorPC((ActorPC)sActor).Map.Info.earth[x, y].ToString());
            MapClient.FromActorPC((ActorPC)sActor).SendSystemMessage("風: " + MapClient.FromActorPC((ActorPC)sActor).Map.Info.wind[x, y].ToString());
            MapClient.FromActorPC((ActorPC)sActor).SendSystemMessage("暗: " + MapClient.FromActorPC((ActorPC)sActor).Map.Info.dark[x, y].ToString());
            MapClient.FromActorPC((ActorPC)sActor).SendSystemMessage("光: " + MapClient.FromActorPC((ActorPC)sActor).Map.Info.holy[x, y].ToString());
            MapClient.FromActorPC((ActorPC)sActor).SendSystemMessage("無: " + MapClient.FromActorPC((ActorPC)sActor).Map.Info.neutral[x, y].ToString());
        }
        #endregion
    }
}