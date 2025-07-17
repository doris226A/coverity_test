using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using SagaDB;
using SagaDB.Item;
using SagaDB.Actor;
using SagaDB.DEMIC;
using SagaLib;
using SagaMap.Manager;
using SagaMap.Skill;
using SagaMap.PC;
using SagaDB.EnhanceTable;
using SagaMap.Skill.Additions.Global;
//using SagaMap.Scripting;

namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        public bool demCLBuy = false;
        public bool demParts = false;

        public bool demic = false;
        public bool chipShop = false;
        uint currentChipCategory = 0;
        public WarehousePlace currentWarehouse2 = WarehousePlace.Chips;
        //ECOKEY 增加EP，方便限時道具計算專用
        public void EP_Add(uint add)
        {
            if (this.chara.TimerItems.ContainsKey("EP_BOUNS")) add *= 2;
            this.Character.EP += add;
            if (this.Character.EP > this.Character.MaxEP)
                this.Character.EP = this.Character.MaxEP;
            this.Character.e.PropertyUpdate(UpdateEvent.EP, (int)add);
        }
        public void SendEP()
        {
            DateTime thisDay = DateTime.Today;
            if (this.Character.EPLoginTime != thisDay)
            {
                EP_Add(10);
                this.Character.EPLoginTime = thisDay;
            }
        }
        public void SendEPGreetingTime(ActorPC targetpc)
        {
            DateTime thisDay = DateTime.Today;
            if (this.Character.EPGreetingTime != thisDay)
            {
                this.Character.CInt["今天打招呼次數"] = 0;
                this.Character.CIDict["打招呼的玩家"] = new VariableHolderA<int, int>();
                this.Character.EPGreetingTime = thisDay;
            }
            if (this.Character.CInt["今天打招呼次數"] <= 10 && !this.Character.CIDict["打招呼的玩家"].ContainsKey((int)targetpc.CharID))
            {
                this.Character.CIDict["打招呼的玩家"][(int)targetpc.CharID] = 1;
                this.Character.CInt["今天打招呼次數"]++;
                int friendCount = MapServer.charDB.GetFriendList(this.Character).Count;
                uint add = (uint)(Math.Min(Math.Max(1 + (friendCount / 10), 1), 11));
                EP_Add(add);
            }
        }
        public void SendCL()
        {
            if (this.chara.Race == PC_RACE.DEM && this.state != SESSION_STATE.AUTHENTIFICATED)
            {
                Packets.Server.SSMG_DEM_COST_LIMIT_UPDATE p1 = new SagaMap.Packets.Server.SSMG_DEM_COST_LIMIT_UPDATE();
                p1.Result = (SagaMap.Packets.Server.SSMG_DEM_COST_LIMIT_UPDATE.Results)0;
                p1.CurrentEP = this.Character.EPUsed;
                p1.EPRequired = (short)(ExperienceManager.Instance.GetEPRequired(this.chara) - this.chara.EPUsed);
                p1.CL = this.chara.CL;
                this.netIO.SendPacket(p1);
            }
        }

        public void OnDEMCostLimitBuy(Packets.Client.CSMG_DEM_COST_LIMIT_BUY p)
        {
            if (demCLBuy)
            {
                short ep = p.EP;
                Packets.Server.SSMG_DEM_COST_LIMIT_UPDATE p1 = new SagaMap.Packets.Server.SSMG_DEM_COST_LIMIT_UPDATE();
                if (this.Character.EP >= ep)
                {
                    this.Character.EP = (uint)(this.Character.EP - ep);
                    ExperienceManager.Instance.ApplyEP(this.Character, ep);
                    PC.StatusFactory.Instance.CalcStatus(this.Character);
                    SendPlayerInfo();
                    p1.Result = SagaMap.Packets.Server.SSMG_DEM_COST_LIMIT_UPDATE.Results.OK;
                }
                else
                    p1.Result = SagaMap.Packets.Server.SSMG_DEM_COST_LIMIT_UPDATE.Results.NOT_ENOUGH_EP;
                p1.CurrentEP = this.Character.EPUsed;
                p1.EPRequired = (short)(ExperienceManager.Instance.GetEPRequired(this.chara) - this.chara.EPUsed);
                p1.CL = this.chara.CL;
                this.netIO.SendPacket(p1);
            }
        }

        public void OnDEMCostLimitClose(Packets.Client.CSMG_DEM_COST_LIMIT_CLOSE p)
        {
            demCLBuy = false;
        }

        public void OnDEMFormChange(Packets.Client.CSMG_DEM_FORM_CHANGE p)
        {
            if (this.Character.Form != p.Form)
            {
                //ECOKEY DEM 解除PE
                SagaMap.Skill.SkillHandler.Instance.PossessionCancel(this.Character, SagaLib.PossessionPosition.NONE);
                //ECOKEY DEM 機械狀態去掉寵物
                if (p.Form == DEM_FORM.MACHINA_FORM)
                {
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.PET))
                    {
                        DeletePartner();
                    }
                }
                else
                {
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.PET))
                    {
                        SendPartner(this.Character.Inventory.Equipments[EnumEquipSlot.PET]);
                    }
                }
                this.Character.Form = p.Form;

                SkillHandler.Instance.CastPassiveSkills(this.chara);
                PC.StatusFactory.Instance.CalcStatus(this.chara);

                this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, this.chara, true);
                SendPlayerInfo();
                SendAttackType();

                Packets.Server.SSMG_DEM_FORM_CHANGE p1 = new SagaMap.Packets.Server.SSMG_DEM_FORM_CHANGE();
                p1.Form = this.chara.Form;
                this.netIO.SendPacket(p1);
            }
            /*if (this.Character.Form != p.Form)
            {
                this.Character.Form = p.Form;

                SkillHandler.Instance.CastPassiveSkills(this.chara);
                PC.StatusFactory.Instance.CalcStatus(this.chara);

                this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, this.chara, true);                
                SendPlayerInfo();
                SendAttackType();

                Packets.Server.SSMG_DEM_FORM_CHANGE p1 = new SagaMap.Packets.Server.SSMG_DEM_FORM_CHANGE();
                p1.Form = this.chara.Form;
                this.netIO.SendPacket(p1);
            }*/
        }

        public void OnDEMPartsUnequip(Packets.Client.CSMG_DEM_PARTS_UNEQUIP p)
        {
            if (this.chara.Race == PC_RACE.DEM && demParts)
            {
                Item item = this.Character.Inventory.GetItem(p.InventoryID);
                if (item == null)
                {
                    return;
                }
                bool ifUnequip = this.Character.Inventory.IsContainerParts(this.Character.Inventory.GetContainerType(item.Slot));
                if (ifUnequip)
                {
                    List<EnumEquipSlot> slots = item.EquipSlot;
                    if (slots.Count > 1)
                    {
                        for (int i = 1; i < slots.Count; i++)
                        {
                            this.Character.Inventory.Parts.Remove(slots[i]);
                        }
                    }

                    Packets.Server.SSMG_ITEM_DELETE p2;
                    Packets.Server.SSMG_ITEM_ADD p3;
                    uint slot = item.Slot;

                    if (this.Character.Inventory.MoveItem(this.Character.Inventory.GetContainerType(item.Slot), (int)item.Slot, ContainerType.BODY, 1))
                    {
                        if (item.Stack == 0)
                        {
                            if (slot == this.Character.Inventory.LastItem.Slot)
                            {
                                Packets.Server.SSMG_ITEM_CONTAINER_CHANGE p1 = new SagaMap.Packets.Server.SSMG_ITEM_CONTAINER_CHANGE();
                                p1.InventorySlot = item.Slot;
                                p1.Target = ContainerType.BODY;
                                this.netIO.SendPacket(p1);
                                Packets.Server.SSMG_ITEM_EQUIP p4 = new SagaMap.Packets.Server.SSMG_ITEM_EQUIP();
                                p4.InventorySlot = 0xffffffff;
                                p4.Target = ContainerType.NONE;
                                p4.Result = 3;
                                PC.StatusFactory.Instance.CalcRange(this.Character);
                                if (item.EquipSlot[0] == EnumEquipSlot.RIGHT_HAND)
                                {
                                    SendAttackType();
                                    SkillHandler.Instance.CastPassiveSkills(this.Character);
                                }
                                p4.Range = this.Character.Range;
                                this.netIO.SendPacket(p4);
                                this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHANGE_EQUIP, null, this.Character, true);

                                PC.StatusFactory.Instance.CalcStatus(this.Character);
                                this.SendPlayerInfo();
                            }
                            else
                            {
                                p2 = new SagaMap.Packets.Server.SSMG_ITEM_DELETE();
                                p2.InventorySlot = slot;
                                this.netIO.SendPacket(p2);
                                if (slot != item.Slot)
                                {
                                    item = this.Character.Inventory.GetItem(item.Slot);
                                    Packets.Server.SSMG_ITEM_COUNT_UPDATE p5 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                                    p5.InventorySlot = item.Slot;
                                    p5.Stack = item.Stack;
                                    this.netIO.SendPacket(p5);
                                    item = this.Character.Inventory.LastItem;
                                    p3 = new SagaMap.Packets.Server.SSMG_ITEM_ADD();
                                    p3.Container = ContainerType.BODY;
                                    p3.InventorySlot = item.Slot;
                                    p3.Item = item;
                                    this.netIO.SendPacket(p3);
                                }
                                else
                                {
                                    item = this.Character.Inventory.LastItem;
                                    Packets.Server.SSMG_ITEM_COUNT_UPDATE p4 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                                    p4.InventorySlot = item.Slot;
                                    p4.Stack = item.Stack;
                                    this.netIO.SendPacket(p4);
                                }
                            }
                        }
                        else
                        {
                            Packets.Server.SSMG_ITEM_COUNT_UPDATE p1 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                            p1.InventorySlot = item.Slot;
                            p1.Stack = item.Stack;
                            this.netIO.SendPacket(p1);
                            if (this.Character.Inventory.LastItem.Stack == 1)
                            {
                                p3 = new SagaMap.Packets.Server.SSMG_ITEM_ADD();
                                p3.Container = ContainerType.BODY;
                                p3.InventorySlot = this.Character.Inventory.LastItem.Slot;
                                p3.Item = this.Character.Inventory.LastItem;
                                this.netIO.SendPacket(p3);
                            }
                            else
                            {
                                item = this.Character.Inventory.LastItem;
                                Packets.Server.SSMG_ITEM_COUNT_UPDATE p4 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                                p4.InventorySlot = item.Slot;
                                p4.Stack = item.Stack;
                                this.netIO.SendPacket(p4);
                            }
                        }
                    }
                    this.Character.Inventory.CalcPayloadVolume();
                    this.SendCapacity();
                }
            }
        }

        public void OnDEMPartsEquip(Packets.Client.CSMG_DEM_PARTS_EQUIP p)
        {
            if (this.chara.Race == PC_RACE.DEM && demParts)
            {
                Item item = this.Character.Inventory.GetItem(p.InventoryID);
                if (item == null)
                {
                    return;
                }
                int result = CheckEquipRequirement(item);
                if (result < 0)
                {
                    Packets.Server.SSMG_ITEM_EQUIP p4 = new SagaMap.Packets.Server.SSMG_ITEM_EQUIP();
                    p4.InventorySlot = 0xffffffff;
                    p4.Target = ContainerType.NONE;
                    p4.Result = result;
                    p4.Range = this.Character.Range;
                    this.netIO.SendPacket(p4);
                    return;
                }
                foreach (EnumEquipSlot i in item.EquipSlot)
                {
                    if (this.Character.Inventory.Parts.ContainsKey(i))
                    {
                        Item oriItem = this.Character.Inventory.Parts[i];

                        foreach (EnumEquipSlot j in oriItem.EquipSlot)
                        {
                            if (!this.Character.Inventory.Parts.ContainsKey(j))
                                continue;
                            Item dummyItem = this.Character.Inventory.Parts[j];
                            if (dummyItem.Stack == 0)
                            {
                                this.Character.Inventory.Parts.Remove(j);
                                continue;
                            }
                            ContainerType container = (ContainerType)(((ContainerType)Enum.Parse(typeof(ContainerType), j.ToString())) + 200);
                            if (this.Character.Inventory.MoveItem(container, (int)dummyItem.Slot, ContainerType.BODY, dummyItem.Stack))
                            {
                                Packets.Server.SSMG_ITEM_CONTAINER_CHANGE p1 = new SagaMap.Packets.Server.SSMG_ITEM_CONTAINER_CHANGE();
                                p1.InventorySlot = dummyItem.Slot;
                                p1.Target = ContainerType.BODY;
                                this.netIO.SendPacket(p1);
                                Packets.Server.SSMG_ITEM_EQUIP p4 = new SagaMap.Packets.Server.SSMG_ITEM_EQUIP();
                                p4.InventorySlot = 0xffffffff;
                                p4.Target = ContainerType.NONE;
                                p4.Result = 1;
                                p4.Range = this.Character.Range;
                                this.netIO.SendPacket(p4);

                                PC.StatusFactory.Instance.CalcStatus(this.Character);
                                this.SendPlayerInfo();
                            }
                        }
                    }
                }
                ushort count = item.Stack;
                if (count == 0) return;

                ContainerType dst = (ContainerType)(((ContainerType)Enum.Parse(typeof(ContainerType), item.EquipSlot[0].ToString())) + 200);
                if (this.Character.Inventory.MoveItem(this.Character.Inventory.GetContainerType(item.Slot), (int)item.Slot, dst, count))
                {
                    if (item.Stack == 0)
                    {
                        Packets.Server.SSMG_ITEM_EQUIP p4 = new SagaMap.Packets.Server.SSMG_ITEM_EQUIP();
                        dst = (ContainerType)((ContainerType)Enum.Parse(typeof(ContainerType), item.EquipSlot[0].ToString()));
                        p4.Target = dst;
                        p4.Result = 2;
                        p4.InventorySlot = item.Slot;
                        PC.StatusFactory.Instance.CalcRange(this.Character);
                        p4.Range = this.Character.Range;
                        this.netIO.SendPacket(p4);
                    }
                    else
                    {
                        Packets.Server.SSMG_ITEM_COUNT_UPDATE p5 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                        p5.InventorySlot = item.Slot;
                        p5.Stack = item.Stack;
                        this.netIO.SendPacket(p5);
                    }
                }
                List<EnumEquipSlot> slots = item.EquipSlot;
                if (slots.Count > 1)
                {
                    for (int i = 1; i < slots.Count; i++)
                    {
                        Item dummy = item.Clone();
                        dummy.Stack = 0;
                        dst = (ContainerType)(((ContainerType)Enum.Parse(typeof(ContainerType), slots[i].ToString())) + 200);
                        this.Character.Inventory.AddItem(dst, dummy);
                    }

                }
                if (item.EquipSlot[0] == EnumEquipSlot.RIGHT_HAND)
                {
                    SendAttackType();
                    SkillHandler.Instance.CastPassiveSkills(this.Character);
                }

                //SkillHandler.Instance.CheckBuffValid(this.Character);

                PC.StatusFactory.Instance.CalcStatus(this.Character);
                this.SendPlayerInfo();

                this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHANGE_EQUIP, null, this.Character, true);
            }
        }

        public void OnDEMPartsClose(Packets.Client.CSMG_DEM_PARTS_CLOSE p)
        {
            demParts = false;
        }
        //ECOKEY DEM黃點點設定
        public void OnDEMDemicEngageTaskConfirm(Packets.Client.CSMG_DEM_DEMIC_ENGAGETASK_REQUEST p)
        {
            Packets.Server.SSMG_DEM_DEMIC_ENGAGETASK_RESULT p1 = new SagaMap.Packets.Server.SSMG_DEM_DEMIC_ENGAGETASK_RESULT();
            if (demic)
            {
                byte page = p.Page;
                if (this.chara.Inventory.DemicChips.ContainsKey(page))
                {
                    if (this.chara.EP > 0)
                    {
                        this.chara.EP--;
                        this.chara.Inventory.DemicChips[page].EngageTask1 = (byte)(p.Location - 1);
                        this.chara.Inventory.DemicChips[page].EngageTask2 = (byte)(p.Location2 - 1);
                        p1.Result = SagaMap.Packets.Server.SSMG_DEM_DEMIC_ENGAGETASK_RESULT.Results.OK;
                    }
                    else
                        p1.Result = SagaMap.Packets.Server.SSMG_DEM_DEMIC_ENGAGETASK_RESULT.Results.NOT_ENOUGH_EP;
                }
                else
                    p1.Result = SagaMap.Packets.Server.SSMG_DEM_DEMIC_ENGAGETASK_RESULT.Results.FAILED;
            }
            else
                p1.Result = SagaMap.Packets.Server.SSMG_DEM_DEMIC_ENGAGETASK_RESULT.Results.FAILED;

            this.netIO.SendPacket(p1);
        }

        public void OnDEMDemicInitialize(Packets.Client.CSMG_DEM_DEMIC_INITIALIZE p)
        {
            if (demic)
            {
                byte page = p.Page;
                DEMICPanel panel = null;
                bool[,] table = null;
                if (this.map.Info.Flag.Test(SagaDB.Map.MapFlags.Dominion))
                {
                    if (this.chara.Inventory.DominionDemicChips.ContainsKey(page))
                    {
                        panel = this.chara.Inventory.DominionDemicChips[page];
                        table = this.chara.Inventory.validTable(page, true);
                    }
                }
                else
                {
                    if (this.chara.Inventory.DemicChips.ContainsKey(page))
                    {
                        panel = this.chara.Inventory.DemicChips[page];
                        table = this.chara.Inventory.validTable(page, false);
                    }
                }

                Packets.Server.SSMG_DEM_DEMIC_INITIALIZED p1 = new SagaMap.Packets.Server.SSMG_DEM_DEMIC_INITIALIZED();
                p1.Page = page;

                if (panel != null)
                {
                    if (this.chara.EP >= 5 || this.chara.TimerItems.ContainsKey("DEMIC_CUSTOM"))
                    {
                        if (!this.chara.TimerItems.ContainsKey("DEMIC_CUSTOM")) this.chara.EP = this.chara.EP - 5;
                        foreach (Chip i in panel.Chips)
                        {
                            if (i.Data.skill1 != 0)
                            {
                                if (this.chara.Skills.ContainsKey(i.Data.skill1))
                                {
                                    if (this.chara.Skills[i.Data.skill1].Level > 1)
                                        this.chara.Skills[i.Data.skill1].Level--;
                                    else
                                        this.chara.Skills.Remove(i.Data.skill1);
                                }
                            }
                            if (i.Data.skill2 != 0)
                            {
                                if (this.chara.Skills.ContainsKey(i.Data.skill2))
                                {
                                    if (this.chara.Skills[i.Data.skill2].Level > 1)
                                        this.chara.Skills[i.Data.skill2].Level--;
                                    else
                                        this.chara.Skills.Remove(i.Data.skill2);
                                }
                            }
                            if (i.Data.skill3 != 0)
                            {
                                if (this.chara.Skills.ContainsKey(i.Data.skill3))
                                {
                                    if (this.chara.Skills[i.Data.skill3].Level > 1)
                                        this.chara.Skills[i.Data.skill3].Level--;
                                    else
                                        this.chara.Skills.Remove(i.Data.skill3);
                                }
                            }

                            AddItem(ItemFactory.Instance.GetItem(i.ItemID), true);
                        }
                        panel.Chips.Clear();
                        //ECOKEY DEM關閉黃點點的隨機功能
                        /*int engageTask;
                        int term = Global.Random.Next(0, 99);
                        if (term <= 10)
                            engageTask = 2;
                        else if (term <= 40)
                            engageTask = 1;
                        else
                            engageTask = 0;
                        panel.EngageTask1 = 255;
                        panel.EngageTask2 = 255;
                        for (int i = 0; i < engageTask; i++)
                        {
                            List<byte[]> valid = new List<byte[]>();
                            for (int j = 0; j < 9; j++)
                            {
                                for (int k = 0; k < 9; k++)
                                {
                                    if (table[k, j])
                                        valid.Add(new byte[] { (byte)k, (byte)j });
                                }
                            }
                            byte[] coord = valid[Global.Random.Next(0, valid.Count - 1)];
                            byte task = (byte)(coord[0] + coord[1] * 9);
                            if (i == 0)
                                panel.EngageTask1 = task;
                            else
                                panel.EngageTask2 = task;
                        }*/

                        SendActorHPMPSP(this.chara);
                        p1.Result = SagaMap.Packets.Server.SSMG_DEM_DEMIC_INITIALIZED.Results.OK;
                        p1.EngageTask = panel.EngageTask1;
                        p1.EngageTask2 = panel.EngageTask2;

                        PC.StatusFactory.Instance.CalcStatus(this.chara);
                        SendPlayerInfo();
                    }
                    else
                        p1.Result = SagaMap.Packets.Server.SSMG_DEM_DEMIC_INITIALIZED.Results.NOT_ENOUGH_EP;
                }
                else
                    p1.Result = SagaMap.Packets.Server.SSMG_DEM_DEMIC_INITIALIZED.Results.FAILED;

                this.netIO.SendPacket(p1);
            }
        }

        public void OnDEMDemicConfirm(Packets.Client.CSMG_DEM_DEMIC_CONFIRM p)
        {
            if (demic)
            {
                short[,] chips = p.Chips;
                byte page = p.Page;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        short chipID = chips[j, i];
                        if (ChipFactory.Instance.ByChipID.ContainsKey(chipID))
                        {
                            Chip chip = new Chip(ChipFactory.Instance.ByChipID[chipID]);
                            if (CountItem(chip.ItemID) > 0)
                            {
                                chip.X = (byte)j;
                                chip.Y = (byte)i;
                                if (this.chara.Inventory.InsertChip(page, chip))
                                {
                                    DeleteItemID(chip.ItemID, 1, true);
                                }
                            }
                        }
                    }
                }
                Packets.Server.SSMG_DEM_DEMIC_CONFIRM_RESULT p1 = new SagaMap.Packets.Server.SSMG_DEM_DEMIC_CONFIRM_RESULT();
                p1.Page = page;
                p1.Result = SagaMap.Packets.Server.SSMG_DEM_DEMIC_CONFIRM_RESULT.Results.OK;
                this.netIO.SendPacket(p1);

                PC.StatusFactory.Instance.CalcStatus(this.chara);
                SkillHandler.Instance.CastPassiveSkills(this.chara);
                SendPlayerInfo();
            }
        }

        public void OnDEMDemicClose(Packets.Client.CSMG_DEM_DEMIC_CLOSE p)
        {
            demic = false;
        }

        public void OnDEMStatsPreCalc(Packets.Client.CSMG_DEM_STATS_PRE_CALC p)
        {
            //backup
            ushort str, dex, intel, agi, vit, mag;
            str = this.Character.Str;
            dex = this.Character.Dex;
            intel = this.Character.Int;
            agi = this.Character.Agi;
            vit = this.Character.Vit;
            mag = this.Character.Mag;

            this.Character.Str = p.Str;
            this.Character.Dex = p.Dex;
            this.Character.Int = p.Int;
            this.Character.Agi = p.Agi;
            this.Character.Vit = p.Vit;
            this.Character.Mag = p.Mag;

            StatusFactory.Instance.CalcStatus(this.Character);
            {
                Packets.Server.SSMG_DEM_PLAYER_STATS_PRE_CALC p1 = new SagaMap.Packets.Server.SSMG_DEM_PLAYER_STATS_PRE_CALC();
                p1.ASPD = this.Character.Status.aspd;
                p1.ATK1Max = this.Character.Status.max_atk1;
                p1.ATK1Min = this.Character.Status.min_atk1;
                p1.ATK2Max = this.Character.Status.max_atk2;
                p1.ATK2Min = this.Character.Status.min_atk2;
                p1.ATK3Max = this.Character.Status.max_atk3;
                p1.ATK3Min = this.Character.Status.min_atk3;
                p1.AvoidCritical = this.Character.Status.avoid_critical;
                p1.AvoidMagic = this.Character.Status.avoid_magic;
                p1.AvoidMelee = this.Character.Status.avoid_melee;
                p1.AvoidRanged = this.Character.Status.avoid_ranged;
                p1.CSPD = this.Character.Status.cspd;
                p1.DefAddition = (ushort)this.Character.Status.def_add;
                p1.DefBase = this.Character.Status.def;
                p1.HitCritical = this.Character.Status.hit_critical;
                p1.HitMagic = this.Character.Status.hit_magic;
                p1.HitMelee = this.Character.Status.hit_melee;
                p1.HitRanged = this.Character.Status.hit_ranged;
                p1.MATKMax = this.Character.Status.max_matk;
                p1.MATKMin = this.Character.Status.min_matk;
                p1.MDefAddition = (ushort)this.Character.Status.mdef_add;
                p1.MDefBase = this.Character.Status.mdef;
                p1.Speed = this.Character.Speed;
                p1.HP = (ushort)this.Character.MaxHP;
                p1.MP = (ushort)this.Character.MaxMP;
                p1.SP = (ushort)this.Character.MaxSP;
                uint count = 0;
                foreach (uint i in this.Character.Inventory.MaxVolume.Values)
                {
                    count += i;
                }
                p1.Capacity = (ushort)count;
                count = 0;
                foreach (uint i in this.Character.Inventory.MaxPayload.Values)
                {
                    count += i;
                }
                p1.Payload = (ushort)count;

                this.netIO.SendPacket(p1);


            }
            /*{
                Packets.Server.SSMG_PLAYER_STATS_PRE_CALC p1 = new SagaMap.Packets.Server.SSMG_PLAYER_STATS_PRE_CALC();
                p1.ASPD = this.Character.Status.aspd;
                p1.ATK1Max = this.Character.Status.max_atk1;
                p1.ATK1Min = this.Character.Status.min_atk1;
                p1.ATK2Max = this.Character.Status.max_atk2;
                p1.ATK2Min = this.Character.Status.min_atk2;
                p1.ATK3Max = this.Character.Status.max_atk3;
                p1.ATK3Min = this.Character.Status.min_atk3;
                p1.AvoidCritical = this.Character.Status.avoid_critical;
                p1.AvoidMagic = this.Character.Status.avoid_magic;
                p1.AvoidMelee = this.Character.Status.avoid_melee;
                p1.AvoidRanged = this.Character.Status.avoid_ranged;
                p1.CSPD = this.Character.Status.cspd;
                p1.DefAddition = (ushort)this.Character.Status.def_add;
                p1.DefBase = this.Character.Status.def;
                p1.HitCritical = this.Character.Status.hit_critical;
                p1.HitMagic = this.Character.Status.hit_magic;
                p1.HitMelee = this.Character.Status.hit_melee;
                p1.HitRanged = this.Character.Status.hit_ranged;
                p1.MATKMax = this.Character.Status.max_matk;
                p1.MATKMin = this.Character.Status.min_matk;
                p1.MDefAddition = (ushort)this.Character.Status.mdef_add;
                p1.MDefBase = this.Character.Status.mdef;
                p1.Speed = this.Character.Speed;
                p1.HP = (ushort)this.Character.MaxHP;
                p1.MP = (ushort)this.Character.MaxMP;
                p1.SP = (ushort)this.Character.MaxSP;
                uint count = 0;
                foreach (uint i in this.Character.Inventory.MaxVolume.Values)
                {
                    count += i;
                }
                p1.Capacity = (ushort)count;
                count = 0;
                foreach (uint i in this.Character.Inventory.MaxPayload.Values)
                {
                    count += i;
                }
                p1.Payload = (ushort)count;

                this.netIO.SendPacket(p1);
            }
            {
                Packets.Server.SSMG_DEM_STATS_PRE_CALC p1 = new SagaMap.Packets.Server.SSMG_DEM_STATS_PRE_CALC();
                p1.ASPD = this.Character.Status.aspd;
                p1.ATK1Max = this.Character.Status.max_atk1;
                p1.ATK1Min = this.Character.Status.min_atk1;
                p1.ATK2Max = this.Character.Status.max_atk2;
                p1.ATK2Min = this.Character.Status.min_atk2;
                p1.ATK3Max = this.Character.Status.max_atk3;
                p1.ATK3Min = this.Character.Status.min_atk3;
                p1.AvoidCritical = this.Character.Status.avoid_critical;
                p1.AvoidMagic = this.Character.Status.avoid_magic;
                p1.AvoidMelee = this.Character.Status.avoid_melee;
                p1.AvoidRanged = this.Character.Status.avoid_ranged;
                p1.CSPD = this.Character.Status.cspd;
                p1.DefAddition = (ushort)this.Character.Status.def_add;
                p1.DefBase = this.Character.Status.def;
                p1.HitCritical = this.Character.Status.hit_critical;
                p1.HitMagic = this.Character.Status.hit_magic;
                p1.HitMelee = this.Character.Status.hit_melee;
                p1.HitRanged = this.Character.Status.hit_ranged;
                p1.MATKMax = this.Character.Status.max_matk;
                p1.MATKMin = this.Character.Status.min_matk;
                p1.MDefAddition = (ushort)this.Character.Status.mdef_add;
                p1.MDefBase = this.Character.Status.mdef;
                p1.Speed = this.Character.Speed;
                p1.HP = (ushort)this.Character.MaxHP;
                p1.MP = (ushort)this.Character.MaxMP;
                p1.SP = (ushort)this.Character.MaxSP;
                uint count = 0;
                foreach (uint i in this.Character.Inventory.MaxVolume.Values)
                {
                    count += i;
                }
                p1.Capacity = (ushort)count;
                count = 0;
                foreach (uint i in this.Character.Inventory.MaxPayload.Values)
                {
                    count += i;
                }
                p1.Payload = (ushort)count;

                this.netIO.SendPacket(p1);
            }*/

            //resotre
            this.Character.Str = str;
            this.Character.Dex = dex;
            this.Character.Int = intel;
            this.Character.Agi = agi;
            this.Character.Vit = vit;
            this.Character.Mag = mag;

            StatusFactory.Instance.CalcStatus(this.Character);
        }

        public void OnDEMChipCategory(Packets.Client.CSMG_DEM_CHIP_CATEGORY p)
        {
            if (chipShop)
            {
                if (ChipShopFactory.Instance.Items.ContainsKey(p.Category))
                {
                    currentChipCategory = p.Category;
                    ChipShopCategory category = ChipShopFactory.Instance.Items[p.Category];
                    Packets.Server.SSMG_DEM_CHIP_SHOP_HEADER p1 = new SagaMap.Packets.Server.SSMG_DEM_CHIP_SHOP_HEADER();
                    p1.CategoryID = p.Category;
                    this.netIO.SendPacket(p1);

                    foreach (ShopChip i in category.Items.Values)
                    {
                        Packets.Server.SSMG_DEM_CHIP_SHOP_DATA p2 = new SagaMap.Packets.Server.SSMG_DEM_CHIP_SHOP_DATA();
                        p2.EXP = (uint)i.EXP;
                        p2.JEXP = (uint)i.JEXP;
                        p2.ItemID = i.ItemID;
                        p2.Description = i.Description;
                        this.netIO.SendPacket(p2);
                    }

                    Packets.Server.SSMG_DEM_CHIP_SHOP_FOOTER p3 = new SagaMap.Packets.Server.SSMG_DEM_CHIP_SHOP_FOOTER();
                    this.netIO.SendPacket(p3);
                }
            }
        }

        public void OnDEMChipClose(Packets.Client.CSMG_DEM_CHIP_CLOSE p)
        {
            chipShop = false;
        }

        public void OnDEMChipBuy(Packets.Client.CSMG_DEM_CHIP_BUY p)
        {
            if (chipShop)
            {
                uint[] items = p.ItemIDs;
                int[] counts = p.Counts;

                for (int i = 0; i < items.Length; i++)
                {
                    var cat = from item in ChipShopFactory.Instance.Items.Values
                              where item.Items.ContainsKey(items[i])
                              select item;

                    if (cat.Count() > 0)
                    {
                        ChipShopCategory category = cat.First();
                        if (counts[i] > 0)
                        {
                            ShopChip chip = category.Items[items[i]];
                            if ((this.chara.CEXP > chip.EXP * (ulong)counts[i]) &&
                                this.chara.JEXP > chip.JEXP * (ulong)counts[i])
                            {
                                this.chara.CEXP -= (uint)(chip.EXP * (ulong)counts[i]);
                                this.chara.JEXP -= (uint)(chip.JEXP * (ulong)counts[i]);
                                SagaDB.Item.Item item = ItemFactory.Instance.GetItem(items[i]);
                                item.Stack = (ushort)counts[i];
                                AddItem(item, true);
                            }
                        }
                    }
                    SendEXP();
                }
            }
        }


        //ECOKEY DEM修復，DEM強化專用數值計算
        public short FindEnhancementValueDEM(Item item, uint itemID)
        {
            short[] hps = new short[31] { 0,
                                          100, 20, 70,  30,  80,  40,  90,  50,  100, 150,
                                          150, 60, 110, 70,  200, 200, 120, 80,  130, 250,
                                          250, 90, 140, 100, 250, 250, 150, 110, 160, 400  };
            short[] atk_def_matk = new short[31] { 0,
                                           10, 3, 5,  3, 6,  3,  7,  3, 8,  13,
                                           13, 3, 9,  3, 15, 15, 10, 3, 11, 20,
                                           20, 3, 12, 3, 22, 22, 13, 3, 14, 25 };
            short[] mdef = new short[31] { 0,
                                           10, 2, 5, 2, 6,  3,  6, 3, 6, 15,
                                           15, 4, 7, 4, 10, 10, 7, 4, 7, 15,
                                           15, 5, 8, 5, 15, 15, 8, 5, 8, 25 };
            short[] cris = new short[31] { 0,
                                           5, 1, 3, 2, 4, 3, 4, 3, 5, 9,
                                           5, 1, 2, 3, 4, 5, 1, 2, 3, 4,
                                           5, 1, 2, 3, 4, 5, 1, 2, 3, 5 };
            ushort r = item.Refine;
            //新增其他結晶
            switch (itemID)
            {
                case 90000043:
                case 90000053:
                case 10087400:
                case 16004600:
                    /*if (item.IsArmor)
                    {
                        return hps[r + 1];
                    }*/
                    if (item.BaseData.itemType == ItemType.PARTS_BODY)
                    {
                        return hps[r + 1];
                    }
                    break;
                case 90000044:
                case 90000054:
                case 10087401:
                case 16004700:
                    if (item.BaseData.itemType == ItemType.PARTS_BODY)
                    {
                        return atk_def_matk[r + 1];
                    }
                    else
                    {
                        return atk_def_matk[r + 1];
                    }
                    break;
                case 90000045:
                case 90000055:
                case 10087403:
                case 16004800:
                    if (item.BaseData.itemType == ItemType.PARTS_BODY)
                    {
                        return mdef[r + 1];
                    }
                    else
                    {
                        return atk_def_matk[r + 1];
                    }
                case 90000046:
                case 90000056:
                case 10087402:
                case 16004500:
                    if (item.BaseData.itemType == ItemType.PARTS_BODY)
                    {
                        return cris[r + 1];
                    }
                    else
                    {
                        return cris[r + 1];
                    }
                    break;
            }
            return 0;
        }

        //ECOKEY DEM修復，DEM強化關閉
        public void OnItemEnhanceCloseDEM(Packets.Client.CSMG_DEM_ITEM_ENHANCE_CLOSE p)
        {
            this.itemEnhance = false;
            this.irisAddSlot = false;
        }
        //ECOKEY DEM修復，DEM強化專用
        public void OnItemEnhanceConfirmDEM(Packets.Client.CSMG_DEM_ITEM_ENHANCE_CONFIRM p)
        {
            if (p.Material >= 90000047 && p.Material <= 90000052)
            {
                OnItemElementsEnhanceConfirmDEM(p);
                return;
            }
            Item item = this.Character.Inventory.GetItem(p.InventorySlot);
            bool failed = false;
            bool success = false;
            Packets.Server.SSMG_DEM_ITEM_ENHANCE_RESULT p1 = new SagaMap.Packets.Server.SSMG_DEM_ITEM_ENHANCE_RESULT();
            p1.Result = 0;
            if (item != null)
            {
                if (CountItem(p.Material) > 0 && item.Refine < 30)
                {
                    if (this.Character.Gold >= 5000)
                    {
                        this.Character.Gold -= 5000;

                        Logger.ShowInfo("Refine Item:" + item.BaseData.name + "[" + p.InventorySlot + "] Protect Item: " +
                            (ItemFactory.Instance.GetItem(p.ProtectItem).ItemID != 10000000 ? ItemFactory.Instance.GetItem(p.ProtectItem).BaseData.name : "None") +
                            Environment.NewLine + "Material: " + (ItemFactory.Instance.GetItem(p.Material).ItemID != 10000000 ? ItemFactory.Instance.GetItem(p.Material).BaseData.name : "None") +
                            " SupportItem: " + (ItemFactory.Instance.GetItem(p.SupportItem).ItemID != 10000000 ? ItemFactory.Instance.GetItem(p.SupportItem).BaseData.name : "None"));

                        var Material = p.Material;

                        uint supportitemid = 0;
                        var enhancesupported = false;
                        if (ItemFactory.Instance.GetItem(p.SupportItem).ItemID != 10000000)
                        {
                            enhancesupported = true;
                            supportitemid = ItemFactory.Instance.GetItem((uint)p.SupportItem).ItemID;
                        }

                        uint protectitemid = 0;
                        var enhanceprotected = false;
                        if (ItemFactory.Instance.GetItem(p.ProtectItem).ItemID != 10000000)
                        {
                            enhanceprotected = true;
                            protectitemid = ItemFactory.Instance.GetItem((uint)p.ProtectItem).ItemID;
                        }

                        int finalrate = 0;
                        string used_material = "";
                        int crystal_addon = 0;
                        int skill_addon = 0;
                        int protect_addon = 0;
                        int support_addon = 0;
                        int matsuri_addon = 0;
                        int recycle_addon = 0;
                        int nextlevel = ((int)item.Refine + (int)1);

                        //BaseRate
                        finalrate += EnhanceTableFactory.Instance.Table[nextlevel].BaseRate;

                        //一般結晶
                        if (p.Material >= 90000043 && p.Material <= 90000046)
                            crystal_addon = EnhanceTableFactory.Instance.Table[nextlevel].Crystal;


                        //強化結晶
                        if (p.Material >= 90000053 && p.Material <= 90000056)
                            crystal_addon = EnhanceTableFactory.Instance.Table[nextlevel].EnhanceCrystal;

                        //超強化結晶
                        if (p.Material >= 16004500 && p.Material <= 16004800)
                            crystal_addon = EnhanceTableFactory.Instance.Table[nextlevel].SPCrystal;

                        //強化王結晶
                        if (p.Material >= 10087400 && p.Material <= 10087403)
                            crystal_addon = EnhanceTableFactory.Instance.Table[nextlevel].KingCrystal;

                        //防爆
                        if (enhanceprotected && protectitemid == 16001300)
                            protect_addon = EnhanceTableFactory.Instance.Table[nextlevel].ExplodeProtect;

                        //防重設
                        if (enhanceprotected && protectitemid == 10118200)
                            protect_addon = EnhanceTableFactory.Instance.Table[nextlevel].ResetProtect;

                        //奧義
                        if (ItemFactory.Instance.GetItem(p.ProtectItem).ItemID == 10087200)
                            support_addon = EnhanceTableFactory.Instance.Table[nextlevel].Okugi;

                        //神髓
                        if (ItemFactory.Instance.GetItem(p.ProtectItem).ItemID == 10087201)
                            support_addon = EnhanceTableFactory.Instance.Table[nextlevel].Shinzui;

                        //強化祭活動
                        if (SagaMap.Configuration.Instance.EnhanceMatsuri)
                            matsuri_addon = EnhanceTableFactory.Instance.Table[nextlevel].Matsuri;


                        //回收活動
                        //未實裝(需連動回收系統)
                        /*
                        if (SagaMap.Configuration.Instance.Recycle)
                            finalrate += EnhanceTableFactory.Instance.Table[nextlevel].Recycle;
                        */

                        //被動技能加成
                        //一般結晶
                        if (p.Material >= 90000043 && p.Material <= 90000046)
                        {

                            if (p.Material == 90000043 && this.Character.Status.Additions.ContainsKey("BoostHp"))
                            {
                                used_material = "強化技能-命";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostHp"] as DefaultPassiveSkill).Variable["BoostHp"]);
                            }
                            if (p.Material == 90000044 && this.Character.Status.Additions.ContainsKey("BoostPower"))
                            {
                                used_material = "強化技能-力";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostPower"] as DefaultPassiveSkill).Variable["BoostPower"]);
                            }

                            if (p.Material == 90000045 && this.Character.Status.Additions.ContainsKey("BoostMagic"))
                            {
                                used_material = "強化技能-魔";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostMagic"] as DefaultPassiveSkill).Variable["BoostMagic"]);
                            }

                            if (p.Material == 90000046 && this.Character.Status.Additions.ContainsKey("BoostCritical"))
                            {
                                used_material = "強化技能-暴擊";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostCritical"] as DefaultPassiveSkill).Variable["BoostCritical"]);
                            }
                        }

                        //強化結晶
                        if (p.Material >= 90000053 && p.Material <= 90000056)
                        {

                            if (p.Material == 90000053 && this.Character.Status.Additions.ContainsKey("BoostHp"))
                            {
                                used_material = "強化技能-命";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostHp"] as DefaultPassiveSkill).Variable["BoostHp"]);
                            }
                            if (p.Material == 90000054 && this.Character.Status.Additions.ContainsKey("BoostPower"))
                            {
                                used_material = "強化技能-力";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostPower"] as DefaultPassiveSkill).Variable["BoostPower"]);
                            }

                            if (p.Material == 90000055 && this.Character.Status.Additions.ContainsKey("BoostMagic"))
                            {
                                used_material = "強化技能-魔";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostMagic"] as DefaultPassiveSkill).Variable["BoostMagic"]);
                            }

                            if (p.Material == 90000056 && this.Character.Status.Additions.ContainsKey("BoostCritical"))
                            {
                                used_material = "強化技能-暴擊";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostCritical"] as DefaultPassiveSkill).Variable["BoostCritical"]);
                            }
                        }

                        //超強化結晶
                        if (p.Material >= 16004500 && p.Material <= 16004800)
                        {

                            if (p.Material == 16004600 && this.Character.Status.Additions.ContainsKey("BoostHp"))
                            {
                                used_material = "強化技能-命";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostHp"] as DefaultPassiveSkill).Variable["BoostHp"]);
                            }
                            if (p.Material == 16004700 && this.Character.Status.Additions.ContainsKey("BoostPower"))
                            {
                                used_material = "強化技能-力";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostPower"] as DefaultPassiveSkill).Variable["BoostPower"]);
                            }

                            if (p.Material == 16004800 && this.Character.Status.Additions.ContainsKey("BoostMagic"))
                            {
                                used_material = "強化技能-魔";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostMagic"] as DefaultPassiveSkill).Variable["BoostMagic"]);
                            }

                            if (p.Material == 16004500 && this.Character.Status.Additions.ContainsKey("BoostCritical"))
                            {
                                used_material = "強化技能-暴擊";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostCritical"] as DefaultPassiveSkill).Variable["BoostCritical"]);
                            }
                        }

                        //強化王結晶
                        if (p.Material >= 10087400 && p.Material <= 10087403)
                        {

                            if (p.Material == 10087400 && this.Character.Status.Additions.ContainsKey("BoostHp"))
                            {
                                used_material = "強化技能-命";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostHp"] as DefaultPassiveSkill).Variable["BoostHp"]);
                            }
                            if (p.Material == 10087401 && this.Character.Status.Additions.ContainsKey("BoostPower"))
                            {
                                used_material = "強化技能-力";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostPower"] as DefaultPassiveSkill).Variable["BoostPower"]);
                            }

                            if (p.Material == 10087403 && this.Character.Status.Additions.ContainsKey("BoostMagic"))
                            {
                                used_material = "強化技能-魔";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostMagic"] as DefaultPassiveSkill).Variable["BoostMagic"]);
                            }

                            if (p.Material == 10087402 && this.Character.Status.Additions.ContainsKey("BoostCritical"))
                            {
                                used_material = "強化技能-暴擊";
                                skill_addon = (int)((int)(this.Character.Status.Additions["BoostCritical"] as DefaultPassiveSkill).Variable["BoostCritical"]);
                            }
                        }


                        //結算
                        finalrate += crystal_addon + matsuri_addon + skill_addon + protect_addon + support_addon + recycle_addon;


                        FromActorPC(Character).SendSystemMessage("----------------強化成功率結算---------------");
                        FromActorPC(Character).SendSystemMessage("正強化裝備到：" + ((int)item.Refine + (int)1));

                        FromActorPC(Character).SendSystemMessage("基本機率：" + (EnhanceTableFactory.Instance.Table[nextlevel].BaseRate / 100) + "%");
                        FromActorPC(Character).SendSystemMessage("結晶加成：" + (crystal_addon / 100) + "%");
                        FromActorPC(Character).SendSystemMessage("補助加成：" + (support_addon / 100) + "%");
                        FromActorPC(Character).SendSystemMessage("防爆加成：" + (protect_addon / 100) + "%");
                        FromActorPC(Character).SendSystemMessage("強化祭加成：" + (matsuri_addon / 100) + "%");
                        FromActorPC(Character).SendSystemMessage("被動技加成：" + used_material + " -" + (skill_addon / 100) + "%");
                        FromActorPC(Character).SendSystemMessage("回收活动加成：" + (recycle_addon / 100) + "%");

                        FromActorPC(Character).SendSystemMessage("你的最終強化成功率為：" + (finalrate / 100) + "%");
                        FromActorPC(Character).SendSystemMessage("----------------強化成功率結算---------------");

                        if (finalrate > 9999)
                            finalrate = 9999;

                        int refrate = Global.Random.Next(0, 9999);
                        if (enhanceprotected)
                        {
                            if (CountItem(protectitemid) < 1)
                            {
                                p1.Result = 0x00;
                                p1.OrignalRefine = item.Refine;
                                p1.ExpectedRefine = (ushort)(item.Refine + 1);
                                this.netIO.SendPacket(p1);
                                p1.Result = 0xfc;
                                this.netIO.SendPacket(p1);
                                this.itemEnhance = false;
                                return;
                            }
                            else
                                DeleteItemID(protectitemid, 1, true);
                        }

                        if (enhancesupported)
                        {
                            if (CountItem(supportitemid) < 1)
                            {
                                p1.Result = 0x00;
                                p1.OrignalRefine = item.Refine;
                                p1.ExpectedRefine = (ushort)(item.Refine + 1);
                                this.netIO.SendPacket(p1);
                                p1.Result = 0xfc;
                                this.netIO.SendPacket(p1);
                                this.itemEnhance = false;
                                return;
                            }
                            else
                                DeleteItemID(supportitemid, 1, true);
                        }


                        if (CountItem(Material) < 1)
                        {
                            p1.Result = 0x00;
                            p1.OrignalRefine = item.Refine;
                            p1.ExpectedRefine = (ushort)(item.Refine + 1);
                            this.netIO.SendPacket(p1);
                            p1.Result = 0xfc;
                            this.netIO.SendPacket(p1);
                            this.itemEnhance = false;
                            return;
                        }

                        if (refrate <= finalrate)
                        {
                            if (item.BaseData.itemType == ItemType.PARTS_BODY)
                            {
                                switch (p.Material)
                                {
                                    //一般水晶
                                    case 90000043:
                                        item.HP += FindEnhancementValueDEM(item, 90000043);
                                        DeleteItemID(90000043, 1, true);
                                        item.LifeEnhance++;
                                        break;
                                    case 90000044:
                                        item.Def += FindEnhancementValueDEM(item, 90000044);
                                        DeleteItemID(90000044, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 90000045:
                                        item.MDef += FindEnhancementValueDEM(item, 90000045);
                                        DeleteItemID(90000045, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 90000046:
                                        item.AvoidCritical += FindEnhancementValueDEM(item, 90000046);
                                        DeleteItemID(90000046, 1, true);
                                        item.CritEnhance++;
                                        break;


                                    //強化水晶
                                    case 90000053:
                                        item.HP += FindEnhancementValueDEM(item, 90000043);
                                        DeleteItemID(90000053, 1, true);
                                        item.LifeEnhance++;
                                        break;
                                    case 90000054:
                                        item.Def += FindEnhancementValueDEM(item, 90000054);
                                        DeleteItemID(90000054, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 90000055:
                                        item.MDef += FindEnhancementValueDEM(item, 90000055);
                                        DeleteItemID(90000055, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 90000056:
                                        item.AvoidCritical += FindEnhancementValueDEM(item, 90000056);
                                        DeleteItemID(90000056, 1, true);
                                        item.CritEnhance++;
                                        break;


                                    //超強化水晶
                                    case 16004600:
                                        item.HP += FindEnhancementValueDEM(item, 16004600);
                                        DeleteItemID(16004600, 1, true);
                                        item.LifeEnhance++;
                                        break;
                                    case 16004700:
                                        item.Def += FindEnhancementValueDEM(item, 16004700);
                                        DeleteItemID(16004700, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 16004800:
                                        item.MDef += FindEnhancementValueDEM(item, 16004800);
                                        DeleteItemID(16004800, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 16004500:
                                        item.AvoidCritical += FindEnhancementValueDEM(item, 16004500);
                                        DeleteItemID(16004500, 1, true);
                                        item.CritEnhance++;
                                        break;


                                    //強化王
                                    case 10087400:
                                        item.HP += FindEnhancementValueDEM(item, 10087400);
                                        DeleteItemID(10087400, 1, true);
                                        item.LifeEnhance++;
                                        break;
                                    case 10087401:
                                        item.Def += FindEnhancementValueDEM(item, 10087401);
                                        DeleteItemID(10087401, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 10087403:
                                        item.MDef += FindEnhancementValueDEM(item, 10087403);
                                        DeleteItemID(10087403, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 10087402:
                                        item.AvoidCritical += FindEnhancementValueDEM(item, 10087402);
                                        DeleteItemID(10087402, 1, true);
                                        item.CritEnhance++;
                                        break;

                                }
                            }
                            if (item.BaseData.itemType == ItemType.PARTS_BLOW ||
                                item.BaseData.itemType == ItemType.PARTS_SLASH ||
                                item.BaseData.itemType == ItemType.PARTS_STAB ||
                                item.BaseData.itemType == ItemType.PARTS_LONGRANGE ||
                                item.BaseData.itemType == ItemType.SETPARTS_BLOW ||
                                item.BaseData.itemType == ItemType.SETPARTS_SLASH ||
                                item.BaseData.itemType == ItemType.SETPARTS_STAB ||
                                item.BaseData.itemType == ItemType.SETPARTS_LONGRANGE)
                            {
                                switch (p.Material)
                                {
                                    //一般水晶
                                    case 90000044:
                                        item.Atk1 += FindEnhancementValueDEM(item, 90000044);
                                        item.Atk2 = item.Atk1;
                                        item.Atk3 = item.Atk1;
                                        DeleteItemID(90000044, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 90000045:
                                        item.MAtk += FindEnhancementValueDEM(item, 90000045);
                                        DeleteItemID(90000045, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 90000046:
                                        item.HitCritical += FindEnhancementValueDEM(item, 90000046);
                                        DeleteItemID(90000046, 1, true);
                                        item.CritEnhance++;
                                        break;

                                    //強化水晶
                                    case 90000054:
                                        item.Atk1 += FindEnhancementValueDEM(item, 90000054);
                                        item.Atk2 = item.Atk1;
                                        item.Atk3 = item.Atk1;
                                        DeleteItemID(90000054, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 90000055:
                                        item.MAtk += FindEnhancementValueDEM(item, 90000055);
                                        DeleteItemID(90000055, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 90000056:
                                        item.HitCritical += FindEnhancementValueDEM(item, 90000056);
                                        DeleteItemID(90000056, 1, true);
                                        item.CritEnhance++;
                                        break;

                                    //超強化水晶
                                    case 16004700:
                                        item.Atk1 += FindEnhancementValueDEM(item, 16004700);
                                        item.Atk2 = item.Atk1;
                                        item.Atk3 = item.Atk1;
                                        DeleteItemID(16004700, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 16004800:
                                        item.MAtk += FindEnhancementValueDEM(item, 16004800);
                                        DeleteItemID(16004800, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 16004500:
                                        item.HitCritical += FindEnhancementValueDEM(item, 16004500);
                                        DeleteItemID(16004500, 1, true);
                                        item.CritEnhance++;
                                        break;

                                    //強化王水晶
                                    case 10087401:
                                        item.Atk1 += FindEnhancementValueDEM(item, 10087401);
                                        item.Atk2 = item.Atk1;
                                        item.Atk3 = item.Atk1;
                                        DeleteItemID(10087401, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 10087403:
                                        item.MAtk += FindEnhancementValueDEM(item, 10087403);
                                        DeleteItemID(10087403, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 10087402:
                                        item.HitCritical += FindEnhancementValueDEM(item, 10087402);
                                        DeleteItemID(10087402, 1, true);
                                        item.CritEnhance++;
                                        break;

                                }
                            }
                            SendEffect(5145);
                            p1.Result = 1;
                            success = true;
                            item.Refine++;
                            SendItemInfo(item);

                            PC.StatusFactory.Instance.CalcStatus(this.chara);
                            SendPlayerInfo();
                        }
                        else
                        {
                            failed = true;
                            SendEffect(5146);
                            if (!enhanceprotected)
                            {
                                DeleteItem(p.InventorySlot, 1, true);

                                //Delete Material..
                                if (p.Material >= 10087400 && p.Material <= 10087403)
                                {
                                    //No Delete Material
                                }
                                else if (p.Material >= 16004500 && p.Material <= 16004800)//超強化結晶不會消失
                                {
                                    //No Delete Material
                                }
                                else
                                {
                                    DeleteItemID(p.Material, 1, true);
                                }

                            }
                            else
                            {
                                //Delete Material..
                                if (p.Material >= 10087400 && p.Material <= 10087403)
                                {
                                    //No Delete Material
                                }
                                else if (p.Material >= 16004500 && p.Material <= 16004800)//超強化結晶不會消失
                                {
                                    //No Delete Material
                                }
                                else
                                {
                                    DeleteItemID(p.Material, 1, true);
                                }
                                //關閉重製
                                //Reset Protected Only
                                /*if (supportitemid != 10118200)
                                {
                                    item.Clear();
                                    SendItemInfo(item);
                                }*/

                                //Explode Protected Only
                                if (protectitemid != 16001300)
                                    DeleteItem(p.InventorySlot, 1, true);

                                PC.StatusFactory.Instance.CalcStatus(this.chara);
                                SendPlayerInfo();
                                failed = true;
                            }

                            p1.Result = 0;
                            p1.OrignalRefine = (ushort)(item.Refine + 1);
                            p1.ExpectedRefine = item.Refine;
                            this.netIO.SendPacket(p1);
                            p1.Result = 0x00;
                        }
                    }
                    else
                    {
                        p1.Result = 0x00;
                        p1.OrignalRefine = item.Refine;
                        p1.ExpectedRefine = (ushort)(item.Refine + 1);
                        this.netIO.SendPacket(p1);
                        p1.Result = 0xff;
                    }
                }
                else
                {
                    p1.Result = 0x00;
                    p1.OrignalRefine = item.Refine;
                    p1.ExpectedRefine = (ushort)(item.Refine + 1);
                    this.netIO.SendPacket(p1);
                    p1.Result = 0xfd;
                }
            }
            else
            {
                p1.Result = 0x00;
                p1.OrignalRefine = item.Refine;
                p1.ExpectedRefine = (ushort)(item.Refine + 1);
                this.netIO.SendPacket(p1);
                p1.Result = 0xfe;
            }
            if (success)
            {
                p1.Result = 1;
                p1.OrignalRefine = (ushort)(item.Refine + 1);
                p1.ExpectedRefine = item.Refine;
                this.netIO.SendPacket(p1);
                /*Packets.Client.CSMG_ITEM_ENHANCE_SELECT p3 = new SagaMap.Packets.Client.CSMG_ITEM_ENHANCE_SELECT();
                p3.InventorySlot = p.InventorySlot;
                OnItemEnhanceSelect(p3);*/

                var res = from enhanctitem in this.chara.Inventory.GetContainer(ContainerType.BODY)
                          where (((enhanctitem.BaseData.itemType == ItemType.PARTS_BODY && ((CountItem(90000044) > 0 || CountItem(90000045) > 0 || CountItem(90000046) > 0 || CountItem(90000043) > 0 || CountItem(10087400) > 0 || CountItem(10087401) > 0) || CountItem(10087402) > 0 || CountItem(10087403) > 0))
                            || (enhanctitem.BaseData.itemType == ItemType.PARTS_BLOW ||
                        enhanctitem.BaseData.itemType == ItemType.PARTS_SLASH ||
                        enhanctitem.BaseData.itemType == ItemType.PARTS_STAB ||
                        enhanctitem.BaseData.itemType == ItemType.PARTS_LONGRANGE ||
                        enhanctitem.BaseData.itemType == ItemType.SETPARTS_BLOW ||
                        item.BaseData.itemType == ItemType.SETPARTS_SLASH ||
                        item.BaseData.itemType == ItemType.SETPARTS_STAB ||
                        item.BaseData.itemType == ItemType.SETPARTS_LONGRANGE && (CountItem(90000044) > 0 || CountItem(90000045) > 0 || CountItem(90000046) > 0 || CountItem(10087401) > 0) || CountItem(10087402) > 0 || CountItem(10087403) > 0))
                            && enhanctitem.Refine < 30 && this.Character.Gold >= 5000)
                          select enhanctitem;
                List<SagaDB.Item.Item> items = res.ToList();

                foreach (var itemsitem in res.ToList())
                {
                    if (itemsitem.BaseData.itemType == ItemType.PARTS_BODY)
                    {
                        //生命结晶
                        if (CountItem(90000043) > 0)
                            if (!items.Exists(x => x.ItemID == 90000043))
                                items.AddRange(GetItem(90000043));
                        //力量结晶
                        if (CountItem(90000044) > 0)
                            if (!items.Exists(x => x.ItemID == 90000044))
                                items.AddRange(GetItem(90000044));
                        //会心结晶
                        if (CountItem(90000046) > 0)
                            if (!items.Exists(x => x.ItemID == 90000046))
                                items.AddRange(GetItem(90000046));
                        //魔力结晶
                        if (CountItem(90000045) > 0)
                            if (!items.Exists(x => x.ItemID == 90000045))
                                items.AddRange(GetItem(90000045));

                        //生命強化结晶
                        if (CountItem(90000053) > 0)
                            if (!items.Exists(x => x.ItemID == 90000053))
                                items.AddRange(GetItem(90000053));
                        //力量強化结晶
                        if (CountItem(90000054) > 0)
                            if (!items.Exists(x => x.ItemID == 90000054))
                                items.AddRange(GetItem(90000054));
                        //会心強化结晶
                        if (CountItem(90000056) > 0)
                            if (!items.Exists(x => x.ItemID == 90000056))
                                items.AddRange(GetItem(90000056));
                        //魔力強化结晶
                        if (CountItem(90000055) > 0)
                            if (!items.Exists(x => x.ItemID == 90000055))
                                items.AddRange(GetItem(90000055));



                        //生命超強化结晶
                        if (CountItem(16004600) > 0)
                            if (!items.Exists(x => x.ItemID == 16004600))
                                items.AddRange(GetItem(16004600));
                        //力量超強化结晶
                        if (CountItem(16004700) > 0)
                            if (!items.Exists(x => x.ItemID == 16004700))
                                items.AddRange(GetItem(16004700));
                        //会心超強化结晶
                        if (CountItem(16004500) > 0)
                            if (!items.Exists(x => x.ItemID == 16004500))
                                items.AddRange(GetItem(16004500));
                        //魔力超強化结晶
                        if (CountItem(16004800) > 0)
                            if (!items.Exists(x => x.ItemID == 16004800))
                                items.AddRange(GetItem(16004800));


                        //强化王的生命
                        if (CountItem(10087400) > 0)
                            if (!items.Exists(x => x.ItemID == 10087400))
                                items.AddRange(GetItem(10087400));
                        //强化王的力量
                        if (CountItem(10087401) > 0)
                            if (!items.Exists(x => x.ItemID == 10087401))
                                items.AddRange(GetItem(10087401));
                        //强化王的会心
                        if (CountItem(10087402) > 0)
                            if (!items.Exists(x => x.ItemID == 10087402))
                                items.AddRange(GetItem(10087402));
                        //强化王的魔力
                        if (CountItem(10087403) > 0)
                            if (!items.Exists(x => x.ItemID == 10087403))
                                items.AddRange(GetItem(10087403));

                    }
                    if (itemsitem.BaseData.itemType == ItemType.PARTS_BLOW ||
                        itemsitem.BaseData.itemType == ItemType.PARTS_SLASH ||
                        itemsitem.BaseData.itemType == ItemType.PARTS_STAB ||
                        itemsitem.BaseData.itemType == ItemType.PARTS_LONGRANGE ||
                        itemsitem.BaseData.itemType == ItemType.SETPARTS_BLOW ||
                        item.BaseData.itemType == ItemType.SETPARTS_SLASH ||
                        item.BaseData.itemType == ItemType.SETPARTS_STAB ||
                        item.BaseData.itemType == ItemType.SETPARTS_LONGRANGE)
                    {

                        //力量结晶
                        if (CountItem(90000044) > 0)
                            if (!items.Exists(x => x.ItemID == 90000044))
                                items.AddRange(GetItem(90000044));
                        //会心结晶
                        if (CountItem(90000046) > 0)
                            if (!items.Exists(x => x.ItemID == 90000046))
                                items.AddRange(GetItem(90000046));
                        //魔力结晶
                        if (CountItem(90000045) > 0)
                            if (!items.Exists(x => x.ItemID == 90000045))
                                items.AddRange(GetItem(90000045));


                        //力量強化结晶
                        if (CountItem(90000054) > 0)
                            if (!items.Exists(x => x.ItemID == 90000054))
                                items.AddRange(GetItem(90000054));
                        //会心強化结晶
                        if (CountItem(90000056) > 0)
                            if (!items.Exists(x => x.ItemID == 90000056))
                                items.AddRange(GetItem(90000056));
                        //魔力強化结晶
                        if (CountItem(90000055) > 0)
                            if (!items.Exists(x => x.ItemID == 90000055))
                                items.AddRange(GetItem(90000055));




                        //力量超強化结晶
                        if (CountItem(16004700) > 0)
                            if (!items.Exists(x => x.ItemID == 16004700))
                                items.AddRange(GetItem(16004700));
                        //会心超強化结晶
                        if (CountItem(16004500) > 0)
                            if (!items.Exists(x => x.ItemID == 16004500))
                                items.AddRange(GetItem(16004500));
                        //魔力超強化结晶
                        if (CountItem(16004800) > 0)
                            if (!items.Exists(x => x.ItemID == 16004800))
                                items.AddRange(GetItem(16004800));


                        //强化王的力量
                        if (CountItem(10087401) > 0)
                            if (!items.Exists(x => x.ItemID == 10087401))
                                items.AddRange(GetItem(10087401));
                        //强化王的会心
                        if (CountItem(10087402) > 0)
                            if (!items.Exists(x => x.ItemID == 10087402))
                                items.AddRange(GetItem(10087402));
                        //强化王的魔力
                        if (CountItem(10087403) > 0)
                            if (!items.Exists(x => x.ItemID == 10087403))
                                items.AddRange(GetItem(10087403));



                    }

                    //Golbal Item

                    //防爆判断
                    if (CountItem(16001300) > 0)
                        if (!items.Exists(x => x.ItemID == 16001300))
                            items.AddRange(GetItem(16001300));

                    //防RESET判断
                    if (CountItem(10118200) > 0)
                        if (!items.Exists(x => x.ItemID == 10118200))
                            items.AddRange(GetItem(10118200));


                    //奥义判断
                    if (CountItem(10087200) > 0)
                        if (!items.Exists(x => x.ItemID == 10087200))
                            items.AddRange(GetItem(10087200));
                    //精髓判断
                    if (CountItem(10087201) > 0)
                        if (!items.Exists(x => x.ItemID == 10087201))
                            items.AddRange(GetItem(10087201));
                }

                items = items.OrderBy(x => x.Slot).ToList();
                if (items.Count > 0)
                {
                    Packets.Server.SSMG_DEM_ITEM_ENHANCE_LIST p2 = new SagaMap.Packets.Server.SSMG_DEM_ITEM_ENHANCE_LIST();
                    p2.Items = items;
                    this.netIO.SendPacket(p2);
                }
                return;
            }
            else
            {
                p1.Result = 0x00;
                p1.OrignalRefine = item.Refine;
                p1.ExpectedRefine = (ushort)(item.Refine + 1);
                this.netIO.SendPacket(p1);
                p1.Result = 0xfd;
            }

            this.netIO.SendPacket(p1);

            if (failed)
            {
                var res = from enhanctitem in this.chara.Inventory.GetContainer(ContainerType.BODY)
                          where (((enhanctitem.BaseData.itemType == ItemType.PARTS_BODY && ((CountItem(90000044) > 0 || CountItem(90000045) > 0 || CountItem(90000046) > 0 || CountItem(90000043) > 0 || CountItem(10087400) > 0 || CountItem(10087401) > 0) || CountItem(10087402) > 0 || CountItem(10087403) > 0))
                                || (enhanctitem.BaseData.itemType == ItemType.PARTS_BLOW ||
                            enhanctitem.BaseData.itemType == ItemType.PARTS_SLASH ||
                            enhanctitem.BaseData.itemType == ItemType.PARTS_STAB ||
                            enhanctitem.BaseData.itemType == ItemType.PARTS_LONGRANGE ||
                            enhanctitem.BaseData.itemType == ItemType.SETPARTS_BLOW && (CountItem(90000044) > 0 || CountItem(90000045) > 0 || CountItem(90000046) > 0 || CountItem(10087401) > 0) || CountItem(10087402) > 0 || CountItem(10087403) > 0))
                                && enhanctitem.Refine < 30 && this.Character.Gold >= 5000)
                          select enhanctitem;
                List<SagaDB.Item.Item> items = res.ToList();

                foreach (var itemsitem in res.ToList())
                {
                    if (itemsitem.BaseData.itemType == ItemType.PARTS_BODY)
                    {
                        //生命结晶
                        if (CountItem(90000043) > 0)
                            if (!items.Exists(x => x.ItemID == 90000043))
                                items.AddRange(GetItem(90000043));
                        //力量结晶
                        if (CountItem(90000044) > 0)
                            if (!items.Exists(x => x.ItemID == 90000044))
                                items.AddRange(GetItem(90000044));
                        //会心结晶
                        if (CountItem(90000046) > 0)
                            if (!items.Exists(x => x.ItemID == 90000046))
                                items.AddRange(GetItem(90000046));
                        //魔力结晶
                        if (CountItem(90000045) > 0)
                            if (!items.Exists(x => x.ItemID == 90000045))
                                items.AddRange(GetItem(90000045));

                        //生命強化结晶
                        if (CountItem(90000053) > 0)
                            if (!items.Exists(x => x.ItemID == 90000053))
                                items.AddRange(GetItem(90000053));
                        //力量強化结晶
                        if (CountItem(90000054) > 0)
                            if (!items.Exists(x => x.ItemID == 90000054))
                                items.AddRange(GetItem(90000054));
                        //会心強化结晶
                        if (CountItem(90000056) > 0)
                            if (!items.Exists(x => x.ItemID == 90000056))
                                items.AddRange(GetItem(90000056));
                        //魔力強化结晶
                        if (CountItem(90000055) > 0)
                            if (!items.Exists(x => x.ItemID == 90000055))
                                items.AddRange(GetItem(90000055));



                        //生命超強化结晶
                        if (CountItem(16004600) > 0)
                            if (!items.Exists(x => x.ItemID == 16004600))
                                items.AddRange(GetItem(16004600));
                        //力量超強化结晶
                        if (CountItem(16004700) > 0)
                            if (!items.Exists(x => x.ItemID == 16004700))
                                items.AddRange(GetItem(16004700));
                        //会心超強化结晶
                        if (CountItem(16004500) > 0)
                            if (!items.Exists(x => x.ItemID == 16004500))
                                items.AddRange(GetItem(16004500));
                        //魔力超強化结晶
                        if (CountItem(16004800) > 0)
                            if (!items.Exists(x => x.ItemID == 16004800))
                                items.AddRange(GetItem(16004800));


                        //强化王的生命
                        if (CountItem(10087400) > 0)
                            if (!items.Exists(x => x.ItemID == 10087400))
                                items.AddRange(GetItem(10087400));
                        //强化王的力量
                        if (CountItem(10087401) > 0)
                            if (!items.Exists(x => x.ItemID == 10087401))
                                items.AddRange(GetItem(10087401));
                        //强化王的会心
                        if (CountItem(10087402) > 0)
                            if (!items.Exists(x => x.ItemID == 10087402))
                                items.AddRange(GetItem(10087402));
                        //强化王的魔力
                        if (CountItem(10087403) > 0)
                            if (!items.Exists(x => x.ItemID == 10087403))
                                items.AddRange(GetItem(10087403));

                    }
                    if (itemsitem.BaseData.itemType == ItemType.PARTS_BLOW ||
                        itemsitem.BaseData.itemType == ItemType.PARTS_SLASH ||
                        itemsitem.BaseData.itemType == ItemType.PARTS_STAB ||
                        itemsitem.BaseData.itemType == ItemType.PARTS_LONGRANGE ||
                        itemsitem.BaseData.itemType == ItemType.SETPARTS_BLOW ||
                        item.BaseData.itemType == ItemType.SETPARTS_SLASH ||
                        item.BaseData.itemType == ItemType.SETPARTS_STAB ||
                        item.BaseData.itemType == ItemType.SETPARTS_LONGRANGE)
                    {

                        //力量结晶
                        if (CountItem(90000044) > 0)
                            if (!items.Exists(x => x.ItemID == 90000044))
                                items.AddRange(GetItem(90000044));
                        //会心结晶
                        if (CountItem(90000046) > 0)
                            if (!items.Exists(x => x.ItemID == 90000046))
                                items.AddRange(GetItem(90000046));
                        //魔力结晶
                        if (CountItem(90000045) > 0)
                            if (!items.Exists(x => x.ItemID == 90000045))
                                items.AddRange(GetItem(90000045));


                        //力量強化结晶
                        if (CountItem(90000054) > 0)
                            if (!items.Exists(x => x.ItemID == 90000054))
                                items.AddRange(GetItem(90000054));
                        //会心強化结晶
                        if (CountItem(90000056) > 0)
                            if (!items.Exists(x => x.ItemID == 90000056))
                                items.AddRange(GetItem(90000056));
                        //魔力強化结晶
                        if (CountItem(90000055) > 0)
                            if (!items.Exists(x => x.ItemID == 90000055))
                                items.AddRange(GetItem(90000055));




                        //力量超強化结晶
                        if (CountItem(16004700) > 0)
                            if (!items.Exists(x => x.ItemID == 16004700))
                                items.AddRange(GetItem(16004700));
                        //会心超強化结晶
                        if (CountItem(16004500) > 0)
                            if (!items.Exists(x => x.ItemID == 16004500))
                                items.AddRange(GetItem(16004500));
                        //魔力超強化结晶
                        if (CountItem(16004800) > 0)
                            if (!items.Exists(x => x.ItemID == 16004800))
                                items.AddRange(GetItem(16004800));


                        //强化王的力量
                        if (CountItem(10087401) > 0)
                            if (!items.Exists(x => x.ItemID == 10087401))
                                items.AddRange(GetItem(10087401));
                        //强化王的会心
                        if (CountItem(10087402) > 0)
                            if (!items.Exists(x => x.ItemID == 10087402))
                                items.AddRange(GetItem(10087402));
                        //强化王的魔力
                        if (CountItem(10087403) > 0)
                            if (!items.Exists(x => x.ItemID == 10087403))
                                items.AddRange(GetItem(10087403));



                    }

                    //Golbal Item

                    //防爆判断
                    if (CountItem(16001300) > 0)
                        if (!items.Exists(x => x.ItemID == 16001300))
                            items.AddRange(GetItem(16001300));

                    //防RESET判断
                    if (CountItem(10118200) > 0)
                        if (!items.Exists(x => x.ItemID == 10118200))
                            items.AddRange(GetItem(10118200));


                    //奥义判断
                    if (CountItem(10087200) > 0)
                        if (!items.Exists(x => x.ItemID == 10087200))
                            items.AddRange(GetItem(10087200));
                    //精髓判断
                    if (CountItem(10087201) > 0)
                        if (!items.Exists(x => x.ItemID == 10087201))
                            items.AddRange(GetItem(10087201));
                }

                items = items.OrderBy(x => x.Slot).ToList();
                if (items.Count > 0)
                {
                    Packets.Server.SSMG_DEM_ITEM_ENHANCE_LIST p2 = new SagaMap.Packets.Server.SSMG_DEM_ITEM_ENHANCE_LIST();
                    p2.Items = items;
                    this.netIO.SendPacket(p2);
                }
                else
                {
                    this.itemEnhance = false;
                    p1 = new SagaMap.Packets.Server.SSMG_DEM_ITEM_ENHANCE_RESULT();
                    p1.Result = 0x00;
                    p1.OrignalRefine = item.Refine;
                    p1.ExpectedRefine = (ushort)(item.Refine + 1);
                    this.netIO.SendPacket(p1);
                    p1.Result = 0xfd;
                    this.netIO.SendPacket(p1);
                }
                return;
            }
        }

        //ECOKEY DEM修復，DEM屬性強化專用
        public void OnItemElementsEnhanceConfirmDEM(Packets.Client.CSMG_DEM_ITEM_ENHANCE_CONFIRM p)
        {
            Item item = this.Character.Inventory.GetItem(p.InventorySlot);
            bool failed = false;
            bool success = false;
            Packets.Server.SSMG_DEM_ITEM_ENHANCE_RESULT p1 = new SagaMap.Packets.Server.SSMG_DEM_ITEM_ENHANCE_RESULT();
            p1.Result = 0;
            if (item != null)
            {

                if (CountItem(p.Material) > 0 && item.Refine_Lucky <= 6)
                {
                    if (this.Character.Gold >= 5000)
                    {
                        this.Character.Gold -= 5000;

                        Logger.ShowInfo("Refine Item:" + item.BaseData.name + "[" + p.InventorySlot + "] Protect Item: " +
                            (ItemFactory.Instance.GetItem(p.ProtectItem).ItemID != 10000000 ? ItemFactory.Instance.GetItem(p.ProtectItem).BaseData.name : "None") +
                            Environment.NewLine + "Material: " + (ItemFactory.Instance.GetItem(p.Material).ItemID != 10000000 ? ItemFactory.Instance.GetItem(p.Material).BaseData.name : "None") +
                            " SupportItem: " + (ItemFactory.Instance.GetItem(p.SupportItem).ItemID != 10000000 ? ItemFactory.Instance.GetItem(p.SupportItem).BaseData.name : "None"));

                        var Material = p.Material;
                        uint supportitemid = 0;
                        var enhancesupported = false;
                        if (ItemFactory.Instance.GetItem(p.SupportItem).ItemID != 10000000)
                        {
                            enhancesupported = true;
                            supportitemid = ItemFactory.Instance.GetItem((uint)p.SupportItem).ItemID;
                        }
                        uint protectitemid = 0;
                        var enhanceprotected = false;
                        if (ItemFactory.Instance.GetItem(p.ProtectItem).ItemID != 10000000)
                        {
                            enhanceprotected = true;
                            protectitemid = ItemFactory.Instance.GetItem((uint)p.ProtectItem).ItemID;
                        }
                        int finalrate = 0;
                        int crystal_addon = 0;
                        int skill_addon = 0;
                        int protect_addon = 0;
                        int support_addon = 0;
                        int matsuri_addon = 0;
                        int recycle_addon = 0;
                        int nextlevel = ((int)item.Refine_Lucky + (int)1);

                        //BaseRate
                        finalrate += EnhanceTableFactory.Instance.Table[nextlevel].BaseRate;

                        //防爆
                        if (enhanceprotected && protectitemid == 16001300)
                            protect_addon = EnhanceTableFactory.Instance.Table[nextlevel].ExplodeProtect;

                        //防重設
                        if (enhanceprotected && protectitemid == 10118200)
                            protect_addon = EnhanceTableFactory.Instance.Table[nextlevel].ResetProtect;

                        //奧義
                        if (ItemFactory.Instance.GetItem(p.ProtectItem).ItemID == 10087200)
                            support_addon = EnhanceTableFactory.Instance.Table[nextlevel].Okugi;

                        //神髓
                        if (ItemFactory.Instance.GetItem(p.ProtectItem).ItemID == 10087201)
                            support_addon = EnhanceTableFactory.Instance.Table[nextlevel].Shinzui;

                        //強化祭活動
                        if (SagaMap.Configuration.Instance.EnhanceMatsuri)
                            matsuri_addon = EnhanceTableFactory.Instance.Table[nextlevel].Matsuri;


                        //回收活動
                        //未實裝(需連動回收系統)
                        /*
                        if (SagaMap.Configuration.Instance.Recycle)
                            finalrate += EnhanceTableFactory.Instance.Table[nextlevel].Recycle;
                        */

                        //結算
                        finalrate += crystal_addon + matsuri_addon + skill_addon + protect_addon + support_addon + recycle_addon;


                        FromActorPC(Character).SendSystemMessage("----------------強化成功率結算---------------");
                        FromActorPC(Character).SendSystemMessage("正強化裝備到：" + ((int)item.Refine_Lucky + (int)1));

                        FromActorPC(Character).SendSystemMessage("基本機率：" + (EnhanceTableFactory.Instance.Table[nextlevel].BaseRate / 100) + "%");
                        FromActorPC(Character).SendSystemMessage("補助加成：" + (support_addon / 100) + "%");
                        FromActorPC(Character).SendSystemMessage("防爆加成：" + (protect_addon / 100) + "%");
                        FromActorPC(Character).SendSystemMessage("強化祭加成：" + (matsuri_addon / 100) + "%");
                        FromActorPC(Character).SendSystemMessage("回收活动加成：" + (recycle_addon / 100) + "%");

                        FromActorPC(Character).SendSystemMessage("你的最終強化成功率為：" + (finalrate / 100) + "%");
                        FromActorPC(Character).SendSystemMessage("----------------強化成功率結算---------------");

                        if (finalrate > 9999)
                            finalrate = 9999;

                        int refrate = Global.Random.Next(0, 9999);
                        finalrate = 9999;
                        if (enhanceprotected)
                        {
                            if (CountItem(protectitemid) < 1)
                            {
                                p1.Result = 0x00;
                                p1.OrignalRefine = (ushort)item.Refine_Lucky;
                                p1.ExpectedRefine = (ushort)(item.Refine_Lucky + 1);
                                this.netIO.SendPacket(p1);
                                p1.Result = 0xfc;
                                this.netIO.SendPacket(p1);
                                this.itemEnhance = false;
                                return;
                            }
                            else
                                DeleteItemID(protectitemid, 1, true);
                        }

                        if (enhancesupported)
                        {
                            if (CountItem(supportitemid) < 1)
                            {
                                p1.Result = 0x00;
                                p1.OrignalRefine = (ushort)item.Refine_Lucky;
                                p1.ExpectedRefine = (ushort)(item.Refine_Lucky + 1);
                                this.netIO.SendPacket(p1);
                                p1.Result = 0xfc;
                                this.netIO.SendPacket(p1);
                                this.itemEnhance = false;
                                return;
                            }
                            else
                                DeleteItemID(supportitemid, 1, true);
                        }


                        if (CountItem(Material) < 1)
                        {
                            p1.Result = 0x00;
                            p1.OrignalRefine = (ushort)item.Refine_Lucky;
                            p1.ExpectedRefine = (ushort)(item.Refine_Lucky + 1);
                            this.netIO.SendPacket(p1);
                            p1.Result = 0xfc;
                            this.netIO.SendPacket(p1);
                            this.itemEnhance = false;
                            return;
                        }

                        if (refrate <= finalrate)
                        {
                            switch (p.Material)
                            {
                                case 90000047://火屬性強化の結晶
                                    item.Refine_Sharp += 5;
                                    DeleteItemID(90000047, 1, true);
                                    item.Refine_Lucky++;
                                    break;
                                case 90000048://水屬性強化の結晶
                                    item.Refine_Enchanted += 5;
                                    DeleteItemID(90000048, 1, true);
                                    item.Refine_Lucky++;
                                    break;
                                case 90000049://風屬性強化の結晶
                                    item.Refine_Vitality += 5;
                                    DeleteItemID(90000049, 1, true);
                                    item.Refine_Lucky++;
                                    break;
                                case 90000050://土屬性強化の結晶
                                    item.Refine_Hit += 5;
                                    DeleteItemID(90000050, 1, true);
                                    item.Refine_Lucky++;
                                    break;
                                case 90000051://光屬性強化の結晶
                                    item.Refine_Regeneration += 4;
                                    DeleteItemID(90000051, 1, true);
                                    item.Refine_Lucky++;
                                    break;
                                case 90000052://闇屬性強化の結晶
                                    item.Refine_Mhit += 4;
                                    DeleteItemID(90000052, 1, true);
                                    item.Refine_Lucky++;
                                    break;
                            }
                            SendEffect(5145);
                            p1.Result = 1;
                            success = true;
                            item.Refine_Lucky++;
                            SendItemInfo(item);

                            FromActorPC(Character).SendSystemMessage("成功屬性強化第" + item.Refine_Lucky + "次。");
                            PC.StatusFactory.Instance.CalcStatus(this.chara);
                            SendPlayerInfo();
                        }
                        else
                        {
                            failed = true;
                            SendEffect(5146);
                            if (!enhanceprotected)
                            {
                                DeleteItem(p.InventorySlot, 1, true);
                                DeleteItemID(p.Material, 1, true);
                            }
                            else
                            {
                                DeleteItemID(p.Material, 1, true);
                                //Explode Protected Only
                                if (protectitemid != 16001300)
                                    DeleteItem(p.InventorySlot, 1, true);

                                PC.StatusFactory.Instance.CalcStatus(this.chara);
                                SendPlayerInfo();
                                failed = true;
                            }

                            p1.Result = 0;
                            p1.OrignalRefine = (ushort)(item.Refine_Lucky + 1);
                            p1.ExpectedRefine = (ushort)item.Refine_Lucky;
                            this.netIO.SendPacket(p1);
                            p1.Result = 0x00;
                        }
                    }
                    else
                    {
                        p1.Result = 0x00;
                        p1.OrignalRefine = (ushort)item.Refine_Lucky;
                        p1.ExpectedRefine = (ushort)(item.Refine_Lucky + 1);
                        this.netIO.SendPacket(p1);
                        p1.Result = 0xff;
                    }
                }
                else
                {
                    p1.Result = 0x00;
                    p1.OrignalRefine = (ushort)item.Refine_Lucky;
                    p1.ExpectedRefine = (ushort)(item.Refine_Lucky + 1);
                    this.netIO.SendPacket(p1);
                    p1.Result = 0xfd;
                }
            }
            else
            {
                p1.Result = 0x00;
                p1.OrignalRefine = (ushort)item.Refine_Lucky;
                p1.ExpectedRefine = (ushort)(item.Refine_Lucky + 1);
                this.netIO.SendPacket(p1);
                p1.Result = 0xfe;
            }
            if (success)
            {
                p1.Result = 1;
                p1.OrignalRefine = (ushort)(item.Refine_Lucky + 1);
                p1.ExpectedRefine = (ushort)item.Refine_Lucky;
                this.netIO.SendPacket(p1);
                var res = from enhanctitem in this.chara.Inventory.GetContainer(ContainerType.BODY)
                          where (((enhanctitem.BaseData.itemType == ItemType.PARTS_BODY) ||
                          (enhanctitem.BaseData.itemType == ItemType.PARTS_BLOW ||
                        enhanctitem.BaseData.itemType == ItemType.PARTS_SLASH ||
                        enhanctitem.BaseData.itemType == ItemType.PARTS_STAB ||
                        enhanctitem.BaseData.itemType == ItemType.PARTS_LONGRANGE ||
                        enhanctitem.BaseData.itemType == ItemType.SETPARTS_BLOW ||
                        item.BaseData.itemType == ItemType.SETPARTS_SLASH ||
                        item.BaseData.itemType == ItemType.SETPARTS_STAB ||
                        item.BaseData.itemType == ItemType.SETPARTS_LONGRANGE && (CountItem(90000047) > 0 || CountItem(90000048) > 0 || CountItem(90000049) > 0 || CountItem(90000050) > 0) || CountItem(90000051) > 0 || CountItem(90000052) > 0))
                            && enhanctitem.Refine_Lucky <= 6 && this.Character.Gold >= 5000)
                          select enhanctitem;
                List<SagaDB.Item.Item> items = res.ToList();

                foreach (var itemsitem in res.ToList())
                {
                    //火屬性強化の結晶
                    if (CountItem(90000047) > 0)
                        if (!items.Exists(x => x.ItemID == 90000047))
                            items.AddRange(GetItem(90000047));
                    //水屬性強化の結晶
                    if (CountItem(90000048) > 0)
                        if (!items.Exists(x => x.ItemID == 90000048))
                            items.AddRange(GetItem(90000048));
                    //風屬性強化の結晶
                    if (CountItem(90000049) > 0)
                        if (!items.Exists(x => x.ItemID == 90000049))
                            items.AddRange(GetItem(90000049));
                    //土屬性強化の結晶
                    if (CountItem(90000050) > 0)
                        if (!items.Exists(x => x.ItemID == 90000050))
                            items.AddRange(GetItem(90000050));
                    //光屬性強化の結晶
                    if (CountItem(90000051) > 0)
                        if (!items.Exists(x => x.ItemID == 90000051))
                            items.AddRange(GetItem(90000051));
                    //闇屬性強化の結晶
                    if (CountItem(90000052) > 0)
                        if (!items.Exists(x => x.ItemID == 90000052))
                            items.AddRange(GetItem(90000052));

                    //Golbal Item

                    //防爆判断
                    if (CountItem(16001300) > 0)
                        if (!items.Exists(x => x.ItemID == 16001300))
                            items.AddRange(GetItem(16001300));

                    //防RESET判断
                    if (CountItem(10118200) > 0)
                        if (!items.Exists(x => x.ItemID == 10118200))
                            items.AddRange(GetItem(10118200));


                    //奥义判断
                    if (CountItem(10087200) > 0)
                        if (!items.Exists(x => x.ItemID == 10087200))
                            items.AddRange(GetItem(10087200));
                    //精髓判断
                    if (CountItem(10087201) > 0)
                        if (!items.Exists(x => x.ItemID == 10087201))
                            items.AddRange(GetItem(10087201));
                }

                items = items.OrderBy(x => x.Slot).ToList();
                if (items.Count > 0)
                {
                    Packets.Server.SSMG_DEM_ITEM_ENHANCE_LIST p2 = new SagaMap.Packets.Server.SSMG_DEM_ITEM_ENHANCE_LIST();
                    p2.Items = items;
                    this.netIO.SendPacket(p2);
                }
                return;
            }
            else
            {
                p1.Result = 0x00;
                p1.OrignalRefine = (ushort)item.Refine_Lucky;
                p1.ExpectedRefine = (ushort)(item.Refine_Lucky + 1);
                this.netIO.SendPacket(p1);
                p1.Result = 0xfd;
            }

            this.netIO.SendPacket(p1);

            if (failed)
            {
                var res = from enhanctitem in this.chara.Inventory.GetContainer(ContainerType.BODY)
                          where (((enhanctitem.BaseData.itemType == ItemType.PARTS_BODY) ||
                          (enhanctitem.BaseData.itemType == ItemType.PARTS_BLOW ||
                        enhanctitem.BaseData.itemType == ItemType.PARTS_SLASH ||
                        enhanctitem.BaseData.itemType == ItemType.PARTS_STAB ||
                        enhanctitem.BaseData.itemType == ItemType.PARTS_LONGRANGE ||
                        enhanctitem.BaseData.itemType == ItemType.SETPARTS_BLOW ||
                        item.BaseData.itemType == ItemType.SETPARTS_SLASH ||
                        item.BaseData.itemType == ItemType.SETPARTS_STAB ||
                        item.BaseData.itemType == ItemType.SETPARTS_LONGRANGE && (CountItem(90000047) > 0 || CountItem(90000048) > 0 || CountItem(90000049) > 0 || CountItem(90000050) > 0) || CountItem(90000051) > 0 || CountItem(90000052) > 0))
                            && enhanctitem.Refine_Lucky <= 6 && this.Character.Gold >= 5000)
                          select enhanctitem;
                List<SagaDB.Item.Item> items = res.ToList();

                foreach (var itemsitem in res.ToList())
                {
                    //火屬性強化の結晶
                    if (CountItem(90000047) > 0)
                        if (!items.Exists(x => x.ItemID == 90000047))
                            items.AddRange(GetItem(90000047));
                    //水屬性強化の結晶
                    if (CountItem(90000048) > 0)
                        if (!items.Exists(x => x.ItemID == 90000048))
                            items.AddRange(GetItem(90000048));
                    //風屬性強化の結晶
                    if (CountItem(90000049) > 0)
                        if (!items.Exists(x => x.ItemID == 90000049))
                            items.AddRange(GetItem(90000049));
                    //土屬性強化の結晶
                    if (CountItem(90000050) > 0)
                        if (!items.Exists(x => x.ItemID == 90000050))
                            items.AddRange(GetItem(90000050));
                    //光屬性強化の結晶
                    if (CountItem(90000051) > 0)
                        if (!items.Exists(x => x.ItemID == 90000051))
                            items.AddRange(GetItem(90000051));
                    //闇屬性強化の結晶
                    if (CountItem(90000052) > 0)
                        if (!items.Exists(x => x.ItemID == 90000052))
                            items.AddRange(GetItem(90000052));
                    //Golbal Item

                    //防爆判断
                    if (CountItem(16001300) > 0)
                        if (!items.Exists(x => x.ItemID == 16001300))
                            items.AddRange(GetItem(16001300));

                    //防RESET判断
                    if (CountItem(10118200) > 0)
                        if (!items.Exists(x => x.ItemID == 10118200))
                            items.AddRange(GetItem(10118200));


                    //奥义判断
                    if (CountItem(10087200) > 0)
                        if (!items.Exists(x => x.ItemID == 10087200))
                            items.AddRange(GetItem(10087200));
                    //精髓判断
                    if (CountItem(10087201) > 0)
                        if (!items.Exists(x => x.ItemID == 10087201))
                            items.AddRange(GetItem(10087201));
                }

                items = items.OrderBy(x => x.Slot).ToList();
                if (items.Count > 0)
                {
                    Packets.Server.SSMG_DEM_ITEM_ENHANCE_LIST p2 = new SagaMap.Packets.Server.SSMG_DEM_ITEM_ENHANCE_LIST();
                    p2.Items = items;
                    this.netIO.SendPacket(p2);
                }
                else
                {
                    this.itemEnhance = false;
                    p1 = new SagaMap.Packets.Server.SSMG_DEM_ITEM_ENHANCE_RESULT();
                    p1.Result = 0x00;
                    p1.OrignalRefine = (ushort)item.Refine_Lucky;
                    p1.ExpectedRefine = (ushort)(item.Refine_Lucky + 1);
                    this.netIO.SendPacket(p1);
                    p1.Result = 0xfd;
                    this.netIO.SendPacket(p1);
                }
                return;
            }
        }
        //ECOKEY DEM修復，DEM部件融合取消
        public void OnItemFusion_DEM(Packets.Client.CSMG_DEM_ITEM_FUSION p)
        {
            itemFusionEffect = p.EffectItem;
            itemFusionView = p.ViewItem;
            this.itemFusion = false;
        }



        //ECOKEY DEM芯片倉庫
        /*public void SendChipsWareItems()
        {
            currentWarehouse2 = WarehousePlace.Chips;
            Packets.Server.SSMG_DEM_CHIPS_WARE_SENDCOUNT p0 = new SagaMap.Packets.Server.SSMG_DEM_CHIPS_WARE_SENDCOUNT();
            //7200
            //ECOKEY 如果沒開過飛空倉庫就新增
            if (!this.Character.Inventory.WareHouse.ContainsKey(currentWarehouse2))
                this.Character.Inventory.WareHouse[currentWarehouse2] = new List<Item>(300);

            //ECOKEY 這句是顯示倉庫最高儲存量
            p0.CurrentCount = 300;//this.Character.Inventory.WareHouse[currentWarehouse].Capacity;
            this.netIO.SendPacket(p0);

            if (!this.Character.Inventory.WareHouse.ContainsKey(currentWarehouse2))
                this.Character.Inventory.WareHouse[currentWarehouse2] = new List<Item>(300);

            Packets.Server.SSMG_DEM_CHIPS_WARE_HEADER p1 = new SagaMap.Packets.Server.SSMG_DEM_CHIPS_WARE_HEADER();//7205
            this.netIO.SendPacket(p1);

            foreach (Item j in this.Character.Inventory.WareHouse[currentWarehouse2])
            {
                Logger.ShowInfo("打開倉庫   " + j.BaseData.name);
                Packets.Server.SSMG_DEM_CHIPS_WARE_ITEM p2 = new SagaMap.Packets.Server.SSMG_DEM_CHIPS_WARE_ITEM();//7206
                p2.Item = j;
                p2.InventorySlot = j.Slot;
                p2.ItemID = j.ItemID;
                //p2.Place = WarehousePlace.Chips;
                this.netIO.SendPacket(p2);
            }

            Packets.Server.SSMG_DEM_CHIPS_WARE_FOOTER p3 = new SagaMap.Packets.Server.SSMG_DEM_CHIPS_WARE_FOOTER();//7207
            this.netIO.SendPacket(p3);
        }

        public void OnChipsWareClose(Packets.Client.CSMG_DEM_CHIPS_WARE_CLOSE p)
        {
            currentWarehouse2 = WarehousePlace.Current;
        }

        public void OnChipsWarePut(Packets.Client.CSMG_DEM_CHIPS_WARE_PUT p)
        {
            Logger.ShowInfo("放囉   " + p.InventoryID + "  " + p.Count);

            int result = 0;
            if (currentWarehouse2 == WarehousePlace.Current)
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
                else if (this.Character.Inventory.WareHouse[currentWarehouse2].Count() >= Configuration.Instance.WarehouseLimit)
                    result = -4;
                else if (item.BaseData.noStore)
                    result = -6;
                else if (currentWarehouse2 == WarehousePlace.Chips && item.BaseData.itemType != ItemType.DEMIC_CHIP)
                    result = -6;
                else if (this.Character.Golem != null && this.Character.Golem.SellShop.ContainsKey(item.Slot))
                    result = -6;
                else
                {
                    Logger.ShowInfo("放嗎?");
                    Logger.LogItemLost(Logger.EventType.ItemWareLost, this.Character.Name + "(" + this.Character.CharID + ")", item.BaseData.name + "(" + item.ItemID + ")",
                        string.Format("WarePut Count:{0}", p.Count), false);
                    DeleteItem(p.InventoryID, p.Count, false);
                    Item newItem = item.Clone();
                    newItem.Stack = p.Count;
                    switch (this.Character.Inventory.AddWareItem(currentWarehouse2, newItem))
                    {
                        case InventoryAddResult.NEW_INDEX:
                            Packets.Server.SSMG_DEM_CHIPS_WARE_ITEM p1 = new SagaMap.Packets.Server.SSMG_DEM_CHIPS_WARE_ITEM();//7206
                            //p1.ID = (ushort)this.Character.TInt["TESTTTT"];
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
            Logger.ShowInfo("結束了?   " + result);
            Packets.Server.SSMG_ITEM_WARE_PUT_RESULT p5 = new SagaMap.Packets.Server.SSMG_ITEM_WARE_PUT_RESULT();
            p5.Result = result;
            this.netIO.SendPacket(p5);
        }


        */
    }
}
