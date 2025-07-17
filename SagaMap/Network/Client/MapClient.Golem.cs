using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using SagaDB;
using SagaDB.Item;
using SagaDB.Actor;
using SagaDB.FGarden;
using SagaLib;
using SagaMap;
using SagaMap.Manager;


namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        public void OnGolemWarehouse(Packets.Client.CSMG_GOLEM_WAREHOUSE p)
        {
            Packets.Server.SSMG_GOLEM_WAREHOUSE p1 = new SagaMap.Packets.Server.SSMG_GOLEM_WAREHOUSE();
            p1.ActorID = this.Character.ActorID;
            p1.Title = this.Character.Golem.Title;
            this.netIO.SendPacket(p1);

            foreach (Item i in this.Character.Inventory.GetContainer(ContainerType.GOLEMWAREHOUSE))
            {
                Packets.Server.SSMG_GOLEM_WAREHOUSE_ITEM p2 = new SagaMap.Packets.Server.SSMG_GOLEM_WAREHOUSE_ITEM();
                p2.InventorySlot = i.Slot;
                p2.Container = ContainerType.GOLEMWAREHOUSE;
                p2.Item = i;
                this.netIO.SendPacket(p2);
            }

            Packets.Server.SSMG_GOLEM_WAREHOUSE_ITEM_FOOTER p3 = new SagaMap.Packets.Server.SSMG_GOLEM_WAREHOUSE_ITEM_FOOTER();
            this.netIO.SendPacket(p3);

        }

        public void OnGolemWarehouseSet(Packets.Client.CSMG_GOLEM_WAREHOUSE_SET p)
        {
            if (this.Character.Golem != null)
                this.Character.Golem.Title = p.Title;
        }

        public void OnGolemWarehouseGet(Packets.Client.CSMG_GOLEM_WAREHOUSE_GET p)
        {
            Item item = this.Character.Inventory.GetItem(p.InventoryID);
            if (item != null)
            {
                ushort count = p.Count;
                if (item.Stack >= count)
                {
                    Item newItem = item.Clone();
                    newItem.Stack = count;
                    if (newItem.Stack > 0)
                    {
                        Logger.LogItemLost(Logger.EventType.ItemGolemLost, this.Character.Name + "(" + this.Character.CharID + ")", newItem.BaseData.name + "(" + newItem.ItemID + ")",
                            string.Format("GolemWarehouseGet Count:{0}", count), false);
                    }

                    this.Character.Inventory.DeleteItem(p.InventoryID, count);
                    Logger.LogItemGet(Logger.EventType.ItemGolemGet, this.Character.Name + "(" + this.Character.CharID + ")", item.BaseData.name + "(" + item.ItemID + ")",
                    string.Format("GolemWarehouse Count:{0}", item.Stack), false);
                    AddItem(newItem, false);
                    Packets.Server.SSMG_GOLEM_WAREHOUSE_GET p1 = new SagaMap.Packets.Server.SSMG_GOLEM_WAREHOUSE_GET();
                    this.netIO.SendPacket(p1);
                }
            }
        }

        public void OnGolemShopBuySell(Packets.Client.CSMG_GOLEM_SHOP_BUY_SELL p)
        {
            Actor actor = this.map.GetActor(p.ActorID);
            Dictionary<uint, ushort> items = p.Items;
            if (actor.type == ActorType.GOLEM)
            {
                ActorGolem golem = (ActorGolem)actor;
                ulong gold = 0;
                foreach (uint i in items.Keys)
                {
                    Item item = this.Character.Inventory.GetItem(i);
                    if (item == null)
                        continue;
                    if (items[i] == 0)
                        continue;

                    //尝试过滤被凭依或自P的道具不允许通过石像被别人收购
                    if (item.PossessionedActor != null || item.PossessionOwner != null)
                        continue;

                    Item newItem = item.Clone();
                    if (item.Stack >= items[i])
                    {
                        uint inventoryID = 0;
                        foreach (uint j in golem.BuyShop.Keys)
                        {
                            if (golem.BuyShop[j].ItemID == newItem.ItemID)
                            {
                                inventoryID = j;
                                break;
                            }
                        }
                        gold += (golem.BuyShop[inventoryID].Price * items[i]);
                        if (golem.BuyLimit < gold)
                        {
                            gold -= (golem.BuyShop[inventoryID].Price * items[i]);
                            break;
                        }
                        newItem.Stack = items[i];
                        Logger.LogItemLost(Logger.EventType.ItemGolemLost, this.Character.Name + "(" + this.Character.CharID + ")", item.BaseData.name + "(" + item.ItemID + ")",
                            string.Format("GolemSell Count:{0}", items[i]), false);
                        DeleteItem(i, items[i], true);
                        golem.BuyShop[inventoryID].Count -= items[i];

                        if (golem.BoughtItem.ContainsKey(item.ItemID))
                        {
                            golem.BoughtItem[item.ItemID].Count += items[i];
                        }
                        else
                        {
                            golem.BoughtItem.Add(item.ItemID, new GolemShopItem());
                            golem.BoughtItem[item.ItemID].Price = golem.BuyShop[inventoryID].Price;
                            golem.BoughtItem[item.ItemID].Count += items[i];
                        }
                        if (newItem.Stack > 0)
                        {
                            Logger.LogItemGet(Logger.EventType.ItemGolemGet, this.Character.Name + "(" + this.Character.CharID + ")", newItem.BaseData.name + "(" + newItem.ItemID + ")",
                                string.Format("GolemBuy Count:{0}", newItem.Stack), false);
                        }
                        golem.Owner.Inventory.AddItem(ContainerType.BODY, newItem);
                    }
                }
                golem.BuyLimit -= (uint)gold;
                this.Character.Gold += (int)gold;
                golem.Owner.Gold -= (int)gold;

                //TODO: 当石像收购交易完成后, 记录交易产生的事务和玩家信息 kk impl
                //保存买家的信息
                MapServer.charDB.SaveChar(golem.Owner, false);
                MapServer.charDB.SaveItem(golem.Owner);
                //保存卖家的信息
                MapServer.charDB.SaveChar(this.chara, false);
                MapServer.charDB.SaveItem(this.chara);
                //保存石像相关事务
                MapServer.charDB.SaveGolemItem(golem);
                MapServer.charDB.SaveGolemTransactions(golem);
            }
        }

        public void OnGolemShopSellBuy(Packets.Client.CSMG_GOLEM_SHOP_SELL_BUY p)
        {
            Actor actor = this.map.GetActor(p.ActorID);
            Dictionary<uint, ushort> items = p.Items;
            Packets.Server.SSMG_GOLEM_SHOP_SELL_ANSWER p1 = new SagaMap.Packets.Server.SSMG_GOLEM_SHOP_SELL_ANSWER();
            if (actor.type == ActorType.GOLEM)
            {
                ActorGolem golem = (ActorGolem)actor;
                ulong gold = 0;
                foreach (uint i in items.Keys)
                {
                    Item item = golem.Owner.Inventory.GetItem(i);

                    //尝试过滤被凭依或自P的道具无法通过石像贩卖
                    if (item.PossessionOwner != null || item.PossessionedActor != null)
                    {
                        p1.Result = -4;
                        this.netIO.SendPacket(p1);
                        return;
                    }

                    if (item == null)
                    {
                        p1.Result = -4;
                        this.netIO.SendPacket(p1);
                        return;
                    }
                    if (items[i] == 0)
                    {
                        p1.Result = -2;
                        this.netIO.SendPacket(p1);
                        return;
                    }
                    if (item.BaseData.noGolemSellShop && TradeConditionFactory.Instance.Items[item.ItemID].GMLevelToIgnoreCondition > this.Character.Account.GMLevel)
                    {
                        p1.Result = -1;
                        this.netIO.SendPacket(p1);
                        return;
                    }
                    if (item.Stack >= items[i])
                    {
                        gold += (golem.SellShop[i].Price * items[i]);
                        if (this.Character.Gold < (long)gold)
                        {
                            p1.Result = -7;
                            this.netIO.SendPacket(p1);
                            return;
                        }
                        if ((long)gold + golem.Owner.Gold >= 999999999999)
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

                        golem.SellShop[i].Count -= items[i];
                        if (golem.SoldItem.ContainsKey(item.ItemID))
                        {
                            golem.SoldItem[item.ItemID].Count += items[i];
                        }
                        else
                        {
                            golem.SoldItem.Add(item.ItemID, new GolemShopItem());
                            golem.SoldItem[item.ItemID].Price = golem.SellShop[i].Price;
                            golem.SoldItem[item.ItemID].Count += items[i];
                        }
                        if (golem.SellShop[i].Count == 0)
                        {
                            golem.SellShop.Remove(i);
                        }

                        if (golem.SellShop.Count == 0)
                        {
                            golem.invisble = true;
                            this.map.OnActorVisibilityChange(golem);
                        }
                        Logger.LogItemGet(Logger.EventType.ItemGolemGet, this.Character.Name + "(" + this.Character.CharID + ")", item.BaseData.name + "(" + item.ItemID + ")",
                        string.Format("GolemBuy Count:{0}", item.Stack), false);
                        AddItem(newItem, true);


                        //結算石象主人的物品
                        this.SendSystemMessage(string.Format(LocalManager.Instance.Strings.GOLEM_SHOP_BUY_ITEM_MESSAGE, golem.Owner.Name, items[i], item.BaseData.name));
                        golem.Owner.Inventory.DeleteItem(item.Slot, items[i]);

                      
                    }
                    else
                    {
                        p1.Result = -5;
                        this.netIO.SendPacket(p1);
                        return;
                    }
                }
                golem.Owner.Gold += (int)gold;
                this.Character.Gold -= (int)gold;


                //TODO: 当石像贩售交易完成后, 记录交易产生的事务和玩家信息 kk impl
                //保存卖家的信息
                MapServer.charDB.SaveChar(golem.Owner, false);
                MapServer.charDB.SaveItem(golem.Owner);
                //保存买家的信息
                MapServer.charDB.SaveChar(this.chara, false);
                MapServer.charDB.SaveItem(chara);
                //保存石像事务
                MapServer.charDB.SaveGolemItem(golem);
                MapServer.charDB.SaveGolemTransactions(golem);
            }
        }

        public void OnGolemShopOpen(Packets.Client.CSMG_GOLEM_SHOP_OPEN p)
        {
            Actor actor = this.map.GetActor(p.ActorID);
            if (actor.type == ActorType.GOLEM)
            {
                ActorGolem golem = (ActorGolem)actor;

                if (golem.GolemType == GolemType.Sell)
                {
                    Packets.Server.SSMG_GOLEM_SHOP_OPEN_OK p1 = new SagaMap.Packets.Server.SSMG_GOLEM_SHOP_OPEN_OK();
                    p1.ActorID = p.ActorID;
                    this.netIO.SendPacket(p1);
                    Packets.Server.SSMG_GOLEM_SHOP_HEADER p2 = new SagaMap.Packets.Server.SSMG_GOLEM_SHOP_HEADER();
                    p2.ActorID = p.ActorID;
                    this.netIO.SendPacket(p2);


                    foreach (uint i in golem.SellShop.Keys)
                    {
                        Item item = golem.Owner.Inventory.GetItem(i);
                        if (item == null)
                            continue;
                        Packets.Server.SSMG_GOLEM_SHOP_ITEM p3 = new SagaMap.Packets.Server.SSMG_GOLEM_SHOP_ITEM();
                        p3.InventorySlot = i;
                        p3.Container = ContainerType.BODY;
                        p3.Price = golem.SellShop[i].Price;
                        p3.ShopCount = golem.SellShop[i].Count;
                        p3.Item = item;
                        this.netIO.SendPacket(p3);


                    }
                    Packets.Server.SSMG_GOLEM_SHOP_FOOTER p4 = new SagaMap.Packets.Server.SSMG_GOLEM_SHOP_FOOTER();
                    this.netIO.SendPacket(p4);
                }
                if (golem.GolemType == GolemType.Buy)
                {
                    Packets.Server.SSMG_GOLEM_SHOP_BUY_HEADER p2 = new SagaMap.Packets.Server.SSMG_GOLEM_SHOP_BUY_HEADER();
                    p2.ActorID = p.ActorID;
                    this.netIO.SendPacket(p2);

                    Packets.Server.SSMG_GOLEM_SHOP_BUY_ITEM p3 = new SagaMap.Packets.Server.SSMG_GOLEM_SHOP_BUY_ITEM();
                    p3.Items = golem.BuyShop;
                    p3.BuyLimit = golem.BuyLimit;
                    this.netIO.SendPacket(p3);
                }
            }
        }

        public void OnGolemShopSellClose(Packets.Client.CSMG_GOLEM_SHOP_SELL_CLOSE p)
        {
            Packets.Server.SSMG_GOLEM_SHOP_SELL_SET p1 = new SagaMap.Packets.Server.SSMG_GOLEM_SHOP_SELL_SET();
            this.netIO.SendPacket(p1);
        }

        public void OnGolemShopSellSetup(Packets.Client.CSMG_GOLEM_SHOP_SELL_SETUP p)
        {
            uint[] ids = p.InventoryIDs;
            ushort[] counts = p.Counts;
            ulong[] prices = p.Prices;

            this.Character.Golem.SellShop.Clear();

            if (ids.Length != 0)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    if (!this.Character.Golem.SellShop.ContainsKey(ids[i]))
                    {
                        GolemShopItem item = new GolemShopItem();
                        item.InventoryID = ids[i];
                        item.ItemID = this.Character.Inventory.GetItem(ids[i]).ItemID;


                        Item item2 = Character.Inventory.GetItem(ids[i]);

                        if (item2.PossessionedActor != null || item2.PossessionOwner != null)
                        {
                            SendSystemMessage("" + item2.BaseData.name + "正被凭依, 无法上架");
                            continue;
                        }

                        if (item2.BaseData.events != 0)
                        {
                            SendSystemMessage("" + item2.BaseData.name + "為無法交易的物品");
                            continue;
                        }

                        if (item2.BaseData.noGolemSellShop && TradeConditionFactory.Instance.Items[item.ItemID].GMLevelToIgnoreCondition > this.Character.Account.GMLevel)
                        {
                            this.SendSystemMessage("" + item2.BaseData.name + "无法上架");
                            continue;
                        }
                        //ECOKEY 重製寵物
                        if (item2.BaseData.itemType == ItemType.PARTNER || item2.BaseData.itemType == ItemType.RIDE_PARTNER)
                        {
                            ActorPartner partner = MapServer.charDB.GetActorPartner(item2.ActorPartnerID, item2);
                            ResetPetQualities(partner);
                        }
                        this.Character.Golem.SellShop.Add(ids[i], item);
                    }
                    if (counts[i] == 0)
                        this.Character.Golem.SellShop.Remove(ids[i]);
                    else
                    {
                        this.Character.Golem.SellShop[ids[i]].Count = counts[i];
                        this.Character.Golem.SellShop[ids[i]].Price = prices[i];
                    }
                }
            }
            this.Character.Golem.Title = p.Comment;
            this.Character.Golem.Name = this.Character.Name;

            //扣減耐用度
           // if (this.Character.Golem.Item.Durability != 0) this.Character.Golem.Item.Durability--;
        }
         
        public void OnGolemShopSell(Packets.Client.CSMG_GOLEM_SHOP_SELL p)
        {
            Packets.Server.SSMG_GOLEM_SHOP_SELL_SETUP p1 = new SagaMap.Packets.Server.SSMG_GOLEM_SHOP_SELL_SETUP();
            p1.Comment = this.Character.Golem.Title;
            p1.Unknown = 0;
            p1.IsAddToGolemCatalog = 1;
            p1.GolemShopType = 0;
            this.netIO.SendPacket(p1);
            //ECOKEY 寵物重製提示
            MapClient client1 = MapClient.FromActorPC(this.Character);
            uint Event = 90000530;
            client1.EventActivate(Event);
        }

        public void OnGolemShopBuyClose(Packets.Client.CSMG_GOLEM_SHOP_BUY_CLOSE p)
        {
            Packets.Server.SSMG_GOLEM_SHOP_BUY_SET p1 = new SagaMap.Packets.Server.SSMG_GOLEM_SHOP_BUY_SET();
            this.netIO.SendPacket(p1);
        }

        public void OnGolemShopBuySetup(Packets.Client.CSMG_GOLEM_SHOP_BUY_SETUP p)
        {
            uint[] ids = p.InventoryIDs;
            ushort[] counts = p.Counts;
            ulong[] prices = p.Prices;
            if (ids.Length != 0)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    if (!this.Character.Golem.BuyShop.ContainsKey(ids[i]))
                    {
                        GolemShopItem item = new GolemShopItem();
                        item.InventoryID = ids[i];
                        item.ItemID = this.Character.Inventory.GetItem(ids[i]).ItemID;
                        this.Character.Golem.BuyShop.Add(ids[i], item);
                    }
                    if (counts[i] == 0)
                        this.Character.Golem.BuyShop.Remove(ids[i]);
                    else
                    {
                        this.Character.Golem.BuyShop[ids[i]].Count = counts[i];
                        this.Character.Golem.BuyShop[ids[i]].Price = prices[i];
                    }
                }
            }
            else
            {
                this.Character.Golem = null;
            }

            this.Character.Golem.BuyLimit = p.BuyLimit;
            this.Character.Golem.Title = p.Comment;
            this.Character.Golem.Name = this.Character.Name;

            //扣減耐用度
            //if (this.Character.Golem.Item.Durability != 0) this.Character.Golem.Item.Durability--;
        }

        public void SetupEmptyGolemBuyShop()
        {
            this.Character.Golem.BuyShop.Clear();

            this.Character.Golem.BuyLimit = 0;
            this.Character.Golem.Title = "";


            Packets.Server.SSMG_GOLEM_SHOP_BUY_SET p = new SagaMap.Packets.Server.SSMG_GOLEM_SHOP_BUY_SET();
            this.netIO.SendPacket(p);


            Packets.Server.SSMG_GOLEM_SHOP_BUY_SETUP p1 = new SagaMap.Packets.Server.SSMG_GOLEM_SHOP_BUY_SETUP();
            p1.BuyLimit = 0;
            p1.Comment = "";
            this.netIO.SendPacket(p1);
        }

        public void OnGolemShopBuy(Packets.Client.CSMG_GOLEM_SHOP_BUY p)
        {
            Packets.Server.SSMG_GOLEM_SHOP_BUY_SETUP p1 = new SagaMap.Packets.Server.SSMG_GOLEM_SHOP_BUY_SETUP();
            p1.BuyLimit = this.Character.Golem.BuyLimit;
            p1.Comment = this.Character.Golem.Title == null ? "" : this.Character.Golem.Title;
            p1.Unknown = 0;
            p1.MaxItemCount = 32;
            p1.IsAddtoGolemCatalog = 1;
            p1.GolemShopType = 0;
            this.Character.Golem.BuyShop.Clear();
            this.netIO.SendPacket(p1);
        }

        public void OnGolemCatalogOpenResult(Packets.Client.CSMG_GOLEM_CATALOG_RESPONSE p)
        {

        }

        public void OnGolemCatalogSearchItemID(Packets.Client.CSMG_GOLEM_CATALOG_SEARCH_ITEM p)
        {
            var itemid = p.ItemID;
            var item = ItemFactory.Instance.GetItem(itemid);
            if (item != null && item.ItemID != 10000000)
                Logger.ShowInfo("尝试通过石像目录搜索道具:" + item.BaseData.name + "[" + itemid + "]");
            else
                Logger.ShowInfo("尝试通过石像目录搜索不存在的道具: " + itemid);

            var args = "18 ED 00 00 9C 43";
            Packet p1 = new Packet();
            byte[] buf = Conversions.HexStr2Bytes(args.Replace(" ", ""));
            p1.data = buf;
            this.netIO.SendPacket(p1);

            Packet p3 = new Packet();
            p3.data = new byte[2];
            p3.ID = 0x18ee;
            this.netIO.SendPacket(p3);

        }
    }
}
