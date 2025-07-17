using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;

using SagaMap.Network.Client;
namespace SagaMap.Tasks.PC
{
    public class plugInTest : MultiRunTask
    {
        ActorPC pc;
        public plugInTest(ActorPC pc)
        {
            this.period = 20000;
            this.dueTime = 20000;
            this.pc = pc;            
        }

        public override void CallBack()
        {
            ClientManager.EnterCriticalArea();
            try
            {
                MapClient client = MapClient.FromActorPC((ActorPC)pc);

                if (pc.Tasks.ContainsKey("plugInTest"))
                    pc.Tasks.Remove("plugInTest");
                Logger log = new Logger("外掛紀錄.txt");
                string logtext = "\r\n" + client.Character.Name + "：" + "被踢下線";
                log.WriteLog(logtext);

                this.Deactivate();
                client.netIO.Disconnect();

            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
            ClientManager.LeaveCriticalArea();
        }
    }
}
