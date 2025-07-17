using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;

using SagaMap.Network.Client;
namespace SagaMap.Tasks.PC
{
    public class CityDown1 : MultiRunTask
    {
        MapClient client;
        public CityDown1(MapClient client)
        {
            this.dueTime = 5000;
            this.period = 5000;
            this.client = client;
        }


        public override void CallBack()
        {
           ClientManager.EnterCriticalArea();
            try
            {
                if (this.client.Character.Buff.ColdGuard == false)
                {
                    if (this.client.Character.PossessionTarget == 0)
                    {
                        if (this.client.Character.HP > 25)
                        this.client.Character.HP -= (uint)(25);
                    else
                        this.client.Character.HP = 1;
                    this.client.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.client.Character, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
                this.client.Character.Tasks.Remove("CityDown1");
                this.Deactivate();
            }
            ClientManager.LeaveCriticalArea();
        }
    }
}
