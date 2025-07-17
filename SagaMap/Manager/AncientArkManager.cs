
using SagaLib;
using System.Linq;
using SagaDB.Actor;
using SagaMap.Network.Client;
using SagaMap.Scripting;
using System.Collections.Generic;
using SagaMap.AncientArks;
using SagaMap.ActorEventHandlers;
using SagaDB.Item;
using static SagaMap.Map;
using SagaDB.Treasure;
using SagaMap.Skill;
using static System.Net.Mime.MediaTypeNames;
using System.Threading.Tasks;


namespace SagaMap.Manager
{
    public class AncientArkManager : Singleton<AncientArkManager>
    {
        public AncientArkManager()
        {

        }

        public void 討罰任務判斷(Map map, ActorMob mob)
        {
            if (map.IsAncientArk)
            {
                if (map.AncientArk.AncientArkID == 100021 && map.AncientArk.Gimmick_ID != 90000028 && map.AncientArk.Gimmick_ID_layer != 0)
                {
                    時間變更(map, -30);
                }
                if (map.AncientArk.Gimmick_ID_layer != 0)
                {
                    SagaMap.AncientArks.AncientArkGimmick gimmick_layer = SagaMap.AncientArks.AncientArkGimmickFactory.Instance.Items[map.AncientArk.Gimmick_ID_layer];

                    if (gimmick_layer.type == AncientArks.AncientArkType.HUNT)
                    {
                        if (mob.MobID == gimmick_layer.Current_ID1 ||
                            mob.MobID == gimmick_layer.Current_ID2 ||
                            mob.MobID == gimmick_layer.Current_ID3 ||
                            (gimmick_layer.Current_ID1 == 0 && mob.MobID != 35060005 && mob.MobID != 35060006))
                        {
                            map.AncientArk.Gimmick_Count++;
                            map.Announce("已經打倒" + map.AncientArk.Gimmick_Count + "隻");
                            if (map.AncientArk.Gimmick_Count == gimmick_layer.CurrentCount)
                            {
                                //map.AncientArk.complete_layer = true;
                                //map.AncientArk.Gimmick_ID_layer = 0;
                                //map.Announce("完成任務");
                                if (gimmick_layer.Time_penalty != 0) 時間變更(map, gimmick_layer.Time_penalty);
                                完成任務_更新任務or下一層(map);
                                //資料更新(map);
                            }
                        }
                        else
                        {
                            if(gimmick_layer.Time_penalty == 30) 時間變更(map, -gimmick_layer.Time_penalty);
                        }
                    }
                    if (gimmick_layer.type == AncientArks.AncientArkType.HUNT_BOX)
                    {
                        if (mob.MobID == gimmick_layer.Current_ID1)
                        {
                            map.AncientArk.Gimmick_Count++;
                            map.Announce("已經破壞" + map.AncientArk.Gimmick_Count + "隻");
                            if (map.AncientArk.Gimmick_Count == gimmick_layer.CurrentCount)
                            {
                                //map.AncientArk.complete_layer = true;
                                //map.AncientArk.Gimmick_ID_layer = 0;
                                完成任務_更新任務or下一層(map);
                            }
                        }
                        if (mob.MobID == gimmick_layer.Current_ID2)
                        {
                            ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)mob.e;
                            懲罰(map, eh.AI.firstAttacker);
                        }
                    }

                    return;
                }
                if (map.AncientArk.Gimmick_ID == 0) return;
                SagaMap.AncientArks.AncientArkGimmick gimmick = SagaMap.AncientArks.AncientArkGimmickFactory.Instance.Items[map.AncientArk.Gimmick_ID];
                if (gimmick.type == AncientArks.AncientArkType.HUNT_NO)
                {
                    if (mob.MobID == gimmick.Current_ID1 ||
                        mob.MobID == gimmick.Current_ID2 ||
                        mob.MobID == gimmick.Current_ID3 ||
                        (gimmick.Current_ID1 == 0 && mob.MobID != 35060005 && mob.MobID != 35060006))
                    {
                        map.AncientArk.Gimmick_Count++;
                        if (map.AncientArk.Gimmick_Count == gimmick.CurrentCount)
                        {
                            //map.Announce("完成任務");
                            完成任務_更新任務or下一層(map);
                            //資料更新(map);
                        }
                    }
                    else
                    {
                        if (gimmick.Time_penalty != 0) 時間變更(map, -gimmick.Time_penalty);
                    }
                }
                if (gimmick.type == AncientArks.AncientArkType.HUNT)
                {
                    if (mob.MobID == gimmick.Current_ID1 ||
                        mob.MobID == gimmick.Current_ID2 ||
                        mob.MobID == gimmick.Current_ID3 ||
                        (gimmick.Current_ID1 == 0 && mob.MobID != 35060005 && mob.MobID != 35060006))
                    {
                        map.AncientArk.Gimmick_Count++;
                        map.Announce("已經打倒" + map.AncientArk.Gimmick_Count + "隻");
                        if (map.AncientArk.Gimmick_Count == gimmick.CurrentCount)
                        {
                            //map.Announce("完成任務");
                            完成任務_更新任務or下一層(map);
                            //資料更新(map);
                        }
                    }
                    else
                    {
                        if (gimmick.Time_penalty != 0) 時間變更(map, -gimmick.Time_penalty);
                    }
                }
                if (gimmick.type == AncientArks.AncientArkType.HUNT_BOX)
                {
                    if (mob.MobID == gimmick.Current_ID1)
                    {
                        map.AncientArk.Gimmick_Count++;
                        map.Announce("已經破壞" + map.AncientArk.Gimmick_Count + "個，寶箱守護者出現！！");
                        map.SpawnMob(14310005, mob.X, mob.Y, 1000, null);
                        map.SpawnMob(14310005, mob.X, mob.Y, 1000, null);
                        map.SpawnMob(14310005, mob.X, mob.Y, 1000, null);
                        map.SpawnMob(14310005, mob.X, mob.Y, 1000, null);
                        map.SpawnMob(14310005, mob.X, mob.Y, 1000, null);
                        map.SpawnMob(14310005, mob.X, mob.Y, 1000, null);
                        map.SpawnMob(14310005, mob.X, mob.Y, 1000, null);
                        map.SpawnMob(14310005, mob.X, mob.Y, 1000, null);
                        if (map.AncientArk.Gimmick_Count == gimmick.CurrentCount)
                        {
                            完成任務_更新任務or下一層(map);
                        }
                    }
                    /*if (mob.MobID == gimmick.Current_ID2)
                    {
                        ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)mob.e;
                        懲罰(map, eh.AI.firstAttacker);
                    }*/
                }
                /*if (gimmick.type == AncientArks.AncientArkType.CANDLE_5)
                {
                    bool dead = false;
                    foreach (Actor i in map.Actors.Values)
                    {
                        if (i.type == ActorType.MOB)
                        {
                            if (((ActorMob)i).MobID == 35060006 && !i.Buff.Dead)
                            {
                                dead = true;
                                break;
                            }
                            if (((ActorMob)i).MobID == 35060005 && !i.Buff.Dead)
                            {
                                dead = true;
                                break;
                            }
                        }
                    }
                    if (!dead)
                    {
                        完成任務_更新任務or下一層(map);
                    }
                }*/
            }
        }
        public void 蠟燭任務判斷(Map map, ActorMob mob,Actor lastAttacker)
        {
            if (map.IsAncientArk)
            {
                if (mob.MobID != 35060006 && mob.MobID != 35060005) return;
                if (map.AncientArk.Gimmick_ID_layer != 0)
                {
                    SagaMap.AncientArks.AncientArkGimmick gimmick_layer = SagaMap.AncientArks.AncientArkGimmickFactory.Instance.Items[map.AncientArk.Gimmick_ID_layer];
                    switch (gimmick_layer.type)
                    {
                        case AncientArkType.CANDLE_1:
                            int count = 0;
                            foreach (Actor i in map.Actors.Values)
                            {
                                if (i.type == ActorType.MOB)
                                {
                                    if (((ActorMob)i).MobID == 35060006 && !i.Buff.Dead) count++;
                                }
                            }
                            if (count >= 10)
                            {
                                完成任務_更新任務or下一層(map);
                            }
                            break;
                        case AncientArkType.CANDLE_2:
                            if (mob.MobID == 35060005)
                            {
                                if (mob.TInt["正確蠟燭"] == 1)
                                {
                                    完成任務_更新任務or下一層(map);
                                }
                                else
                                {
                                    懲罰(map, lastAttacker);
                                }
                            }
                            break;
                        case AncientArkType.CANDLE_3:
                            if (mob.MobID == 35060005 && lastAttacker != null && lastAttacker.type == ActorType.PC)
                            {
                                switch (mob.TInt["職業"])
                                {
                                    case 1:
                                        if (((ActorPC)lastAttacker).JobBasic == PC_JOB.SWORDMAN ||
                                            ((ActorPC)lastAttacker).JobBasic == PC_JOB.FENCER ||
                                            ((ActorPC)lastAttacker).JobBasic == PC_JOB.SCOUT ||
                                            ((ActorPC)lastAttacker).JobBasic == PC_JOB.ARCHER)
                                        {
                                            完成任務_更新任務or下一層(map);
                                        }
                                        return;
                                    case 2:
                                        if (((ActorPC)lastAttacker).JobBasic == PC_JOB.WIZARD ||
                                            ((ActorPC)lastAttacker).JobBasic == PC_JOB.SHAMAN ||
                                            ((ActorPC)lastAttacker).JobBasic == PC_JOB.VATES ||
                                            ((ActorPC)lastAttacker).JobBasic == PC_JOB.WARLOCK)
                                        {
                                            完成任務_更新任務or下一層(map);
                                        }
                                        return;
                                    case 3:
                                        if (((ActorPC)lastAttacker).JobBasic == PC_JOB.TATARABE ||
                                            ((ActorPC)lastAttacker).JobBasic == PC_JOB.FARMASIST ||
                                            ((ActorPC)lastAttacker).JobBasic == PC_JOB.RANGER ||
                                            ((ActorPC)lastAttacker).JobBasic == PC_JOB.MERCHANT)
                                        {
                                            完成任務_更新任務or下一層(map);
                                        }
                                        return;
                                }
                                懲罰(map, lastAttacker);
                            }
                            break;
                        case AncientArkType.CANDLE_4:
                            if (mob.MobID == 35060005 && lastAttacker != null && lastAttacker.type == ActorType.PC)
                            {
                                switch (mob.TInt["種族"])
                                {
                                    case 1:
                                        if (((ActorPC)lastAttacker).Race == PC_RACE.EMIL)
                                        {
                                            完成任務_更新任務or下一層(map);
                                        }
                                        return;
                                    case 2:
                                        if (((ActorPC)lastAttacker).Race == PC_RACE.TITANIA)
                                        {
                                            完成任務_更新任務or下一層(map);
                                        }
                                        return;
                                    case 3:
                                        if (((ActorPC)lastAttacker).Race == PC_RACE.DOMINION)
                                        {
                                            完成任務_更新任務or下一層(map);
                                        }
                                        return;
                                }
                                懲罰(map, lastAttacker);
                            }
                            break;
                        case AncientArkType.CANDLE_5:
                            bool dead = false;
                            foreach (Actor i in map.Actors.Values)
                            {
                                if (i.type == ActorType.MOB)
                                {
                                    if (((ActorMob)i).MobID == 35060006 && !i.Buff.Dead)
                                    {
                                        dead = true;
                                        break;
                                    }
                                    if (((ActorMob)i).MobID == 35060005 && !i.Buff.Dead)
                                    {
                                        dead = true;
                                        break;
                                    }
                                }
                            }
                            if (!dead)
                            {
                                完成任務_更新任務or下一層(map);
                            }
                            break;
                        case AncientArkType.CANDLE_6:
                            if (lastAttacker != null && lastAttacker.type == ActorType.PC && ((ActorPC)lastAttacker).Party != null)
                            {
                                int num = 0;
                                if (mob.MobID == 35060006)
                                {
                                    foreach (uint i in ((SagaMap.ActorEventHandlers.MobEventHandler)mob.e).AI.DamageTable.Keys)
                                    {
                                        foreach (ActorPC j in ((ActorPC)lastAttacker).Party.Members.Values)
                                        {
                                            if (j.ActorID == i)
                                            {
                                                num++;
                                            }
                                        }
                                    }
                                }
                                if (num >= ((ActorPC)lastAttacker).Party.MemberCount)
                                {
                                    完成任務_更新任務or下一層(map);
                                }
                            }
                            break;
                    }
                    return;
                }
                if (map.AncientArk.Gimmick_ID == 0) return;
                SagaMap.AncientArks.AncientArkGimmick gimmick = SagaMap.AncientArks.AncientArkGimmickFactory.Instance.Items[map.AncientArk.Gimmick_ID];
                switch (gimmick.type)
                {
                    case AncientArkType.CANDLE_1:
                        int count = 0;
                        foreach (Actor i in map.Actors.Values)
                        {
                            if (i.type == ActorType.MOB)
                            {
                                if (((ActorMob)i).MobID == 35060006 && !i.Buff.Dead) count++;
                            }
                        }
                        if (count >= 10)
                        {
                            完成任務_更新任務or下一層(map);
                        }
                        break;
                    case AncientArkType.CANDLE_2:
                        if (mob.MobID == 35060005)
                        {
                            if (mob.TInt["正確蠟燭"] == 1)
                            {
                                完成任務_更新任務or下一層(map);
                            }
                            else
                            {
                                懲罰(map, lastAttacker);
                            }
                        }
                        break;
                    case AncientArkType.CANDLE_3:
                        if (mob.MobID == 35060005 && lastAttacker != null && lastAttacker.type == ActorType.PC)
                        {
                            switch (mob.TInt["職業"])
                            {
                                case 1:
                                    if (((ActorPC)lastAttacker).JobBasic == PC_JOB.SWORDMAN ||
                                        ((ActorPC)lastAttacker).JobBasic == PC_JOB.FENCER ||
                                        ((ActorPC)lastAttacker).JobBasic == PC_JOB.SCOUT ||
                                        ((ActorPC)lastAttacker).JobBasic == PC_JOB.ARCHER)
                                    {
                                        完成任務_更新任務or下一層(map);
                                    }
                                    return;
                                case 2:
                                    if (((ActorPC)lastAttacker).JobBasic == PC_JOB.WIZARD ||
                                        ((ActorPC)lastAttacker).JobBasic == PC_JOB.SHAMAN ||
                                        ((ActorPC)lastAttacker).JobBasic == PC_JOB.VATES ||
                                        ((ActorPC)lastAttacker).JobBasic == PC_JOB.WARLOCK)
                                    {
                                        完成任務_更新任務or下一層(map);
                                    }
                                    return;
                                case 3:
                                    if (((ActorPC)lastAttacker).JobBasic == PC_JOB.TATARABE ||
                                        ((ActorPC)lastAttacker).JobBasic == PC_JOB.FARMASIST ||
                                        ((ActorPC)lastAttacker).JobBasic == PC_JOB.RANGER ||
                                        ((ActorPC)lastAttacker).JobBasic == PC_JOB.MERCHANT)
                                    {
                                        完成任務_更新任務or下一層(map);
                                    }
                                    return;
                            }
                            懲罰(map, lastAttacker);
                        }
                        break;
                    case AncientArkType.CANDLE_4:
                        if (mob.MobID == 35060005 && lastAttacker != null && lastAttacker.type == ActorType.PC)
                        {
                            switch (mob.TInt["種族"])
                            {
                                case 1:
                                    if (((ActorPC)lastAttacker).Race == PC_RACE.EMIL)
                                    {
                                        完成任務_更新任務or下一層(map);
                                    }
                                    return;
                                case 2:
                                    if (((ActorPC)lastAttacker).Race == PC_RACE.TITANIA)
                                    {
                                        完成任務_更新任務or下一層(map);
                                    }
                                    return;
                                case 3:
                                    if (((ActorPC)lastAttacker).Race == PC_RACE.DOMINION)
                                    {
                                        完成任務_更新任務or下一層(map);
                                    }
                                    return;
                            }
                            懲罰(map, lastAttacker);
                        }
                        break;
                    case AncientArkType.CANDLE_5:
                        bool dead = false;
                        foreach (Actor i in map.Actors.Values)
                        {
                            if (i.type == ActorType.MOB)
                            {
                                if (((ActorMob)i).MobID == 35060006 && !i.Buff.Dead)
                                {
                                    dead = true;
                                    break;
                                }
                                if (((ActorMob)i).MobID == 35060005 && !i.Buff.Dead)
                                {
                                    dead = true;
                                    break;
                                }
                            }
                        }
                        if (!dead)
                        {
                            完成任務_更新任務or下一層(map);
                        }
                        break;
                    case AncientArkType.CANDLE_6:
                        if (lastAttacker != null && lastAttacker.type == ActorType.PC && ((ActorPC)lastAttacker).Party != null)
                        {
                            int num = 0;
                            if (mob.MobID == 35060006)
                            {
                                foreach (uint i in ((SagaMap.ActorEventHandlers.MobEventHandler)mob.e).AI.DamageTable.Keys)
                                {
                                    foreach (ActorPC j in ((ActorPC)lastAttacker).Party.Members.Values)
                                    {
                                        if (j.ActorID == i)
                                        {
                                            num++;
                                        }
                                    }
                                }
                            }
                            if (num >= ((ActorPC)lastAttacker).Party.MemberCount)
                            {
                                完成任務_更新任務or下一層(map);
                            }
                        }
                        break;
                }
            }
        }
        void 懲罰(Map map,Actor actor)
        {
            if(actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                int ran = SagaLib.Global.Random.Next(1, 7);
                switch (ran)
                {
                    case 1://出現怪物
                        map.Announce("錯誤！！懲罰：出現怪物");
                        map.SpawnMob(14310005, actor.X, actor.Y, 1000, null);
                        map.SpawnMob(14310005, actor.X, actor.Y, 1000, null);
                        map.SpawnMob(14310005, actor.X, actor.Y, 1000, null);
                        break;
                    case 2://時間縮短
                        map.Announce("錯誤！！懲罰：時間縮短");
                        時間變更(map, -30);
                        break;
                    case 3://變身
                        map.Announce("錯誤！！懲罰：變身");
                        //SkillHandler.Instance.TranceMob(pc, 26100019);
                        //SkillHandler.Instance.TranceMob(pc, 15180000);
                        pc.TranceID = 15180000;
                        break;
                    case 4://變小
                        map.Announce("錯誤！！懲罰：變小");
                        SkillHandler.Instance.ChangePlayerSize(pc, 500);
                        break;
                    case 5://變大
                        map.Announce("錯誤！！懲罰：變大");
                        SkillHandler.Instance.ChangePlayerSize(pc, 2000);
                        break;
                    case 6://回到入口
                        map.Announce("錯誤！！...什麼也沒發生");
                        /*
                        map.Announce("錯誤！！懲罰：回到入口");
                        SagaMap.AncientArks.AncientArk aa = SagaMap.AncientArks.AncientArkFactory.Instance.AncientArks[pc.AncientArkID];
                        byte next_room = aa.Layer_Rooms[pc.AncientArk_Layer].ToList()[0].Key;
                        pc.AncientArk_Room = next_room;
                        if (pc.PossesionedActors.Count != 0)
                        {
                            List<ActorPC> possessioned = pc.PossesionedActors;
                            foreach (ActorPC i in possessioned)
                            {
                                if (i == null || i == pc || !i.Online) continue;
                                i.AncientArk_Room = next_room;
                            }
                        }
                        Map nextMap = aa.Layer_Rooms[pc.AncientArk_Layer][next_room];
                        map.SendActorToMap(pc, nextMap.ID, Global.PosX8to16(AncientArkWarpFactory.Instance.Items[nextMap.OriID].Warp_X[AncientArkGateType.Enter], nextMap.Width), Global.PosY8to16(AncientArkWarpFactory.Instance.Items[nextMap.OriID].Warp_Y[AncientArkGateType.Enter], nextMap.Height));
                        */
                        break;
                    case 7://死亡
                        map.Announce("錯誤！！懲罰：死亡");
                        SagaMap.Skill.SkillHandler.Instance.FixAttack(pc, pc, new SkillArg(), Elements.Neutral, (float)((float)pc.MaxHP * 100.0f));
                        break;
                }
            }
            else
            {
                int ran = SagaLib.Global.Random.Next(1, 7);
                switch (ran)
                {
                    case 1://出現怪物
                        map.Announce("錯誤！！懲罰：出現怪物");
                        map.SpawnMob(14310005, actor.X, actor.Y, 1000, null);
                        map.SpawnMob(14310005, actor.X, actor.Y, 1000, null);
                        map.SpawnMob(14310005, actor.X, actor.Y, 1000, null);
                        break;
                    case 2://時間縮短
                        map.Announce("錯誤！！懲罰：時間縮短");
                        時間變更(map, -30);
                        break;
                }
            }
            
        }

        /*void 下一層傳送點出現(ActorPC pc, AncientArk aa)
        {
            byte layer = pc.AncientArk_Layer;
            byte room = pc.AncientArk_Room;
            if (aa.Layer_Rooms[layer][room].AncientArk.complete)
            {
                foreach(AncientArkGateType i in aa.Detail.Rooms[layer][room].Rooms_warp.Keys)
                {
                    uint x = AncientArkWarpFactory.Instance.Items[aa.Layer_Rooms[pc.AncientArk_Layer][pc.AncientArk_Room].OriID].Warp_X[i];
                    uint y = AncientArkWarpFactory.Instance.Items[aa.Layer_Rooms[pc.AncientArk_Layer][pc.AncientArk_Room].OriID].Warp_Y[i];
                    uint eventID = 0;
                    uint effectID = 9002;
                    switch (i)
                    {
                        case AncientArkGateType.North:
                            eventID = 92001501;
                            break;
                        case AncientArkGateType.East:
                            eventID = 92001502;
                            break;
                        case AncientArkGateType.South:
                            eventID = 92001503;
                            break;
                        case AncientArkGateType.West:
                            eventID = 92001504;
                            break;
                        case AncientArkGateType.Next:
                            eventID = 92001505;
                            effectID = 9005;
                            break;
                        case AncientArkGateType.End:
                            eventID = 92001506;
                            effectID = 9005;
                            break;
                    }
                    Packets.Server.SSMG_NPC_SET_EVENT_AREA p1 = new SagaMap.Packets.Server.SSMG_NPC_SET_EVENT_AREA();
                    p1.StartX = x;
                    p1.EndX = x;
                    p1.StartY = y;
                    p1.EndY = y;
                    p1.EventID = eventID;
                    p1.EffectID = effectID;
                    MapClient.FromActorPC(pc).netIO.SendPacket(p1);
                }
            }
            //9000 小藍
            //9001 小橘紅
            //9002 大藍
            //9003 大紫藍
            //9004小紅
            //9005大紅
        }
        void 進入圖書館(ActorPC pc)
        {

        }*/


        public void 完成任務_更新任務or下一層(Map map)
        {
            SagaMap.AncientArks.AncientArk aa = SagaMap.AncientArks.AncientArkFactory.Instance.AncientArks[map.AncientArk.AncientArkID];
            if (map.AncientArk.QuestTask != null)
            {
                map.AncientArk.QuestTask.Deactivate();
                map.AncientArk.QuestTask = null;
            }
            if(map.AncientArk.Gimmick_ID_layer != 0)
            {
                map.AncientArk.complete_layer = true;
                map.AncientArk.Gimmick_ID_layer = 0;
            }
            
            map.AncientArk.Gimmick_Complete++;
            if (map.AncientArk.Gimmick_Complete >= aa.Detail.Rooms[map.AncientArk.Layer][map.AncientArk.Room_ID].gimmick_room_id.Count)
            {
                map.AncientArk.complete = true;
                map.AncientArk.Gimmick_Count = 0;
                map.AncientArk.Gimmick_ID = 0;
                if (aa.Detail.Rooms[map.AncientArk.Layer][map.AncientArk.Room_ID].gimmick_layer_id[0] != 0)
                {
                    if (!map.AncientArk.complete_layer)
                    {
                        map.AncientArk.Gimmick_ID_layer = aa.Detail.Rooms[map.AncientArk.Layer][map.AncientArk.Room_ID].gimmick_layer_id[0];
                        foreach (Actor i in map.Actors.Values)
                        {
                            if (i.type == ActorType.PC)
                            {
                                ActorPC pc = (ActorPC)i;
                                if (pc == null || !pc.Online) continue;
                                層任務顯示(pc, map);
                            }
                        }
                        map.Announce("追加了新的地圖通關條件");
                    }
                    else
                    {
                        foreach (Actor i in map.Actors.Values)
                        {
                            if (i.type == ActorType.PC)
                            {
                                ActorPC pc = (ActorPC)i;
                                if (pc == null || !pc.Online) continue;
                                傳送點顯示(pc, aa,true);
                                層任務顯示(pc, map);
                            }
                        }
                        map.Announce("出現下一層傳送點！");
                    }
                    return;

                }
                foreach (Actor i in map.Actors.Values)
                {
                    if (i.type == ActorType.PC)
                    {
                        ActorPC pc = (ActorPC)i;
                        if (pc == null || !pc.Online) continue;
                        傳送點顯示(pc, aa,false);
                    }
                }
                map.Announce("新的房間傳送點出現了！");
            }
            else
            {
                map.Announce("追加了新的房間通關條件");
                map.AncientArk.Gimmick_Count = 0;
                map.AncientArk.Gimmick_ID = aa.Detail.Rooms[map.AncientArk.Layer][map.AncientArk.Room_ID].gimmick_room_id[map.AncientArk.Gimmick_Complete];
                任務_時間流逝(map);
            }
            資料更新(map);
        }

        void 層任務顯示(ActorPC pc, Map map)
        {
            SagaMap.AncientArks.AncientArk aa = SagaMap.AncientArks.AncientArkFactory.Instance.AncientArks[map.AncientArk.AncientArkID];
            任務_時間流逝(map);
            if (aa.Detail.Rooms[pc.AncientArk_Layer][pc.AncientArk_Room].gimmick_layer_id[0] != 0)
            {
                SagaMap.Packets.Server.SSMG_AARCH_QUEST_DETAIL_LAYER p1 = new SagaMap.Packets.Server.SSMG_AARCH_QUEST_DETAIL_LAYER();
                p1.Map_Room_ID = pc.AncientArk_Room;
                p1.MapID_Ori = map.OriID;
                p1.Unknown = aa.Detail.ID;
                p1.player = pc;
                p1.Gimmick = aa;
                MapClient.FromActorPC(pc).netIO.SendPacket(p1);
                foreach (Map i in aa.Layer_Rooms[pc.AncientArk_Layer].Values)
                {
                    if (i.AncientArk.complete)
                    {
                        SagaMap.Packets.Server.SSMG_AARCH_QUEST_DETAIL p = new SagaMap.Packets.Server.SSMG_AARCH_QUEST_DETAIL();
                        p.Map_Room_ID = i.AncientArk.Room_ID;
                        p.MapID_Ori = i.OriID;
                        p.Unknown = aa.Detail.ID;
                        p.player = pc;
                        p.Gimmick = aa;
                        MapClient.FromActorPC(pc).netIO.SendPacket(p);
                    }
                }
                SagaMap.Packets.Server.SSMG_AARCH_QUEST_DETAIL p2 = new SagaMap.Packets.Server.SSMG_AARCH_QUEST_DETAIL();
                p2.Map_Room_ID = pc.AncientArk_Room;
                p2.MapID_Ori = map.OriID;
                p2.Unknown = aa.Detail.ID;
                p2.player = pc;
                p2.Gimmick = aa;
                MapClient.FromActorPC(pc).netIO.SendPacket(p2);
            }
            
        }

        void 資料更新(Map map)
        {
            SagaMap.AncientArks.AncientArk aa = SagaMap.AncientArks.AncientArkFactory.Instance.AncientArks[map.AncientArk.AncientArkID];
            foreach (Actor i in map.Actors.Values) 
            { 
                if(i.type == ActorType.PC)
                {
                    ActorPC pc = (ActorPC)i;
                    if (pc == null || !pc.Online) continue;
                    SagaMap.Packets.Server.SSMG_AARCH_QUEST_DETAIL p1 = new SagaMap.Packets.Server.SSMG_AARCH_QUEST_DETAIL();
                    p1.Map_Room_ID = pc.AncientArk_Room;
                    p1.MapID_Ori = map.OriID;
                    p1.Unknown = aa.Detail.ID;
                    p1.player = pc;
                    p1.Gimmick = aa;
                    MapClient.FromActorPC(pc).netIO.SendPacket(p1);
                }
            }
        }
        void 傳送點顯示(ActorPC pc, AncientArk aa,bool layer)
        {
            foreach (AncientArkGateType i in aa.Detail.Rooms[pc.AncientArk_Layer][pc.AncientArk_Room].Rooms_warp.Keys)
            {
                if (!layer && (i == AncientArkGateType.Next || i == AncientArkGateType.End)) continue;
                uint x = AncientArkWarpFactory.Instance.Items[aa.Layer_Rooms[pc.AncientArk_Layer][pc.AncientArk_Room].OriID].Warp_X[i];
                uint y = AncientArkWarpFactory.Instance.Items[aa.Layer_Rooms[pc.AncientArk_Layer][pc.AncientArk_Room].OriID].Warp_Y[i];
                uint eventID = 0;
                uint effectID = 9002;
                switch (i)
                {
                    case AncientArkGateType.North:
                        eventID = 92001501;
                        break;
                    case AncientArkGateType.East:
                        eventID = 92001502;
                        break;
                    case AncientArkGateType.South:
                        eventID = 92001503;
                        break;
                    case AncientArkGateType.West:
                        eventID = 92001504;
                        break;
                    case AncientArkGateType.Next:
                        eventID = 92001505;
                        effectID = 9005;
                        break;
                    case AncientArkGateType.End:
                        eventID = 92001506;
                        effectID = 9005;
                        break;
                }
                Packets.Server.SSMG_NPC_SET_EVENT_AREA p2 = new SagaMap.Packets.Server.SSMG_NPC_SET_EVENT_AREA();
                p2.StartX = x;
                p2.EndX = x;
                p2.StartY = y;
                p2.EndY = y;
                p2.EventID = eventID;
                p2.EffectID = effectID;
                MapClient.FromActorPC(pc).netIO.SendPacket(p2);
            }
        }
        public void 單人資料更新(ActorPC pc, Map map)
        {
            SagaMap.AncientArks.AncientArk aa = SagaMap.AncientArks.AncientArkFactory.Instance.AncientArks[map.AncientArk.AncientArkID];
            if (pc.AncientArkID == 0 || pc.AncientArk_Layer == 0 || pc.AncientArk_Room == 0) return;
            if (map.AncientArk.complete)
            {
                if (map.AncientArk.complete_layer)
                {
                    傳送點顯示(pc, aa, true);
                }
                else
                {
                    層任務顯示(pc, map);
                    傳送點顯示(pc, aa, false);
                    公告_回答問題(MapClient.FromActorPC(pc), map);
                }
            }
            else
            {
                任務_時間流逝(map);
                公告_回答問題(MapClient.FromActorPC(pc), map);
            }
            
            /*if (aa.Detail.Rooms[pc.AncientArk_Layer].gimmick_count == 0)
            {
                return;
            }*/
            if (pc == null || !pc.Online) return;

            /*SagaMap.Packets.Server.SSMG_AARCH_QUEST_DETAIL_LAYER pLayer = new SagaMap.Packets.Server.SSMG_AARCH_QUEST_DETAIL_LAYER();
            pLayer.Map_Room_ID = pc.AncientArk_Room;
            pLayer.MapID_Ori = map.OriID;
            pLayer.Unknown = aa.Detail.ID;
            pLayer.player = pc;
            pLayer.Gimmick = aa;
            MapClient.FromActorPC(pc).netIO.SendPacket(pLayer);*/
            
            SagaMap.Packets.Server.SSMG_AARCH_QUEST_DETAIL p1 = new SagaMap.Packets.Server.SSMG_AARCH_QUEST_DETAIL();
            p1.Map_Room_ID = pc.AncientArk_Room;
            p1.MapID_Ori = map.OriID;
            p1.Unknown = aa.Detail.ID;
            p1.player = pc;
            p1.Gimmick = aa;
            MapClient.FromActorPC(pc).netIO.SendPacket(p1);
            //Logger.ShowInfo("單人資料更新 map.OriID  " + map.OriID);
            //Logger.ShowInfo("單人資料更新 pc.AncientArk_Layer  " + pc.AncientArk_Layer);
            //Logger.ShowInfo("單人資料更新 MapID  " + map.ID);

            SagaMap.Packets.Server.SSMG_QUEST_LIST ppp3 = new SagaMap.Packets.Server.SSMG_QUEST_LIST();
            ppp3.data = new byte[10];
            ppp3.offset = 2;
            ppp3.ID = 0x0606;
            ppp3.PutUInt(0);
            ppp3.PutUInt(8);
            ppp3.PutUInt(0xffffffff);
            ppp3.PutUInt(0xffffffff);
            ppp3.PutByte(0);
            MapClient.FromActorPC(pc).netIO.SendPacket(ppp3);
            ppp3 = new SagaMap.Packets.Server.SSMG_QUEST_LIST();
            ppp3.data = new byte[10];
            ppp3.offset = 2;
            ppp3.ID = 0x0606;
            ppp3.PutUInt(1);
            ppp3.PutUInt(8);
            ppp3.PutUInt(0xffffffff);
            ppp3.PutUInt(0xffffffff);
            ppp3.PutByte(0);
            MapClient.FromActorPC(pc).netIO.SendPacket(ppp3);
        }


        public void 開場時間設定(ActorPC pc, AncientArk aa)
        {
            SagaMap.Packets.Server.SSMG_AARCH_QUEST_TIME p1 = new SagaMap.Packets.Server.SSMG_AARCH_QUEST_TIME();
            p1.Quest_ID = aa.Detail.ID;
            p1.Time = aa.Time;
            MapClient.FromActorPC(pc).netIO.SendPacket(p1);
        }

        public void 生成寶箱(Map map, AncientArk aa)
        {
            aa.Time = 600;
            int box_count = 1;
            if (aa.Creator != null && aa.Creator.Party != null)
            {
                box_count = aa.Creator.Party.Members.Count;
            }
            //map.SpawnCustomMob(10370050,map.ID,32,32,10, box_count,6000,)


            ActorMob mob_big = map.SpawnMob(10370051, Global.PosX8to16(37, map.Width), Global.PosY8to16(32, map.Height), 1000, null);
            ActorEventHandlers.MobEventHandler eh_big = (ActorEventHandlers.MobEventHandler)mob_big.e;
            eh_big.Dying += new MobCallback(Ondie);

            List<ActorMob> mobs = new List<ActorMob>();
            for (int i = 0; i < box_count; i++)
            {
                byte x = (byte)SagaLib.Global.Random.Next(20, 40);
                byte y = (byte)SagaLib.Global.Random.Next(20, 40);
                ActorMob mob = map.SpawnMob(10370050, Global.PosX8to16(x, map.Width), Global.PosY8to16(y, map.Height), 1000, null);
                ActorEventHandlers.MobEventHandler eh = (ActorEventHandlers.MobEventHandler)mob.e;
                eh.Dying += new MobCallback(Ondie);
            }
        }
        void Ondie(MobEventHandler e, ActorPC pc)
        {
            Map map = e.AI.map;
            List<ActorItem> items = new List<ActorItem>();
            SagaMap.AncientArks.AncientArk aa = SagaMap.AncientArks.AncientArkFactory.Instance.AncientArks[map.AncientArk.AncientArkID];
            if(aa.Creator != null && aa.Creator.Party != null)
            {

                foreach (ActorPC i in aa.Creator.Party.Members.Values)
                {
                    if (i == null || !i.Online || i.MapID != map.ID) continue;
                    TreasureItem res = TreasureFactory.Instance.GetRandomItem(aa.Detail.Treasurefile);
                    SagaDB.Item.Item itemDroped = ItemFactory.Instance.GetItem(res.ID, true);
                    itemDroped.Stack = 1;

                    ActorItem actor = new ActorItem(itemDroped);
                    actor.Owner = i;
                    actor.Roll = false;
                    actor.Party = true;
                    actor.MapID = map.ID;
                    actor.e = new SagaMap.ActorEventHandlers.ItemEventHandler(actor);

                    int x = SagaLib.Global.Random.Next(-300, 300) + e.AI.X_Ori;
                    int y = SagaLib.Global.Random.Next(-300, 300) + e.AI.X_Ori;
                    short[] pos;
                    pos = new short[2];
                    pos[0] = (short)x;
                    pos[1] = (short)y;
                    actor.X = pos[0];
                    actor.Y = pos[1];
                    map.RegisterActor(actor);
                    actor.invisble = false;
                    map.OnActorVisibilityChange(actor);
                    items.Add(actor);

                    Tasks.Item.DeleteItem task = new Tasks.Item.DeleteItem(actor);
                    task.Activate();
                    actor.Tasks.Add("DeleteItem", task);
                }
                foreach (ActorPC i in aa.Creator.Party.Members.Values)
                {
                    if (i == null || !i.Online || i.MapID != map.ID) continue;
                    foreach (ActorItem j in items)
                    {
                        if(j.Owner == i)
                        {
                       
                            EffectArg arg = new EffectArg();
                            arg.actorID = i.ActorID;
                            arg.effectID = 7117;
                            arg.x = Global.PosX16to8(j.X, map.Width);
                            arg.y = Global.PosY16to8(j.Y, map.Height);
                            MapClient.FromActorPC(i).SendNPCShowEffect(arg.actorID, arg.x, arg.y, arg.height, arg.effectID, arg.oneTime);
                            //map.SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE.SHOW_EFFECT, arg, i, true);
                        }
                        else
                        {
                            EffectArg arg = new EffectArg();
                            arg.actorID = i.ActorID;
                            arg.effectID = 7116;
                            arg.x = Global.PosX16to8(j.X, map.Width);
                            arg.y = Global.PosY16to8(j.Y, map.Height);
                            MapClient.FromActorPC(i).SendNPCShowEffect(arg.actorID, arg.x, arg.y, arg.height, arg.effectID, arg.oneTime);
                            //map.SendEventToAllActorsWhoCanSeeActor(EVENT_TYPE.SHOW_EFFECT, arg, i, true);
                        }
                    }
                }
            }
            
        }
        //80270000寶箱防
        //10002018出口傳送點

        public void 通關_紀錄玩家任務進度(ActorPC pc, AncientArk aa)
        {
            開場時間設定(pc,aa);
            if (pc.AncientArk_QuestID != 0 && aa.Detail.ID == pc.AncientArk_QuestID)
            {
                switch (aa.Detail.ID)
                {
                    case 100000:
                        if (pc.AncientArk_Clear == 0) pc.AncientArk_Clear = 1;
                        break;
                    case 100001:
                        if (pc.AncientArk_Clear == 1) pc.AncientArk_Clear = 2;
                        break;
                    case 100002:
                        if (pc.AncientArk_Clear == 2) pc.AncientArk_Clear = 3;
                        break;
                    case 100003:
                        if (pc.AncientArk_Clear == 3) pc.AncientArk_Clear = 4;
                        break;
                    case 100004:
                        if (pc.AncientArk_Clear == 4) pc.AncientArk_Clear = 5;
                        break;
                    case 100005:
                        if (pc.AncientArk_Clear == 5) pc.AncientArk_Clear = 6;
                        break;
                    case 100006:
                        if (pc.AncientArk_Clear == 6) pc.AncientArk_Clear = 7;
                        break;
                    case 100007:
                        if (pc.AncientArk_Clear == 7) pc.AncientArk_Clear = 8;
                        break;
                }
            }

            pc.AncientArk_QuestID = 0;
            pc.AncientArkID = 0;
            pc.AncientArk_Room = 0;
            pc.AncientArk_Layer = 0;
        }

        List<byte[]> 蠟燭位置(Map map)
        {
            List<byte[]> map_Loc = new List<byte[]>();
            switch (map.OriID)
            {
                case 80200000:
                    map_Loc = new List<byte[]> { new byte[] { 56, 6 }, new byte[] { 68, 19 }, new byte[] { 124, 24 }, new byte[] { 124, 91 }, new byte[] { 70, 76 }, new byte[] { 46, 121 }, new byte[] { 116, 111 }, new byte[] { 4, 62 }, new byte[] { 69, 47 }, new byte[] { 8, 84 } };
                    break;
                case 80210000:
                    map_Loc = new List<byte[]> { new byte[] { 41, 36 }, new byte[] { 6, 73 }, new byte[] { 59, 71 }, new byte[] { 8, 114 }, new byte[] { 107, 72 }, new byte[] { 83, 119 }, new byte[] { 51, 89 }, new byte[] { 85, 53 }, new byte[] { 107, 20 }, new byte[] { 22, 4 } };
                    break;
                case 80220000:
                    map_Loc = new List<byte[]> { new byte[] { 24, 19 }, new byte[] { 5, 39 }, new byte[] { 66, 9 }, new byte[] { 35, 89 }, new byte[] { 14, 58 }, new byte[] { 82, 103 }, new byte[] { 67, 57 }, new byte[] { 109, 123 }, new byte[] { 101, 71 }, new byte[] { 122, 8 } };
                    break;
                case 80230000:
                    map_Loc = new List<byte[]> { new byte[] { 4, 52 }, new byte[] { 26, 27 }, new byte[] { 78, 31 }, new byte[] { 24, 84 }, new byte[] { 51, 115 }, new byte[] { 109, 78 }, new byte[] { 72, 122 }, new byte[] { 105, 54 }, new byte[] { 123, 9 }, new byte[] { 58, 6 } };
                    break;
                case 80240000:
                    map_Loc = new List<byte[]> { new byte[] { 10, 9 }, new byte[] { 56, 5 }, new byte[] { 117, 10 }, new byte[] { 107, 56 }, new byte[] { 66, 34 }, new byte[] { 41, 85 }, new byte[] { 53, 119 }, new byte[] { 112, 113 }, new byte[] { 17, 87 }, new byte[] { 40, 42 } };
                    break;
                case 80250000:
                    map_Loc = new List<byte[]> { new byte[] { 14, 4 }, new byte[] { 11, 59 }, new byte[] { 2, 98 }, new byte[] { 25, 91 }, new byte[] { 30, 49 }, new byte[] { 72, 5 }, new byte[] { 102, 33 }, new byte[] { 121, 69 }, new byte[] { 82, 98 }, new byte[] { 113, 111 } };
                    break;
            }
            return map_Loc;
        } 

        public void 蠟燭_點燃所有蠟燭(Map map)
        {
            List<byte[]> coordinates = 蠟燭位置(map);
            for (int i = 0; i < 10; i++)
            {
                int index = SagaLib.Global.Random.Next(1, coordinates.Count) - 1;
                byte x = (byte)coordinates[index][0];
                byte y = (byte)coordinates[index][1];
                coordinates.RemoveAt(index);
                ActorMob mob = map.SpawnMob(35060005, Global.PosX8to16(x, map.Width), Global.PosY8to16(y, map.Height), 0, null);
                SagaMap.ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
                eh.AI.Mode = new SagaMap.Mob.AIMode(6);
                eh.Dying += new MobCallback(蠟燭_點燃所有蠟燭_點火);
            }
            /*for (int i = 0; i < 10; i++)
            {
                //byte x = (byte)SagaLib.Global.Random.Next(1, 120);
                //byte y = (byte)SagaLib.Global.Random.Next(1, 120);

                byte x = (byte)(78 + i);
                byte y = (byte)(76 + i);
                ActorMob mob = map.SpawnMob(35060005, Global.PosX8to16(x, map.Width), Global.PosY8to16(y, map.Height), 0, null);
                SagaMap.ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
                eh.AI.Mode = new SagaMap.Mob.AIMode(6);
                eh.Dying += new MobCallback(蠟燭_點燃所有蠟燭_點火);
            }*/
        }

        void 蠟燭_點燃所有蠟燭_點火(MobEventHandler e, ActorPC pc)
        {
            Map map = e.AI.map;
            byte x = Global.PosX16to8(e.mob.X, map.Width);
            byte y = Global.PosY16to8(e.mob.Y, map.Height);
            ActorMob mob = map.SpawnMob(35060006, e.mob.X, e.mob.Y, 0, null);
            SagaMap.ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
            eh.AI.Mode = new SagaMap.Mob.AIMode(6);
            eh.Dying += new MobCallback(蠟燭_點燃所有蠟燭_熄滅);
            mob.TInt["職業"] = e.mob.TInt["職業"];
            mob.TInt["種族"] = e.mob.TInt["種族"];
            蠟燭任務判斷(map, e.mob, pc);
        }
        void 蠟燭_點燃所有蠟燭_熄滅(MobEventHandler e, ActorPC pc)
        {
            Map map = e.AI.map;
            byte x = Global.PosX16to8(e.mob.X, map.Width);
            byte y = Global.PosY16to8(e.mob.Y, map.Height);
            ActorMob mob = map.SpawnMob(35060005, e.mob.X, e.mob.Y, 0, null);
            SagaMap.ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
            eh.AI.Mode = new SagaMap.Mob.AIMode(6);
            eh.Dying += new MobCallback(蠟燭_點燃所有蠟燭_點火);
            mob.TInt["職業"] = e.mob.TInt["職業"];
            mob.TInt["種族"] = e.mob.TInt["種族"];
        }


        public void 蠟燭_點燃正確的蠟燭(Map map)
        {
            List<byte[]> coordinates = 蠟燭位置(map);
            for (int i = 0; i < 9; i++)
            {
                int index = SagaLib.Global.Random.Next(1, coordinates.Count) - 1;
                byte x = (byte)coordinates[index][0];
                byte y = (byte)coordinates[index][1];
                coordinates.RemoveAt(index);
                ActorMob mob = map.SpawnMob(35060005, Global.PosX8to16(x, map.Width), Global.PosY8to16(y, map.Height), 0, null);
                SagaMap.ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
                eh.AI.Mode = new SagaMap.Mob.AIMode(6);
                eh.Dying += new MobCallback(蠟燭_點燃所有蠟燭_點火);
            }
            byte x1 = coordinates[0][0];
            byte y1 = coordinates[0][1];
            ActorMob mob_A = map.SpawnMob(35060005, Global.PosX8to16(x1, map.Width), Global.PosY8to16(y1, map.Height), 0, null);
            mob_A.TInt["正確蠟燭"] = 1;
            SagaMap.ActorEventHandlers.MobEventHandler ehA = (SagaMap.ActorEventHandlers.MobEventHandler)mob_A.e;
            ehA.AI.Mode = new SagaMap.Mob.AIMode(6);
            ehA.Dying += new MobCallback(蠟燭_點燃所有蠟燭_點火);
            /*for (int i = 0; i < 10; i++)
            {
                byte x = (byte)(78 + i);
                byte y = (byte)(76 + i);
                ActorMob mob = map.SpawnMob(35060005, Global.PosX8to16(x, map.Width), Global.PosY8to16(y, map.Height), 0, null);
                SagaMap.ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
                eh.AI.Mode = new SagaMap.Mob.AIMode(6);
                eh.Dying += new MobCallback(蠟燭_點燃所有蠟燭_點火);
            }
            byte x1 = 77;
            byte y1 = 75;
            ActorMob mob_A = map.SpawnMob(35060005, Global.PosX8to16(x1, map.Width), Global.PosY8to16(y1, map.Height), 0, null);
            mob_A.TInt["正確蠟燭"] = 1;
            SagaMap.ActorEventHandlers.MobEventHandler ehA = (SagaMap.ActorEventHandlers.MobEventHandler)mob_A.e;
            ehA.AI.Mode = new SagaMap.Mob.AIMode(6);
            ehA.Dying += new MobCallback(蠟燭_點燃所有蠟燭_點火);*/
        }
        public void 蠟燭_點燃特定職業的蠟燭(Map map)
        {
            List<byte[]> coordinates = 蠟燭位置(map);

            int num = (byte)SagaLib.Global.Random.Next(1, 3);
            for (int i = 0; i < 5; i++)
            {
                int index = SagaLib.Global.Random.Next(1, coordinates.Count) - 1;
                byte x = (byte)coordinates[index][0];
                byte y = (byte)coordinates[index][1];
                coordinates.RemoveAt(index);
                ActorMob mob = map.SpawnMob(35060005, Global.PosX8to16(x, map.Width), Global.PosY8to16(y, map.Height), 0, null);

                mob.TInt["職業"] = num;
                SagaMap.ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
                eh.AI.Mode = new SagaMap.Mob.AIMode(6);
                eh.Dying += new MobCallback(蠟燭_點燃所有蠟燭_點火);
            }
            /*for (int i = 0; i < 5; i++)
            {
                byte x = (byte)(78 + i);
                byte y = (byte)(76 + i);
                ActorMob mob = map.SpawnMob(35060005, Global.PosX8to16(x, map.Width), Global.PosY8to16(y, map.Height), 0, null);

                int num = (byte)SagaLib.Global.Random.Next(1, 3);
                mob.TInt["職業"] = num;
                SagaMap.ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
                eh.AI.Mode = new SagaMap.Mob.AIMode(6);
                eh.Dying += new MobCallback(蠟燭_點燃所有蠟燭_點火);
            }*/
        }
        public void 蠟燭_點燃特定種族的蠟燭(Map map)
        {
            List<byte[]> coordinates = 蠟燭位置(map);
            int num = (byte)SagaLib.Global.Random.Next(1, 3);
            for (int i = 0; i < 5; i++)
            {
                int index = SagaLib.Global.Random.Next(1, coordinates.Count) - 1;
                byte x = (byte)coordinates[index][0];
                byte y = (byte)coordinates[index][1];
                coordinates.RemoveAt(index);
                ActorMob mob = map.SpawnMob(35060005, Global.PosX8to16(x, map.Width), Global.PosY8to16(y, map.Height), 0, null);

                mob.TInt["種族"] = num;
                SagaMap.ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
                eh.AI.Mode = new SagaMap.Mob.AIMode(6);
                eh.Dying += new MobCallback(蠟燭_點燃所有蠟燭_點火);
            }
            /*for (int i = 0; i < 5; i++)
            {
                byte x = (byte)(78 + i);
                byte y = (byte)(76 + i);
                ActorMob mob = map.SpawnMob(35060005, Global.PosX8to16(x, map.Width), Global.PosY8to16(y, map.Height), 0, null);

                int num = (byte)SagaLib.Global.Random.Next(1, 3);
                mob.TInt["種族"] = num;
                SagaMap.ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
                eh.AI.Mode = new SagaMap.Mob.AIMode(6);
                eh.Dying += new MobCallback(蠟燭_點燃所有蠟燭_點火);
            }*/
        }

        /*public void 蠟燭_消滅所有蠟燭(Map map)
        {
            for (int i = 0; i < 5; i++)
            {
                //byte x = (byte)SagaLib.Global.Random.Next(1, 120);
                //byte y = (byte)SagaLib.Global.Random.Next(1, 120);

                byte x = (byte)(78 + i);
                byte y = (byte)(76 + i);
                ActorMob mob = map.SpawnMob(35060005, Global.PosX8to16(x, map.Width), Global.PosY8to16(y, map.Height), 0, null);
                SagaMap.ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
                eh.AI.Mode = new SagaMap.Mob.AIMode(6);
                eh.Dying += new MobCallback(蠟燭_點燃所有蠟燭_點火);
            }
        }*/
        public void 蠟燭_全員摸蠟燭(Map map)
        {
            List<byte[]> coordinates = 蠟燭位置(map);
            int index = SagaLib.Global.Random.Next(1, coordinates.Count) - 1;
            byte x = (byte)coordinates[index][0];
            byte y = (byte)coordinates[index][1];
            coordinates.RemoveAt(index);
            ActorMob mob = map.SpawnMob(35060006, Global.PosX8to16(x, map.Width), Global.PosY8to16(y, map.Height), 0, null);
            SagaMap.ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
            eh.AI.Mode = new SagaMap.Mob.AIMode(6);
            eh.Defending += new MobCallback(蠟燭_全員摸蠟燭_被打);
            mob.MaxHP = 99999999;
            mob.HP = 99999999;
            /*byte x = 78;
            byte y = 76;
            ActorMob mob = map.SpawnMob(35060006, Global.PosX8to16(x, map.Width), Global.PosY8to16(y, map.Height), 0, null);
            SagaMap.ActorEventHandlers.MobEventHandler eh = (SagaMap.ActorEventHandlers.MobEventHandler)mob.e;
            eh.AI.Mode = new SagaMap.Mob.AIMode(6);
            eh.Defending += new MobCallback(蠟燭_全員摸蠟燭_被打);
            mob.MaxHP = 99999999;
            mob.HP = 99999999;*/
        }

        void 蠟燭_全員摸蠟燭_被打(MobEventHandler e, ActorPC pc)
        {
            Map map = e.AI.map;
            if (map.AncientArk.Gimmick_ID == 0) return;
            if (pc.Party != null)
            {
                int num = 0;
                if (e.mob.MobID == 35060006)
                {
                    foreach (uint i in e.AI.DamageTable.Keys)
                    {
                        foreach (ActorPC j in pc.Party.Members.Values)
                        {
                            if (j.ActorID == i)
                            {
                                num++;
                            }
                        }
                    }
                }
                if (num >= pc.Party.MemberCount)
                {
                    完成任務_更新任務or下一層(map);
                }
            }
        }

        void 時間變更(Map map ,int time_update)
        {
            SagaMap.AncientArks.AncientArk aa = SagaMap.AncientArks.AncientArkFactory.Instance.AncientArks[map.AncientArk.AncientArkID];
            aa.Time = aa.Time + time_update;
            foreach(byte i in aa.Layer_Rooms.Keys)
            {
                foreach (Map j in aa.Layer_Rooms[i].Values)
                {
                    foreach (Actor a in j.Actors.Values)
                    {
                        if(a.type == ActorType.PC && ((ActorPC)a).Online)
                        {
                            SagaMap.Packets.Server.SSMG_AARCH_QUEST_TIME_UPDATE p1 = new SagaMap.Packets.Server.SSMG_AARCH_QUEST_TIME_UPDATE();
                            p1.Time = (uint)aa.Time;
                            MapClient.FromActorPC((ActorPC)a).netIO.SendPacket(p1);
                        }
                    }
                }
            }
        }

        void 任務_時間流逝(Map map)
        {
            if (map.AncientArk.QuestTask != null) return;
            if (map.AncientArk.Gimmick_ID_layer != 0)
            {
                SagaMap.AncientArks.AncientArkGimmick gimmick_layer = SagaMap.AncientArks.AncientArkGimmickFactory.Instance.Items[map.AncientArk.Gimmick_ID_layer];

                if (gimmick_layer.type == AncientArks.AncientArkType.WAIT)
                {
                    map.Announce("剩餘時間：" + gimmick_layer.CurrentCount + "秒");
                    map.AncientArk.Gimmick_Count = gimmick_layer.CurrentCount;
                    map.AncientArk.QuestTask = new Tasks.AncientArk.AncientArkQuest(map, gimmick_layer.CurrentCount);
                    map.AncientArk.QuestTask.Activate();
                }
                
                return;
            }
            if (map.AncientArk.Gimmick_ID != 0) 
            {
                SagaMap.AncientArks.AncientArkGimmick gimmick = SagaMap.AncientArks.AncientArkGimmickFactory.Instance.Items[map.AncientArk.Gimmick_ID];
                if (gimmick.type == AncientArks.AncientArkType.WAIT)
                {
                    map.Announce("剩餘時間：" + gimmick.CurrentCount + "秒");
                    map.AncientArk.Gimmick_Count = gimmick.CurrentCount;
                    map.AncientArk.QuestTask = new Tasks.AncientArk.AncientArkQuest(map, gimmick.CurrentCount);
                    map.AncientArk.QuestTask.Activate();
                }
                if (gimmick.type == AncientArks.AncientArkType.CANDLE_3 ||
                    gimmick.type == AncientArks.AncientArkType.CANDLE_4)
                {
                    map.AncientArk.Gimmick_Count = gimmick.CurrentCount;
                    map.AncientArk.QuestTask = new Tasks.AncientArk.AncientArkQuest(map, gimmick.CurrentCount);
                    map.AncientArk.QuestTask.Activate();
                }
                if (gimmick.type == AncientArks.AncientArkType.ASK)
                {
                    map.AncientArk.Gimmick_Count = 300;
                    map.AncientArk.QuestTask = new Tasks.AncientArk.AncientArkQuest(map, 300);
                    map.AncientArk.QuestTask.Activate();
                }
            }
        }

        public void 任務_回答問題(string ask, Map map)
        {
            /*if (map.AncientArk.Gimmick_ID_layer != 0)
            {
                
                SagaMap.AncientArks.AncientArkGimmick gimmick_layer = SagaMap.AncientArks.AncientArkGimmickFactory.Instance.Items[map.AncientArk.Gimmick_ID_layer];
                if (gimmick_layer.type == AncientArks.AncientArkType.ASK)
                {
                    
                    if (AncientArkGimmickFactory.Instance.Quest_Ask[map.AncientArk.Ask_id]["A"] == "")
                    {
                        map.AncientArk.complete_layer = true;
                        map.AncientArk.Gimmick_ID_layer = 0;
                        完成任務_更新任務or下一層(map);
                    }
                }
                return;
            }
            if (map.AncientArk.Gimmick_ID != 0) 
            {
                SagaMap.AncientArks.AncientArkGimmick gimmick = SagaMap.AncientArks.AncientArkGimmickFactory.Instance.Items[map.AncientArk.Gimmick_ID];
                if (gimmick.type == AncientArks.AncientArkType.ASK)
                {
                    if (ask == "回答問題")
                    {
                        完成任務_更新任務or下一層(map);
                    }
                }
            }*/
            if(map.AncientArk.Gimmick_ID_layer == 91000001)
            {
                string lowerText = ask.ToLower();  // 轉換為小寫
                switch (lowerText)
                {
                    case "埃米爾":
                    case "人族":
                    case "emil":
                        foreach (Actor i in map.Actors.Values)
                        {
                            if (i.type == ActorType.PC)
                            {
                                ActorPC pc = (ActorPC)i;
                                if (pc == null || !pc.Online || pc.Race != PC_RACE.EMIL) continue;
                                pc.TInt["AA_Race_Attack"] = 1;
                            }
                        }
                        map.Announce("您的回答是：" + lowerText);

                        //map.AncientArk.complete_layer = true;
                        //map.AncientArk.Gimmick_ID_layer = 0;
                        完成任務_更新任務or下一層(map);
                        break;
                    case "泰達尼亞":
                    case "塔妮亞":
                    case "天使":
                    case "titania":
                        foreach (Actor i in map.Actors.Values)
                        {
                            if (i.type == ActorType.PC)
                            {
                                ActorPC pc = (ActorPC)i;
                                if (pc == null || !pc.Online || pc.Race != PC_RACE.TITANIA) continue;
                                pc.TInt["AA_Race_Attack"] = 1;
                            }
                        }
                        map.Announce("您的回答是：" + lowerText);

                        //map.AncientArk.complete_layer = true;
                        //map.AncientArk.Gimmick_ID_layer = 0;
                        完成任務_更新任務or下一層(map);
                        break;
                    case "多米尼翁":
                    case "道米尼":
                    case "惡魔":
                    case "dominion":
                        foreach (Actor i in map.Actors.Values)
                        {
                            if (i.type == ActorType.PC)
                            {
                                ActorPC pc = (ActorPC)i;
                                if (pc == null || !pc.Online || pc.Race != PC_RACE.DOMINION) continue;
                                pc.TInt["AA_Race_Attack"] = 1;

                            }
                        }
                        map.Announce("您的回答是：" + lowerText);

                        //map.AncientArk.complete_layer = true;
                        //map.AncientArk.Gimmick_ID_layer = 0;
                        完成任務_更新任務or下一層(map);
                        break;
                    case "人":
                    case "人類":
                    case "human":
                        map.Announce("您的回答是：" + lowerText);

                        //map.AncientArk.complete_layer = true;
                        //map.AncientArk.Gimmick_ID_layer = 0;
                        完成任務_更新任務or下一層(map);
                        break;
                }
                return;
            }
            if (map.AncientArk.Ask != null)
            {
                if (map.AncientArk.Ask.ask.Contains(ask))
                {
                    map.AncientArk.Ask = null;
                    map.Announce("回答正確");
                    完成任務_更新任務or下一層(map);
                }
                else
                {
                    map.Announce("回答錯誤");
                    時間變更(map, -30);
                }
            }

        }
        public void 公告_回答問題(MapClient pc, Map map)
        {
            if (map.AncientArk.Gimmick_ID_layer == 91000001)
            {
                //map.Announce("Announce  什麼動物早上四條腿、中午兩條腿、晚上三條腿？");
                pc.SendAnnounce("什麼動物早上四條腿、中午兩條腿、晚上三條腿？What animal walks on four legs in the morning, two legs at noon, and three legs in the evening?");
                return;
            }
            if (map.AncientArk.Ask != null)
            {
                pc.SendAnnounce(map.AncientArk.Ask.question);
            }

        }
    }
}