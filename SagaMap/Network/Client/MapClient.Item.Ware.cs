﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using SagaDB;
using SagaDB.Item;
using SagaDB.Actor;
using SagaLib;
using SagaMap;
using SagaMap.Manager;
using SagaMap.Skill;

namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        public WarehousePlace currentWarehouse = WarehousePlace.Current;

        public void OnItemWarePage(Packets.Client.CSMG_ITEM_WARE_PAGE p)
        {
            uint page = p.PageID;
            WarehousePlace place = (WarehousePlace)page;

            //this.SendSystemMessage("Warhouse:" + place.ToString());
            this.currentWarehouse = place;
            this.SendWareItems(place);
        }

        public void OnItemWareClose(Packets.Client.CSMG_ITEM_WARE_CLOSE p)
        {
            currentWarehouse = WarehousePlace.Current;
        }

        public void OnItemWareGet(Packets.Client.CSMG_ITEM_WARE_GET p)
        {
            int result = 0;
            if (currentWarehouse == WarehousePlace.Current)
                result = 1;
            else
            {
                Item item = this.Character.Inventory.GetItem(currentWarehouse, p.InventoryID);

                if (item == null)
                    result = -2;
                else if (item.Stack < p.Count)
                    result = -3;
                //ECOKEY 配合道具堆疊999的提示
                else if (p.Count > 999)
                {
                    result = -1;
                    SendSystemMessage("道具堆疊數量修復，請用拖曳的方式領出道具，最高999個");
                }
                else if (!item.Stackable && (this.Character.Inventory.Items[ContainerType.BODY].Count + this.Character.Inventory.Equipments.Count >= 100))
                    result = -5;
                else
                {

                    Item newItem;
                    switch (this.Character.Inventory.DeleteWareItem(currentWarehouse, item.Slot, p.Count))
                    {
                        case InventoryDeleteResult.ALL_DELETED:
                            Packets.Server.SSMG_ITEM_DELETE p1 = new SagaMap.Packets.Server.SSMG_ITEM_DELETE();
                            p1.InventorySlot = item.Slot;
                            this.netIO.SendPacket(p1);
                            newItem = item.Clone();
                            newItem.Stack = p.Count;
                            Logger.LogItemGet(Logger.EventType.ItemWareGet, this.Character.Name + "(" + this.Character.CharID + ")", item.BaseData.name + "(" + item.ItemID + ")",
                                string.Format("WareGet Count:{0}", item.Stack), false);
                            AddItem(newItem, false);
                            this.SendSystemMessage(string.Format(LocalManager.Instance.Strings.ITEM_WARE_GET, item.BaseData.name, p.Count));
                            break;
                        case InventoryDeleteResult.STACK_UPDATED:
                            Packets.Server.SSMG_ITEM_COUNT_UPDATE p2 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                            p2.InventorySlot = item.Slot;
                            p2.Stack = item.Stack;
                            this.netIO.SendPacket(p2);
                            newItem = item.Clone();
                            newItem.Stack = p.Count;
                            Logger.LogItemGet(Logger.EventType.ItemWareGet, this.Character.Name + "(" + this.Character.CharID + ")", item.BaseData.name + "(" + item.ItemID + ")",
                                string.Format("WareGet Count:{0}", item.Stack), false);
                            AddItem(newItem, false);
                            this.SendSystemMessage(string.Format(LocalManager.Instance.Strings.ITEM_WARE_GET, item.BaseData.name, p.Count));
                            break;
                        case InventoryDeleteResult.ERROR:
                            result = -99;
                            break;
                    }
                }
            }
            Packets.Server.SSMG_ITEM_WARE_GET_RESULT p5 = new SagaMap.Packets.Server.SSMG_ITEM_WARE_GET_RESULT();
            p5.Result = result;
            this.netIO.SendPacket(p5);
        }

        public void OnItemWarePut(Packets.Client.CSMG_ITEM_WARE_PUT p)
        {
            int result = 0;
            if (currentWarehouse == WarehousePlace.Current)
                result = -1;
            else
            {
                Item item = this.Character.Inventory.GetItem(p.InventoryID);
                if (item == null)
                    result = -2;
                else if (item.PossessionOwner != null)
                    result = -2;
                else if (item.PossessionedActor != null)
                    result = -2;
                else if (item.Stack < p.Count)
                    result = -3;
                else if (this.Character.Inventory.WareHouse[currentWarehouse].Count() >= Configuration.Instance.WarehouseLimit)
                    result = -4;
                else if (item.BaseData.noStore && TradeConditionFactory.Instance.Items[item.ItemID].GMLevelToIgnoreCondition > this.Character.Account.GMLevel)
                    result = -6;
                else if (currentWarehouse == WarehousePlace.MainFGarden && item.BaseData.noGardenStore && TradeConditionFactory.Instance.Items[item.ItemID].GMLevelToIgnoreCondition > this.Character.Account.GMLevel)
                    result = -6;
                else if (this.Character.Golem != null && this.Character.Golem.SellShop.ContainsKey(item.Slot))
                    result = -6;
                else
                {
                    Logger.LogItemLost(Logger.EventType.ItemWareLost, this.Character.Name + "(" + this.Character.CharID + ")", item.BaseData.name + "(" + item.ItemID + ")",
                        string.Format("WarePut Count:{0}", p.Count), false);
                    DeleteItem(p.InventoryID, p.Count, false);
                    Item newItem = item.Clone();
                    newItem.Stack = p.Count;
                    switch (this.Character.Inventory.AddWareItem(currentWarehouse, newItem))
                    {
                        case InventoryAddResult.NEW_INDEX:
                            Packets.Server.SSMG_ITEM_WARE_ITEM p1 = new SagaMap.Packets.Server.SSMG_ITEM_WARE_ITEM();
                            p1.Place = WarehousePlace.Current;
                            p1.InventorySlot = newItem.Slot;
                            p1.Item = newItem;
                            this.netIO.SendPacket(p1);
                            break;
                        case InventoryAddResult.STACKED:
                            Packets.Server.SSMG_ITEM_COUNT_UPDATE p2 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                            p2.InventorySlot = item.Slot;
                            p2.Stack = item.Stack;
                            this.netIO.SendPacket(p2);
                            break;
                        case InventoryAddResult.MIXED:
                            Packets.Server.SSMG_ITEM_COUNT_UPDATE p3 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                            p3.InventorySlot = item.Slot;
                            p3.Stack = item.Stack;
                            this.netIO.SendPacket(p3);
                            Packets.Server.SSMG_ITEM_WARE_ITEM p4 = new SagaMap.Packets.Server.SSMG_ITEM_WARE_ITEM();
                            p4.InventorySlot = this.Character.Inventory.LastItem.Slot;
                            p4.Item = this.Character.Inventory.LastItem;
                            p4.Place = WarehousePlace.Current;
                            this.netIO.SendPacket(p4);
                            break;
                    }
                    this.SendSystemMessage(string.Format(LocalManager.Instance.Strings.ITEM_WARE_PUT, item.BaseData.name, p.Count));
                }
            }
            Packets.Server.SSMG_ITEM_WARE_PUT_RESULT p5 = new SagaMap.Packets.Server.SSMG_ITEM_WARE_PUT_RESULT();
            p5.Result = result;
            this.netIO.SendPacket(p5);
        }

        public void SendWareItems(WarehousePlace place)
        {
            Packets.Server.SSMG_ITEM_WARE_HEADER p = new SagaMap.Packets.Server.SSMG_ITEM_WARE_HEADER();
            p.Place = place;
            p.CountCurrent = this.Character.Inventory.WareHouse[place].Count;
            p.CountAll = Configuration.Instance.WarehouseLimit;
            p.CountMax = Configuration.Instance.WarehouseLimit;
            p.Gold = this.Character.Account.Bank;
            this.netIO.SendPacket(p);

            foreach (WarehousePlace i in this.Character.Inventory.WareHouse.Keys)
            {
                if (i == WarehousePlace.Current)
                    continue;
                if (i != place) continue;
                foreach (Item j in this.Character.Inventory.WareHouse[i])
                {
                    //if (j.Refine == 0)
                    //    j.Clear();

                    Packets.Server.SSMG_ITEM_WARE_ITEM p1 = new SagaMap.Packets.Server.SSMG_ITEM_WARE_ITEM();
                    p1.Item = j;
                    p1.InventorySlot = j.Slot;
                    if (i == place)
                        p1.Place = WarehousePlace.Current;
                    else
                        p1.Place = i;
                    this.netIO.SendPacket(p1);
                }
            }
            Packets.Server.SSMG_ITEM_WARE_FOOTER p2 = new SagaMap.Packets.Server.SSMG_ITEM_WARE_FOOTER();
            this.netIO.SendPacket(p2);
        }

        public void OnDeposit(Packets.Client.CSMG_ITEM_WARE_DEPOSIT p)
        {
         /*   long gold = (long)p.Gold;
            if (gold > this.Character.Gold)
                gold = this.Character.Gold;

            long balance = 999999999999 - (long)this.Character.Account.Bank;

            if (gold > balance)
                gold = balance;

            this.Character.Gold -= gold;
            this.Character.Account.Bank += (ulong)gold;
            Packets.Server.SSMG_ITEM_WARE_DEPOSIT_RESULT p1 = new Packets.Server.SSMG_ITEM_WARE_DEPOSIT_RESULT();
            p1.Balance = this.Character.Account.Bank;
            this.netIO.SendPacket(p1);*/
        }

        public void OnWithdrawMoney(Packets.Client.CSMG_ITEM_WARE_WITHDRAW_MONEY p)
        {
         /*   ulong gold = p.Gold;
            if (gold > this.Character.Account.Bank)
                gold = this.Character.Account.Bank;

            long balance = 999999999999 - this.Character.Gold;

            if ((long)gold > balance)
                gold = (ulong)balance;

            this.Character.Gold += (long)gold;
            this.Character.Account.Bank -= gold;
            Packets.Server.SSMG_ITEM_WARE_WITHDRAW_MONEY_RESULT p1 = new Packets.Server.SSMG_ITEM_WARE_WITHDRAW_MONEY_RESULT();
            p1.Balance = this.Character.Account.Bank;
            this.netIO.SendPacket(p1);*/
        }
    }
}