using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;
namespace SagaMap.Tasks.System
{
    public class KnightWar50point : MultiRunTask
    {
        uint mapid;
        public KnightWar50point(uint mapid)
        {
            this.period = 5000;
            this.dueTime = 10000;
            this.mapid = mapid;
        }
        public override void CallBack()
        {
            try
            {
                byte arrX = 0;
                byte arrY = 0;
                bool check = false;
                Map map = SagaMap.Manager.MapManager.Instance.GetMap(mapid);
                foreach (Actor j in map.Actors.Values)
                {
                    if (j.type == ActorType.MOB)
                    {
                        ActorMob m = (ActorMob)j;
                        if (m.MobID == 40050050)
                        {
                            arrX = SagaLib.Global.PosX16to8(m.X, map.Width);
                            arrY = SagaLib.Global.PosY16to8(m.Y, map.Height);
                            check = true;
                        }
                    }
                }
                if (check)
                {
                    foreach (Actor j in map.Actors.Values)
                    {
                        if (j.type == ActorType.PC)
                        {
                            SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)j.e;
                            eh.Client.SendAnnounce("電氣石已出現！");
                            Packets.Server.SSMG_NPC_NAVIGATION p = new SagaMap.Packets.Server.SSMG_NPC_NAVIGATION();
                            p.X = arrX;
                            p.Y = arrY;
                            eh.Client.netIO.SendPacket(p);
                        }
                    }
                    this.Deactivate();
                }

            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
        }
    }
}
