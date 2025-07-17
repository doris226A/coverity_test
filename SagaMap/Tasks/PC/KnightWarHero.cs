using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;

using SagaMap.Network.Client;
namespace SagaMap.Tasks.PC
{
    public class KnightWarHero : MultiRunTask
    {
        ActorPC pc;
        public KnightWarHero(ActorPC pc)
        {
            this.dueTime = 120000;
            this.period = 120000;
            this.pc = pc;
        }

        public override void CallBack()
        {
            ClientManager.EnterCriticalArea();
            try
            {
                switch (pc.Mode)
                {
                    case PlayerMode.KNIGHT_EAST_HERO:
                        pc.Mode = PlayerMode.KNIGHT_EAST;
                        break;
                    case PlayerMode.KNIGHT_WEST_HERO:
                        pc.Mode = PlayerMode.KNIGHT_WEST;
                        break;
                    case PlayerMode.KNIGHT_SOUTH_HERO:
                        pc.Mode = PlayerMode.KNIGHT_SOUTH;
                        break;
                    case PlayerMode.KNIGHT_NORTH_HERO:
                        pc.Mode = PlayerMode.KNIGHT_NORTH;
                        break;
                }
                SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                if (pc.Tasks.ContainsKey("KnightWarHero"))
                    pc.Tasks.Remove("KnightWarHero");

                this.Deactivate();
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
                this.Deactivate();
            }
            ClientManager.LeaveCriticalArea();
        }
    }
}
