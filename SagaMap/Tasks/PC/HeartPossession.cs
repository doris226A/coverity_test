using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;
namespace SagaMap.Tasks.PC
{
    public class HeartPossession : MultiRunTask
    {
        ActorPC client;
        public HeartPossession(ActorPC client, int time)
        {
            this.dueTime = time;
            this.period = time;
            this.client = client;
        }

        public override void CallBack()
        {
            ClientManager.EnterCriticalArea();
            try
            {
                SagaMap.Manager.TamaireRentalManager.Instance.TerminateRental(client, 0);
                if (client.Tasks.ContainsKey("HeartPossession"))
                    client.Tasks.Remove("HeartPossession");
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
