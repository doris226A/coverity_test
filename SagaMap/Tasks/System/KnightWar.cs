using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaDB.Actor;
using SagaDB.Item;
using SagaDB.KnightWar;

using SagaMap.Manager;
using SagaMap.Network.Client;


namespace SagaMap.Tasks.System
{
    public class KnightWar : MultiRunTask
    {
        public KnightWar()
        {
            this.period = 60000;
            this.dueTime = 0;
        }
        static KnightWar instance;
        int part = 1;
        //List<ActorPC> players = new List<ActorPC>();

        public static KnightWar Instance
        {
            get
            {
                if (instance == null)
                    instance = new KnightWar();
                return instance;
            }
        }
        #region "大逃殺專用"

        //第一次大逃殺活動開始
        public void readyKnightEvent()
        {
            DateTime now = new DateTime(1970, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            foreach (SagaDB.KnightWar.KnightWar i in KnightWarFactory.Instance.Items.Values)
            {
                if (i.ID != 1)
                    continue;
                DateTime time = i.StartTime;
                TimeSpan ts = time.Subtract(now);
                int nowmin = (int)ts.TotalMinutes;
                if (nowmin == 30)
                {
                    MapClientManager.Instance.Announce("大逃殺活動即將在30分鐘後開始，請有興趣的玩家，前往競技場報名參加。");
                }
                if (nowmin == 15)
                {
                    MapClientManager.Instance.Announce("大逃殺活動即將在15分鐘後開始，請有興趣的玩家，請前往競技場報名參加。");
                }
                if (nowmin == 5)
                {
                    MapClientManager.Instance.Announce("大逃殺活動即將在5分鐘後開始，請有興趣的玩家，請前往競技場報名參加。");
                }
                if (nowmin == 1)
                {
                    MapClientManager.Instance.Announce("大逃殺活動即將在1分鐘後開始，玩家請勿離開準備室");
                }
                if (now.Hour == i.StartTime.Hour && now.Minute == i.StartTime.Minute)
                {
                    #region "活動開始 指定地圖隨機位置傳送+mode變更"
                    MapClientManager.Instance.Announce("大逃殺活動開始！逃離魔王的追捕吧！");
                    List<ActorPC> actors = new List<ActorPC>();
                    foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080011).Actors.Values)
                    {
                        if (j.type == ActorType.PC)
                        {
                            actors.Add((ActorPC)j);
                        }
                    }

                    foreach (ActorPC pc in actors)
                    {
                        pc.Mode = PlayerMode.KNIGHT_NORTH;
                        SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                        SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                        Map newMap = MapManager.Instance.GetMap(i.MapID);
                        int point = Global.Random.Next(1, 3);
                        eh.Client.Map.SendActorToMap(pc, i.MapID, Global.PosX8to16(i.Loc[point.ToString()].x, newMap.Width), Global.PosY8to16(i.Loc[point.ToString()].y, newMap.Height));
                    }
                    i.Started = true;
                    #endregion
                }
                if (i.Started == true)
                {
                    List<ActorPC> actors2 = new List<ActorPC>();
                    #region "活動結束 傳送回準備室+送禮"
                    if (now.Minute == (i.StartTime.AddMinutes(i.Duration).Minute - 1))
                    {
                        //準備室的人
                        foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080011).Actors.Values)
                        {
                            if (j.type == ActorType.PC)
                            {
                                ActorPC pc = (ActorPC)j;
                                SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                                eh.Client.SendAnnounce("大逃殺活動即將在1分鐘後結束");
                            }
                        }
                        //活動地圖的人
                        foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(i.MapID).Actors.Values)
                        {
                            if (j.type == ActorType.PC)
                            {
                                ActorPC pc = (ActorPC)j;
                                SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                                eh.Client.SendAnnounce("大逃殺活動即將在1分鐘後結束");
                            }
                        }
                    }
                    if (now.Minute == (i.StartTime.AddMinutes(i.Duration).Minute))
                    {
                        //需要傳訊息的所有人
                        List<ActorPC> actors = new List<ActorPC>();
                        //List<ActorPC> GMactors = new List<ActorPC>();
                        //GM數量
                        int GMnum = 0;
                        //存活玩家數量
                        int PCnum = 0;
                        //活動地圖的人
                        foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(i.MapID).Actors.Values)
                        {
                            if (j.type == ActorType.PC)
                            {
                                ActorPC player = (ActorPC)j;
                                actors.Add((ActorPC)j);
                                if (player.HP > 0)
                                {
                                    if (player.Account.GMLevel == 0)
                                    {
                                        PCnum++;
                                    }
                                    if (player.Account.GMLevel > 0)
                                    {
                                        GMnum++;
                                    }
                                }
                            }
                        }
                        //準備室的人
                        foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080011).Actors.Values)
                        {
                            if (j.type == ActorType.PC)
                            {
                                ActorPC player = (ActorPC)j;
                                actors.Add((ActorPC)j);
                            }
                        }
                        if (PCnum > 0)
                        {
                            if (GMnum == 0)
                            {
                                MapClientManager.Instance.Announce("大逃殺活動結束，魔王們再起不能，恭喜玩家獲得勝利！");
                            }
                            else
                            {
                                MapClientManager.Instance.Announce("大逃殺活動結束，共有" + PCnum + "位玩家逃離魔王的追殺，恭喜玩家獲得勝利！");
                            }
                            foreach (ActorPC pc in actors)
                            {
                                if (pc == null)
                                    continue;
                                SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                                //給道具
                                SagaDB.Item.Item item = ItemFactory.Instance.GetItem(93000046);
                                item.Stack = 10;
                                item.Identified = true;
                                eh.Client.AddItem(item, true);
                                //
                                pc.Mode = PlayerMode.NORMAL;
                                SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                                Map newMap = MapManager.Instance.GetMap(20080011);
                                eh.Client.Map.SendActorToMap(pc, 20080011, Global.PosX8to16(20, newMap.Width), Global.PosY8to16(20, newMap.Height));
                                eh.Client.SendAnnounce("下一場大逃殺活動即將在10分鐘後開始，請勿離開準備室。");
                            }
                        }
                        else
                        {
                            if (GMnum == 1)
                            {
                                MapClientManager.Instance.Announce("大逃殺活動結束！僅剩的魔王抹了一把冷汗，玩家請再接再厲！");
                            }
                            else
                            {
                                MapClientManager.Instance.Announce("大逃殺活動結束！魔王們發出了險惡的笑聲，玩家請再接再厲！");
                            }
                            foreach (ActorPC pc in actors)
                            {
                                if (pc == null)
                                    continue;
                                SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;

                                //給道具
                                SagaDB.Item.Item item = ItemFactory.Instance.GetItem(93000046);
                                item.Stack = 1;
                                item.Identified = true;
                                eh.Client.AddItem(item, true);
                                //
                                pc.Mode = PlayerMode.NORMAL;
                                SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                                Map newMap = MapManager.Instance.GetMap(20080011);
                                eh.Client.Map.SendActorToMap(pc, 20080011, Global.PosX8to16(20, newMap.Width), Global.PosY8to16(20, newMap.Height));
                                eh.Client.SendAnnounce("下一場大逃殺活動即將在10分鐘後開始，請勿離開準備室");
                            }
                        }
                        part++;
                        i.Started = false;
                    }
                    #endregion
                }
            }
        }
        //第二次以上大逃殺活動開始
        public void partKnightEvent()
        {
            DateTime now = DateTime.Now;
            foreach (SagaDB.KnightWar.KnightWar i in KnightWarFactory.Instance.Items.Values)
            {
                if (i.ID == 1)
                    continue;
                if (now.Hour == i.StartTime.Hour && now.Minute == (i.StartTime.Minute - 5))
                {
                    //準備室的人
                    foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080011).Actors.Values)
                    {
                        if (j.type == ActorType.PC)
                        {
                            ActorPC pc = (ActorPC)j;
                            SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                            eh.Client.SendAnnounce("下一場大逃殺活動即將在5分鐘後開始，請勿離開準備室");
                        }
                    }
                    //活動地圖的人
                    foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(i.MapID).Actors.Values)
                    {
                        if (j.type == ActorType.PC)
                        {
                            ActorPC pc = (ActorPC)j;
                            SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                            eh.Client.SendAnnounce("下一場大逃殺活動即將在5分鐘後開始，請勿離開準備室");
                        }
                    }
                }
                if (now.Hour == i.StartTime.Hour && now.Minute == (i.StartTime.Minute - 1))
                {
                    //準備室的人
                    foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080011).Actors.Values)
                    {
                        if (j.type == ActorType.PC)
                        {
                            ActorPC pc = (ActorPC)j;
                            SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                            eh.Client.SendAnnounce("下一場大逃殺活動即將在1分鐘後開始，請勿離開準備室");
                        }
                    }
                    //活動地圖的人
                    foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(i.MapID).Actors.Values)
                    {
                        if (j.type == ActorType.PC)
                        {
                            ActorPC pc = (ActorPC)j;
                            SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                            eh.Client.SendAnnounce("下一場大逃殺活動即將在1分鐘後開始，請勿離開準備室");
                        }
                    }
                }
                if (now.Hour == i.StartTime.Hour && now.Minute == i.StartTime.Minute)
                {
                    #region "活動開始 指定地圖隨機位置傳送+mode變更"
                    MapClientManager.Instance.Announce("第" + part + "場大逃殺活動開始！逃離魔王的追捕吧！");
                    List<ActorPC> actors = new List<ActorPC>();
                    foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080011).Actors.Values)
                    {
                        if (j.type == ActorType.PC)
                        {
                            actors.Add((ActorPC)j);
                        }
                    }

                    foreach (ActorPC pc in actors)
                    {
                        pc.Mode = PlayerMode.KNIGHT_NORTH;
                        SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                        SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                        Map newMap = MapManager.Instance.GetMap(i.MapID);
                        int point = Global.Random.Next(1, 3);
                        eh.Client.Map.SendActorToMap(pc, i.MapID, Global.PosX8to16(i.Loc[point.ToString()].x, newMap.Width), Global.PosY8to16(i.Loc[point.ToString()].y, newMap.Height));
                    }
                    i.Started = true;
                    #endregion
                }
                if (i.Started == true)
                {
                    List<ActorPC> actors2 = new List<ActorPC>();
                    #region "活動結束 傳送回準備室+送禮"
                    if (now.Minute == (i.StartTime.AddMinutes(i.Duration).Minute - 1))
                    {
                        //準備室的人
                        foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080011).Actors.Values)
                        {
                            if (j.type == ActorType.PC)
                            {
                                ActorPC pc = (ActorPC)j;
                                SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                                eh.Client.SendAnnounce("大逃殺活動即將在1分鐘後結束");
                            }
                        }
                        //活動地圖的人
                        foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(i.MapID).Actors.Values)
                        {
                            if (j.type == ActorType.PC)
                            {
                                ActorPC pc = (ActorPC)j;
                                SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                                eh.Client.SendAnnounce("大逃殺活動即將在1分鐘後結束");
                            }
                        }
                    }
                    if (now.Minute == (i.StartTime.AddMinutes(i.Duration).Minute))
                    {
                        //需要傳訊息的所有人
                        List<ActorPC> actors = new List<ActorPC>();
                        //List<ActorPC> GMactors = new List<ActorPC>();
                        //GM數量
                        int GMnum = 0;
                        //存活玩家數量
                        int PCnum = 0;
                        //活動地圖的人
                        foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(i.MapID).Actors.Values)
                        {
                            if (j.type == ActorType.PC)
                            {
                                ActorPC player = (ActorPC)j;
                                actors.Add((ActorPC)j);
                                if (player.HP > 0)
                                {
                                    if (player.Account.GMLevel == 0)
                                    {
                                        PCnum++;
                                    }
                                    if (player.Account.GMLevel > 0)
                                    {
                                        GMnum++;
                                    }
                                }
                            }
                        }
                        //準備室的人
                        foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080011).Actors.Values)
                        {
                            if (j.type == ActorType.PC)
                            {
                                ActorPC player = (ActorPC)j;
                                actors.Add((ActorPC)j);
                            }
                        }
                        if (PCnum > 0)
                        {
                            if (GMnum == 0)
                            {
                                MapClientManager.Instance.Announce("大逃殺活動結束，魔王們再起不能，恭喜玩家獲得勝利！");
                            }
                            else
                            {
                                MapClientManager.Instance.Announce("大逃殺活動結束，共有" + PCnum + "位玩家逃離魔王的追殺，恭喜玩家獲得勝利！");
                            }
                            foreach (ActorPC pc in actors)
                            {
                                if (pc == null)
                                    continue;
                                SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                                //給道具
                                SagaDB.Item.Item item = ItemFactory.Instance.GetItem(93000046);
                                item.Stack = 10;
                                item.Identified = true;
                                eh.Client.AddItem(item, true);
                                //
                                pc.Mode = PlayerMode.NORMAL;
                                SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                                Map newMap = MapManager.Instance.GetMap(20080011);
                                eh.Client.Map.SendActorToMap(pc, 20080011, Global.PosX8to16(20, newMap.Width), Global.PosY8to16(20, newMap.Height));
                                if (part == 4)
                                {
                                    eh.Client.SendAnnounce("今日大逃殺活動已結束，感謝各位的參與");
                                }
                                else
                                {
                                    eh.Client.SendAnnounce("下一場大逃殺活動即將在10分鐘後開始，請勿離開準備室");
                                }
                            }
                        }
                        else
                        {
                            if (GMnum == 1)
                            {
                                MapClientManager.Instance.Announce("大逃殺活動結束！僅剩的魔王抹了一把冷汗，玩家請再接再厲！");
                            }
                            else
                            {
                                MapClientManager.Instance.Announce("大逃殺活動結束！魔王們發出了險惡的笑聲，玩家請再接再厲！");
                            }
                            foreach (ActorPC pc in actors)
                            {
                                if (pc == null)
                                    continue;
                                SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;

                                //給道具
                                SagaDB.Item.Item item = ItemFactory.Instance.GetItem(93000046);
                                item.Stack = 1;
                                item.Identified = true;
                                eh.Client.AddItem(item, true);
                                //
                                pc.Mode = PlayerMode.NORMAL;
                                SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                                Map newMap = MapManager.Instance.GetMap(20080011);
                                eh.Client.Map.SendActorToMap(pc, 20080011, Global.PosX8to16(20, newMap.Width), Global.PosY8to16(20, newMap.Height));
                                if (part == 4)
                                {
                                    eh.Client.SendAnnounce("今日大逃殺活動已結束，感謝各位的參與");
                                }
                                else
                                {
                                    eh.Client.SendAnnounce("下一場大逃殺活動即將在10分鐘後開始，請勿離開準備室");
                                }
                            }
                        }
                        if (part == 4)
                        {
                            part = 1;
                        }
                        else
                        {
                            part++;
                        }
                        i.Started = false;
                    }
                    #endregion
                }
            }
        }

        #endregion
        //騎士團活動
        public void readyKnightWar()
        {
            DateTime now = new DateTime(1970, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            foreach (SagaDB.KnightWar.KnightWar i in KnightWarFactory.Instance.Items.Values)
            {
                DateTime time = i.StartTime;
                TimeSpan ts = time.Subtract(now);
                int nowmin = (int)ts.TotalMinutes;
                if (!i.Started && nowmin <= 30)
                {
                    KnightWarManager.Instance.DetailUI((int)ts.TotalSeconds);
                }
                if (nowmin == 30)
                {
                    MapClientManager.Instance.Announce("騎士團演習即將在30分鐘後開始，請有興趣的玩家，前往競技場報名參加。");
                }
                if (nowmin == 15)
                {
                    MapClientManager.Instance.Announce("騎士團演習即將在15分鐘後開始，請有興趣的玩家，前往競技場報名參加。");
                }
                if (nowmin == 5)
                {
                    MapClientManager.Instance.Announce("騎士團演習即將在5分鐘後開始，請有興趣的玩家，前往競技場報名參加。");
                }
                if (nowmin == 1)
                {
                    MapClientManager.Instance.Announce("騎士團演習即將在1分鐘後開始，玩家請勿離開大廳");
                }
                if (now.Hour == i.StartTime.Hour && now.Minute == i.StartTime.Minute)
                {
                    if (check())
                    {
                        //開始 位置傳送+mode變更
                        MapClientManager.Instance.Announce("騎士團演習開始！");
                        KnightWarManager.Instance.StartKnightWar(i);
                        i.Started = true;
                        return;
                    }
                    else
                    {
                        MapClientManager.Instance.Announce("演習參與人數少於10人，因此本場騎士團演習將取消，對前來參與的玩家深感抱歉。");
                        if (Tasks.System.KnightWar.Instance.Activated == true)
                        {
                            Tasks.System.KnightWar.Instance.Deactivate();
                        }
                        return;
                    }

                }
                if (i.Started == true)
                {
                    if (now.Minute == (i.StartTime.AddMinutes(i.Duration).Minute - 5))
                    {
                        MapClientManager.Instance.Announce("演習即將在5分鐘後結束。");
                    }
                    if (now.Minute == (i.StartTime.AddMinutes(i.Duration).Minute - 1))
                    {
                        MapClientManager.Instance.Announce("演習即將在1分鐘後結束。");
                    }
                    #region "結束 傳送回準備室+送禮"
                    if (now.Minute == (i.StartTime.AddMinutes(i.Duration).Minute))
                    {
                        KnightWarManager.Instance.EndKnightWar(i);
                        i.Started = false;
                        if (Tasks.System.KnightWar.Instance.Activated == true)
                        {
                            Tasks.System.KnightWar.Instance.Deactivate();
                        }
                    }
                    #endregion
                }
            }
        }

        bool check()
        {
            int num = 0;
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080007).Actors.Values)
            {
                if (j.type == ActorType.PC)
                    num++;
            }
            //西軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080008).Actors.Values)
            {
                if (j.type == ActorType.PC)
                    num++;
            }
            //南軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080009).Actors.Values)
            {
                if (j.type == ActorType.PC)
                    num++;
            }
            //北軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080010).Actors.Values)
            {
                if (j.type == ActorType.PC)
                    num++;
            }
            //如果需要GM群私下測試，可將此處註解掉
            if (num > 10)
            {
                return true;
            }
            else
            {
                return false;
            }
            return true;
        }

        public override void CallBack()
        {
            try
            {
                SagaDB.KnightWar.KnightWar nowwar = KnightWarFactory.Instance.GetCurrentMovie();
                if (nowwar == null)
                {
                    nowwar = KnightWarFactory.Instance.GetNextKnightWar();
                }
                if (nowwar.VAR == "KnightWar")
                {
                    readyKnightWar();
                }
                //readyKnightWar();
                //大逃殺專用
                if (nowwar.VAR == "BattleRoyale")
                {
                    if (part == 1)
                    {
                        readyKnightEvent();
                    }
                    if (part != 1)
                    {
                        partKnightEvent();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
        }
    }
}
