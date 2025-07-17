using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;

using SagaMap.Network.Client;
namespace SagaMap.Tasks.PC
{
    public class KnightWarDelay : MultiRunTask
    {
        ActorPC pc;
        PlayerMode mode;
        int count = 30;
        public KnightWarDelay(ActorPC pc, PlayerMode mode)
        {
            this.dueTime = 1000;
            this.period = 1000;
            this.pc = pc;
            this.mode = mode;
        }

        public override void CallBack()
        {
            ClientManager.EnterCriticalArea();
            try
            {
                count--;
                if (count <= 0)
                {
                    pc.Mode = mode;
                    SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                    MapClient.FromActorPC((ActorPC)pc).SendSystemMessage("騎士團開始！");
                    if (pc.Tasks.ContainsKey("KnightWarDelay"))
                        pc.Tasks.Remove("KnightWarDelay");

                    this.Deactivate();
                }
                else
                {
                    if (count <= 5)
                    {
                        MapClient.FromActorPC((ActorPC)pc).SendSystemMessage("騎士團開始倒數" + count + "秒");
                    }
                }


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
