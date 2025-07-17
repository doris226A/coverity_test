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
using SagaDB.Furniture;

namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        public void OnFGardenFurnitureUse(Packets.Client.CSMG_FGARDEN_FURNITURE_USE p)
        {
            Map map = MapManager.Instance.GetMap(this.Character.MapID);
            Actor actor = map.GetActor(p.ActorID);
            if (actor == null)
                return;
            if (actor.type != ActorType.FURNITURE)
                return;
            ActorFurniture furniture = (ActorFurniture)actor;
            Item a = ItemFactory.Instance.GetItem(furniture.ItemID);
            Furniture f = FurnitureFactory.Instance.GetFurniture(furniture.ItemID);

            if (a.BaseData.itemType == ItemType.PARTNER || a.BaseData.itemType == ItemType.RIDE_PARTNER)
            {
                Character.TInt["家具夥伴ID"] = (int)furniture.ActorID;
                EventActivate(25555);
                return;
            }
            if (f == null)
            {
                SendSystemMessage("該家具沒有資料");
                return;
            }
            if (furniture.ItemID == 31163201 || furniture.ItemID == 31071100 || furniture.ItemID == 31103800 || furniture.ItemID == 31116600 || furniture.ItemID == 31116300
               || furniture.ItemID == 31092300 || furniture.ItemID == 31117500 || furniture.ItemID == 31118100 || furniture.ItemID == 31118700 || furniture.ItemID == 31165400
               || furniture.ItemID == 31135100 || furniture.ItemID == 31071000 || furniture.ItemID == 31102800 || furniture.ItemID == 31119700 || furniture.ItemID == 31071200
               || furniture.ItemID == 31169300 || furniture.ItemID == 31121500 || furniture.ItemID == 31151300 || furniture.ItemID == 31151700 || furniture.ItemID == 31152400
               || furniture.ItemID == 31153400 || furniture.ItemID == 31158900 || furniture.ItemID == 31159500)
            {
                if (furniture.Motion != f.DefaultMotion)
                {
                    furniture.Motion = f.DefaultMotion;
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.MOTION, null, furniture, false);
                }
                else
                {
                    furniture.Motion = f.Motion[0];
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.MOTION, null, furniture, false);
                }
                EventActivate(furniture.ItemID);
                return;
            }
            if (f.Motion.Count() <= 0)
            {
                SendSystemMessage("什麼都沒發生");
                return;
            }
            if (f.Motion.Count() > 1)
            {
                if (a.BaseData.eventID != 0)
                {
                    Character.TInt["家具ID"] = (int)furniture.ActorID;
                    if (a.BaseData.eventID == 31101500)
                    {
                        EventActivate(a.BaseData.eventID);
                        return;
                    }
                    EventActivate(31000000);
                    return;
                }
            }
            else
            {
                if (a.BaseData.itemType == ItemType.PARTNER || a.BaseData.itemType == ItemType.RIDE_PARTNER)
                {
                    Character.TInt["家具夥伴ID"] = (int)furniture.ActorID;
                    EventActivate(25555);
                }
                //單選
                if (furniture.Motion != f.DefaultMotion)
                {
                    furniture.Motion = f.DefaultMotion;
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.MOTION, null, furniture, false);
                }
                else
                {
                    furniture.Motion = f.Motion[0];
                    map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.MOTION, null, furniture, false);
                }
                EventActivate(31080000);
            }


        }

        public void OnFGardenFurnitureReconfig(Packets.Client.CSMG_FGARDEN_FURNITURE_RECONFIG p)
        {
            if (this.Character.FGarden == null)
                   return;
               Map map = MapManager.Instance.GetMap(this.Character.MapID);
               Actor actor = map.GetActor(p.ActorID);
               if (actor == null)
                   return;
               if (actor.type != ActorType.FURNITURE)
                   return;
               if (this.Character.MapID != this.Character.FGarden.MapID && this.Character.MapID != this.Character.FGarden.RoomMapID)
               {
                   Packets.Server.SSMG_FG_FURNITURE_RECONFIG p1 = new SagaMap.Packets.Server.SSMG_FG_FURNITURE_RECONFIG();
                   p1.ActorID = actor.ActorID;
                   p1.X = actor.X;
                   p1.Y = actor.Y;
                   p1.Z = ((ActorFurniture)actor).Z;
                   p1.Dir = actor.Dir;
                   this.netIO.SendPacket(p1);
                   return;
               }
               map.MoveActor(Map.MOVE_TYPE.START, actor, new short[] { p.X, p.Y, p.Z }, p.Dir, 200);
        }

        public void OnFGardenFurnitureRemove(Packets.Client.CSMG_FGARDEN_FURNITURE_REMOVE p)
        {
            if (this.Character.FGarden == null)
                return;
            if (this.Character.MapID != this.Character.FGarden.MapID && this.Character.MapID != this.Character.FGarden.RoomMapID)
                return;
            Map map = null;
            map = MapManager.Instance.GetMap(this.Character.MapID);
            Actor actor = map.GetActor(p.ActorID);
            if (actor == null)
                return;
            if (actor.type != ActorType.FURNITURE)
                return;
            ActorFurniture furniture = (ActorFurniture)actor;
            map.DeleteActor(actor);
            Item item = ItemFactory.Instance.GetItem(furniture.ItemID);
            item.PictID = furniture.PictID;
            item.ActorPartnerID = furniture.ActorPartnerID;//ECOKEY 寵物紀錄
            item.Durability = furniture.Durability;//ECOKEY 寵物親密度紀錄
            if (this.Character.MapID == this.Character.FGarden.MapID)
                this.Character.FGarden.Furnitures[FurniturePlace.GARDEN].Remove(furniture);
            else
                this.Character.FGarden.Furnitures[FurniturePlace.ROOM].Remove(furniture);
            AddItem(item, false);
            SendSystemMessage(string.Format(LocalManager.Instance.Strings.FG_FUTNITURE_REMOVE, furniture.Name, (this.Character.FGarden.Furnitures[FurniturePlace.GARDEN].Count +
                    this.Character.FGarden.Furnitures[FurniturePlace.ROOM].Count), Configuration.Instance.MaxFurnitureCount));

            if (map.Creator.AInt["FGarden_Heavenly"] >= 4)//ECOKEY 收起天空家具時，將天空變回原狀
            {
                uint[] j = new uint[23] { 31163201, 31071100, 31103800, 31116600, 31116300, 31092300, 31117500, 31118100, 31118700, 31165400, 31135100, 31071000, 31102800, 31119700, 31071200, 31169300, 31121500, 31151300, 31151700, 31152400, 31153400, 31158900, 31159500 };
                if (j.Contains(furniture.ItemID)) map.Creator.AInt["FGarden_Heavenly"] = 0;
            }
        }

        public void OnFGardenFurnitureSetup(Packets.Client.CSMG_FGARDEN_FURNITURE_SETUP p)
        {
            if (this.Character.FGarden == null)
                return;
            if (this.Character.MapID != this.Character.FGarden.MapID && this.Character.MapID != this.Character.FGarden.RoomMapID)
                return;
            if ((this.Character.FGarden.Furnitures[FurniturePlace.GARDEN].Count +
                this.Character.FGarden.Furnitures[FurniturePlace.ROOM].Count) < Configuration.Instance.MaxFurnitureCount)
            {
                Item item = this.Character.Inventory.GetItem(p.InventorySlot);
                //先禁止正太放在飛空
                if (item.ItemID == 10133700 || item.ItemID == 10133701 || item.ItemID == 10136100 || item.ItemID == 10136101)
                {
                    SendSystemMessage("此寵物暫時禁止放在飛空。");
                    return;
                }
                ActorFurniture actor = new ActorFurniture();

                DeleteItem(p.InventorySlot, 1, false);

                actor.MapID = this.Character.MapID;
                actor.ItemID = item.ItemID;
                Map map = MapManager.Instance.GetMap(actor.MapID);
                actor.X = p.X;
                actor.Y = p.Y;
                actor.Z = p.Z;
                //actor.Dir = p.Dir;
                actor.Xaxis = p.AxleX;
                actor.Yaxis = p.AxleY;
                actor.Zaxis = p.AxleZ;
                actor.Name = item.BaseData.name;
                actor.PictID = item.PictID;
                actor.ActorPartnerID = item.ActorPartnerID;//ECOKEY 寵物紀錄
                actor.Durability = item.Durability;//ECOKEY 寵物親密度紀錄
                actor.e = new ActorEventHandlers.NullEventHandler();
                map.RegisterActor(actor);
                actor.invisble = false;
                map.OnActorVisibilityChange(actor);

                if (this.Character.MapID == this.Character.FGarden.MapID)
                    this.Character.FGarden.Furnitures[FurniturePlace.GARDEN].Add(actor);
                else
                    this.Character.FGarden.Furnitures[FurniturePlace.ROOM].Add(actor);
                SendSystemMessage(string.Format(LocalManager.Instance.Strings.FG_FUTNITURE_SETUP, actor.Name, (this.Character.FGarden.Furnitures[FurniturePlace.GARDEN].Count +
                    this.Character.FGarden.Furnitures[FurniturePlace.ROOM].Count), Configuration.Instance.MaxFurnitureCount));
            }
            else
            {
                SendSystemMessage(LocalManager.Instance.Strings.FG_FUTNITURE_MAX);
            }            
        }

       public void OnFGardenEquipt(Packets.Client.CSMG_FGARDEN_EQUIPT p)
        {
            if (this.Character.FGarden == null)
                return;
            if (this.Character.MapID != this.Character.FGarden.MapID && this.Character.MapID != this.Character.FGarden.RoomMapID)
                return;
            if (p.InventorySlot != 0xFFFFFFFF)
            {
                Item item = this.Character.Inventory.GetItem(p.InventorySlot);
                if (item == null)
                    return;
                if (this.Character.FGarden.FGardenEquipments[p.Place] != 0)
                {
                    uint itemID = this.Character.FGarden.FGardenEquipments[p.Place];
                    if (itemID != 0)
                    {
                        Item equipt = ItemFactory.Instance.GetItem(itemID, true);//ECOKEY 染色新增
                        equipt.Dye = (byte)this.Character.AInt[p.Place.ToString()];//ECOKEY 染色新增，在收回物品時，保持有染色狀態
                        AddItem(equipt, false);//ECOKEY 染色新增
                        this.Character.AInt[p.Place.ToString()] = 0;//ECOKEY 染色新增
                    }
                    Packets.Server.SSMG_FG_EQUIPT p1 = new SagaMap.Packets.Server.SSMG_FG_EQUIPT();
                    p1.ItemID = 0;
                    p1.Place = p.Place;
                    this.netIO.SendPacket(p1);
                }
                if (p.Place == SagaDB.FGarden.FGardenSlot.GARDEN_MODELHOUSE && this.Character.FGarden.FGardenEquipments[SagaDB.FGarden.FGardenSlot.GARDEN_MODELHOUSE] == 0)
                {
                    Packets.Server.SSMG_NPC_SET_EVENT_AREA p1 = new SagaMap.Packets.Server.SSMG_NPC_SET_EVENT_AREA();
                    p1.EventID = 10000315;
                    p1.StartX = 6;
                    p1.StartY = 7;
                    p1.EndX = 6;
                    p1.EndY = 7;
                    this.netIO.SendPacket(p1);
                }
                this.Character.FGarden.FGardenEquipments[p.Place] = item.ItemID;
                Packets.Server.SSMG_FG_EQUIPT p2 = new SagaMap.Packets.Server.SSMG_FG_EQUIPT();
                p2.ItemID = item.ItemID;
                p2.Place = p.Place;
                p2.Color = item.Dye;//ECOKEY 染色新增，顯示有染色的顏色
                this.Character.AInt[p.Place.ToString()] = item.Dye;//ECOKEY 染色新增，記錄此物品染什麼顏色
                this.netIO.SendPacket(p2);
                DeleteItem(p.InventorySlot, 1, false);
            }
            else
            {
                uint itemID = this.Character.FGarden.FGardenEquipments[p.Place];
                if (itemID != 0)
                {
                    Item equipt = ItemFactory.Instance.GetItem(itemID, true);//ECOKEY 染色新增
                    equipt.Dye = (byte)this.Character.AInt[p.Place.ToString()];//ECOKEY 染色新增，在收回物品時，保持有染色狀態
                    AddItem(equipt, false);//ECOKEY 染色新增
                    this.Character.AInt[p.Place.ToString()] = 0;//ECOKEY 染色新增
                }
                //AddItem(ItemFactory.Instance.GetItem(itemID, true), false);
                this.Character.FGarden.FGardenEquipments[p.Place] = 0;
                Packets.Server.SSMG_FG_EQUIPT p1 = new SagaMap.Packets.Server.SSMG_FG_EQUIPT();
                p1.ItemID = 0;
                p1.Place = p.Place;
                this.netIO.SendPacket(p1);
                if (p.Place == SagaDB.FGarden.FGardenSlot.GARDEN_MODELHOUSE)
                {
                    Packets.Server.SSMG_NPC_CANCEL_EVENT_AREA p2 = new SagaMap.Packets.Server.SSMG_NPC_CANCEL_EVENT_AREA();
                    p2.StartX = 6;
                    p2.StartY = 7;
                    p2.EndX = 6;
                    p2.EndY = 7;
                    this.netIO.SendPacket(p2);
                }
            }
        }
       /* public void OnFGardenEquipt(Packets.Client.CSMG_FGARDEN_EQUIPT p)
        {
            if (this.Character.FGarden == null)
                return;
            if (this.Character.MapID != this.Character.FGarden.MapID && this.Character.MapID != this.Character.FGarden.RoomMapID)
                return;
            if (p.InventorySlot != 0xFFFFFFFF)
            {
                Item item = this.Character.Inventory.GetItem(p.InventorySlot);
                if (item == null)
                    return;
                if (this.Character.FGarden.FGardenEquipments[p.Place] != 0)
                {
                    uint itemID = this.Character.FGarden.FGardenEquipments[p.Place];
                    AddItem(ItemFactory.Instance.GetItem(itemID, true), false);
                    Packets.Server.SSMG_FG_EQUIPT p1 = new SagaMap.Packets.Server.SSMG_FG_EQUIPT();
                    p1.ItemID = 0;
                    p1.Place = p.Place;
                    this.netIO.SendPacket(p1);
                }
                if (p.Place == SagaDB.FGarden.FGardenSlot.GARDEN_MODELHOUSE && this.Character.FGarden.FGardenEquipments[SagaDB.FGarden.FGardenSlot.GARDEN_MODELHOUSE] == 0)
                {
                    Packets.Server.SSMG_NPC_SET_EVENT_AREA p1 = new SagaMap.Packets.Server.SSMG_NPC_SET_EVENT_AREA();
                    p1.EventID = 10000315;
                    p1.StartX = 6;
                    p1.StartY = 7;
                    p1.EndX = 6;
                    p1.EndY = 7;
                    this.netIO.SendPacket(p1);
                }
                this.Character.FGarden.FGardenEquipments[p.Place] = item.ItemID;
                Packets.Server.SSMG_FG_EQUIPT p2 = new SagaMap.Packets.Server.SSMG_FG_EQUIPT();
                p2.ItemID = item.ItemID;
                p2.Place = p.Place;
                this.netIO.SendPacket(p2);
                DeleteItem(p.InventorySlot, 1, false);
            }
            else
            {
                uint itemID = this.Character.FGarden.FGardenEquipments[p.Place];
                if (itemID != 0)
                    AddItem(ItemFactory.Instance.GetItem(itemID, true), false);
                this.Character.FGarden.FGardenEquipments[p.Place] = 0;
                Packets.Server.SSMG_FG_EQUIPT p1 = new SagaMap.Packets.Server.SSMG_FG_EQUIPT();
                p1.ItemID = 0;
                p1.Place = p.Place;
                this.netIO.SendPacket(p1);
                if (p.Place == SagaDB.FGarden.FGardenSlot.GARDEN_MODELHOUSE)
                {
                    Packets.Server.SSMG_NPC_CANCEL_EVENT_AREA p2 = new SagaMap.Packets.Server.SSMG_NPC_CANCEL_EVENT_AREA();
                    p2.StartX = 6;
                    p2.StartY = 7;
                    p2.EndX = 6;
                    p2.EndY = 7;
                    this.netIO.SendPacket(p2);
                }
            }
        }*/
    }
}
