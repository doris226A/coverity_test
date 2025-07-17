using SagaDB.Actor;
using SagaDB.Item;
using SagaLib;
using SagaMap.Manager;
using System;
using System.Collections.Generic;

namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        #region 商人商店
        //an添加 (MarkChat)
        public void OnPlayerShopOpen(Packets.Client.CSMG_PLAYER_SHOP_OPEN p)
        {
            Actor actor = this.map.GetActor(p.ActorID);//mark3
            ActorPC pc = (ActorPC)actor;

            if (pc.Fictitious)
            {
                int EventID = pc.TInt["虚拟玩家EventID"];
                EventActivate((uint)EventID);
                Character.TInt["触发的虚拟玩家ID"] = (int)pc.ActorID;
            }
            else
            {
                MapClient client = MapClient.FromActorPC(pc);
                string shopOpenMessage = LocalManager.Instance.Strings.SHOP_OPEN ?? "Shop is open for {0}";
                client.SendSystemMessage(string.Format(shopOpenMessage, this.Character.Name));
                // client.SendSystemMessage(string.Format(LocalManager.Instance.Strings.SHOP_OPEN, this.Character.Name));
                Packets.Server.SSMG_PLAYER_SHOP_HEADER2 p1 = new SagaMap.Packets.Server.SSMG_PLAYER_SHOP_HEADER2();
                p1.ActorID = p.ActorID;
                this.netIO.SendPacket(p1);
                Packets.Server.SSMG_PLAYER_SHOP_HEADER p2 = new SagaMap.Packets.Server.SSMG_PLAYER_SHOP_HEADER();
                p2.ActorID = p.ActorID;
                this.netIO.SendPacket(p2);

                Logger logger = new Logger("OpenShop.txt");

                foreach (uint i in pc.Playershoplist.Keys)
                {
                    Item item = pc.Inventory.GetItem(i);
                    if (item == null)
                        continue;
                    Packets.Server.SSMG_PLAYER_SHOP_ITEM p3 = new SagaMap.Packets.Server.SSMG_PLAYER_SHOP_ITEM();
                    p3.InventorySlot = i;
                    p3.Container = ContainerType.BODY;
                    p3.Price = pc.Playershoplist[i].Price;
                    p3.ShopCount = pc.Playershoplist[i].Count;
                    p3.Item = item;
                    this.netIO.SendPacket(p3);

                    logger.WriteLog(p3.DumpData());
                }
                Packets.Server.SSMG_PLAYER_SHOP_FOOTER p4 = new SagaMap.Packets.Server.SSMG_PLAYER_SHOP_FOOTER();
                this.netIO.SendPacket(p4);

            }
        }
        public void OnPlayerSetShopSetup(Packets.Client.CSMG_PLAYER_SETSHOP_SETUP p)//mark11
        {
            Logger logger = new Logger("SetupShop.txt");

            if (p.Comment.Length < 1)
            {
                SendSystemMessage("請輸入商店描述");
                return;
            }

            if (this.Character.PossessionTarget != 0)
            {
                SendSystemMessage("正處於憑依狀態的玩家無法開店");
                return;
            }

            uint[] ids = p.InventoryIDs;
            ushort[] counts = p.Counts;
            ulong[] prices = p.Prices;
            this.Shoptitle = p.Comment;
            try
            {
                string logs = "Player(" + this.Character.CharID + ") SETUP Player Shop: " + p.Comment + "\n";

                if (ids.Length != 0)
                {
                    //if (this.Character.Playershoplist != null)
                    this.Character.Playershoplist.Clear();

                    for (int i = 0; i < ids.Length; i++)
                    {
                        if (!this.Character.Playershoplist.ContainsKey(ids[i]))
                        {
                            PlayerShopItem item = new PlayerShopItem();
                            item.InventoryID = ids[i];
                            item.ItemID = this.Character.Inventory.GetItem(ids[i]).ItemID;
                            //if(ItemFactory.Instance.GetItem(item.ItemID).BaseData.itemType != ItemType.FACECHANGE
                            //&& item.ItemID != 950000005 && item.ItemID != 100000000 && item.ItemID != 110128500 && item.ItemID != 110132000 && item.ItemID != 110165300)

                            Item item2 = Character.Inventory.GetItem(ids[i]);
                            //ECOKEY 重製寵物
                            if (item2.BaseData.itemType == ItemType.PARTNER || item2.BaseData.itemType == ItemType.RIDE_PARTNER)
                            {
                                ActorPartner partner = MapServer.charDB.GetActorPartner(item2.ActorPartnerID, item2);
                                ResetPetQualities(partner);
                            }
                            if (item2.PossessionedActor != null)
                            {
                                SendSystemMessage("" + item2.BaseData.name + "正被玩家 " + item2.PossessionedActor.Name + " 憑依, 無法交易");
                                continue;
                            }

                            if (item2.PossessionOwner != null)
                            {
                                SendSystemMessage("" + item2.BaseData.name + "正被玩家 " + item2.PossessionOwner.Name + " 憑依, 無法交易");
                                continue;
                            }

                            if (item2.BaseData.events != 0)
                            {
                                SendSystemMessage("" + item2.BaseData.name + "為無法交易的物品");
                                continue;
                            }

                            if (item2.BaseData.noVending && TradeConditionFactory.Instance.Items[item2.ItemID].GMLevelToIgnoreCondition > this.Character.Account.GMLevel)
                            {
                                SendSystemMessage("" + item2.BaseData.name + "為無法上架的物品");
                                continue;
                            }

                            if (counts[i] > item2.Stack)
                                counts[i] = item2.Stack;

                            logs += "Item: " + item2.BaseData.name + " (" + item2.BaseData.id + ")\nQTY: " + counts[i] + "Price: " + prices[i] + "\n";
                            Character.Playershoplist.Add(ids[i], item);

                        }
                        if (counts[i] == 0)
                        {
                            this.Character.Playershoplist.Remove(ids[i]);
                        }
                        else
                        {
                            this.Character.Playershoplist[ids[i]].Count = counts[i];
                            this.Character.Playershoplist[ids[i]].Price = prices[i];
                            SendShopGoodInfo(ids[i], counts[i], prices[i]);
                        }
                    }
                }
                else
                {
                    this.Character.Playershoplist.Clear();
                }

                logger.WriteLog(logs);

            }
            catch (Exception ex1)
            {
                Logger.ShowError(ex1);
            }
            Packets.Server.SSMG_PLAYER_SHOP_APPEAR p1 = new SagaMap.Packets.Server.SSMG_PLAYER_SHOP_APPEAR();
            p1.ActorID = this.Character.ActorID;
            p1.Title = this.Shoptitle;
            if (ids.Length != 0 && this.Shoptitle != "" && Character.Playershoplist.Count > 0)
            {
                this.Shopswitch = 1;
                p1.button = 1;
            }
            else
            {
                this.Shopswitch = 0;
                this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.PLAYERSHOP_CHANGE_CLOSE, null, this.Character, true);
                p1.button = 0;
            }
            this.netIO.SendPacket(p1);
        }
        public void SendShopGoodInfo(uint slotid, ushort count, ulong gold)
        {
            Packets.Server.SSMG_PLAYER_SHOP_GOLD_UPDATA p = new Packets.Server.SSMG_PLAYER_SHOP_GOLD_UPDATA();
            p.SlotID = slotid;
            p.Count = count;
            p.gold = gold;
            netIO.SendPacket(p);
        }
        public void OnPlayerShopSellBuy(Packets.Client.CSMG_PLAYER_SHOP_SELL_BUY p)
        {
            Actor actor = this.map.GetActor(p.ActorID);
            Dictionary<uint, ushort> items = p.Items;
            Packets.Server.SSMG_PLAYER_SHOP_ANSWER p1 = new SagaMap.Packets.Server.SSMG_PLAYER_SHOP_ANSWER();

            if (actor.type == ActorType.PC)
            {
                ActorPC pc = (ActorPC)actor;
                MapClient client = FromActorPC(pc);
                long gold = 0;

                if (client.shopswitch == 0)
                {
                    p1.Result = -4;
                    this.netIO.SendPacket(p1);
                    return;
                }

                foreach (uint i in items.Keys)
                {
                    Item item = pc.Inventory.GetItem(i);

                    if (item == null)
                    {
                        p1.Result = -4;
                        this.netIO.SendPacket(p1);
                        return;
                    }
                    if (item.ItemID == 950000006 || item.ItemID == 950000007)
                    {
                        p1.Result = -1;
                        this.netIO.SendPacket(p1);
                        return;
                    }
                    if (items[i] == 0)
                    {
                        p1.Result = -7;
                        this.netIO.SendPacket(p1);
                        return;
                    }
                    /*if (item.IsEquipt)
                    {
                        p1.Result = -4;
                        this.netIO.SendPacket(p1);
                        return;
                    }*/
                    if (item.Stack >= items[i])
                    {
                        gold += (long)(pc.Playershoplist[i].Price * items[i]);
                        long singleprice = (long)(pc.Playershoplist[i].Price * items[i]);
                        if (this.Character.Gold < gold)
                        {
                            p1.Result = -7;
                            this.netIO.SendPacket(p1);
                            return;
                        }
                        if (gold + pc.Gold >= 999999999999)
                        {
                            p1.Result = -10;
                            this.netIO.SendPacket(p1);
                            return;
                        }

                        Item newItem = item.Clone();
                        newItem.Stack = items[i];
                        if (newItem.Stack > 0)
                        {
                            Logger.LogItemLost(Logger.EventType.ItemGolemLost, this.Character.Name + "(" + this.Character.CharID + ")", newItem.BaseData.name + "(" + newItem.ItemID + ")",
                                string.Format("GolemSell Count:{0}", items[i]), false);
                        }
                        InventoryDeleteResult result = pc.Inventory.DeleteItem(i, items[i]);
                        pc.Playershoplist[i].Count -= items[i];

                        SendShopGoodInfo(i, pc.Playershoplist[i].Count, pc.Playershoplist[i].Price);

                        if (pc.Playershoplist[i].Count == 0)
                            pc.Playershoplist.Remove(i);
                        //返回卖家info
                        switch (result)
                        {
                            case InventoryDeleteResult.STACK_UPDATED:
                                Packets.Server.SSMG_ITEM_COUNT_UPDATE p2 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                                p2.InventorySlot = item.Slot;
                                p2.Stack = item.Stack;
                                client.netIO.SendPacket(p2);
                                break;
                            case InventoryDeleteResult.ALL_DELETED:
                                Packets.Server.SSMG_ITEM_DELETE p3 = new SagaMap.Packets.Server.SSMG_ITEM_DELETE();
                                p3.InventorySlot = item.Slot;
                                client.netIO.SendPacket(p3);
                                break;
                        }


                        client.Character.Inventory.CalcPayloadVolume();
                        client.SendCapacity();
                        client.SendSystemMessage("玩家: " + this.Character.Name + " 向您購買了" + newItem.Stack + " 個[" + newItem.BaseData.name + "]，售價：" + singleprice.ToString() + "G");
                        client.SendSystemMessage(string.Format(LocalManager.Instance.Strings.ITEM_DELETED, item.BaseData.name, items[i]));
                        Logger.LogItemGet(Logger.EventType.ItemGolemGet, this.Character.Name + "(" + this.Character.CharID + ")", item.BaseData.name + "(" + item.ItemID + ")",
                        string.Format("GolemBuy Count:{0}", item.Stack), false);
                        this.SendSystemMessage("給玩家: " + client.Character.Name + " 購買了" + newItem.Stack + " 個[" + newItem.BaseData.name + "]，花費：" + singleprice.ToString() + " G");
                        if (newItem.BaseData.itemType == ItemType.PARTNER)
                            newItem.ActorPartnerID = 0;
                        AddItem(newItem, true);


                        Logger log = new Logger("玩家交易記錄.txt");
                        string text = "\r\n玩家: " + Character.Name + " 向玩家：" + client.Character.Name + " 購買了" + newItem.Stack + " 個[" + newItem.BaseData.name + "] ，花費：" + singleprice.ToString() + "G";
                        text += "\r\n買方IP/MAC：" + Character.Account.LastIP + "/" + Character.Account.MacAddress + " 賣家IP/MAC：" + client.Character.Account.LastIP + "/ " + client.Character.Account.MacAddress;
                        if (newItem.Refine > 10)
                            text += "\r\n裝備道具：" + newItem.BaseData.name + " 強化次數" + newItem.Refine;
                        log.WriteLog(text);


                        if (Character.Account.MacAddress == client.Character.Account.MacAddress || Character.Account.LastIP == client.Character.Account.LastIP)
                        {
                            Logger log2 = new Logger("同IP或MAC的玩家交易記錄.txt");
                            string text2 = "\r\n玩家: " + Character.Name + " 向玩家：" + client.Character.Name + " 購買了" + newItem.Stack + " 個[" + newItem.BaseData.name + "] ，花費：" + singleprice.ToString() + "G";
                            text2 += "\r\n買方IP/MAC：" + Character.Account.LastIP + "/" + Character.Account.MacAddress + " 賣家IP/MAC：" + client.Character.Account.LastIP + "/ " + client.Character.Account.MacAddress;
                            if (newItem.Refine > 10)
                                text2 += "\r\n裝備道具：" + newItem.BaseData.name + " 強化次數" + newItem.Refine;
                            log2.WriteLog(text2);
                        }

                    }
                    else
                    {
                        p1.Result = -7;
                        this.netIO.SendPacket(p1);
                        return;
                    }
                }
                Character.Gold -= gold;
                pc.Gold += gold;

                if (pc.Playershoplist.Count == 0)
                {
                    pc.Fictitious = false;
                    client.Shopswitch = 0;
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, pc, true);
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.PLAYERSHOP_CHANGE_CLOSE, null, this.Character, true);
                }
            }
        }
        public void OnPlayerShopBuyClose(Packets.Client.CSMG_PLAYER_SETSHOP_CLOSE p)
        {
            //this.Shopswitch = 0;
            //this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.PLAYERSHOP_CHANGE_CLOSE, null, this.Character, true);
            //Packets.Server.SSMG_PLAYER_SETSHOP_SET p1 = new SagaMap.Packets.Server.SSMG_PLAYER_SETSHOP_SET();
            //this.netIO.SendPacket(p1);

            Packets.Server.SSMG_PLAYER_SHOP_CLOSE p2 = new Packets.Server.SSMG_PLAYER_SHOP_CLOSE();
            this.netIO.SendPacket(p2);
        }
        public void OnPlayerShopChangeClose(Packets.Client.CSMG_PLAYER_SETSHOP_OPEN p)
        {
            Character.Playershoplist.Clear();
            Character.Fictitious = false;
            this.Shopswitch = 0;
            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.PLAYERSHOP_CHANGE_CLOSE, null, this.Character, true);
        }
        public void OnPlayerShopChange(Packets.Client.CSMG_PLAYER_SETSHOP_SETUP p)
        {
            this.Shoptitle = p.Comment;
            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.PLAYERSHOP_CHANGE, null, this.Character, true);
            if (this.Shopswitch == 0 && this.Shoptitle == "")
            {
                this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.PLAYERSHOP_CHANGE_CLOSE, null, this.Character, true);
            }
        }
        public void OnPlayerSetShop(Packets.Client.CSMG_PLAYER_SETSHOP_OPEN p)
        {

            Packets.Server.SSMG_PLAYER_SETSHOP_OPEN_SETUP p1 = new SagaMap.Packets.Server.SSMG_PLAYER_SETSHOP_OPEN_SETUP();
            Packets.Server.SSMG_PLAYER_SHOP_APPEAR p2 = new Packets.Server.SSMG_PLAYER_SHOP_APPEAR();
            p2.ActorID = this.Character.ActorID;
            p2.Title = this.Shoptitle;
            this.Shopswitch = 1;
            p2.button = 0;
            p1.Comment = this.Shoptitle;
            this.netIO.SendPacket(p1);
            this.netIO.SendPacket(p2);
            //ECOKEY 寵物重製提示
            MapClient client1 = MapClient.FromActorPC(this.Character);
            uint Event = 90000530;
            client1.EventActivate(Event);
        }
        #endregion

    }
}
