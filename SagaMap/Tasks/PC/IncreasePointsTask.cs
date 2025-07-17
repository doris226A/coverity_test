using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;

using SagaMap.Network.Client;
namespace SagaMap.Tasks.PC
{
    public class IncreasePointsTask : MultiRunTask
    {
        ActorPC pc;
        public IncreasePointsTask(ActorPC pc)
        {
            this.dueTime = 3600000;
            this.period = 3600000; // 设置任务执行周期为60分钟
            this.pc = pc;
        }

        public override void CallBack()
        {
            // 检查玩家是否在线
            if (!pc.Online)
            {
                this.Deactivate(); // 如果玩家不在线，停止任务
                return;
            }

            // 每隔一分钟增加玩家的 NCShopPoints 属性值
            //雖然不知道有沒有用，新增線程判斷
            ClientManager.EnterCriticalArea();
            pc.NCShopPoints += 1;
            MapClient.FromActorPC((ActorPC)pc).SendSystemMessage("玩家在線已達到1小時，獲得了1NC點數");
            ClientManager.LeaveCriticalArea();
        }
    }
}