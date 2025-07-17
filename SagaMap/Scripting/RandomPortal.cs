using System;
using System.Collections.Generic;
using System.Text;

using SagaDB.Actor;
using SagaMap.Scripting;

using SagaLib;

namespace SagaMap.Scripting
{
    public abstract class RandomPortal : Event
    {
        public uint mapID;
        public byte x1, y1;
        public byte x2, y2;

        public void Init(uint eventID, uint mapID, byte x1, byte y1, byte x2, byte y2)
        {
            this.EventID = eventID;
            this.mapID = mapID;
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }

        public override void OnEvent(ActorPC pc)
        {
            byte x, y;

            x = (byte)Global.Random.Next(x1, x2);
            y = (byte)Global.Random.Next(y1, y2);

            Map map = SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID);
            if (map.IsMapInstance)
            {
                //ECOKEY 副本能傳送出去
                if (map.ClientExitMap != 0)
                {
                    Warp(pc, map.ClientExitMap, map.ClientExitX, map.ClientExitY);
                    return;
                }
                Say(pc, 0, "處於無法跳躍普通傳送點的地圖。 ", "可能處於副本中");
                return;
            }

            Warp(pc, mapID, x, y);
        }
    }
}