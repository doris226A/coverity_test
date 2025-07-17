using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SagaMap.Mob;
using SagaLib;
using SagaDB.Actor;
using SagaDB.Item;
using SagaDB.ODWar;

using SagaMap.Manager;
using SagaMap.Network.Client;


using SagaMap.Scripting;
using SagaDB.Mob;
using SagaMap.ActorEventHandlers;

namespace SagaMap.Tasks.System
{
    public class GiantSkinRoad : MultiRunTask
    {
        private bool canSpawn = true; // 控制是否可以生成怪物
        private uint spawnCount = 0; // 已生成怪物计数
        private const uint maxSpawnCount = 1; // 允许生成的最大怪物数量
        private uint spawnCount1 = 0; // 已生成怪物计数
        private const uint maxSpawnCount1 = 1; // 允许生成的最大怪物数量

        public GiantSkinRoad()
        {
            this.period = 60000; // 每分钟运行一次
            this.dueTime = 0;
        }

        static GiantSkinRoad instance;

        public static GiantSkinRoad Instance
        {
            get
            {
                if (instance == null)
                    instance = new GiantSkinRoad();
                return instance;
            }
        }

        public override void CallBack()
        {
            byte x, y;
            DateTime now = DateTime.Now;
            if (now.Hour >= 0 && now.Hour <= 23)
            {
                if ((now.Minute == 0) && spawnCount > 0)
                {
                    MapClientManager.Instance.Announce("注意！東部平原上的巨大皮露露仍然在肆意破壞中！");
                }
                if ((now.Minute == 0) && canSpawn && spawnCount < maxSpawnCount)
                {
                    SagaMap.Map map = SagaMap.Manager.MapManager.Instance.GetMap(10025000);
                    if (map != null)
                    {
                        MapClientManager.Instance.Announce("注意！東部平原巨大皮露露已經從異世界偷渡到key世界！");
                        x = (byte)Global.Random.Next(32, 233);
                        y = (byte)Global.Random.Next(5, 235);
                        map.SpawnCustomMob(92400000, map.ID, 92400000, 0, 0, (byte)x, (byte)y, 0, 1, 3600, 活動怪物.巨大皮路路Info(), 活動怪物.巨大皮路路AI(), Ondie, 1);
                        spawnCount++;
                    }
                }
                
            }

            if (now.Hour >= 0 && now.Hour <= 23)
            {
                if ((now.Minute == 0) && spawnCount1 > 0)
                {
                    MapClientManager.Instance.Announce("注意！西部平原上的巨大皮露露仍然在肆意破壞中！");
                }
                if ((now.Minute == 0) && canSpawn && spawnCount1 < maxSpawnCount1)
                {
                    SagaMap.Map map1 = SagaMap.Manager.MapManager.Instance.GetMap(10022000);
                    if (map1 != null) // 判断地图是否存在
                    {
                        MapClientManager.Instance.Announce("注意！西部平原巨大皮露露已經從異世界偷渡到key世界！");
                        x = (byte)Global.Random.Next(32, 233);
                        y = (byte)Global.Random.Next(5, 235);
                        map1.SpawnCustomMob(92400000, map1.ID, 92400000, 0, 0, (byte)x, (byte)y, 0, 1, 3600, 活動怪物.巨大皮路路Info(), 活動怪物.巨大皮路路AI(), Ondie1, 1);
                    spawnCount1++;
                    }
                }
            }
            
        }


        void Ondie(MobEventHandler e, ActorPC pc)
        {
            MapClientManager.Instance.Announce("東部平原巨大皮露露已經逃回了異世界,現在平原暫時已經安全了！");
            spawnCount = 0; // 重置已生成怪物计数
        }

        void Ondie1(MobEventHandler e, ActorPC pc)
        {
            MapClientManager.Instance.Announce("西部平原巨大皮露露已經逃回了異世界,現在平原暫時已經安全了！");
            spawnCount1 = 0; // 重置已生成怪物计数
        }
    }
}