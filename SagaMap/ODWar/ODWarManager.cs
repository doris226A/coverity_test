using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;
using SagaDB.ODWar;
using SagaDB.Actor;

using SagaMap.Manager;
using SagaMap.Network.Client;
using SagaMap.Skill.Additions.Global;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using SagaDB;

namespace SagaMap.Scripting
{
    public enum SymbolReviveResult
    {
        Success,
        NotDown,
        StillTrash,
        Faild,
    }
}

namespace SagaMap.Manager
{
    public class ODWarManager : Singleton<ODWarManager>
    {
        public ODWarManager()
        {
        }

        public void StartODWar(uint mapID)
        {
            if (IsDefence(mapID))
            {
                spawnSymbol(mapID);
            }
            else
            {
                spawnSymbolTrash(mapID);
            }
        }

        public bool IsDefence(uint mapID)
        {//
            
            return (ScriptManager.Instance.VariableHolder.AInt["ODWar" + mapID.ToString() + "Captured"] == 0);
        }

        public void spawnSymbol_another(uint mapID, MapClient client)
        {
            SagaDB.ODWar.ODWar war = ODWarFactory.Instance.Items[mapID];
            Map map = MapManager.Instance.GetMap(mapID);
            foreach (SagaDB.ODWar.ODWar.Symbol i in war.Symbols.Values)
            {
                if (i.broken == false)
                {
                    short x = Global.PosX8to16(i.x, map.Width);
                    short y = Global.PosY8to16(i.y, map.Height);
                    List<Actor> actors = map.GetActorsArea(x, y, 500, true);
                    bool live = false;
                    foreach (Actor actor in actors)
                    {
                        if (actor.type == ActorType.MOB && ((ActorMob)actor).MobID == i.mobID)
                        {
                            live = true;
                            break;
                        }
                        if (actor.type == ActorType.MOB && ((ActorMob)actor).MobID == 14460000)
                        {
                            live = true;
                            break;
                        }
                    }
                    if (!live)
                    {
                        i.actorID = map.SpawnMob(i.mobID, x, y, 2000, null).ActorID;
                        client.SendSystemMessage("補了象徵 " + i.id);
                    }

                }

            }
        }

        void spawnSymbol(uint mapID)
        {
            SagaDB.ODWar.ODWar war = ODWarFactory.Instance.Items[mapID];
            Map map = MapManager.Instance.GetMap(mapID);
            foreach (SagaDB.ODWar.ODWar.Symbol i in war.Symbols.Values)
            {
                short x = Global.PosX8to16(i.x, map.Width);
                short y = Global.PosY8to16(i.y, map.Height);

                i.actorID = map.SpawnMob(i.mobID, x, y, 2000, null).ActorID;
                i.broken = false;
            }
        }

        void spawnSymbolTrash(uint mapID)
        {
            SagaDB.ODWar.ODWar war = ODWarFactory.Instance.Items[mapID];
            Map map = MapManager.Instance.GetMap(mapID);
            foreach (SagaDB.ODWar.ODWar.Symbol i in war.Symbols.Values)
            {
                short x = Global.PosX8to16(i.x, map.Width);
                short y = Global.PosY8to16(i.y, map.Height);
                i.actorID = map.SpawnMob(war.SymbolTrash, x, y, 2000, null).ActorID;
                i.broken = true;
            }
        }

        public void SymbolDown(uint mapID, ActorMob mob)
        {
            Logger.ShowInfo("標誌死掉  " + mapID);
            SagaDB.ODWar.ODWar war = ODWarFactory.Instance.Items[mapID];
            Map map = MapManager.Instance.GetMap(mapID);
            bool alldown = true;
            foreach (int i in war.Symbols.Keys)
            {
                SagaDB.ODWar.ODWar.Symbol sym = war.Symbols[i];
                Logger.ShowInfo("i  " + i);
                Logger.ShowInfo("sym.actorID  " + sym.actorID);
                Logger.ShowInfo("mob.ActorID  " + mob.ActorID);
                if (sym.actorID == mob.ActorID)
                {
                    Logger.ShowInfo("mob.MobID  " + mob.MobID);
                    Logger.ShowInfo("sym.mobID  " + sym.mobID);
                    if (mob.MobID == war.SymbolTrash)
                    {
                        sym.actorID = 0;
                    }
                    else
                    {
                        if (mob.MobID == sym.mobID)
                        {
                            sym.actorID = map.SpawnMob(war.SymbolTrash, mob.X, mob.Y, 10, null).ActorID;
                            sym.broken = true;
                            map.Announce(string.Format(LocalManager.Instance.Strings.ODWAR_SYMBOL_DOWN, i));
                        }
                    }
                }
                if (!sym.broken)
                    alldown = false;
            }
            if (IsDefence(mapID) && alldown)
            {
                EndODWar(mapID, false);
            }
        }

        public void UpdateScore(uint mapID, uint actorID, int delta)
        {
            if (ODWarFactory.Instance.Items.ContainsKey(mapID))
            {
                SagaDB.ODWar.ODWar war = ODWarFactory.Instance.Items[mapID];
                if (!war.Score.ContainsKey(actorID))
                    war.Score.Add(actorID, 0);
                war.Score[actorID] += delta;
                if (war.Score[actorID] < 0)
                    war.Score[actorID] = 0;


                Map map = MapManager.Instance.GetMap(mapID);
                Actor actor = map.GetActor(actorID);
                if (actor == null)
                    return;
                if (actor.type != ActorType.PC)
                    return;
                ActorPC pc = (ActorPC)actor;
                if (!pc.Online)
                    return;
                Packets.Server.SSMG_KNIGHTWAR_SCORE p1 = new Packets.Server.SSMG_KNIGHTWAR_SCORE();
                p1.Score = war.Score[actorID];
                MapClient.FromActorPC(pc).netIO.SendPacket(p1);
            }
        }

        public SagaMap.Scripting.SymbolReviveResult ReviveSymbol(uint mapID, int number)
        {
            SagaDB.ODWar.ODWar war = ODWarFactory.Instance.Items[mapID];
            Map map = MapManager.Instance.GetMap(mapID);
            if (war.Symbols.ContainsKey(number))
            {
                if (war.Symbols[number].broken)
                {
                    // 修改條件，允許在 StillTrash 狀態下修復
                    if (war.Symbols[number].actorID == 0 && war.Symbols[number].broken)//ecokey 修正
                    {
                        short x = Global.PosX8to16(war.Symbols[number].x, map.Width);
                        short y = Global.PosY8to16(war.Symbols[number].y, map.Height);
                        Actor actor = map.SpawnMob(war.Symbols[number].mobID, x, y, 10, null);
                        actor.HP = actor.MaxHP / 2;
                        map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, actor, false);
                        war.Symbols[number].actorID = actor.ActorID;
                        war.Symbols[number].broken = false;

                        /*  // 記錄當前的actorID以刪除殘骸
                          uint oldActorID = war.Symbols[number].actorID;

                          // 刪除舊的殘障物
                          if (war.Symbols[number].actorID != 0)
                          {
                              Actor oldActor = map.GetActor(war.Symbols[number].actorID);
                              if (oldActor != null && oldActor.ActorID == war.SymbolTrash)
                              {
                                  map.DeleteActor(oldActor);
                              }
                          }*/

                        map.Announce(string.Format(LocalManager.Instance.Strings.ODWAR_SYMBOL_ACTIVATE, number));
                        if (!IsDefence(mapID))
                        {
                            bool win = true;
                            foreach (SagaDB.ODWar.ODWar.Symbol i in war.Symbols.Values)
                            {
                                if (i.broken)
                                    win = false;
                            }
                            if (win)
                            {
                                EndODWar(mapID, true);
                            }
                        }
                        return SagaMap.Scripting.SymbolReviveResult.Success;
                    }
                    else
                        return SagaMap.Scripting.SymbolReviveResult.StillTrash;
                }
                else
                    return SagaMap.Scripting.SymbolReviveResult.NotDown;
            }
            else
                return SagaMap.Scripting.SymbolReviveResult.Faild;
        }

        public void SpawnBoss(uint mapID)
        {
            SagaDB.ODWar.ODWar war = ODWarFactory.Instance.Items[mapID];
            Map map = MapManager.Instance.GetMap(mapID);
            foreach (SagaDB.ODWar.ODWar.Symbol i in war.Symbols.Values)
            {
                short x = Global.PosX8to16(i.x, map.Width);
                short y = Global.PosY8to16(i.y, map.Height);
                uint mobID = war.Boss[Global.Random.Next(0, war.Boss.Count - 1)];
                short[] pos = map.GetRandomPosAroundPos(x, y, 1500);
                map.SpawnMob(mobID, pos[0], pos[1], 2000, null);
            }
            MapClientManager.Instance.Announce("小心！！敵軍BOSS攻進來了！");
        }

        public void SpawnMob(uint mapID, bool strong)
        {
            SagaDB.ODWar.ODWar war = ODWarFactory.Instance.Items[mapID];
            Map map = MapManager.Instance.GetMap(mapID);
            foreach (SagaDB.ODWar.ODWar.Symbol i in war.Symbols.Values)
            {
                short x = Global.PosX8to16(i.x, map.Width);
                short y = Global.PosY8to16(i.y, map.Height);
                if (strong)
                {
                    for (int j = 0; j < war.WaveStrong.DEMChamp; j++)
                    {
                        uint mobID = war.DEMChamp[Global.Random.Next(0, war.DEMChamp.Count - 1)];
                        short[] pos = map.GetRandomPosAroundPos(x, y, 1500);
                        map.SpawnMob(mobID, pos[0], pos[1], 2000, null);
                    }
                    for (int j = 0; j < war.WaveStrong.DEMNormal; j++)
                    {
                        uint mobID = war.DEMNormal[Global.Random.Next(0, war.DEMNormal.Count - 1)];
                        short[] pos = map.GetRandomPosAroundPos(x, y, 1500);
                        map.SpawnMob(mobID, pos[0], pos[1], 2000, null);
                    }
                }
                else
                {
                    for (int j = 0; j < war.WaveWeak.DEMChamp; j++)
                    {
                        uint mobID = war.DEMChamp[Global.Random.Next(0, war.DEMChamp.Count - 1)];
                        short[] pos = map.GetRandomPosAroundPos(x, y, 1500);
                        map.SpawnMob(mobID, pos[0], pos[1], 2000, null);
                    }
                    for (int j = 0; j < war.WaveWeak.DEMNormal; j++)
                    {
                        uint mobID = war.DEMNormal[Global.Random.Next(0, war.DEMNormal.Count - 1)];
                        short[] pos = map.GetRandomPosAroundPos(x, y, 1500);
                        map.SpawnMob(mobID, pos[0], pos[1], 2000, null);
                    }
                }
            }
            if (strong)
            {
                MapClientManager.Instance.Announce("注意！！強大的敵軍攻進來了！");
            }
            else
            {
                MapClientManager.Instance.Announce("敵軍進攻了！");
            }
        }

        /// <summary>
        /// 是否可以申请城市攻防战
        /// </summary>
        /// <param name="mapID"></param>
        /// <returns></returns>
        public bool CanApply(uint mapID)
        {
            if (!IsDefence(mapID))
                return false;
            ODWar war = ODWarFactory.Instance.Items[mapID];
            if (war.StartTime.ContainsKey((int)DateTime.Today.DayOfWeek))
            {
                if (DateTime.Now.Hour == war.StartTime[(int)DateTime.Today.DayOfWeek] - 1)
                {
                    if (DateTime.Now.Minute > 50)
                        return true;
                    else
                        return false;
                }
                else if (DateTime.Now.Hour == war.StartTime[(int)DateTime.Today.DayOfWeek])
                {
                    if (DateTime.Now.Minute < 15)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
                /*if (DateTime.Now.Hour < war.StartTime[(int)DateTime.Today.DayOfWeek])
                    return true;
                else
                {
                    if (DateTime.Now.Minute < 15)
                        return true;
                    else
                        return false;
                }*/
            }
            else
                return false;
        }

        /// <summary>
        /// 结束都市攻防战
        /// </summary>
        /// <param name="mapID"></param>
        /// <returns>是否胜利</returns>
        public void EndODWar(uint mapID, bool win)
        {
            SagaDB.ODWar.ODWar war = ODWarFactory.Instance.Items[mapID];
            Map map = MapManager.Instance.GetMap(mapID);

            if (IsDefence(mapID))
            {
                if (!win)
                {
                    ScriptManager.Instance.VariableHolder.AInt["ODWar" + mapID.ToString() + "Captured"] = 1;
                    MapClientManager.Instance.Announce(LocalManager.Instance.Strings.ODWAR_LOSE);
                    List<Actor> actors = map.Actors.Values.ToList();
                    foreach (Actor i in actors)
                    {
                        if (i.type == ActorType.MOB)
                        {
                            ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)i.e;
                            if (!eh.AI.Mode.Symbol && !eh.AI.Mode.SymbolTrash)
                                eh.OnDie();
                        }
                    }
                }
                else
                {
                    MapClientManager.Instance.Announce(LocalManager.Instance.Strings.ODWAR_WIN);
                    MapClientManager.Instance.Announce(LocalManager.Instance.Strings.ODWAR_WIN2);
                    MapClientManager.Instance.Announce(LocalManager.Instance.Strings.ODWAR_WIN3);
                    MapClientManager.Instance.Announce(LocalManager.Instance.Strings.ODWAR_WIN4);

                    List<Actor> actors = map.Actors.Values.ToList();
                    foreach (Actor i in actors)
                    {
                        if (i.type == ActorType.MOB)
                        {
                            ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)i.e;
                            if (!eh.AI.Mode.Symbol && !eh.AI.Mode.SymbolTrash)
                                eh.OnDie();
                        }
                    }
                }
                SendResult(mapID, win);
            }
            else
            {
                if (win)
                {
                    ScriptManager.Instance.VariableHolder.AInt["ODWar" + mapID.ToString() + "Captured"] = 0;
                    MapClientManager.Instance.Announce(LocalManager.Instance.Strings.ODWAR_CAPTURE);
                    List<Actor> actors = map.Actors.Values.ToList();
                    foreach (Actor i in actors)
                    {
                        if (i.type == ActorType.MOB)
                        {
                            ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)i.e;
                            if (!eh.AI.Mode.Symbol && !eh.AI.Mode.SymbolTrash)
                                eh.OnDie();
                        }
                    }
                }
            }
            /*List<Actor> actors2 = map.Actors.Values.ToList();
            foreach (Actor i in actors2)
            {
                if (i.type == ActorType.PC)
                {
                    if (((ActorPC)i).Online)
                    {
                        Packets.Server.SSMG_NPC_SET_EVENT_AREA p1 = new SagaMap.Packets.Server.SSMG_NPC_SET_EVENT_AREA();
                        p1.StartX = 220;
                        p1.EndX = 220;
                        p1.StartY = 244;
                        p1.EndY = 244;
                        p1.EventID = 10001658;
                        p1.EffectID = 9005;
                        MapClient.FromActorPC(((ActorPC)i)).netIO.SendPacket(p1);
                    }
                }
            }*/
            war.Score.Clear();
            war.Started = false;
        }

        void SendResult(uint mapID, bool win)
        {
            SagaDB.ODWar.ODWar war = ODWarFactory.Instance.Items[mapID];
            Map map = MapManager.Instance.GetMap(mapID);

            /*foreach (uint i in war.Score.Keys)
            {
                Actor actor = map.GetActor(i);
                if (actor == null)
                    continue;
                if (actor.type != ActorType.PC)
                    continue;

                uint score = (uint)war.Score[i];
                ActorPC pc = (ActorPC)actor;
                if (!pc.Online)
                    continue;
                if (score > 3000)
                    score = 3000;

                if (pc.WRPRanking <= 10)
                    score = (uint)(score * 1.5f);
                if (!win)
                    score = (uint)(score * 0.75f);
                if (win)
                {
                    if (score < 200)
                        score = 200;
                }
                uint exp = (uint)(score * 0.6f);

                pc.CP += score;
                //ExperienceManager.Instance.ApplyExp(pc, exp, exp, 1f);

                Packets.Server.SSMG_ODWAR_RESULT p = new SagaMap.Packets.Server.SSMG_ODWAR_RESULT();
                p.Win = win;
                p.EXP = exp;
                p.JEXP = exp;
                p.CP = score;
                MapClient.FromActorPC(pc).netIO.SendPacket(p);
            }*/
            if (!win)
            {
                foreach (uint i in war.Score.Keys)
                {
                    Actor actor = map.GetActor(i);
                    if (actor == null)
                        continue;
                    if (actor.type != ActorType.PC)
                        continue;
                    try
                    {
                        ActorPC pc = (ActorPC)actor;
                        if (!pc.Online)
                            continue;
                        Logger logger = new Logger("攻防戰貢獻紀錄.txt");
                        string log = "\r\n" + pc.Name + "（" + pc.CharID + "）的貢獻點：" + war.Score[i] + "  此玩家排名：" + pc.WRPRanking;
                        logger.WriteLog(log);
                        pc.CP += 10;
                        SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                        SagaDB.Item.Item itemcoin = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                        itemcoin.Stack = 2;
                        eh.Client.AddItem(itemcoin, true);
                        ExperienceManager.Instance.ApplyExp(pc, 100000, 100000, 1f);
                    }
                    catch (Exception ex)
                    {
                        Logger.ShowError(ex);
                        Logger.ShowError("出錯玩家：" + actor.Name);
                        Logger logger = new Logger("攻防戰貢獻紀錄.txt");
                        string log = "\r\n  跳過玩家：" + actor.Name;
                        logger.WriteLog(log);
                    }
                }
            }
            else
            {
                int symAlive = 0;
                foreach (int i in war.Symbols.Keys.ToArray())
                {
                    SagaDB.ODWar.ODWar.Symbol sym = war.Symbols[i];
                    if (!sym.broken)
                    {
                        symAlive++;
                    }
                }
                foreach (uint i in war.Score.Keys.ToArray())
                {
                    Actor actor = map.GetActor(i);
                    if (actor == null)
                        continue;
                    if (actor.type != ActorType.PC)
                        continue;
                    try
                    {
                        ActorPC pc = (ActorPC)actor;
                        if (!pc.Online)
                            continue;
                        Logger logger = new Logger("攻防戰貢獻紀錄.txt");
                        string log = "\r\n" + pc.Name + "（" + pc.CharID + "）的貢獻點：" + war.Score[i] + "  此玩家排名：" + pc.WRPRanking;
                        logger.WriteLog(log);
                        SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;

                        uint score = (uint)war.Score[i];
                        uint score_cp = score / 100 > 3000 ? 3000 : (uint)score / 100;
                        uint score_exp = score > 4000000 ? 4000000 : (uint)score;
                        
                        SagaDB.Item.Item itemcoin = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                        List<int> stacks = new List<int>() { 0, 3, 5, 8, 10, 15, 20, 25 };
                        itemcoin.Stack = (ushort)stacks[symAlive];
                        if (pc.WRPRanking <= 10)
                        {
                            itemcoin.Stack += 10;
                            score_cp = (uint)(score_cp * 2);
                            score_exp = (uint)(score_exp * 2);
                            pc.WRP = 0;
                        }

                        pc.CP += score_cp;
                        ExperienceManager.Instance.ApplyExp(pc, score_exp, score_exp, 1f);
                        eh.Client.AddItem(itemcoin, true);
                    }
                    catch (Exception ex)
                    {
                        Logger.ShowError(ex);
                        Logger.ShowError("出錯玩家：" + actor.Name);
                        Logger logger = new Logger("攻防戰貢獻紀錄.txt");
                        string log = "\r\n  跳過玩家：" + actor.Name;
                        logger.WriteLog(log);
                    }
                }
                WRPRankingManager.Instance.UpdateRanking();
            }

            foreach (Actor i in map.Actors.Values)
            {
                if (i == null)
                    continue;
                if (i.type != ActorType.PC)
                    continue;
                ActorPC pc = (ActorPC)i;
                if (!pc.Online)
                    continue;
                SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                Map newMap = MapManager.Instance.GetMap(32003001);
                eh.Client.Map.SendActorToMap(pc, 32003001, Global.PosX8to16(20, newMap.Width), Global.PosY8to16(50, newMap.Height));
            }

        }


        public void ShowWRPRanking(uint mapID)
        {
            Map map = MapManager.Instance.GetMap(mapID);
            List<Actor> actors2 = map.Actors.Values.ToList();
            foreach (Actor i in actors2)
            {
                if (i.type == ActorType.PC)
                {
                    if (((ActorPC)i).Online)
                    {
                        MapClient.FromActorPC((ActorPC)i).SendWRPRanking((ActorPC)i);
                    }
                }
            }
        }

    }
}
