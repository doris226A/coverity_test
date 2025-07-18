﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using SagaDB.Partner;
using SagaMap.Partner;
using SagaDB;
using SagaDB.Item;
using SagaDB.Actor;
using SagaDB.Npc;
using SagaDB.Quests;
using SagaLib;
using SagaMap;
using SagaMap.Manager;

namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        public bool trading = false;//ECOKEY 其他地方需要讀取
       // bool trading = false;
        bool confirmed = false;
        bool performed = false;
        public bool npcTrade = false;
        List<uint> tradeItems = null;
        List<ushort> tradeCounts = null;
        long tradingGold = 0;
        ActorPC tradingTarget;
        public List<Item> npcTradeItem = null;

        public void OnTradeRequest(Packets.Client.CSMG_TRADE_REQUEST p)
        {
            Actor actor = this.Map.GetActor(p.ActorID);
            if (actor == null)
                return;
            if (actor.type != ActorType.PC)
                return;
            ActorPC pc = (ActorPC)actor;
            MapClient client = MapClient.FromActorPC(pc);
            int result = CheckTradeRequest(client);

            if (result == 0)
            {
                this.tradingTarget = pc;
                client.SendTradeRequest(this.Character);
            }

            Packets.Server.SSMG_TRADE_REQUEST_RESULT p1 = new SagaMap.Packets.Server.SSMG_TRADE_REQUEST_RESULT();
            p1.Result = result;
            this.netIO.SendPacket(p1);
        }

        private int CheckTradeRequest(MapClient client)
        {
            if (this.trading == true)
                return -1; //トレード中です
            if (this.scriptThread != null)
                return -2; //イベント中です
            if (client.trading == true)
                return -3; //相手がトレード中です
            if (client.scriptThread != null)
                return -4; //相手がイベント中です
            if (this.Character.Golem != null)
                return -7; //ゴーレムショップ起動中です
            if (client.Character.Golem != null)
                return -8; //相手がゴーレムショップ起動中です
            if (this.Character.PossessionTarget != 0)
                return -9; //憑依中です
            if (client.Character.PossessionTarget != 0)
                return -10; //相手が憑依中です
            if (!client.Character.canTrade)
                return -11; //相手のトレード設定が不許可になっています
            if (this.Character.Buff.FishingState || this.Character.Buff.Dead || this.Character.Buff.Confused || this.Character.Buff.Frosen || this.Character.Buff.Paralysis || this.Character.Buff.Sleep || this.Character.Buff.Stone || this.Character.Buff.Stun
                || client.Character.Buff.Dead || client.Character.Buff.Confused || client.Character.Buff.Frosen || client.Character.Buff.Paralysis || client.Character.Buff.Sleep || client.Character.Buff.Stone || client.Character.Buff.Stun)
                return -12; //トレードを行える状態ではありません
            if (Math.Abs(this.Character.X - client.Character.X) > 300 || Math.Abs(this.Character.Y - client.Character.Y) > 300)
                return -13; //トレード相手との距離が離れすぎています
            return 0;
        }


        public void OnTradeRequestAnswer(Packets.Client.CSMG_TRADE_REQUEST_ANSWER p)
        {
            if (this.tradingTarget == null)
                return;
            if (tradingTarget.MapID != this.Character.MapID)
                return;

            //如果交易的双方有人在凭依, 则交易终止
            if (tradingTarget.PossessionTarget != 0 || this.Character.PossessionTarget != 0)
                return;

            MapClient client = MapClient.FromActorPC(tradingTarget);
            switch (p.Answer)
            {
                case 1:
                    this.trading = true;
                    client.trading = true;

                    this.confirmed = false;
                    this.performed = false;
                    client.confirmed = false;
                    client.performed = false;

                    this.SendTradeStart();
                    this.SendTradeStatus(true, false);
                    client.SendTradeStart();
                    client.SendTradeStatus(true, false);
                    break;
                default:
                    this.tradingTarget = null;
                    client.tradingTarget = null;
                    Packets.Server.SSMG_TRADE_REQUEST_RESULT p1 = new SagaMap.Packets.Server.SSMG_TRADE_REQUEST_RESULT();
                    p1.Result = -6;
                    client.netIO.SendPacket(p1);
                    break;
            }
        }

        List<ItemType> CP10TypeList()
        {
            List<ItemType> list = new List<ItemType>();
            list.Add(ItemType.FURNITURE);

            return list;
        }

        long GetGoldForRecycle(List<uint> tradeItems, List<ushort> tradeCounts)
        {
            List<uint> zero = ZeroPriceList();
            long gold = 0;
            for (int i = 0; i < this.tradeItems.Count; i++)
            {
                Item item = this.Character.Inventory.GetItem(this.tradeItems[i]).Clone();
                if (item.Stack < this.tradeCounts[i])
                {
                    this.tradeCounts[i] = item.Stack;
                    //SendSystemMessage("你试图通过某种方法修改交易数量！你已经被记录于系统，请联系管理员接受处理。");
                    //Character.Account.Banned = true;
                    Logger log = new Logger("玩家异常.txt");
                    string logtext = "\r\n" + Character.Name + "使用了交易，修改了数量：" + this.tradeCounts[i] + "/" + item.Stack;
                    log.WriteLog(logtext);
                }
                item.Stack = this.tradeCounts[i];

                uint g = item.BaseData.price;
                if (g < 5) g = 10;
                if (zero.Contains(item.ItemID))
                    g = 0;
                if (g > 500) g = 500;
                if (item.BaseData.itemType == ItemType.FURNITURE)//家具类
                    g = 2000;
                if (item.EquipSlot.Count >= 1)//装备类
                {
                    g = (uint)(1000 + 1000 * item.EquipSlot.Count);
                    if (g > 5000)
                        g = 5000;
                }
                gold += (g * item.Stack) / 100;
            }
            return gold;
        }

        void OnTradeItemNPC(Packets.Client.CSMG_TRADE_ITEM p)
        {
            if (this.tradeItems != null)
            {
                if (this.tradeItems.Count != 0)
                {
                    this.confirmed = false;
                    this.performed = false;
                    this.SendTradeStatus(true, false);
                }
            }
            this.tradeItems = p.InventoryID;
            this.tradeCounts = p.Count;
            this.tradingGold = p.Gold;


            long gold = GetGoldForRecycle(tradeItems, tradeCounts);
            Packets.Server.SSMG_TRADE_GOLD p3 = new SagaMap.Packets.Server.SSMG_TRADE_GOLD();
            p3.Gold = 0;// gold;
            this.netIO.SendPacket(p3);
            tradingGold = gold;// gold;
        }

        public void OnTradeItem(Packets.Client.CSMG_TRADE_ITEM p)
        {
            if (npcTrade)
            {
                OnTradeItemNPC(p);
                return;
            }
            if (tradingTarget == null)
                return;
            MapClient client = MapClient.FromActorPC(tradingTarget);
            if (this.tradeItems != null)
            {
                if (this.tradeItems.Count != 0)
                {
                    this.confirmed = false;
                    client.confirmed = false;
                    this.performed = false;
                    client.performed = false;
                    this.SendTradeStatus(true, false);
                    client.SendTradeStatus(true, false);
                }
            }
            this.tradeItems = p.InventoryID;
            this.tradeCounts = p.Count;
            this.tradingGold = p.Gold;

            Packets.Server.SSMG_TRADE_ITEM_HEAD p1 = new SagaMap.Packets.Server.SSMG_TRADE_ITEM_HEAD();
            client.netIO.SendPacket(p1);

            for (int i = 0; i < this.tradeItems.Count; i++)
            {
                Packets.Server.SSMG_TRADE_ITEM_INFO p2 = new SagaMap.Packets.Server.SSMG_TRADE_ITEM_INFO();
                Item item = this.Character.Inventory.GetItem(this.tradeItems[i]).Clone();

                if (this.Character.Account.GMLevel < 100)
                {
                    if (item.BaseData.itemType == ItemType.DEMIC_CHIP)
                    {
                        this.tradeItems[i] = 0;
                        this.tradeCounts[i] = 0;
                        continue;
                    }
                }

                //ECOKEY 寵物重製
                if (item.BaseData.itemType == ItemType.PARTNER || item.BaseData.itemType == ItemType.RIDE_PARTNER || item.BaseData.itemType == ItemType.RIDE_PET)
                {
                    MapClient client1 = MapClient.FromActorPC(this.Character);
                    uint Event = 90000530;
                    client1.EventActivate(Event);
                }

                if (item.BaseData.noTrade && TradeConditionFactory.Instance.Items[item.ItemID].GMLevelToIgnoreCondition > this.Character.Account.GMLevel)
                {
                    this.tradeItems[i] = 0;
                    this.tradeCounts[i] = 0;
                    this.SendSystemMessage("無法交易物品[" + item.BaseData.name + "]" + item.ItemID);
                    continue;
                }

                if (item.PossessionOwner != null)
                {
                    if (item.PossessionOwner.CharID != this.chara.CharID)
                    {
                        this.tradeItems[i] = 0;
                        this.tradeCounts[i] = 0;
                        continue;
                    }
                }

             /*   //ECOKEY
                    if (item.BaseData.itemType == ItemType.PARTNER)
                    {
                        MapClient client1 = MapClient.FromActorPC(this.Character);
                        uint Event = 90000530;
                        client1.EventActivate(Event);
                    }*/
              
                if (item.Stack < this.tradeCounts[i])
                {
                    this.tradeCounts[i] = item.Stack;
                    this.tradeItems[i] = 0;
                    this.tradeCounts[i] = 0;
                    this.SendSystemMessage("無法交易物品[" + item.BaseData.name + "]" + item.ItemID);
                    Logger log = new Logger("玩家异常.txt");
                    string logtext = "\r\n" + Character.Name + "交易Cheating，修改了数量：" + this.tradeCounts[i] + " | Server Stack: "+ item.Stack;
                    continue;
                    //ECOKEY寫入交易紀錄
                    Logger log1 = new Logger("玩家交易紀錄.txt");
                    string logtext1 = $"\r\n交易紀錄：玩家1：{this.Character.Name}，玩家2：{tradingTarget.Name}，道具ID：{item.ItemID}，交易金幣：{tradingGold}";
                    log1.WriteLog(logtext1);
                    continue;
                }
                item.Stack = this.tradeCounts[i];
                p2.Item = item;
                p2.InventorySlot = this.tradeItems[i];
                p2.Container = ContainerType.BODY;
                Logger.ShowInfo("嘗試交易道具:" + item.ItemID + "[" + item.BaseData.name + "] " + item.Stack + "個  , 道具欄ID: " + this.tradeItems[i]);
                client.netIO.SendPacket(p2);
            }
            Packets.Server.SSMG_TRADE_GOLD p3 = new SagaMap.Packets.Server.SSMG_TRADE_GOLD();
            p3.Gold = tradingGold;
            client.netIO.SendPacket(p3);
            Packets.Server.SSMG_TRADE_ITEM_FOOT p4 = new SagaMap.Packets.Server.SSMG_TRADE_ITEM_FOOT();
            client.netIO.SendPacket(p4);

            //ECOKEY寫入Gold交易紀錄
            Logger log2 = new Logger("玩家交易金錢紀錄.txt");
            string logtext2 = $"\r\n玩家1：{this.Character.Name}向玩家2：{tradingTarget.Name}交易金幣："+ p.Gold +" ";
            log2.WriteLog(logtext2);
        }

        public void OnTradeConfirm(Packets.Client.CSMG_TRADE_CONFIRM p)
        {
            if (npcTrade)
            {
                switch (p.State)
                {
                    case 0:
                        this.confirmed = false;
                        break;
                    case 1:
                        this.confirmed = true;
                        break;
                }
                if (this.confirmed)
                {
                    this.SendTradeStatus(false, true);
                }
            }
            if (tradingTarget == null)
                return;
            switch (p.State)
            {
                case 0:
                    this.confirmed = false;
                    break;
                case 1:
                    this.confirmed = true;
                    break;
            }
            if (this.confirmed && MapClient.FromActorPC(tradingTarget).confirmed)
            {
                this.SendTradeStatus(false, true);
                MapClient.FromActorPC(tradingTarget).SendTradeStatus(false, true);
            }
        }

        public void OnTradePerform(Packets.Client.CSMG_TRADE_PERFORM p)
        {
            if (this.npcTrade)
            {
                PerformTradeNPC();
                return;
            }
            if (tradingTarget == null)
                return;
            switch (p.State)
            {
                case 0:
                    this.performed = false;
                    break;
                case 1:
                    this.performed = true;
                    break;
            }
            MapClient client = MapClient.FromActorPC(tradingTarget);

            //ECOKEY 阻止複製道具BUG
            if (!client.Character.Online || !Character.Online)
            {
                //this.SendSystemMessage("嘿！！！！你幹什麼東西。what are you doing!?");
                //client.SendSystemMessage("嘿！！！！你幹什麼東西。what are you doing!?");
                if (this.tradeItems != null)
                {
                    for (int i = 0; i < this.tradeItems.Count; i++)
                    {
                        if (this.tradeItems[i] == 0) continue;
                        if (this.Character.Inventory.GetItem(this.tradeItems[i]).PossessionOwner != null || this.Character.Inventory.GetItem(this.tradeItems[i]).PossessionedActor != null)
                            continue;
                        Item item = this.Character.Inventory.GetItem(this.tradeItems[i]).Clone();
                        if (item.BaseData.itemType == ItemType.PARTNER || item.BaseData.itemType == ItemType.RIDE_PET || item.BaseData.itemType == ItemType.RIDE_PARTNER)
                        {
                            ActorPartner partner = MapServer.charDB.GetActorPartner(item.ActorPartnerID, item);
                            ResetPetQualities(partner);
                        }
                        if (item.Stack < this.tradeCounts[i])
                            this.tradeCounts[i] = item.Stack;
                        item.Stack = this.tradeCounts[i];
                        Logger log = new Logger("試圖複製道具.txt");
                        string logtext = "\r\n" + DateTime.Now + "，玩家1：" + Character.Name + "，玩家2：" + client.Character.Name + "，交易：" + item.BaseData.name + "(" + item.ItemID + ")" + "/" + item.Stack;
                        log.WriteLog(logtext);
                    }
                }
                if (client.tradeItems != null)
                {
                    for (int i = 0; i < client.tradeItems.Count; i++)
                    {
                        if (client.tradeItems[i] == 0) continue;
                        if (client.Character.Inventory.GetItem(client.tradeItems[i]).PossessionOwner != null || client.Character.Inventory.GetItem(client.tradeItems[i]).PossessionedActor != null)
                            continue;
                        Item item = client.Character.Inventory.GetItem(client.tradeItems[i]).Clone();
                        if (item.BaseData.itemType == ItemType.PARTNER || item.BaseData.itemType == ItemType.RIDE_PET || item.BaseData.itemType == ItemType.RIDE_PARTNER)
                        {
                            ActorPartner partner = MapServer.charDB.GetActorPartner(item.ActorPartnerID, item);
                            ResetPetQualities(partner);
                        }
                        if (item.Stack < client.tradeCounts[i])
                            client.tradeCounts[i] = item.Stack;
                        item.Stack = client.tradeCounts[i];
                        Logger log = new Logger("試圖複製道具.txt");
                        string logtext = "\r\n" + DateTime.Now + "，玩家1：" + Character.Name + "，玩家2：" + client.Character.Name + "，交易：" + item.BaseData.name + "(" + item.ItemID + ")" + "/" + item.Stack;
                        log.WriteLog(logtext);
                    }
                }
                this.SendTradeEnd(2);
                client.SendTradeEnd(2);
                return;
            }
            //超過100禁止交易
            if (client.tradeItems != null && this.Character.Inventory.Items[ContainerType.BODY].Count + client.tradeItems.Count >= 95)
            {
                Packets.Server.SSMG_TRADE_REQUEST_RESULT p1 = new SagaMap.Packets.Server.SSMG_TRADE_REQUEST_RESULT();
                p1.ID = 2583;
                p1.Result = -6;
                this.netIO.SendPacket(p1);

                this.SendTradeEnd(3);
                client.SendTradeEnd(3);
                return;
            }
            //禁止一次交易超過50個物品
            if (this.tradeItems != null && this.tradeItems.Count >= 50)
            {
                Packets.Server.SSMG_TRADE_REQUEST_RESULT p1 = new SagaMap.Packets.Server.SSMG_TRADE_REQUEST_RESULT();
                p1.ID = 2583;
                p1.Result = -1;
                this.netIO.SendPacket(p1);

                this.SendTradeEnd(3);
                client.SendTradeEnd(3);
                return;
            }

            if (this.performed && client.performed)
            {
                if (this.Character.Gold >= this.tradingGold &&
                    client.Character.Gold >= client.tradingGold &&
                    this.Character.Gold + client.tradingGold <= 999999999999 && //金钱上限为1亿
                    client.Character.Gold + this.tradingGold <= 999999999999)
                {
                    this.SendTradeEnd(2);
                    this.PerformTrade();
                    client.SendTradeEnd(2);
                    client.PerformTrade();
                }
                this.SendTradeEnd(3);
                client.SendTradeEnd(3);
            }
        }

        public void OnTradeCancel(Packets.Client.CSMG_TRADE_CANCEL p)
        {
            if (npcTrade)
            {
                this.npcTradeItem = new List<Item>();
                this.npcTrade = false;
                this.SendTradeEnd(3);
                return;
            }
            if (tradingTarget == null)
                return;

            MapClient.FromActorPC(tradingTarget).SendTradeEnd(3);
            this.SendTradeEnd(3);
        }

        public List<uint> ZeroPriceList()
        {
            List<uint> l = KujiListFactory.Instance.ZeroPriceList;
            return l;
        }

        void PerformTradeNPC()
        {
            this.npcTradeItem = new List<Item>();
            this.SendTradeEnd(2);
            if (this.tradeItems != null)
            {
                for (int i = 0; i < this.tradeItems.Count; i++)
                {
                    Item item = this.Character.Inventory.GetItem(this.tradeItems[i]).Clone();
                    item.Stack = this.tradeCounts[i];
                    Logger.LogItemLost(Logger.EventType.ItemNPCLost, this.Character.Name + "(" + this.Character.CharID + ")", item.BaseData.name + "(" + item.ItemID + ")",
                            string.Format("NPCTrade Count:{0}", item.Stack), false);
                    this.DeleteItem(this.tradeItems[i], this.tradeCounts[i], true);
                    this.npcTradeItem.Add(item);
                }
            }
            //this.Character.Gold -= (int)this.tradingGold;
            if (Character.TInt["垃圾箱记录"] == 1)
            {
                Character.CP += (uint)tradingGold;
                Character.TInt["垃圾箱记录"] = 0;
            }

            //国庆活动
            /*Character.AInt["国庆CP_个人收集"] += (int)tradingGold;
            ScriptManager.Instance.VariableHolder.AInt["国庆CP_全服收集"] += (int)tradingGold;
            ScriptManager.Instance.VariableHolder.Adict["国庆CP_排行榜"]["国庆CP_" + Character.Account.AccountID.ToString()] += (int)tradingGold;
            ScriptManager.Instance.VariableHolder.AStr["国庆CP_" + Character.Account.AccountID.ToString()] = Character.Name;*/

            this.SendGoldUpdate();
            this.performed = true;
            this.npcTrade = false;

            this.SendTradeEnd(1);
        }

        public void PerformTrade()
        {
            if (tradingTarget == null)
                return;

            MapClient client = MapClient.FromActorPC(tradingTarget);

            //凭依状态下交易默认为强制结束?
            if (tradingTarget.PossessionTarget != 0 || this.Character.PossessionTarget != 0)
            {
                this.SendTradeEnd(2);
                client.SendTradeEnd(2);
                return;
            }

            if (this.tradeItems != null)
            {
                for (int i = 0; i < this.tradeItems.Count; i++)
                {
                    if (this.tradeItems[i] == 0)
                        continue;

                    //过滤被凭依的或者自P被捡起来到的道具不允许被直接交易
                    if (this.Character.Inventory.GetItem(this.tradeItems[i]).PossessionOwner != null || this.Character.Inventory.GetItem(this.tradeItems[i]).PossessionedActor != null)
                        continue;

                    Item item = this.Character.Inventory.GetItem(this.tradeItems[i]).Clone();

                    //ECOKEY 寵物重製
                    if (item.BaseData.itemType == ItemType.PARTNER || item.BaseData.itemType == ItemType.RIDE_PET || item.BaseData.itemType == ItemType.RIDE_PARTNER)
                    {
                        ActorPartner partner = MapServer.charDB.GetActorPartner(item.ActorPartnerID, item);
                        ResetPetQualities(partner);
                    }

                    if (item.Stack < this.tradeCounts[i])
                        this.tradeCounts[i] = item.Stack;
                    
                    item.Stack = this.tradeCounts[i];

                    Logger.LogItemLost(Logger.EventType.ItemTradeLost, this.Character.Name + "(" + this.Character.CharID + ")", item.BaseData.name + "(" + item.ItemID + ")",
                            string.Format("Trade Count:{0} To:{1}({2})", tradeCounts[i], client.Character.Name, client.Character.CharID), false);
                    this.DeleteItem(this.tradeItems[i], this.tradeCounts[i], true);
                    Logger.LogItemGet(Logger.EventType.ItemTradeGet, client.Character.Name + "(" + client.Character.CharID + ")", item.BaseData.name + "(" + item.ItemID + ")",
                        string.Format("Trade Count:{0} From:{1}({2})", item.Stack, this.Character.Name, this.Character.CharID), false);
                    client.AddItem(item, true);
                }
            }
            this.Character.Gold -= this.tradingGold;
            this.SendGoldUpdate();
            client.Character.Gold += this.tradingGold;
            client.SendGoldUpdate();
        }

        /// <summary>
        /// 结束交易
        /// </summary>
        /// <param name="type">1，清空变量，2，发送结束封包，3，两个都执行</param>
        public void SendTradeEnd(int type)
        {
            if (type == 1 || type == 3)
            {
                this.tradeCounts = null;
                this.tradeItems = null;
                this.trading = false;
                this.tradingGold = 0;
                this.tradingTarget = null;
                this.confirmed = false;
                this.performed = false;
            }
            if (type == 2 || type == 3)
            {
                Packets.Server.SSMG_TRADE_END p = new SagaMap.Packets.Server.SSMG_TRADE_END();
                this.netIO.SendPacket(p);
            }
        }

        public void SendTradeStatus(bool canConfirm, bool canPerform)
        {
            Packets.Server.SSMG_TRADE_STATUS p = new SagaMap.Packets.Server.SSMG_TRADE_STATUS();
            p.Confirm = canConfirm;
            p.Perform = canPerform;
            this.netIO.SendPacket(p);
        }

        public void SendTradeStart()
        {
            if (this.tradingTarget == null)
                return;
            Packets.Server.SSMG_TRADE_START p = new SagaMap.Packets.Server.SSMG_TRADE_START();
            p.SetPara(this.tradingTarget.Name, 0);
            this.netIO.SendPacket(p);
        }

        public void SendTradeStartNPC(string name)
        {
            if (this.npcTrade)
            {
                Packets.Server.SSMG_TRADE_START p = new SagaMap.Packets.Server.SSMG_TRADE_START();
                p.SetPara(name, 1);
                this.netIO.SendPacket(p);
                SendTradeStatus(true, false);
            }
        }

        public void SendTradeRequest(ActorPC pc)
        {
            Packets.Server.SSMG_TRADE_REQUEST p = new SagaMap.Packets.Server.SSMG_TRADE_REQUEST();
            p.Name = pc.Name;
            this.netIO.SendPacket(p);

            this.tradingTarget = pc;
        }
    }
}
