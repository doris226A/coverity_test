using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SagaLib;
using SagaDB.Actor;
using SagaDB.KnightWar;

using SagaMap.Manager;
using SagaMap.Network.Client;
using SagaMap.Scripting;


namespace SagaMap.Manager
{
    public class KnightWarManager : Singleton<KnightWarManager>
    {
        //轉生前levelEXP
        Dictionary<int, uint> LevelExp = new Dictionary<int, uint>(){
            { 85, 160000 },
            { 96, 1500000 },
            { 97, 3250000 },
            { 98, 3500000 },
            { 99, 3500000 },
            { 100, 4000000 },
            { 101, 4250000 },
            { 102, 4500000 },
            { 103, 4250000 },
            { 104, 5000000 },
            { 105, 5500000 },
            { 106, 6000000 },
            { 107, 6500000 },
            { 108, 6500000 },
            { 109, 6500000 },
            { 110, 6500000 },
        };
        //轉生後levelEXP
        Dictionary<int, uint> ReLevelExp = new Dictionary<int, uint>(){
            { 57, 390000 },
            { 58, 435000 },
            { 59, 435000 },
            { 60, 435000 },
            { 78, 900000 },
            { 79, 1150000 },
            { 80, 1200000 },
            { 81, 1250000 },
            { 82, 1300000 },
            { 83, 1350000 },
            { 91, 1400000 },
            { 92, 2450000 },
            { 93, 2450000 },
            { 94, 2450000 },
            { 95, 2450000 },
            { 96, 2450000 },
            { 97, 2450000 },
            { 98, 3500000 },
            { 99, 3750000 },
            { 100, 4000000 },
            { 101, 4250000 },
            { 102, 4250000 },
            { 103, 4250000 },
            { 104, 4250000 },
            { 105, 4250000 },
            { 106, 4250000 },
            { 107, 4250000 },
            { 108, 7000000 },
            { 109, 7500000 },
            { 110, 8000000 },
        };
        //轉生前JobEXP
        Dictionary<int, uint> JobExp = new Dictionary<int, uint>(){
            { 42, 490000 },
            { 43, 612000 },
            { 44, 612000 },
            { 45, 612000 },
            { 46, 770000 },
            { 47, 770000 },
            { 48, 770000 },
            { 49, 1010000 },
            { 50, 1010000 },
        };
        //轉生後JobEXP
        Dictionary<int, uint> ReJobExp = new Dictionary<int, uint>(){
            { 29, 175000 },
            { 36, 200000 },
            { 37, 625000 },
            { 38, 750000 },
            { 39, 875000 },
            { 40, 1750000 },
            { 41, 1750000 },
            { 42, 1750000 },
            { 43, 1750000 },
            { 44, 1750000 },
            { 45, 3000000 },
            { 46, 3000000 },
            { 47, 3000000 },
            { 48, 3000000 },
            { 49, 3500000 },
            { 50, 3500000 },
        };
        public KnightWarManager()
        {
        }

        /// <summary>
        /// 騎士團開始並傳送
        /// </summary>
        public void StartKnightWarPC(ActorPC pc, SagaDB.KnightWar.KnightWar war, PlayerMode mode)
        {
            pc.CInt["KNIGHTWAR_DeathCount"] = 0;
            pc.CInt["KNIGHTWAR_Heal"] = 0;
            pc.CInt["KNIGHTWAR_Rock"] = 0;
            pc.CInt["KNIGHTWAR_Kill"] = 0;
            pc.CInt["KNIGHTWAR_Score"] = 0;
            SagaDB.Item.Item item = pc.Inventory.GetItem(10011400, SagaDB.Item.Inventory.SearchType.ITEM_ID);
            if (item != null)
                MapClient.FromActorPC((ActorPC)pc).DeleteItem(item.Slot, item.Stack, true);
            //pc.CInt["KNIGHT_MAP"] = (int)pc.MapID;
            //pc.Mode = mode;
            //SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);

            if (!pc.Tasks.ContainsKey("KnightWarDelay"))
            {
                Tasks.PC.KnightWarDelay task = new SagaMap.Tasks.PC.KnightWarDelay(pc, mode);
                pc.Tasks.Add("KnightWarDelay", task);
                task.Activate();
            }

            SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
            Map newMap = MapManager.Instance.GetMap(war.MapID);

            //傳送玩家
            byte num = (byte)Global.Random.Next(-1, 1);
            eh.Client.Map.SendActorToMap(pc, war.MapID, Global.PosX8to16((byte)(war.Loc[mode.ToString()].x + num), newMap.Width), Global.PosY8to16((byte)(war.Loc[mode.ToString()].y + num), newMap.Height));
            ShowKnightNPC(pc, war);
        }

        /// <summary>
        /// 騎士團顯示專用NPC
        /// </summary>
        public void ShowKnightNPC(ActorPC pc, SagaDB.KnightWar.KnightWar war)
        {
            if (war.NpcLoc.Count != 0)
            {
                foreach (SagaDB.KnightWar.KnightWar.NpcLocation i in war.NpcLoc.Values)
                {
                    Packets.Server.SSMG_NPC_SHOWPICT_LOCATION p = new Packets.Server.SSMG_NPC_SHOWPICT_LOCATION();
                    p.NPCID = i.npcid;
                    p.X = i.x;
                    p.Y = i.y;
                    p.Dir = i.dir;
                    MapClient.FromActorPC(pc).netIO.SendPacket(p);
                }
            }

        }
        /// <summary>
        /// 騎士團開始
        /// </summary>
        public void StartKnightWar(SagaDB.KnightWar.KnightWar war)
        {
            ScriptManager.Instance.VariableHolder.AInt["KNIGHT_EAST"] = 0;
            ScriptManager.Instance.VariableHolder.AInt["KNIGHT_WEST"] = 0;
            ScriptManager.Instance.VariableHolder.AInt["KNIGHT_SOUTH"] = 0;
            ScriptManager.Instance.VariableHolder.AInt["KNIGHT_NORTH"] = 0;
            Map newMap = MapManager.Instance.GetMap(war.MapID);
            /*switch (war.JoinNum)
            {
                case 2:
                    newMap.SpawnMob(40051000, Global.PosX8to16(war.Loc["KNIGHT_EAST"].x, newMap.Width), Global.PosY8to16(war.Loc["KNIGHT_EAST"].y, newMap.Height), 1, null);
                    newMap.SpawnMob(40052000, Global.PosX8to16(war.Loc["KNIGHT_WEST"].x, newMap.Width), Global.PosY8to16(war.Loc["KNIGHT_WEST"].y, newMap.Height), 1, null);
                    break;
                case 3:
                    //0409臨時更改，三軍去除東軍
                    //newMap.SpawnMob(40051000, Global.PosX8to16(war.Loc["KNIGHT_EAST"].x, newMap.Width), Global.PosY8to16(war.Loc["KNIGHT_EAST"].y, newMap.Height), 1, null);
                    newMap.SpawnMob(40052000, Global.PosX8to16(war.Loc["KNIGHT_WEST"].x, newMap.Width), Global.PosY8to16(war.Loc["KNIGHT_WEST"].y, newMap.Height), 1, null);
                    newMap.SpawnMob(40053000, Global.PosX8to16(war.Loc["KNIGHT_SOUTH"].x, newMap.Width), Global.PosY8to16(war.Loc["KNIGHT_SOUTH"].y, newMap.Height), 1, null);
                    newMap.SpawnMob(40054000, Global.PosX8to16(war.Loc["KNIGHT_NORTH"].x, newMap.Width), Global.PosY8to16(war.Loc["KNIGHT_NORTH"].y, newMap.Height), 1, null);
                    break;
                case 4:
                    newMap.SpawnMob(40051000, Global.PosX8to16(war.Loc["KNIGHT_EAST"].x, newMap.Width), Global.PosY8to16(war.Loc["KNIGHT_EAST"].y, newMap.Height), 1, null);
                    newMap.SpawnMob(40052000, Global.PosX8to16(war.Loc["KNIGHT_WEST"].x, newMap.Width), Global.PosY8to16(war.Loc["KNIGHT_WEST"].y, newMap.Height), 1, null);
                    newMap.SpawnMob(40053000, Global.PosX8to16(war.Loc["KNIGHT_SOUTH"].x, newMap.Width), Global.PosY8to16(war.Loc["KNIGHT_SOUTH"].y, newMap.Height), 1, null);
                    newMap.SpawnMob(40054000, Global.PosX8to16(war.Loc["KNIGHT_NORTH"].x, newMap.Width), Global.PosY8to16(war.Loc["KNIGHT_NORTH"].y, newMap.Height), 1, null);
                    break;
            }*/
            List<ActorPC> actors = new List<ActorPC>();
            //東軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080007).Actors.Values)
            {
                if (j.type == ActorType.PC)
                {
                    actors.Add((ActorPC)j);
                }
            }
            foreach (ActorPC j in actors)
            {
                KnightWarManager.Instance.StartKnightWarPC(j, war, PlayerMode.KNIGHT_EAST);
            }
            actors.Clear();
            //西軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080008).Actors.Values)
            {
                if (j.type == ActorType.PC)
                    actors.Add((ActorPC)j);
            }
            foreach (ActorPC j in actors)
            {
                KnightWarManager.Instance.StartKnightWarPC(j, war, PlayerMode.KNIGHT_WEST);
            }
            actors.Clear();
            //南軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080009).Actors.Values)
            {
                if (j.type == ActorType.PC)
                    actors.Add((ActorPC)j);
            }
            foreach (ActorPC j in actors)
            {
                KnightWarManager.Instance.StartKnightWarPC(j, war, PlayerMode.KNIGHT_SOUTH);
            }
            actors.Clear();
            //北軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080010).Actors.Values)
            {
                if (j.type == ActorType.PC)
                    actors.Add((ActorPC)j);
            }
            foreach (ActorPC j in actors)
            {
                KnightWarManager.Instance.StartKnightWarPC(j, war, PlayerMode.KNIGHT_NORTH);
            }
            actors.Clear();
        }
        /// <summary>
        /// 騎士團取得軍團分數
        /// </summary>
        public void GetPoint(ActorPC pc, int point, String mode)
        {
            ScriptManager.Instance.VariableHolder.AInt[mode] += point;
            //活動地圖的人
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).Actors.Values)
            {
                if (j.type == ActorType.PC)
                {
                    ActorPC player = (ActorPC)j;
                    Packets.Server.SSMG_KNIGHTWAR_STATUS p = new Packets.Server.SSMG_KNIGHTWAR_STATUS();
                    p.EastPoint = ScriptManager.Instance.VariableHolder.AInt["KNIGHT_EAST"];
                    p.WestPoint = ScriptManager.Instance.VariableHolder.AInt["KNIGHT_WEST"];
                    p.SouthPoint = ScriptManager.Instance.VariableHolder.AInt["KNIGHT_SOUTH"];
                    p.NorthPoint = ScriptManager.Instance.VariableHolder.AInt["KNIGHT_NORTH"];
                    MapClient.FromActorPC(player).netIO.SendPacket(p);
                }
            }
        }
        /// <summary>
        /// 騎士團取得自己分數
        /// </summary>
        public void GetPointSelf(ActorPC pc, int point, String mode)
        {
            pc.CInt[mode] += point;
            Packets.Server.SSMG_KNIGHTWAR_SCORE p = new Packets.Server.SSMG_KNIGHTWAR_SCORE();
            p.DeathCount = pc.CInt["KNIGHTWAR_DeathCount"];
            p.Score = pc.CInt["KNIGHTWAR_Score"];
            MapClient.FromActorPC(pc).netIO.SendPacket(p);
        }

        Dictionary<Country, int> win = new Dictionary<Country, int>();
        Dictionary<PlayerMode, int> win2 = new Dictionary<PlayerMode, int>();
        List<Country> winnum = new List<Country>();
        List<PlayerMode> winnum2 = new List<PlayerMode>();
        /// <summary>
        /// 騎士團結束結算
        /// </summary>
        public void EndKnightWar(SagaDB.KnightWar.KnightWar war)
        {
            win = new Dictionary<Country, int>() {
                { Country.East ,ScriptManager.Instance.VariableHolder.AInt["KNIGHT_EAST"] },
                { Country.West , ScriptManager.Instance.VariableHolder.AInt["KNIGHT_WEST"] },
                { Country.South , ScriptManager.Instance.VariableHolder.AInt["KNIGHT_SOUTH"] },
                { Country.North ,ScriptManager.Instance.VariableHolder.AInt["KNIGHT_NORTH"] } };
            win2 = new Dictionary<PlayerMode, int>() {
                { PlayerMode.KNIGHT_EAST ,ScriptManager.Instance.VariableHolder.AInt["KNIGHT_EAST"] },
                { PlayerMode.KNIGHT_WEST , ScriptManager.Instance.VariableHolder.AInt["KNIGHT_WEST"] },
                { PlayerMode.KNIGHT_SOUTH , ScriptManager.Instance.VariableHolder.AInt["KNIGHT_SOUTH"] },
                { PlayerMode.KNIGHT_NORTH ,ScriptManager.Instance.VariableHolder.AInt["KNIGHT_NORTH"] } };
            var winA = from objDic in win orderby objDic.Value descending select objDic;
            var winB = from objDic in win2 orderby objDic.Value descending select objDic;
            foreach (KeyValuePair<Country, int> kvp in winA)
            {
                winnum.Add(kvp.Key);
            }
            foreach (KeyValuePair<PlayerMode, int> kvp in winB)
            {
                winnum2.Add(kvp.Key);
            }
            switch (winnum[0])
            {
                case Country.East:
                    MapClientManager.Instance.Announce("騎士團演習結束！本次勝利軍團為「東軍」！");
                    break;
                case Country.West:
                    MapClientManager.Instance.Announce("騎士團演習結束！本次勝利軍團為「西軍」！");
                    break;
                case Country.South:
                    MapClientManager.Instance.Announce("騎士團演習結束！本次勝利軍團為「南軍」！");
                    break;
                case Country.North:
                    MapClientManager.Instance.Announce("騎士團演習結束！本次勝利軍團為「北軍」！");
                    break;
            }
            int DeathCount = 0;
            int Heal = 0;
            int Rock = 0;
            int Kill = 0;
            ActorPC DeathCount_PC = new ActorPC();
            ActorPC Heal_PC = new ActorPC();
            ActorPC Rock_PC = new ActorPC();
            ActorPC Kill_PC = new ActorPC();
            SagaMap.ActorEventHandlers.PCEventHandler eh = new ActorEventHandlers.PCEventHandler();


            List<ActorPC> actors = new List<ActorPC>();
            //東軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080007).Actors.Values)
            {
                if (j.type == ActorType.PC)
                {
                    actors.Add((ActorPC)j);
                    ActorPC a = (ActorPC)j;
                    a.Mode = PlayerMode.KNIGHT_EAST;
                }
            }
            //西軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080008).Actors.Values)
            {
                if (j.type == ActorType.PC)
                {
                    actors.Add((ActorPC)j);
                    ActorPC a = (ActorPC)j;
                    a.Mode = PlayerMode.KNIGHT_WEST;
                }
            }
            //南軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080009).Actors.Values)
            {
                if (j.type == ActorType.PC)
                {
                    actors.Add((ActorPC)j);
                    ActorPC a = (ActorPC)j;
                    a.Mode = PlayerMode.KNIGHT_SOUTH;
                }
            }
            //北軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080010).Actors.Values)
            {
                if (j.type == ActorType.PC)
                {
                    actors.Add((ActorPC)j);
                    ActorPC a = (ActorPC)j;
                    a.Mode = PlayerMode.KNIGHT_NORTH;
                }
            }
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(war.MapID).Actors.Values)
            {
                if (j.type == ActorType.PC)
                {
                    ActorPC pc = (ActorPC)j;
                    actors.Add((ActorPC)j);
                    if (pc.Tasks.ContainsKey("KnightWarHero"))
                    {
                        switch (pc.Mode)
                        {
                            case PlayerMode.KNIGHT_EAST_HERO:
                                pc.Mode = PlayerMode.KNIGHT_EAST;
                                break;
                            case PlayerMode.KNIGHT_WEST_HERO:
                                pc.Mode = PlayerMode.KNIGHT_WEST;
                                break;
                            case PlayerMode.KNIGHT_SOUTH_HERO:
                                pc.Mode = PlayerMode.KNIGHT_SOUTH;
                                break;
                            case PlayerMode.KNIGHT_NORTH_HERO:
                                pc.Mode = PlayerMode.KNIGHT_NORTH;
                                break;
                        }
                        SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                        pc.Tasks["KnightWarHero"].Deactivate();
                        pc.Tasks.Remove("KnightWarHero");
                    }
                    if (pc.Tasks.ContainsKey("KnightWarDelay"))
                    {
                        pc.Tasks["KnightWarDelay"].Deactivate();
                        if (pc.Tasks.ContainsKey("KnightWarDelay"))
                            pc.Tasks.Remove("KnightWarDelay");
                        switch (pc.CInt["KNIGHT_MAP"])
                        {
                            case 20080007:
                                pc.Mode = PlayerMode.KNIGHT_EAST;
                                break;
                            case 20080008:
                                pc.Mode = PlayerMode.KNIGHT_WEST;
                                break;
                            case 20080009:
                                pc.Mode = PlayerMode.KNIGHT_SOUTH;
                                break;
                            case 20080010:
                                pc.Mode = PlayerMode.KNIGHT_NORTH;
                                break;
                        }
                    }
                }
            }
            Dictionary<ActorPC, int> DeathRank_cache = new Dictionary<ActorPC, int>();
            Dictionary<ActorPC, int> HealRank_cache = new Dictionary<ActorPC, int>();
            Dictionary<ActorPC, int> RockRank_cache = new Dictionary<ActorPC, int>();
            Dictionary<ActorPC, int> KillRank_cache = new Dictionary<ActorPC, int>();
            //結束時解除PE
            foreach (ActorPC j in actors)
            {
                if (j.PossessionTarget != 0)
                {
                    Packets.Client.CSMG_POSSESSION_CANCEL p = new SagaMap.Packets.Client.CSMG_POSSESSION_CANCEL();
                    p.PossessionPosition = PossessionPosition.NONE;
                    MapClient.FromActorPC(j).OnPossessionCancel(p);
                }
            }


            foreach (ActorPC j in actors)
            {
                try
                {
                    ActorPC pc = (ActorPC)j;
                    eh = (SagaMap.ActorEventHandlers.PCEventHandler)pc.e;
                    SagaDB.Item.Item itemROCK = pc.Inventory.GetItem(10011400, SagaDB.Item.Inventory.SearchType.ITEM_ID);
                    if (itemROCK != null)
                        MapClient.FromActorPC((ActorPC)pc).DeleteItem(itemROCK.Slot, itemROCK.Stack, true);
                    if (pc.CInt["KNIGHTWAR_Score"] < 20)
                    {
                        pc.Mode = PlayerMode.NORMAL;
                        SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                        eh.Client.SendAnnounce("很抱歉，您的分數太低，無法取得本場獎勵。");
                        if (pc.MapID != (uint)pc.CInt["KNIGHT_MAP"])
                        {
                            Map newMap = MapManager.Instance.GetMap((uint)pc.CInt["KNIGHT_MAP"]);
                            eh.Client.Map.SendActorToMap(pc, (uint)pc.CInt["KNIGHT_MAP"], Global.PosX8to16(20, newMap.Width), Global.PosY8to16(20, newMap.Height));
                        }
                        pc.CInt["NextMoveEventID"] = 192000015;
                        continue;
                    }
                    int Rank = 0;
                    for (int i = 0; i < winnum2.Count; i++)
                    {
                        if (pc.Mode == winnum2[i])
                        {
                            Rank = i;
                            break;
                        }
                    }
                    //經驗計算
                    int pclv = pc.Level;
                    int pcjob = pc.CurrentJobLevel;
                    if (!pc.Rebirth)
                    {
                        if (pc.Level <= 85) pclv = 85;
                        if (pc.Level >= 86 && pc.Level <= 96) pclv = 96;
                        if (pc.Job != PC_JOB.NOVICE &&
                            pc.Job != PC_JOB.SWORDMAN &&
                            pc.Job != PC_JOB.FENCER &&
                            pc.Job != PC_JOB.SCOUT &&
                            pc.Job != PC_JOB.ARCHER &&
                            pc.Job != PC_JOB.WIZARD &&
                            pc.Job != PC_JOB.SHAMAN &&
                            pc.Job != PC_JOB.VATES &&
                            pc.Job != PC_JOB.WARLOCK &&
                            pc.Job != PC_JOB.TATARABE &&
                            pc.Job != PC_JOB.FARMASIST &&
                            pc.Job != PC_JOB.RANGER &&
                            pc.Job != PC_JOB.MERCHANT)
                        {
                            if (pc.CurrentJobLevel <= 42) pcjob = 42;
                            if (pc.CurrentJobLevel >= 50) pcjob = 50;
                            if (LevelExp.ContainsKey(pclv))
                                pc.TInt["pclvexp"] = (int)LevelExp[pclv];
                            if (JobExp.ContainsKey(pcjob))
                                pc.TInt["pcjobexp"] = (int)JobExp[pcjob];
                        }
                        else
                        {
                            if (LevelExp.ContainsKey(pclv))
                                pc.TInt["pclvexp"] = (int)LevelExp[pclv];
                            pc.TInt["pcjobexp"] = 450000;
                        }
                    }
                    else
                    {
                        if (pc.Level <= 57) pclv = 57;
                        if (pc.Level >= 61 && pc.Level <= 78) pclv = 78;
                        if (pc.Level >= 84 && pc.Level <= 91) pclv = 91;
                        if (pc.CurrentJobLevel <= 29) pcjob = 29;
                        if (pc.CurrentJobLevel >= 30 && pc.CurrentJobLevel <= 36) pcjob = 36;
                        pc.TInt["pclvexp"] = (int)ReLevelExp[pclv];
                        pc.TInt["pcjobexp"] = (int)ReJobExp[pcjob];
                    }
                    //暫時關閉獎勵
                    //給戰場幣
                    SagaDB.Item.Item itemcoin = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                    switch (Rank)
                    {
                        case 0:
                            itemcoin.Stack = 40;
                            eh.Client.AddItem(itemcoin, true);
                            pc.TInt["pccp"] = 100;
                            pc.TInt["pclvexpPenalty"] = pc.TInt["pclvexp"] * 2;
                            pc.TInt["pcjobexpPenalty"] = pc.TInt["pcjobexp"] * 2;
                            break;
                        case 1:
                            itemcoin.Stack = 20;
                            eh.Client.AddItem(itemcoin, true);
                            pc.TInt["pccp"] = 80;
                            pc.TInt["pclvexpPenalty"] = (int)(pc.TInt["pclvexp"] * 0.5f);
                            pc.TInt["pcjobexpPenalty"] = (int)(pc.TInt["pcjobexp"] * 0.5f);
                            break;
                        case 2:
                            itemcoin.Stack = 8;
                            eh.Client.AddItem(itemcoin, true);
                            pc.TInt["pccp"] = 50;
                            pc.TInt["pclvexpPenalty"] = (int)(pc.TInt["pclvexp"] * 0.25f);
                            pc.TInt["pcjobexpPenalty"] = (int)(pc.TInt["pcjobexp"] * 0.25f);
                            break;
                        case 3:
                            itemcoin.Stack = 5;
                            eh.Client.AddItem(itemcoin, true);
                            pc.TInt["pccp"] = 20;
                            pc.TInt["pclvexpPenalty"] = 0;
                            pc.TInt["pcjobexpPenalty"] = 0;
                            break;
                    }
                    if (pc.CInt["KNIGHTWAR_DeathCount"] > DeathCount)
                    {
                        DeathCount = pc.CInt["KNIGHTWAR_DeathCount"];
                        DeathCount_PC = pc;
                    }
                    if (pc.CInt["KNIGHTWAR_Heal"] > Heal)
                    {
                        Heal = pc.CInt["KNIGHTWAR_Heal"];
                        Heal_PC = pc;
                    }
                    if (pc.CInt["KNIGHTWAR_Rock"] > Rock)
                    {
                        Rock = pc.CInt["KNIGHTWAR_Rock"];
                        Rock_PC = pc;
                    }
                    if (pc.CInt["KNIGHTWAR_Kill"] > Kill)
                    {
                        Kill = pc.CInt["KNIGHTWAR_Kill"];
                        Kill_PC = pc;
                    }
                    if (pc.CInt["KNIGHTWAR_DeathCount"] > 0)
                        DeathRank_cache.Add(pc, pc.CInt["KNIGHTWAR_DeathCount"]);
                    if (pc.CInt["KNIGHTWAR_Heal"] > 0)
                        HealRank_cache.Add(pc, pc.CInt["KNIGHTWAR_Heal"]);
                    if (pc.CInt["KNIGHTWAR_Rock"] > 0)
                        RockRank_cache.Add(pc, pc.CInt["KNIGHTWAR_Rock"]);
                    if (pc.CInt["KNIGHTWAR_Kill"] > 0)
                        KillRank_cache.Add(pc, pc.CInt["KNIGHTWAR_Kill"]);

                    if (pc.MapID != (uint)pc.CInt["KNIGHT_MAP"])
                    {
                        pc.Mode = PlayerMode.NORMAL;
                        SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                        Map newMap = MapManager.Instance.GetMap((uint)pc.CInt["KNIGHT_MAP"]);
                        eh.Client.Map.SendActorToMap(pc, (uint)pc.CInt["KNIGHT_MAP"], Global.PosX8to16(20, newMap.Width), Global.PosY8to16(20, newMap.Height));
                    }
                    else
                    {
                        pc.Mode = PlayerMode.NORMAL;
                        SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                    }
                    //pc.CInt.Remove("KNIGHT_MAP");
                    pc.CInt["NextMoveEventID"] = 192000015;
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                    Logger.ShowError("出錯玩家：" + j.Name);
                }

            }



            var Deathlist = from objDic in DeathRank_cache orderby objDic.Value descending select objDic;
            var Heallist = from objDic in HealRank_cache orderby objDic.Value descending select objDic;
            var Rocklist = from objDic in RockRank_cache orderby objDic.Value descending select objDic;
            var Killlist = from objDic in KillRank_cache orderby objDic.Value descending select objDic;

            foreach (KeyValuePair<ActorPC, int> kvp in Deathlist)
            {
                if (DeathRank.Count >= 10)
                    break;
                DeathRank.Add(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<ActorPC, int> kvp in Heallist)
            {
                if (HealRank.Count >= 10)
                    break;
                HealRank.Add(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<ActorPC, int> kvp in Rocklist)
            {
                if (RockRank.Count >= 10)
                    break;
                RockRank.Add(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<ActorPC, int> kvp in Killlist)
            {
                if (KillRank.Count >= 10)
                    break;
                KillRank.Add(kvp.Key, kvp.Value);
            }
            if (DeathRank.Keys.ToArray()[0] != null)
            {
                try
                {
                    ActorPC[] DeathRank_array = DeathRank.Keys.ToArray();
                    MapClientManager.Instance.Announce("恭喜「" + DeathRank_array[0].Name + "」獲得了「死神附體獎」第一名！共戰死了" + DeathRank_array[0].CInt["KNIGHTWAR_DeathCount"] + "次。");
                    eh = (SagaMap.ActorEventHandlers.PCEventHandler)DeathRank_array[0].e;
                    SagaDB.Item.Item itemcoin1 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                    itemcoin1.Stack = 10;
                    eh.Client.AddItem(itemcoin1, true);
                    DeathRank_array[0].TInt["pclvexpBonus"] = (int)(DeathRank_array[0].TInt["pclvexp"] * 0.2f);
                    DeathRank_array[0].TInt["pcjobexpBonus"] = (int)(DeathRank_array[0].TInt["pcjobexp"] * 0.2f);

                    MapClientManager.Instance.Announce("恭喜「" + DeathRank_array[1].Name + "」獲得了「死神附體獎」第二名！共戰死了" + DeathRank_array[1].CInt["KNIGHTWAR_DeathCount"] + "次。");
                    eh = (SagaMap.ActorEventHandlers.PCEventHandler)DeathRank_array[1].e;
                    SagaDB.Item.Item itemcoin1_2 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                    itemcoin1_2.Stack = 10;
                    eh.Client.AddItem(itemcoin1_2, true);

                    MapClientManager.Instance.Announce("恭喜「" + DeathRank_array[2].Name + "」獲得了「死神附體獎」第三名！共戰死了" + DeathRank_array[2].CInt["KNIGHTWAR_DeathCount"] + "次。");
                    eh = (SagaMap.ActorEventHandlers.PCEventHandler)DeathRank_array[2].e;
                    SagaDB.Item.Item itemcoin1_3 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                    itemcoin1_3.Stack = 10;
                    eh.Client.AddItem(itemcoin1_3, true);
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
            }
            if (HealRank.Keys.ToArray()[0] != null)
            {
                try
                {

                    ActorPC[] HealRank_array = HealRank.Keys.ToArray();
                    MapClientManager.Instance.Announce("恭喜「" + HealRank_array[0].Name + "」獲得了「南丁格爾獎」第一名！共恢復了" + HealRank_array[0].CInt["KNIGHTWAR_Heal"] + "的血量。");
                    eh = (SagaMap.ActorEventHandlers.PCEventHandler)HealRank_array[0].e;
                    SagaDB.Item.Item itemcoin1 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                    itemcoin1.Stack = 10;
                    eh.Client.AddItem(itemcoin1, true);
                    HealRank_array[0].TInt["pclvexpBonus"] = (int)(HealRank_array[0].TInt["pclvexp"] * 0.2f);
                    HealRank_array[0].TInt["pcjobexpBonus"] = (int)(HealRank_array[0].TInt["pcjobexp"] * 0.2f);

                    MapClientManager.Instance.Announce("恭喜「" + HealRank_array[1].Name + "」獲得了「南丁格爾獎」第二名！共恢復了" + HealRank_array[1].CInt["KNIGHTWAR_Heal"] + "的血量。");
                    eh = (SagaMap.ActorEventHandlers.PCEventHandler)HealRank_array[1].e;
                    SagaDB.Item.Item itemcoin1_2 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                    itemcoin1_2.Stack = 10;
                    eh.Client.AddItem(itemcoin1_2, true);

                    MapClientManager.Instance.Announce("恭喜「" + HealRank_array[2].Name + "」獲得了「南丁格爾獎」第三名！共恢復了" + HealRank_array[2].CInt["KNIGHTWAR_Heal"] + "的血量。");
                    eh = (SagaMap.ActorEventHandlers.PCEventHandler)HealRank_array[2].e;
                    SagaDB.Item.Item itemcoin1_3 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                    itemcoin1_3.Stack = 10;
                    eh.Client.AddItem(itemcoin1_3, true);
                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
            }
            if (RockRank.Keys.ToArray()[0] != null)
            {
                try
                {
                    ActorPC[] RockRank_array = RockRank.Keys.ToArray();
                    MapClientManager.Instance.Announce("恭喜「" + RockRank_array[0].Name + "」獲得了「光輝收集獎」第一名！共獲得了" + RockRank_array[0].CInt["KNIGHTWAR_Heal"] + "的收集分數。");
                    eh = (SagaMap.ActorEventHandlers.PCEventHandler)RockRank_array[0].e;
                    SagaDB.Item.Item itemcoin1 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                    itemcoin1.Stack = 10;
                    eh.Client.AddItem(itemcoin1, true);
                    RockRank_array[0].TInt["pclvexpBonus"] = (int)(RockRank_array[0].TInt["pclvexp"] * 0.2f);
                    RockRank_array[0].TInt["pcjobexpBonus"] = (int)(RockRank_array[0].TInt["pcjobexp"] * 0.2f);

                    MapClientManager.Instance.Announce("恭喜「" + RockRank_array[1].Name + "」獲得了「光輝收集獎」第二名！共獲得了" + RockRank_array[1].CInt["KNIGHTWAR_Heal"] + "的收集分數。");
                    eh = (SagaMap.ActorEventHandlers.PCEventHandler)RockRank_array[1].e;
                    SagaDB.Item.Item itemcoin1_2 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                    itemcoin1_2.Stack = 10;
                    eh.Client.AddItem(itemcoin1_2, true);

                    MapClientManager.Instance.Announce("恭喜「" + RockRank_array[2].Name + "」獲得了「光輝收集獎」第三名！共獲得了" + RockRank_array[2].CInt["KNIGHTWAR_Heal"] + "的收集分數。");
                    eh = (SagaMap.ActorEventHandlers.PCEventHandler)RockRank_array[2].e;
                    SagaDB.Item.Item itemcoin1_3 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                    itemcoin1_3.Stack = 10;
                    eh.Client.AddItem(itemcoin1_3, true);

                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
            }
            if (KillRank.Keys.ToArray()[0] != null)
            {
                try
                {
                    ActorPC[] KillRank_array = KillRank.Keys.ToArray();
                    MapClientManager.Instance.Announce("恭喜「" + KillRank_array[0].Name + "」獲得了「殺手獎」第一名！共擊殺了" + KillRank_array[0].CInt["KNIGHTWAR_Heal"] + "次玩家。");
                    eh = (SagaMap.ActorEventHandlers.PCEventHandler)KillRank_array[0].e;
                    SagaDB.Item.Item itemcoin1 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                    itemcoin1.Stack = 10;
                    eh.Client.AddItem(itemcoin1, true);
                    KillRank_array[0].TInt["pclvexpBonus"] = (int)(KillRank_array[0].TInt["pclvexp"] * 0.2f);
                    KillRank_array[0].TInt["pcjobexpBonus"] = (int)(KillRank_array[0].TInt["pcjobexp"] * 0.2f);

                    MapClientManager.Instance.Announce("恭喜「" + KillRank_array[1].Name + "」獲得了「殺手獎」第二名！共擊殺了" + KillRank_array[1].CInt["KNIGHTWAR_Heal"] + "次玩家。");
                    eh = (SagaMap.ActorEventHandlers.PCEventHandler)KillRank_array[1].e;
                    SagaDB.Item.Item itemcoin1_2 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                    itemcoin1_2.Stack = 10;
                    eh.Client.AddItem(itemcoin1_2, true);

                    MapClientManager.Instance.Announce("恭喜「" + KillRank_array[2].Name + "」獲得了「殺手獎」第三名！共擊殺了" + KillRank_array[2].CInt["KNIGHTWAR_Heal"] + "次玩家。");
                    eh = (SagaMap.ActorEventHandlers.PCEventHandler)KillRank_array[2].e;
                    SagaDB.Item.Item itemcoin1_3 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                    itemcoin1_3.Stack = 10;
                    eh.Client.AddItem(itemcoin1_3, true);

                }
                catch (Exception ex)
                {
                    Logger.ShowError(ex);
                }
            }
            /*if (DeathCount_PC.Name != null)
            {
                MapClientManager.Instance.Announce("恭喜「" + DeathCount_PC.Name + "」獲得了「死神附體獎」！共戰死了" + DeathCount_PC.CInt["KNIGHTWAR_DeathCount"] + "次。");
                eh = (SagaMap.ActorEventHandlers.PCEventHandler)DeathCount_PC.e;
                SagaDB.Item.Item itemcoin1 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                itemcoin1.Stack = 10;
                eh.Client.AddItem(itemcoin1, true);
                DeathCount_PC.TInt["pclvexpBonus"] = (int)(DeathCount_PC.TInt["pclvexp"] * 0.2f);
                DeathCount_PC.TInt["pcjobexpBonus"] = (int)(DeathCount_PC.TInt["pcjobexp"] * 0.2f);
            }
            if (Heal_PC.Name != null)
            {
                MapClientManager.Instance.Announce("恭喜「" + Heal_PC.Name + "」獲得了「南丁格爾獎」！共恢復了" + Heal_PC.CInt["KNIGHTWAR_Heal"] + "的血量。");
                eh = (SagaMap.ActorEventHandlers.PCEventHandler)Heal_PC.e;
                SagaDB.Item.Item itemcoin2 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                itemcoin2.Stack = 10;
                eh.Client.AddItem(itemcoin2, true);
                Heal_PC.TInt["pclvexpBonus"] = (int)(Heal_PC.TInt["pclvexp"] * 0.2f);
                Heal_PC.TInt["pcjobexpBonus"] = (int)(Heal_PC.TInt["pcjobexp"] * 0.2f);
            }
            if (Rock_PC.Name != null)
            {
                MapClientManager.Instance.Announce("恭喜「" + Rock_PC.Name + "」獲得了「光輝收集獎」！共獲得了" + Rock_PC.CInt["KNIGHTWAR_Rock"] + "的收集分數。");
                eh = (SagaMap.ActorEventHandlers.PCEventHandler)Rock_PC.e;
                SagaDB.Item.Item itemcoin3 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                itemcoin3.Stack = 10;
                eh.Client.AddItem(itemcoin3, true);
                Rock_PC.TInt["pclvexpBonus"] = (int)(Rock_PC.TInt["pclvexp"] * 0.2f);
                Rock_PC.TInt["pcjobexpBonus"] = (int)(Rock_PC.TInt["pcjobexp"] * 0.2f);
            }
            if (Kill_PC.Name != null)
            {
                MapClientManager.Instance.Announce("恭喜「" + Kill_PC.Name + "」獲得了「殺手獎」！共擊殺了" + Kill_PC.CInt["KNIGHTWAR_Kill"] + "次玩家。");
                eh = (SagaMap.ActorEventHandlers.PCEventHandler)Kill_PC.e;
                SagaDB.Item.Item itemcoin4 = SagaDB.Item.ItemFactory.Instance.GetItem(93000046);
                itemcoin4.Stack = 10;
                eh.Client.AddItem(itemcoin4, true);
                Kill_PC.TInt["pclvexpBonus"] = (int)(Kill_PC.TInt["pclvexp"] * 0.2f);
                Kill_PC.TInt["pcjobexpBonus"] = (int)(Kill_PC.TInt["pcjobexp"] * 0.2f);
            }*/
        }


        /// <summary>
        /// 騎士團攻擊對手死亡結算
        /// </summary>
        public void KnightWarAttack(Actor sActor, Actor dActor)
        {
            if (KnightWarFactory.Instance.Items[1].MapID == sActor.MapID)
            {
                if (((ActorPC)sActor).Mode == PlayerMode.NORMAL)
                {
                    PlayerMode sActor_m = PlayerMode.NORMAL;
                    ActorMob mob = (ActorMob)dActor;
                    ActorPC pc = (ActorPC)sActor;
                    switch (pc.CInt["KNIGHT_MAP"])
                    {
                        case 20080009:
                            sActor_m = PlayerMode.KNIGHT_SOUTH;
                            break;
                        case 20080010:
                            sActor_m = PlayerMode.KNIGHT_NORTH;
                            break;
                        default:
                            sActor_m = PlayerMode.NORMAL;
                            break;
                    }
                    if (sActor_m != PlayerMode.NORMAL && mob.BaseData.mobType == SagaDB.Mob.MobType.KNIGHTS_WAR_INFO_MATERIAL)
                    {
                        int point = 0;
                        switch (mob.MobID)
                        {
                            case 40050001:
                                point = 1;
                                break;
                            case 40050003:
                                point = 3;
                                break;
                            case 40050005:
                                point = 5;
                                break;
                            case 40050050:
                                //紫水晶生成時有公告和箭頭
                                SagaMap.Tasks.System.KnightWar50point kt = new Tasks.System.KnightWar50point(mob.MapID);
                                kt.Activate();
                                point = 50;
                                break;
                        }
                        switch (sActor_m)
                        {
                            case PlayerMode.KNIGHT_EAST:
                                GetPoint(pc, point, "KNIGHT_EAST");
                                GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                break;
                            case PlayerMode.KNIGHT_WEST:
                                GetPoint(pc, point, "KNIGHT_WEST");
                                GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                break;
                            case PlayerMode.KNIGHT_SOUTH:
                                GetPoint(pc, point, "KNIGHT_SOUTH");
                                GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                break;
                            case PlayerMode.KNIGHT_NORTH:
                                GetPoint(pc, point, "KNIGHT_NORTH");
                                GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                break;
                            case PlayerMode.KNIGHT_EAST_HERO:
                                point *= 2;
                                GetPoint(pc, point, "KNIGHT_EAST");
                                GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                break;
                            case PlayerMode.KNIGHT_WEST_HERO:
                                point *= 2;
                                GetPoint(pc, point, "KNIGHT_WEST");
                                GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                break;
                            case PlayerMode.KNIGHT_SOUTH_HERO:
                                point *= 2;
                                GetPoint(pc, point, "KNIGHT_SOUTH");
                                GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                break;
                            case PlayerMode.KNIGHT_NORTH_HERO:
                                point *= 2;
                                GetPoint(pc, point, "KNIGHT_NORTH");
                                GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                break;
                        }
                        if (!pc.Tasks.ContainsKey("KnightWarHero") && mob.MobID == 40050050)
                        {
                            string mode = "";
                            switch (sActor_m)
                            {
                                case PlayerMode.KNIGHT_EAST:
                                    mode = "東軍";
                                    pc.Mode = PlayerMode.KNIGHT_EAST_HERO;
                                    break;
                                case PlayerMode.KNIGHT_WEST:
                                    mode = "西軍";
                                    pc.Mode = PlayerMode.KNIGHT_WEST_HERO;
                                    break;
                                case PlayerMode.KNIGHT_SOUTH:
                                    mode = "南軍";
                                    pc.Mode = PlayerMode.KNIGHT_SOUTH_HERO;
                                    break;
                                case PlayerMode.KNIGHT_NORTH:
                                    mode = "北軍";
                                    pc.Mode = PlayerMode.KNIGHT_NORTH_HERO;
                                    break;
                            }
                            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).Actors.Values)
                            {
                                if (j.type == ActorType.PC)
                                {
                                    SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)j.e;
                                    eh.Client.SendAnnounce(mode + "有玩家獲得了英雄狀態！");
                                    //刪除箭頭
                                    Packets.Server.SSMG_NPC_NAVIGATION_CANCEL p = new SagaMap.Packets.Server.SSMG_NPC_NAVIGATION_CANCEL();
                                    eh.Client.netIO.SendPacket(p);
                                }
                            }
                            SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                            Tasks.PC.KnightWarHero task = new SagaMap.Tasks.PC.KnightWarHero(pc);
                            pc.Tasks.Add("KnightWarHero", task);
                            task.Activate();
                        }
                    }
                    return;
                }
                else
                {
                    //打玩家
                    if (dActor.type == ActorType.PC)
                    {
                        GetPointSelf((ActorPC)sActor, 2, "KNIGHTWAR_Score");
                        GetPointSelf((ActorPC)dActor, 1, "KNIGHTWAR_DeathCount");
                        switch (((ActorPC)sActor).Mode)
                        {
                            case PlayerMode.KNIGHT_EAST:
                                KnightWarManager.Instance.GetPoint((ActorPC)sActor, 2, "KNIGHT_EAST");
                                break;
                            case PlayerMode.KNIGHT_WEST:
                                KnightWarManager.Instance.GetPoint((ActorPC)sActor, 2, "KNIGHT_WEST");
                                break;
                            case PlayerMode.KNIGHT_SOUTH:
                                KnightWarManager.Instance.GetPoint((ActorPC)sActor, 2, "KNIGHT_SOUTH");
                                break;
                            case PlayerMode.KNIGHT_NORTH:
                                KnightWarManager.Instance.GetPoint((ActorPC)sActor, 2, "KNIGHT_NORTH");
                                break;
                            case PlayerMode.KNIGHT_EAST_HERO:
                                KnightWarManager.Instance.GetPoint((ActorPC)sActor, 2, "KNIGHT_EAST");
                                break;
                            case PlayerMode.KNIGHT_WEST_HERO:
                                KnightWarManager.Instance.GetPoint((ActorPC)sActor, 2, "KNIGHT_WEST");
                                break;
                            case PlayerMode.KNIGHT_SOUTH_HERO:
                                KnightWarManager.Instance.GetPoint((ActorPC)sActor, 2, "KNIGHT_SOUTH");
                                break;
                            case PlayerMode.KNIGHT_NORTH_HERO:
                                KnightWarManager.Instance.GetPoint((ActorPC)sActor, 2, "KNIGHT_NORTH");
                                break;
                        }
                        ActorPC pc = (ActorPC)dActor;
                        ActorPC pc2 = (ActorPC)sActor;
                        KnightWarPrize(pc2, "KNIGHTWAR_Kill", 1);
                        /*if (pc.PossesionedActors.Count != 0)
                        {
                            foreach (ActorPC i in pc.PossesionedActors)
                            {
                                Packets.Client.CSMG_POSSESSION_CANCEL p = new SagaMap.Packets.Client.CSMG_POSSESSION_CANCEL();
                                p.PossessionPosition = PossessionPosition.NONE;
                                MapClient.FromActorPC(i).OnPossessionCancel(p);
                            }
                        }*/
                        //掉東西
                        Map map = MapManager.Instance.GetMap(pc.MapID);
                        /*SagaDB.Item.Item item = pc.Inventory.GetItem(10011400, SagaDB.Item.Inventory.SearchType.ITEM_ID);
                        if (item != null)
                        {
                            //map.AddItemDrop(item.ItemID, null, null, false, false, false);
                            ActorItem actor = new ActorItem(item);
                            actor.e = new ActorEventHandlers.ItemEventHandler(actor);
                            actor.Owner = pc2;
                            actor.Party = false;
                            actor.MapID = pc.MapID;
                            actor.X = pc.X;
                            actor.Y = pc.Y;
                            map.RegisterActor(actor);
                            actor.invisble = false;
                            map.OnActorVisibilityChange(actor);
                            Skill.SkillHandler.Instance.TakeItem(pc, 10011400, item.Stack);
                            Tasks.Item.DeleteItem task = new Tasks.Item.DeleteItem(actor);
                            task.Activate();
                            actor.Tasks.Add("DeleteItem", task);
                        }*/
                        //map.AddItemDrop(i.ItemID, null, null, false, false, false);
                        if (pc.Tasks.ContainsKey("KnightWarHero"))
                        {
                            switch (pc.Mode)
                            {
                                case PlayerMode.KNIGHT_EAST_HERO:
                                    pc.Mode = PlayerMode.KNIGHT_EAST;
                                    break;
                                case PlayerMode.KNIGHT_WEST_HERO:
                                    pc.Mode = PlayerMode.KNIGHT_WEST;
                                    break;
                                case PlayerMode.KNIGHT_SOUTH_HERO:
                                    pc.Mode = PlayerMode.KNIGHT_SOUTH;
                                    break;
                                case PlayerMode.KNIGHT_NORTH_HERO:
                                    pc.Mode = PlayerMode.KNIGHT_NORTH;
                                    break;
                            }
                            SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                            pc.Tasks["KnightWarHero"].Deactivate();
                            pc.Tasks.Remove("KnightWarHero");

                            switch (pc2.Mode)
                            {
                                case PlayerMode.KNIGHT_EAST:
                                    pc2.Mode = PlayerMode.KNIGHT_EAST_HERO;
                                    break;
                                case PlayerMode.KNIGHT_WEST:
                                    pc2.Mode = PlayerMode.KNIGHT_WEST_HERO;
                                    break;
                                case PlayerMode.KNIGHT_SOUTH:
                                    pc2.Mode = PlayerMode.KNIGHT_SOUTH_HERO;
                                    break;
                                case PlayerMode.KNIGHT_NORTH:
                                    pc2.Mode = PlayerMode.KNIGHT_NORTH_HERO;
                                    break;
                            }
                            SagaMap.Manager.MapManager.Instance.GetMap(pc2.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc2, true);
                            Tasks.PC.KnightWarHero task = new SagaMap.Tasks.PC.KnightWarHero(pc2);
                            pc2.Tasks.Add("KnightWarHero", task);
                            task.Activate();
                        }
                    }
                    //打怪
                    if (dActor.type == ActorType.MOB)
                    {
                        ActorMob mob = (ActorMob)dActor;
                        ActorPC pc = (ActorPC)sActor;
                        if (mob.BaseData.mobType == SagaDB.Mob.MobType.KNIGHTS_WAR_INFO_MATERIAL)
                        {
                            int point = 0;
                            switch (mob.MobID)
                            {
                                case 40050001:
                                    point = 1;
                                    break;
                                case 40050003:
                                    point = 3;
                                    break;
                                case 40050005:
                                    point = 5;
                                    break;
                                case 40050050:
                                    //紫水晶生成時有公告和箭頭
                                    SagaMap.Tasks.System.KnightWar50point kt = new Tasks.System.KnightWar50point(mob.MapID);
                                    kt.Activate();
                                    point = 50;
                                    break;
                            }
                            switch (pc.Mode)
                            {
                                case PlayerMode.KNIGHT_EAST:
                                    GetPoint(pc, point, "KNIGHT_EAST");
                                    GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                    break;
                                case PlayerMode.KNIGHT_WEST:
                                    GetPoint(pc, point, "KNIGHT_WEST");
                                    GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                    break;
                                case PlayerMode.KNIGHT_SOUTH:
                                    GetPoint(pc, point, "KNIGHT_SOUTH");
                                    GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                    break;
                                case PlayerMode.KNIGHT_NORTH:
                                    GetPoint(pc, point, "KNIGHT_NORTH");
                                    GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                    break;
                                case PlayerMode.KNIGHT_EAST_HERO:
                                    point *= 2;
                                    GetPoint(pc, point, "KNIGHT_EAST");
                                    GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                    break;
                                case PlayerMode.KNIGHT_WEST_HERO:
                                    point *= 2;
                                    GetPoint(pc, point, "KNIGHT_WEST");
                                    GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                    break;
                                case PlayerMode.KNIGHT_SOUTH_HERO:
                                    point *= 2;
                                    GetPoint(pc, point, "KNIGHT_SOUTH");
                                    GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                    break;
                                case PlayerMode.KNIGHT_NORTH_HERO:
                                    point *= 2;
                                    GetPoint(pc, point, "KNIGHT_NORTH");
                                    GetPointSelf((ActorPC)pc, point, "KNIGHTWAR_Score");
                                    break;
                            }
                            if (!pc.Tasks.ContainsKey("KnightWarHero") && mob.MobID == 40050050)
                            {
                                string mode = "";
                                switch (pc.Mode)
                                {
                                    case PlayerMode.KNIGHT_EAST:
                                        mode = "東軍";
                                        pc.Mode = PlayerMode.KNIGHT_EAST_HERO;
                                        break;
                                    case PlayerMode.KNIGHT_WEST:
                                        mode = "西軍";
                                        pc.Mode = PlayerMode.KNIGHT_WEST_HERO;
                                        break;
                                    case PlayerMode.KNIGHT_SOUTH:
                                        mode = "南軍";
                                        pc.Mode = PlayerMode.KNIGHT_SOUTH_HERO;
                                        break;
                                    case PlayerMode.KNIGHT_NORTH:
                                        mode = "北軍";
                                        pc.Mode = PlayerMode.KNIGHT_NORTH_HERO;
                                        break;
                                }
                                foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).Actors.Values)
                                {
                                    if (j.type == ActorType.PC)
                                    {
                                        SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)j.e;
                                        eh.Client.SendAnnounce(mode + "有玩家獲得了英雄狀態！");
                                        //刪除箭頭
                                        Packets.Server.SSMG_NPC_NAVIGATION_CANCEL p = new SagaMap.Packets.Server.SSMG_NPC_NAVIGATION_CANCEL();
                                        eh.Client.netIO.SendPacket(p);
                                    }
                                }
                                SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                                Tasks.PC.KnightWarHero task = new SagaMap.Tasks.PC.KnightWarHero(pc);
                                pc.Tasks.Add("KnightWarHero", task);
                                task.Activate();
                            }
                        }
                        int east = 0;
                        int west = 0;
                        int south = 0;
                        int nouth = 0;
                        int allnum = 0;
                        //東方象徵
                        if (mob.BaseData.id == 40051000)
                        {
                            int lostpoint = ScriptManager.Instance.VariableHolder.AInt["KNIGHT_EAST"] / 4;
                            Map map = SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID);
                            foreach (var item in ((SagaMap.ActorEventHandlers.MobEventHandler)mob.e).AI.DamageTable)
                            {
                                Actor dst = map.GetActor(item.Key);
                                if (dst != null)
                                {
                                    if (dst.type == ActorType.PC)
                                    {
                                        ActorPC pcDamage = (ActorPC)dst;

                                        switch (pcDamage.Mode)
                                        {
                                            case PlayerMode.KNIGHT_EAST:
                                                east += item.Value;
                                                break;
                                            case PlayerMode.KNIGHT_WEST:
                                                west += item.Value;
                                                break;
                                            case PlayerMode.KNIGHT_SOUTH:
                                                south += item.Value;
                                                break;
                                            case PlayerMode.KNIGHT_NORTH:
                                                nouth += item.Value;
                                                break;
                                        }
                                    }
                                }
                            }

                            allnum = east + west + south + nouth;
                            GetPoint(pc, -lostpoint, "KNIGHT_EAST");
                            if (west != 0) GetPoint(pc, (int)((float)west / (float)allnum * (float)lostpoint), "KNIGHT_WEST");
                            if (south != 0) GetPoint(pc, (int)((float)south / (float)allnum * (float)lostpoint), "KNIGHT_SOUTH");
                            if (nouth != 0) GetPoint(pc, (int)((float)nouth / (float)allnum * (float)lostpoint), "KNIGHT_NORTH");
                            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).Actors.Values)
                            {
                                if (j.type == ActorType.PC)
                                {
                                    SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)j.e;
                                    eh.Client.SendAnnounce("東方象徵已被擊破");
                                }
                            }
                        }
                        //西方象徵
                        if (mob.BaseData.id == 40052000)
                        {
                            int lostpoint = ScriptManager.Instance.VariableHolder.AInt["KNIGHT_WEST"] / 4;
                            Map map = SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID);
                            foreach (var item in ((SagaMap.ActorEventHandlers.MobEventHandler)mob.e).AI.DamageTable)
                            {
                                Actor dst = map.GetActor(item.Key);
                                if (dst != null)
                                {
                                    if (dst.type == ActorType.PC)
                                    {
                                        ActorPC pcDamage = (ActorPC)dst;
                                        switch (pcDamage.Mode)
                                        {
                                            case PlayerMode.KNIGHT_EAST:
                                                east++;
                                                break;
                                            case PlayerMode.KNIGHT_WEST:
                                                west++;
                                                break;
                                            case PlayerMode.KNIGHT_SOUTH:
                                                south++;
                                                break;
                                            case PlayerMode.KNIGHT_NORTH:
                                                nouth++;
                                                break;
                                        }
                                    }
                                }
                            }
                            allnum = east + west + south + nouth;
                            GetPoint(pc, -lostpoint, "KNIGHT_WEST");
                            if (east != 0) GetPoint(pc, (int)((float)east / (float)allnum * (float)lostpoint), "KNIGHT_EAST");
                            if (south != 0) GetPoint(pc, (int)((float)south / (float)allnum * (float)lostpoint), "KNIGHT_SOUTH");
                            if (nouth != 0) GetPoint(pc, (int)((float)nouth / (float)allnum * (float)lostpoint), "KNIGHT_NORTH");
                            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).Actors.Values)
                            {
                                if (j.type == ActorType.PC)
                                {
                                    SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)j.e;
                                    eh.Client.SendAnnounce("西方象徵已被擊破");
                                }
                            }
                        }
                        //南方象徵
                        if (mob.BaseData.id == 40053000)
                        {
                            int lostpoint = ScriptManager.Instance.VariableHolder.AInt["KNIGHT_SOUTH"] / 4;
                            Map map = SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID);
                            foreach (var item in ((SagaMap.ActorEventHandlers.MobEventHandler)mob.e).AI.DamageTable)
                            {
                                Actor dst = map.GetActor(item.Key);
                                if (dst != null)
                                {
                                    if (dst.type == ActorType.PC)
                                    {
                                        ActorPC pcDamage = (ActorPC)dst;
                                        switch (pcDamage.Mode)
                                        {
                                            case PlayerMode.KNIGHT_EAST:
                                                east++;
                                                break;
                                            case PlayerMode.KNIGHT_WEST:
                                                west++;
                                                break;
                                            case PlayerMode.KNIGHT_SOUTH:
                                                south++;
                                                break;
                                            case PlayerMode.KNIGHT_NORTH:
                                                nouth++;
                                                break;
                                        }
                                    }
                                }
                            }
                            allnum = east + west + south + nouth;
                            GetPoint(pc, -lostpoint, "KNIGHT_SOUTH");
                            if (east != 0) GetPoint(pc, (int)((float)east / (float)allnum * (float)lostpoint), "KNIGHT_EAST");
                            if (west != 0) GetPoint(pc, (int)((float)west / (float)allnum * (float)lostpoint), "KNIGHT_WEST");
                            if (nouth != 0) GetPoint(pc, (int)((float)nouth / (float)allnum * (float)lostpoint), "KNIGHT_NORTH");
                            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).Actors.Values)
                            {
                                if (j.type == ActorType.PC)
                                {
                                    SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)j.e;
                                    eh.Client.SendAnnounce("南方象徵已被擊破");
                                }
                            }
                        }
                        //北方象徵
                        if (mob.BaseData.id == 40054000)
                        {
                            int lostpoint = ScriptManager.Instance.VariableHolder.AInt["KNIGHT_NORTH"] / 4;
                            Map map = SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID);
                            foreach (var item in ((SagaMap.ActorEventHandlers.MobEventHandler)mob.e).AI.DamageTable)
                            {
                                Actor dst = map.GetActor(item.Key);
                                if (dst != null)
                                {
                                    if (dst.type == ActorType.PC)
                                    {
                                        ActorPC pcDamage = (ActorPC)dst;
                                        switch (pcDamage.Mode)
                                        {
                                            case PlayerMode.KNIGHT_EAST:
                                                east++;
                                                break;
                                            case PlayerMode.KNIGHT_WEST:
                                                west++;
                                                break;
                                            case PlayerMode.KNIGHT_SOUTH:
                                                south++;
                                                break;
                                            case PlayerMode.KNIGHT_NORTH:
                                                nouth++;
                                                break;
                                        }
                                    }
                                }
                            }
                            allnum = east + west + south + nouth;
                            GetPoint(pc, -lostpoint, "KNIGHT_NORTH");
                            if (east != 0) GetPoint(pc, (int)((float)east / (float)allnum * (float)lostpoint), "KNIGHT_EAST");
                            if (west != 0) GetPoint(pc, (int)((float)west / (float)allnum * (float)lostpoint), "KNIGHT_WEST");
                            if (south != 0) GetPoint(pc, (int)((float)south / (float)allnum * (float)lostpoint), "KNIGHT_SOUTH");
                            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).Actors.Values)
                            {
                                if (j.type == ActorType.PC)
                                {
                                    SagaMap.ActorEventHandlers.PCEventHandler eh = (SagaMap.ActorEventHandlers.PCEventHandler)j.e;
                                    eh.Client.SendAnnounce("北方象徵已被擊破");
                                }
                            }
                        }
                    }
                }
            }

        }


        /// <summary>
        /// 騎士團單人積分計算
        /// </summary>
        public void KnightWarPrize(ActorPC pc, string prize, int num)
        {
            /*1.死亡次數 KNIGHTWAR_DeathCount
            2.回復量  KNIGHTWAR_Heal
            4.最佳打石頭  KNIGHTWAR_Rock
            5.殺人前10  KNIGHTWAR_Kill*/
            if (pc.Mode == PlayerMode.KNIGHT_EAST ||
                pc.Mode == PlayerMode.KNIGHT_WEST ||
                pc.Mode == PlayerMode.KNIGHT_SOUTH ||
                pc.Mode == PlayerMode.KNIGHT_NORTH ||
                pc.Mode == PlayerMode.KNIGHT_EAST_HERO ||
                pc.Mode == PlayerMode.KNIGHT_WEST_HERO ||
                pc.Mode == PlayerMode.KNIGHT_SOUTH_HERO ||
                pc.Mode == PlayerMode.KNIGHT_NORTH_HERO)
            {
                pc.CInt[prize] += num;
            }
        }
        /// <summary>
        /// 騎士團死亡時解除憑依
        /// </summary>
        public void KnightWarDie(ActorPC pc)
        {
            if (pc.Mode == PlayerMode.KNIGHT_EAST ||
                pc.Mode == PlayerMode.KNIGHT_WEST ||
                pc.Mode == PlayerMode.KNIGHT_SOUTH ||
                pc.Mode == PlayerMode.KNIGHT_NORTH ||
                pc.Mode == PlayerMode.KNIGHT_EAST_HERO ||
                pc.Mode == PlayerMode.KNIGHT_WEST_HERO ||
                pc.Mode == PlayerMode.KNIGHT_SOUTH_HERO ||
                pc.Mode == PlayerMode.KNIGHT_NORTH_HERO)
            {
                if (pc.PossesionedActors.Count != 0)
                {
                    foreach (ActorPC i in pc.PossesionedActors)
                    {
                        Packets.Client.CSMG_POSSESSION_CANCEL p = new SagaMap.Packets.Client.CSMG_POSSESSION_CANCEL();
                        p.PossessionPosition = PossessionPosition.NONE;
                        MapClient.FromActorPC(i).OnPossessionCancel(p);
                    }
                }
            }
        }

        /// <summary>
        /// 騎士團顯示進場分數
        /// </summary>
        public void KnightWarPointUI(ActorPC pc)
        {
            KnightWar war = KnightWarFactory.Instance.GetCurrentMovie();
            if (war != null)
            {
                if (pc.MapID == 10023001 ||
                    pc.MapID == 10032001 ||
                    pc.MapID == 10034001 ||
                    pc.MapID == 10042001 ||
                    pc.MapID == 10056001 ||
                    pc.MapID == 10064001 ||
                    pc.MapID == 20020001 ||
                    pc.MapID == 20200000)//ECOKEY 騎士團新增山岳地圖
                {
                    DateTime now = new DateTime(1970, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    DateTime time = war.StartTime.AddMinutes(war.Duration);
                    TimeSpan ts = time.Subtract(now);
                    int nowmin = (int)ts.TotalMinutes + 1;
                    Packets.Server.SSMG_KNIGHTWAR_TOTAL_SCORE p = new Packets.Server.SSMG_KNIGHTWAR_TOTAL_SCORE();
                    p.Second = nowmin * 60;
                    p.EastPoint = ScriptManager.Instance.VariableHolder.AInt["KNIGHT_EAST"];
                    p.WestPoint = ScriptManager.Instance.VariableHolder.AInt["KNIGHT_WEST"];
                    p.SouthPoint = ScriptManager.Instance.VariableHolder.AInt["KNIGHT_SOUTH"];
                    p.NorthPoint = ScriptManager.Instance.VariableHolder.AInt["KNIGHT_NORTH"];
                    MapClient.FromActorPC(pc).netIO.SendPacket(p);
                    Packets.Server.SSMG_KNIGHTWAR_SCORE p1 = new Packets.Server.SSMG_KNIGHTWAR_SCORE();
                    p1.DeathCount = pc.CInt["KNIGHTWAR_DeathCount"];
                    p1.Score = pc.CInt["KNIGHTWAR_Score"];
                    MapClient.FromActorPC(pc).netIO.SendPacket(p1);

                    ShowKnightNPC(pc, war);
                }
            }
        }

        Dictionary<ActorPC, int> DeathRank = new Dictionary<ActorPC, int>();
        Dictionary<ActorPC, int> HealRank = new Dictionary<ActorPC, int>();
        Dictionary<ActorPC, int> RockRank = new Dictionary<ActorPC, int>();
        Dictionary<ActorPC, int> KillRank = new Dictionary<ActorPC, int>();
        /// <summary>
        /// 騎士團顯示結算畫面
        /// </summary>
        public void EndUI(ActorPC pc)
        {
            SagaMap.Packets.Server.SSMG_KNIGHTWAR_RESULT p1 = new SagaMap.Packets.Server.SSMG_KNIGHTWAR_RESULT();
            p1.Rank1Country = winnum[0];
            p1.Rank2Country = winnum[1];
            p1.Rank3ountry = winnum[2];
            p1.Rank4Country = winnum[3];
            p1.Rank1Point = win[winnum[0]];
            p1.Rank2Point = win[winnum[1]];
            p1.Rank3Point = win[winnum[2]];
            p1.Rank4Point = win[winnum[3]];
            p1.ExpBonus = pc.TInt["pclvexp"];
            p1.ExpPenalty = pc.TInt["pclvexpPenalty"];
            p1.ExpScoreBonus = pc.TInt["pclvexpBonus"];
            p1.JexpBonus = pc.TInt["pcjobexp"];
            p1.JexpPenalty = pc.TInt["pcjobexpPenalty"];
            p1.JexpScoreBonus = pc.TInt["pcjobexpBonus"];
            p1.CPBouns = pc.TInt["pccp"];
            MapClient.FromActorPC(pc).netIO.SendPacket(p1);

            //暫時關閉獎勵
            float percentage = 1.0f - (float)(pc.CInt["KNIGHTWAR_DeathCount"] / 100);
            uint lvexp = (uint)(pc.TInt["pclvexp"] + pc.TInt["pclvexpPenalty"] + pc.TInt["pclvexpBonus"]);
            uint jobexp = (uint)(pc.TInt["pcjobexp"] + pc.TInt["pcjobexpPenalty"] + pc.TInt["pcjobexpBonus"]);
            ExperienceManager.Instance.ApplyExp(pc, lvexp, jobexp, percentage);
            pc.CP += (uint)pc.TInt["pccp"];
            MapClient.FromActorPC((ActorPC)pc).SendPlayerInfo();

            while (DeathRank.Count < 10)
            {
                DeathRank.Add(new ActorPC(), 0);
            }
            while (HealRank.Count < 10)
            {
                HealRank.Add(new ActorPC(), 0);
            }
            while (RockRank.Count < 10)
            {
                RockRank.Add(new ActorPC(), 0);
            }
            while (KillRank.Count < 10)
            {
                KillRank.Add(new ActorPC(), 0);
            }

            //死亡次數
            SagaMap.Packets.Server.SSMG_KNIGHTWAR_RESULT_DETAIL p2 = new SagaMap.Packets.Server.SSMG_KNIGHTWAR_RESULT_DETAIL();
            p2.page = 0;
            p2.type = 0;
            p2.RankCount = 10;
            p2.Rank1Point = DeathRank.Values.ToArray()[0];
            p2.Rank2Point = DeathRank.Values.ToArray()[1];
            p2.Rank3Point = DeathRank.Values.ToArray()[2];
            p2.Rank4Point = DeathRank.Values.ToArray()[3];
            p2.Rank5Point = DeathRank.Values.ToArray()[4];
            p2.Rank6Point = DeathRank.Values.ToArray()[5];
            p2.Rank7Point = DeathRank.Values.ToArray()[6];
            p2.Rank8Point = DeathRank.Values.ToArray()[7];
            p2.Rank9Point = DeathRank.Values.ToArray()[8];
            p2.Rank10Point = DeathRank.Values.ToArray()[9];

            p2.num = 10;
            p2.player1 = DeathRank.Keys.ToArray()[0];
            p2.player2 = DeathRank.Keys.ToArray()[1];
            p2.player3 = DeathRank.Keys.ToArray()[2];
            p2.player4 = DeathRank.Keys.ToArray()[3];
            p2.player5 = DeathRank.Keys.ToArray()[4];
            p2.player6 = DeathRank.Keys.ToArray()[5];
            p2.player7 = DeathRank.Keys.ToArray()[6];
            p2.player8 = DeathRank.Keys.ToArray()[7];
            p2.player9 = DeathRank.Keys.ToArray()[8];
            p2.player10 = DeathRank.Keys.ToArray()[9];
            MapClient.FromActorPC(pc).netIO.SendPacket(p2);

            //治療量
            SagaMap.Packets.Server.SSMG_KNIGHTWAR_RESULT_DETAIL p3 = new SagaMap.Packets.Server.SSMG_KNIGHTWAR_RESULT_DETAIL();
            p3.page = 1;
            p3.type = 1;
            p3.RankCount = 10;
            p3.Rank1Point = HealRank.Values.ToArray()[0];
            p3.Rank2Point = HealRank.Values.ToArray()[1];
            p3.Rank3Point = HealRank.Values.ToArray()[2];
            p3.Rank4Point = HealRank.Values.ToArray()[3];
            p3.Rank5Point = HealRank.Values.ToArray()[4];
            p3.Rank6Point = HealRank.Values.ToArray()[5];
            p3.Rank7Point = HealRank.Values.ToArray()[6];
            p3.Rank8Point = HealRank.Values.ToArray()[7];
            p3.Rank9Point = HealRank.Values.ToArray()[8];
            p3.Rank10Point = HealRank.Values.ToArray()[9];

            p3.num = 10;
            p3.player1 = HealRank.Keys.ToArray()[0];
            p3.player2 = HealRank.Keys.ToArray()[1];
            p3.player3 = HealRank.Keys.ToArray()[2];
            p3.player4 = HealRank.Keys.ToArray()[3];
            p3.player5 = HealRank.Keys.ToArray()[4];
            p3.player6 = HealRank.Keys.ToArray()[5];
            p3.player7 = HealRank.Keys.ToArray()[6];
            p3.player8 = HealRank.Keys.ToArray()[7];
            p3.player9 = HealRank.Keys.ToArray()[8];
            p3.player10 = HealRank.Keys.ToArray()[9];
            MapClient.FromActorPC(pc).netIO.SendPacket(p3);

            //石頭
            SagaMap.Packets.Server.SSMG_KNIGHTWAR_RESULT_DETAIL p4 = new SagaMap.Packets.Server.SSMG_KNIGHTWAR_RESULT_DETAIL();
            p4.page = 2;
            p4.type = 2;
            p4.RankCount = 10;
            p4.Rank1Point = RockRank.Values.ToArray()[0];
            p4.Rank2Point = RockRank.Values.ToArray()[1];
            p4.Rank3Point = RockRank.Values.ToArray()[2];
            p4.Rank4Point = RockRank.Values.ToArray()[3];
            p4.Rank5Point = RockRank.Values.ToArray()[4];
            p4.Rank6Point = RockRank.Values.ToArray()[5];
            p4.Rank7Point = RockRank.Values.ToArray()[6];
            p4.Rank8Point = RockRank.Values.ToArray()[7];
            p4.Rank9Point = RockRank.Values.ToArray()[8];
            p4.Rank10Point = RockRank.Values.ToArray()[9];

            p4.num = 10;
            p4.player1 = RockRank.Keys.ToArray()[0];
            p4.player2 = RockRank.Keys.ToArray()[1];
            p4.player3 = RockRank.Keys.ToArray()[2];
            p4.player4 = RockRank.Keys.ToArray()[3];
            p4.player5 = RockRank.Keys.ToArray()[4];
            p4.player6 = RockRank.Keys.ToArray()[5];
            p4.player7 = RockRank.Keys.ToArray()[6];
            p4.player8 = RockRank.Keys.ToArray()[7];
            p4.player9 = RockRank.Keys.ToArray()[8];
            p4.player10 = RockRank.Keys.ToArray()[9];
            MapClient.FromActorPC(pc).netIO.SendPacket(p4);

            //殺人
            SagaMap.Packets.Server.SSMG_KNIGHTWAR_RESULT_DETAIL p5 = new SagaMap.Packets.Server.SSMG_KNIGHTWAR_RESULT_DETAIL();
            p5.page = 3;
            p5.type = 4;
            p5.RankCount = 10;
            p5.Rank1Point = KillRank.Values.ToArray()[0];
            p5.Rank2Point = KillRank.Values.ToArray()[1];
            p5.Rank3Point = KillRank.Values.ToArray()[2];
            p5.Rank4Point = KillRank.Values.ToArray()[3];
            p5.Rank5Point = KillRank.Values.ToArray()[4];
            p5.Rank6Point = KillRank.Values.ToArray()[5];
            p5.Rank7Point = KillRank.Values.ToArray()[6];
            p5.Rank8Point = KillRank.Values.ToArray()[7];
            p5.Rank9Point = KillRank.Values.ToArray()[8];
            p5.Rank10Point = KillRank.Values.ToArray()[9];

            p5.num = 10;
            p5.player1 = KillRank.Keys.ToArray()[0];
            p5.player2 = KillRank.Keys.ToArray()[1];
            p5.player3 = KillRank.Keys.ToArray()[2];
            p5.player4 = KillRank.Keys.ToArray()[3];
            p5.player5 = KillRank.Keys.ToArray()[4];
            p5.player6 = KillRank.Keys.ToArray()[5];
            p5.player7 = KillRank.Keys.ToArray()[6];
            p5.player8 = KillRank.Keys.ToArray()[7];
            p5.player9 = KillRank.Keys.ToArray()[8];
            p5.player10 = KillRank.Keys.ToArray()[9];
            MapClient.FromActorPC(pc).netIO.SendPacket(p5);
        }

        /// <summary>
        /// 騎士團顯示目前報名情況
        /// </summary>
        public void DetailUI(int time)
        {
            int East = 0;
            int West = 0;
            int South = 0;
            int Nouth = 0;
            List<ActorPC> actors = new List<ActorPC>();
            //東軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080007).Actors.Values)
            {
                if (j.type == ActorType.PC)
                {
                    actors.Add((ActorPC)j);
                    East++;
                }
            }
            //西軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080008).Actors.Values)
            {
                if (j.type == ActorType.PC)
                {
                    actors.Add((ActorPC)j);
                    West++;
                }

            }
            //南軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080009).Actors.Values)
            {
                if (j.type == ActorType.PC)
                {
                    actors.Add((ActorPC)j);
                    South++;
                }

            }
            //北軍
            foreach (Actor j in SagaMap.Manager.MapManager.Instance.GetMap(20080010).Actors.Values)
            {
                if (j.type == ActorType.PC)
                {
                    actors.Add((ActorPC)j);
                    Nouth++;
                }
            }
            foreach (ActorPC j in actors)
            {
                SagaMap.Packets.Server.SSMG_KNIGHTWAR_APPLICATION p1 = new SagaMap.Packets.Server.SSMG_KNIGHTWAR_APPLICATION();
                p1.Time = time;
                p1.EastCount = East;
                p1.WestCount = West;
                p1.SouthCount = South;
                p1.NorthCount = Nouth;
                p1.AA = 0;
                p1.BB = 0;
                p1.CC = 0;
                p1.DD = 0;
                p1.mapid = 31301001;
                p1.type = 2;
                MapClient.FromActorPC(j).netIO.SendPacket(p1);
            }
        }


    }
}