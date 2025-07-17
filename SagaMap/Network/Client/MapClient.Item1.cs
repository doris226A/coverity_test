using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using SagaDB;
using SagaDB.Item;
using SagaDB.Actor;
using SagaMap.Skill.Additions.Global;
using SagaDB.Skill;
using SagaLib;
using SagaMap.Manager;
using SagaMap.Skill;
using SagaDB.EnhanceTable;
using SagaDB.DualJob;
using SagaDB.MasterEnchance;
using SagaMap.Packets.Server;
using SagaMap.Partner;

namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        public uint kujiboxID0 = 120000000;
        public uint kujinum_max = 1000;
        public bool itemEnhance = false;
        public bool itemFusion = false;
        public bool itemMasterEnhance = false;
        public uint itemFusionEffect, itemFusionView;

        public void OnItemChange(Packets.Client.CSMG_ITEM_CHANGE p)
        {
            List<uint> lstInventory = p.SlotList;
            ItemTransform it = ItemTransformFactory.Instance.Items[p.ChangeID];
            Item newitem = ItemFactory.Instance.GetItem(it.Product);
            SSMG_ITEM_CHANGE_RESULT p3 = new SSMG_ITEM_CHANGE_RESULT();
            foreach (var InvItem in lstInventory)
            {
                if (InvItem == 0)
                    continue;
                Item item = this.Character.Inventory.GetItem(InvItem);

                if (!ItemTransformFactory.Instance.Items.ContainsKey(p.ChangeID))
                {
                    if (this.account.GMLevel >= 200)
                        this.SendSystemMessage("錯誤！沒找到ChangeID！");
                    p3.Result = -1;
                    this.netIO.SendPacket(p3);
                    return;
                }

                //道具锁
                if (item.ChangeMode || item.ChangeMode2)
                {
                    p3.Result = -2;
                    this.netIO.SendPacket(p3);
                    return;
                }

                item.ChangeMode = true;

                //unknown
                Packets.Server.SSMG_ITEM_CHANGE_ADD p1 = new SagaMap.Packets.Server.SSMG_ITEM_CHANGE_ADD();
                this.netIO.SendPacket(p1);
                //添加道具锁
                Packets.Server.SSMG_ITEM_INFO p2 = new SagaMap.Packets.Server.SSMG_ITEM_INFO();
                p2.Item = item;
                p2.InventorySlot = InvItem;
                p2.Container = this.Character.Inventory.GetContainerType(item.Slot);
                this.netIO.SendPacket(p2);

                //添加pet道具
                //pet属性设置
                newitem.Refine = item.Refine;
                newitem.Durability = item.Durability;

                foreach (var stuff in it.Stuffs)
                {
                    foreach (var stuffitem in stuff.Value)
                    {
                        switch (stuffitem.Ability)
                        {
                            case "MAX_HP":
                                newitem.HP += (short)((float)item.HP * (float)stuffitem.Value / 100.0f);
                                break;
                            case "MAX_MP":
                                newitem.MP += (short)((float)item.MP * (float)stuffitem.Value / 100.0f);
                                break;
                            case "MAX_SP":
                                newitem.SP += (short)((float)item.SP * (float)stuffitem.Value / 100.0f);
                                break;
                            case "ATK_MAX":
                                newitem.Atk1 += (short)((float)item.Atk1 * (float)stuffitem.Value / 100.0f);
                                newitem.Atk2 += (short)((float)item.Atk2 * (float)stuffitem.Value / 100.0f);
                                newitem.Atk3 += (short)((float)item.Atk3 * (float)stuffitem.Value / 100.0f);
                                break;
                            case "MATK_MAX":
                                newitem.MAtk += (short)((float)item.MAtk * (float)stuffitem.Value / 100.0f);
                                break;
                            case "DEF_MINUS":
                                newitem.Def += (short)((float)item.Def * (float)stuffitem.Value / 100.0f);
                                break;
                            case "MDEF_MINUS":
                                newitem.MDef += (short)((float)item.MDef * (float)stuffitem.Value / 100.0f);
                                break;
                            case "SHORT_HIT":
                                newitem.HitMelee += (short)((float)item.HitMelee * (float)stuffitem.Value / 100.0f);
                                break;
                            case "DIST_HIT":
                                newitem.HitRanged += (short)((float)item.HitRanged * (float)stuffitem.Value / 100.0f);
                                break;
                            case "MAG_HIT":
                                newitem.HitMagic += (short)((float)item.HitMagic * (float)stuffitem.Value / 100.0f);
                                break;
                            case "SHORT_AVOID":
                                newitem.AvoidMelee += (short)((float)item.AvoidMelee * (float)stuffitem.Value / 100.0f);
                                break;
                            case "DIST_AVOID":
                                newitem.AvoidRanged += (short)((float)item.AvoidRanged * (float)stuffitem.Value / 100.0f);
                                break;
                            case "MAGREGIST":
                                newitem.AvoidMagic += (short)((float)item.AvoidMagic * (float)stuffitem.Value / 100.0f);
                                break;
                            case "CRITICAL":
                                newitem.HitCritical += (short)((float)item.HitCritical * (float)stuffitem.Value / 100.0f);
                                break;
                            case "CRIAVOID":
                                newitem.AvoidCritical += (short)((float)item.AvoidCritical * (float)stuffitem.Value / 100.0f);
                                break;
                            case "HEALRATE":
                                newitem.HPRecover += (short)((float)item.HPRecover * (float)stuffitem.Value / 100.0f);
                                break;
                            case "MAGICHEALRATE":
                                newitem.MPRecover += (short)((float)item.MPRecover * (float)stuffitem.Value / 100.0f);
                                break;
                            case "ATKSPEED":
                                newitem.ASPD += (short)((float)item.ASPD * (float)stuffitem.Value / 100.0f);
                                break;
                            case "CASTSPEED":
                                newitem.CSPD += (short)((float)item.CSPD * (float)stuffitem.Value / 100.0f);
                                break;
                            case "MOVE_SPEED":
                                newitem.SpeedUp += (short)((float)item.SpeedUp * (float)stuffitem.Value / 100.0f);
                                break;
                            case "ALL":
                                newitem.HP += (short)((float)item.HP * (float)stuffitem.Value / 100.0f);
                                newitem.MP += (short)((float)item.MP * (float)stuffitem.Value / 100.0f);
                                newitem.SP += (short)((float)item.SP * (float)stuffitem.Value / 100.0f);
                                newitem.Atk1 += (short)((float)item.Atk1 * (float)stuffitem.Value / 100.0f);
                                newitem.Atk2 += (short)((float)item.Atk2 * (float)stuffitem.Value / 100.0f);
                                newitem.Atk3 += (short)((float)item.Atk3 * (float)stuffitem.Value / 100.0f);
                                newitem.MAtk += (short)((float)item.MAtk * (float)stuffitem.Value / 100.0f);
                                newitem.Def += (short)((float)item.Def * (float)stuffitem.Value / 100.0f);
                                newitem.MDef += (short)((float)item.MDef * (float)stuffitem.Value / 100.0f);
                                newitem.HitMelee += (short)((float)item.HitMelee * (float)stuffitem.Value / 100.0f);
                                newitem.HitRanged += (short)((float)item.HitRanged * (float)stuffitem.Value / 100.0f);
                                newitem.HitMagic += (short)((float)item.HitMagic * (float)stuffitem.Value / 100.0f);
                                newitem.AvoidMelee += (short)((float)item.AvoidMelee * (float)stuffitem.Value / 100.0f);
                                newitem.AvoidRanged += (short)((float)item.AvoidRanged * (float)stuffitem.Value / 100.0f);
                                newitem.AvoidMagic += (short)((float)item.AvoidMagic * (float)stuffitem.Value / 100.0f);
                                newitem.HitCritical += (short)((float)item.HitCritical * (float)stuffitem.Value / 100.0f);
                                newitem.AvoidCritical += (short)((float)item.AvoidCritical * (float)stuffitem.Value / 100.0f);
                                newitem.HPRecover += (short)((float)item.HPRecover * (float)stuffitem.Value / 100.0f);
                                newitem.MPRecover += (short)((float)item.MPRecover * (float)stuffitem.Value / 100.0f);
                                break;
                        }
                    }
                }
            }

            //白框
            newitem.ChangeMode2 = true;
            this.AddItem(newitem, false);
            SendItemInfo(newitem);
            p3.Result = 0;
            this.netIO.SendPacket(p3);
        }

        public class ItemDuplicateDefine : IEqualityComparer<Item>
        {
            public bool Equals(Item x, Item y)
            {
                return x.ItemID == y.ItemID;
            }

            public int GetHashCode(Item obj)
            {
                return obj.ToString().GetHashCode();
            }
        }

        public void OnItemChangeCancel(Packets.Client.CSMG_ITEM_CHANGE_CANCEL p)
        {
            Item PetItem = this.Character.Inventory.GetItem(p.InventorySlot);
            SSMG_ITEM_CHANGE_RESULT p1 = new SSMG_ITEM_CHANGE_RESULT();

            if (PetItem == null)
            {
                p1.Result = -1;
                this.netIO.SendPacket(p1);
                return;
            }

            if (PetItem.PossessionOwner != null || PetItem.PossessionedActor != null)
            {
                p1.Result = -1;
                this.netIO.SendPacket(p1);
                return;
            }

            var transform = ItemTransformFactory.Instance.Items.First(x => x.Value.Product == PetItem.ItemID);

            if (transform.Value.Stuffs.Count == 0)
            {
                this.SendSystemMessage(PetItem.BaseData.name + " Transform Faild");
                p1.Result = -1;
                this.netIO.SendPacket(p1);
                return;
            }

            var transforminfo = transform.Value;

            var sourcecondition = this.Character.Inventory.Items[ContainerType.BODY].Where(x => (x.ChangeMode || x.ChangeMode2) && transforminfo.Stuffs.Keys.Contains(x.ItemID)).Distinct(new ItemDuplicateDefine()).ToList().Count;

            if (sourcecondition != transforminfo.Stuffs.Count)
            {
                this.SendSystemMessage(PetItem.BaseData.name + " Transform Faild. Source Item Qty is not correct.");
                p1.Result = -2;
                this.netIO.SendPacket(p1);
                return;
            }

            foreach (var item in transforminfo.Stuffs)
            {
                Item StuffItem = this.Character.Inventory.GetItem2(item.Key);
                StuffItem.Durability = PetItem.Durability;
                StuffItem.ChangeMode = false;
                SendItemInfo(StuffItem);
            }

            this.DeleteItem(PetItem.Slot, 1, false);
            p1.Result = 0;
            this.netIO.SendPacket(p1);
        }

        public void OnItemMasterEnhanceClose(Packets.Client.CSMG_ITEM_MASTERENHANCE_CLOSE p)
        {
            this.itemMasterEnhance = false;
            //关闭界面处理
        }

        public void SendMasterEnhanceAbleEquiList()
        {
            SSMG_ITEM_MASTERENHANCE_RESULT p2 = new SSMG_ITEM_MASTERENHANCE_RESULT();
            p2.Result = 0;
            Packet packet = new Packet(10);
            packet.data = new byte[10];
            packet.ID = 0x1f59;

            if (this.chara.Gold < 5000)
            {
                p2.Result = -1;
                this.netIO.SendPacket(p2);
                this.netIO.SendPacket(packet);
                this.itemMasterEnhance = false;
                return;
            }

            var lst = this.Character.Inventory.GetContainer(ContainerType.BODY);
            var itemlist = lst.Where(x => (x.IsWeapon || x.IsArmor) && !x.Potential && x.PossessionOwner == null && x.PossessionedActor == null).ToList();
            SSMG_ITEM_MASTERENHANCE_LIST p = new SSMG_ITEM_MASTERENHANCE_LIST();
            p.Items = itemlist;

            if (itemlist.Count > 0)
                this.netIO.SendPacket(p);
            else
            {
                p2.Result = -2;
                this.netIO.SendPacket(p2);
                this.netIO.SendPacket(packet);
                this.itemMasterEnhance = false;
            }
        }

        public void OnItemMasterEnhanceSelect(Packets.Client.CSMG_ITEM_MASTERENHANCE_SELECT p)
        {
            Item item = this.chara.Inventory.GetItem(p.InventorySlot);
            List<MasterEnhanceMaterial> lst = new List<MasterEnhanceMaterial>();

            foreach (var itemkey in MasterEnhanceMaterialFactory.Instance.Items.Keys)
            {
                if (CountItem(itemkey) > 0)
                    lst.Add(MasterEnhanceMaterialFactory.Instance.Items[itemkey]);
            }

            SSMG_ITEM_MASTERENHANCE_DETAIL p1 = new SSMG_ITEM_MASTERENHANCE_DETAIL();
            p1.Items = lst;
            this.netIO.SendPacket(p1);
        }

        public void OnItemMasterEnhanceConfirm(Packets.Client.CSMG_ITEM_MASTERENHANCE_CONFIRM p)
        {
            SSMG_ITEM_MASTERENHANCE_RESULT p2 = new SSMG_ITEM_MASTERENHANCE_RESULT();
            p2.Result = 0;

            Packet packet = new Packet(10);
            packet.data = new byte[10];
            packet.ID = 0x1f59;

            Item item = this.chara.Inventory.GetItem(p.InventorySlot);
            if (item == null)
            {
                p2.Result = -4;

                this.netIO.SendPacket(p2);
                this.netIO.SendPacket(packet);
                this.itemMasterEnhance = false;
                return;
            }

            if (item.PossessionedActor != null || item.PossessionOwner != null)
            {
                p2.Result = -4;

                this.netIO.SendPacket(p2);
                this.netIO.SendPacket(packet);
                this.itemMasterEnhance = false;
                return;
            }

            var materialid = p.ItemID;

            if (CountItem(materialid) <= 0)
            {
                p2.Result = -3;

                this.netIO.SendPacket(p2);
                this.netIO.SendPacket(packet);
                this.itemMasterEnhance = false;
                return;
            }

            DeleteItemID(materialid, 1, true);

            var Material = MasterEnhanceMaterialFactory.Instance.Items[materialid];
            var value = (short)Global.Random.Next(Material.MinValue, Material.MaxValue);

            item.Potential = true;
            switch (Material.Ability)
            {
                case MasterEnhanceType.STR:
                    item.Str += value;
                    break;
                case MasterEnhanceType.DEX:
                    item.Dex += value;
                    break;
                case MasterEnhanceType.INT:
                    item.Int += value;
                    break;
                case MasterEnhanceType.VIT:
                    item.Vit += value;
                    break;
                case MasterEnhanceType.AGI:
                    item.Agi += value;
                    break;
                case MasterEnhanceType.MAG:
                    item.Mag += value;
                    break;
                default:
                    break;
            }

            SendEffect(5145);

            SendItemInfo(item);

            p2.Result = 0;
            this.netIO.SendPacket(p2);

            SendMasterEnhanceAbleEquiList();
        }
        #region ECOKEY 融合分解
        public uint itemBreak;
        public uint itemBreak_price;
        public uint itemBreak_rate;
        public void OnItemBreakCancel(Packets.Client.CSMG_ITEM_BREAK_CANCEL p)
        {
            itemBreak = 0;
            this.itemFusion = false;
        }
        public void OnItemBreak(Packets.Client.CSMG_ITEM_BREAK p)
        {
            itemBreak = p.Item;
            if (itemBreak != 0)
            {
                SagaDB.Item.Item effectItem = this.Character.Inventory.GetItem(itemBreak);
                if (effectItem != null)
                {
                    this.Character.Gold -= itemBreak_price;
                    if (Global.Random.Next(0, 99) < itemBreak_rate)
                    {

                        if (effectItem.PictID == 60021500 || effectItem.PictID == 60021501)
                        {
                            SagaMap.Skill.SkillHandler.Instance.GiveItem(this.Character, 60021550, 1, true);
                        }
                        else if (effectItem.ActorPartnerID == 0)
                        {
                            SagaMap.Skill.SkillHandler.Instance.GiveItem(this.Character, effectItem.PictID, 1, true);
                        }
                        else
                        {
                            SagaMap.Skill.SkillHandler.Instance.GiveItem(this.Character, effectItem.ActorPartnerID, 1, true);
                        }

                        effectItem.PictID = 0;
                        effectItem.ActorPartnerID = 0;
                        SendItemInfo(effectItem);

                        SagaMap.Skill.SkillHandler.Instance.ShowEffect(this.Character, this.Character, 5191);

                        SagaMap.Packets.Server.SSMG_ITEM_FUSION_RESULT p2 = new SagaMap.Packets.Server.SSMG_ITEM_FUSION_RESULT();
                        p2.ID = 8152;
                        p2.Result = SagaMap.Packets.Server.SSMG_ITEM_FUSION_RESULT.FusionResult.OK;
                        netIO.SendPacket(p2);
                    }
                    else
                    {
                        SagaMap.Packets.Server.SSMG_ITEM_FUSION_RESULT p2 = new SagaMap.Packets.Server.SSMG_ITEM_FUSION_RESULT();
                        p2.ID = 8152;
                        p2.Result = SagaMap.Packets.Server.SSMG_ITEM_FUSION_RESULT.FusionResult.GENDER_NOT_FIT;
                        netIO.SendPacket(p2);
                        SendSystemMessage("解除融合失敗");
                    }
                }
                else
                {
                    SagaMap.Packets.Server.SSMG_ITEM_FUSION_RESULT p2 = new SagaMap.Packets.Server.SSMG_ITEM_FUSION_RESULT();
                    p2.ID = 8152;
                    p2.Result = SagaMap.Packets.Server.SSMG_ITEM_FUSION_RESULT.FusionResult.FAILED;
                    netIO.SendPacket(p2);
                }
            }
        }
        #endregion
        public void OnItemFusionCancel(Packets.Client.CSMG_ITEM_FUSION_CANCEL p)
        {
            itemFusionEffect = 0;
            itemFusionView = 0;
            this.itemFusion = false;
        }
        public void OnItemFusion(Packets.Client.CSMG_ITEM_FUSION p)
        {
            itemFusionEffect = p.EffectItem;
            itemFusionView = p.ViewItem;
            this.itemFusion = false;
        }
        public void OnItemEnhanceClose(Packets.Client.CSMG_ITEM_ENHANCE_CLOSE p)
        {
            this.itemEnhance = false;
            this.irisAddSlot = false;
        }
        public void OnItemEnhanceSelect(Packets.Client.CSMG_ITEM_ENHANCE_SELECT p)
        {
            Item item = this.chara.Inventory.GetItem(p.InventorySlot);
            if (item != null)
            {
                List<Packets.Server.EnhanceDetail> list = new List<SagaMap.Packets.Server.EnhanceDetail>();
                if (item.IsWeapon && item.PossessionedActor == null && item.PossessionOwner == null)
                {
                    if (CountItem(90000044) > 0)
                    {
                        Packets.Server.EnhanceDetail detail = new SagaMap.Packets.Server.EnhanceDetail();
                        detail.material = 90000044;
                        detail.type = SagaMap.Packets.Server.EnhanceType.Atk;
                        detail.value = FindEnhancementValue(item, 90000044);
                        list.Add(detail);
                    }
                    if (CountItem(90000045) > 0)
                    {
                        Packets.Server.EnhanceDetail detail = new SagaMap.Packets.Server.EnhanceDetail();
                        detail.material = 90000045;
                        detail.type = SagaMap.Packets.Server.EnhanceType.MAtk;
                        detail.value = FindEnhancementValue(item, 90000045);
                        list.Add(detail);
                    }
                    if (CountItem(90000046) > 0)
                    {
                        Packets.Server.EnhanceDetail detail = new SagaMap.Packets.Server.EnhanceDetail();
                        detail.material = 90000046;
                        detail.type = SagaMap.Packets.Server.EnhanceType.Cri;
                        detail.value = FindEnhancementValue(item, 90000046);
                        list.Add(detail);
                    }
                }
                if (item.IsArmor && item.PossessionedActor == null && item.PossessionOwner == null)
                {
                    if (CountItem(90000043) > 0)
                    {
                        Packets.Server.EnhanceDetail detail = new SagaMap.Packets.Server.EnhanceDetail();
                        detail.material = 90000043;
                        detail.type = SagaMap.Packets.Server.EnhanceType.HP;
                        detail.value = FindEnhancementValue(item, 90000043);
                        list.Add(detail);
                    }
                    if (CountItem(90000044) > 0)
                    {
                        Packets.Server.EnhanceDetail detail = new SagaMap.Packets.Server.EnhanceDetail();
                        detail.material = 90000044;
                        detail.type = SagaMap.Packets.Server.EnhanceType.Def;
                        detail.value = FindEnhancementValue(item, 90000044);
                        list.Add(detail);
                    }
                    if (CountItem(90000045) > 0)
                    {
                        Packets.Server.EnhanceDetail detail = new SagaMap.Packets.Server.EnhanceDetail();
                        detail.material = 90000045;
                        detail.type = SagaMap.Packets.Server.EnhanceType.MDef;
                        detail.value = FindEnhancementValue(item, 90000045);
                        list.Add(detail);
                    }
                    if (CountItem(90000046) > 0)
                    {
                        Packets.Server.EnhanceDetail detail = new SagaMap.Packets.Server.EnhanceDetail();
                        detail.material = 90000046;
                        detail.type = SagaMap.Packets.Server.EnhanceType.AvoidCri;
                        detail.value = FindEnhancementValue(item, 90000046);
                        list.Add(detail);
                    }
                }
                if (item.BaseData.itemType == ItemType.SHIELD && item.PossessionedActor == null && item.PossessionOwner == null)
                {
                    if (CountItem(90000044) > 0)
                    {
                        Packets.Server.EnhanceDetail detail = new SagaMap.Packets.Server.EnhanceDetail();
                        detail.material = 90000044;
                        detail.type = SagaMap.Packets.Server.EnhanceType.Def;
                        detail.value = FindEnhancementValue(item, 90000044);
                        list.Add(detail);
                    }
                    if (CountItem(90000045) > 0)
                    {
                        Packets.Server.EnhanceDetail detail = new SagaMap.Packets.Server.EnhanceDetail();
                        detail.material = 90000045;
                        detail.type = SagaMap.Packets.Server.EnhanceType.MDef;
                        detail.value = FindEnhancementValue(item, 90000045);
                        list.Add(detail);
                    }
                }
                if (item.BaseData.itemType == ItemType.ACCESORY_NECK && item.PossessionedActor == null && item.PossessionOwner == null)
                {
                    if (CountItem(90000045) > 0)
                    {
                        Packets.Server.EnhanceDetail detail = new SagaMap.Packets.Server.EnhanceDetail();
                        detail.material = 90000045;
                        detail.type = SagaMap.Packets.Server.EnhanceType.MDef;
                        detail.value = FindEnhancementValue(item, 90000045);
                        list.Add(detail);
                    }
                }

                Packets.Server.SSMG_ITEM_ENHANCE_DETAIL p1 = new SagaMap.Packets.Server.SSMG_ITEM_ENHANCE_DETAIL();
                p1.Items = list;
                this.netIO.SendPacket(p1);
            }
        }
        //ECOKEY 解決混強問題
        public short FindEnhancementValue2(Item item, uint itemID)
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
                    if (item.IsArmor)
                    {
                        return hps[r + 1];
                    }
                    break;
                case 90000044:
                case 90000054:
                case 10087401:
                case 16004700:
                    if (item.IsWeapon)
                    {
                        return atk_def_matk[r + 1];
                    }
                    else
                    {
                        if (item.IsArmor || item.BaseData.itemType == ItemType.SHIELD)
                        {
                            return atk_def_matk[r + 1];
                        }
                    }
                    break;
                case 90000045:
                case 90000055:
                case 10087403:
                case 16004800:
                    if (item.IsWeapon)
                    {
                        return atk_def_matk[r + 1];
                    }
                    else
                    {
                        return mdef[r + 1];
                    }
                case 90000046:
                case 90000056:
                case 10087402:
                case 16004500:
                    if (item.IsWeapon)
                    {
                        return cris[r + 1];
                    }
                    if (item.IsArmor)
                    {
                        return cris[r + 1];
                    }
                    break;
            }
            return 0;
        }

        public short FindEnhancementValue(Item item, uint itemID)
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
            //新增其他結晶
            switch (itemID)
            {
                case 90000043:
                case 90000053:
                case 10087400:
                case 16004600:
                    if (item.IsArmor)
                    {
                        short i = 0;
                        for (int j = 0; j <= item.LifeEnhance; j++)
                            i += hps[j];
                        return (short)(i + hps[item.LifeEnhance + 1]);
                    }
                    break;
                case 90000044:
                case 90000054:
                case 10087401:
                case 16004700:
                    if (item.IsWeapon)
                    {
                        short i = 0;
                        for (int j = 0; j <= item.PowerEnhance; j++)
                            i += atk_def_matk[j];
                        return (short)(i + atk_def_matk[item.PowerEnhance + 1]);
                    }
                    else
                    {
                        if (item.IsArmor || item.BaseData.itemType == ItemType.SHIELD)
                        {
                            short i = 0;
                            for (int j = 0; j <= item.PowerEnhance; j++)
                                i += atk_def_matk[j];
                            return (short)(i + atk_def_matk[item.PowerEnhance + 1]);
                        }
                    }
                    break;
                case 90000045:
                case 90000055:
                case 10087403:
                case 16004800:
                    if (item.IsWeapon)
                    {
                        short i = 0;
                        for (int j = 0; j <= item.MagEnhance; j++)
                            i += atk_def_matk[j];
                        return (short)(i + atk_def_matk[item.MagEnhance + 1]);
                    }
                    else
                    {
                        short i = 0;
                        for (int j = 0; j <= item.MagEnhance; j++)
                            i += mdef[j];
                        return (short)(i + mdef[item.MagEnhance + 1]);
                    }
                case 90000046:
                case 90000056:
                case 10087402:
                case 16004500:
                    if (item.IsWeapon)
                    {
                        short i = 0;
                        for (int j = 0; j <= item.CritEnhance; j++)
                            i += cris[j];
                        return (short)(i + cris[item.CritEnhance + 1]);
                    }
                    if (item.IsArmor)
                    {
                        short i = 0;
                        for (int j = 0; j <= item.CritEnhance; j++)
                            i += cris[j];
                        return (short)(i + cris[item.CritEnhance + 1]);
                    }
                    break;
            }
            return 0;
        }

        /// <summary>
        /// 取得玩家身上指定道具的信息
        /// </summary>
        /// <param name="ID">道具ID</param>
        /// <returns>道具清单</returns>
        protected List<SagaDB.Item.Item> GetItem(uint ID)
        {
            List<SagaDB.Item.Item> result = new List<SagaDB.Item.Item>();
            for (int i = 2; i < 6; i++)
            {
                List<SagaDB.Item.Item> list = this.chara.Inventory.Items[(ContainerType)i];
                var query = from it in list
                            where it.ItemID == ID
                            select it;
                result.AddRange(query);
            }
            return result;
        }

        public void OnItemEnhanceConfirm(Packets.Client.CSMG_ITEM_ENHANCE_CONFIRM p)
        {
            Item item = this.Character.Inventory.GetItem(p.InventorySlot);
            bool failed = false;
            bool success = false;
            Packets.Server.SSMG_ITEM_ENHANCE_RESULT p1 = new SagaMap.Packets.Server.SSMG_ITEM_ENHANCE_RESULT();
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


                        Logger.ShowInfo("BaseLevel: " + p.BaseLevel + " JLevel: " + p.JobLevel);
                        Logger.ShowInfo("ExpRate: " + p.ExpRate + " JExpRate: " + p.JExpRate);

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

                        //重寫！ - KK
                        //成功率加成
                        //解决了保护和辅助道具强行使用的问题 by [黑白照] 2018.07.02 
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
                            //ECOKEY 解決混強問題，以下這四個if都有改動，要覆蓋（0413再修復）
                            if (item.IsArmor)
                            {
                                switch (p.Material)
                                {
                                    //一般水晶
                                    case 90000043:
                                        item.HP += FindEnhancementValue2(item, 90000043);
                                        DeleteItemID(90000043, 1, true);
                                        item.LifeEnhance++;
                                        break;
                                    case 90000044:
                                        item.Def += FindEnhancementValue2(item, 90000044);
                                        DeleteItemID(90000044, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 90000045:
                                        item.MDef += FindEnhancementValue2(item, 90000045);
                                        DeleteItemID(90000045, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 90000046:
                                        item.AvoidCritical += FindEnhancementValue2(item, 90000046);
                                        DeleteItemID(90000046, 1, true);
                                        item.CritEnhance++;
                                        break;


                                    //強化水晶
                                    case 90000053:
                                        item.HP += FindEnhancementValue2(item, 90000043);
                                        DeleteItemID(90000053, 1, true);
                                        item.LifeEnhance++;
                                        break;
                                    case 90000054:
                                        item.Def += FindEnhancementValue2(item, 90000054);
                                        DeleteItemID(90000054, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 90000055:
                                        item.MDef += FindEnhancementValue2(item, 90000055);
                                        DeleteItemID(90000055, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 90000056:
                                        item.AvoidCritical += FindEnhancementValue2(item, 90000056);
                                        DeleteItemID(90000056, 1, true);
                                        item.CritEnhance++;
                                        break;


                                    //超強化水晶
                                    case 16004600:
                                        item.HP += FindEnhancementValue2(item, 16004600);
                                        DeleteItemID(16004600, 1, true);
                                        item.LifeEnhance++;
                                        break;
                                    case 16004700:
                                        item.Def += FindEnhancementValue2(item, 16004700);
                                        DeleteItemID(16004700, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 16004800:
                                        item.MDef += FindEnhancementValue2(item, 16004800);
                                        DeleteItemID(16004800, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 16004500:
                                        item.AvoidCritical += FindEnhancementValue2(item, 16004500);
                                        DeleteItemID(16004500, 1, true);
                                        item.CritEnhance++;
                                        break;


                                    //強化王
                                    case 10087400:
                                        item.HP += FindEnhancementValue2(item, 10087400);
                                        DeleteItemID(10087400, 1, true);
                                        item.LifeEnhance++;
                                        break;
                                    case 10087401:
                                        item.Def += FindEnhancementValue2(item, 10087401);
                                        DeleteItemID(10087401, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 10087403:
                                        item.MDef += FindEnhancementValue2(item, 10087403);
                                        DeleteItemID(10087403, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 10087402:
                                        item.AvoidCritical += FindEnhancementValue2(item, 10087402);
                                        DeleteItemID(10087402, 1, true);
                                        item.CritEnhance++;
                                        break;

                                }
                            }
                            if (item.IsWeapon)
                            {
                                switch (p.Material)
                                {
                                    //一般水晶
                                    case 90000044:
                                        item.Atk1 += FindEnhancementValue2(item, 90000044);
                                        item.Atk2 += FindEnhancementValue2(item, 90000044);
                                        item.Atk3 += FindEnhancementValue2(item, 90000044);
                                        DeleteItemID(90000044, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 90000045:
                                        item.MAtk += FindEnhancementValue2(item, 90000045);
                                        DeleteItemID(90000045, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 90000046:
                                        item.HitCritical += FindEnhancementValue2(item, 90000046);
                                        DeleteItemID(90000046, 1, true);
                                        item.CritEnhance++;
                                        break;

                                    //強化水晶
                                    case 90000054:
                                        item.Atk1 += FindEnhancementValue2(item, 90000054);
                                        item.Atk2 += FindEnhancementValue2(item, 90000054);
                                        item.Atk3 += FindEnhancementValue2(item, 90000054);
                                        DeleteItemID(90000054, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 90000055:
                                        item.MAtk += FindEnhancementValue2(item, 90000055);
                                        DeleteItemID(90000055, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 90000056:
                                        item.HitCritical += FindEnhancementValue2(item, 90000056);
                                        DeleteItemID(90000056, 1, true);
                                        item.CritEnhance++;
                                        break;

                                    //超強化水晶
                                    case 16004700:
                                        item.Atk1 += FindEnhancementValue2(item, 16004700);
                                        item.Atk2 += FindEnhancementValue2(item, 16004700);
                                        item.Atk3 += FindEnhancementValue2(item, 16004700);
                                        DeleteItemID(16004700, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 16004800:
                                        item.MAtk += FindEnhancementValue2(item, 16004800);
                                        DeleteItemID(16004800, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 16004500:
                                        item.HitCritical += FindEnhancementValue2(item, 16004500);
                                        DeleteItemID(16004500, 1, true);
                                        item.CritEnhance++;
                                        break;

                                    //強化王水晶
                                    case 10087401:
                                        item.Atk1 += FindEnhancementValue2(item, 10087401);
                                        item.Atk2 += FindEnhancementValue2(item, 10087401);
                                        item.Atk3 += FindEnhancementValue2(item, 10087401);
                                        DeleteItemID(10087401, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 10087403:
                                        item.MAtk += FindEnhancementValue2(item, 10087403);
                                        DeleteItemID(10087403, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 10087402:
                                        item.HitCritical += FindEnhancementValue2(item, 10087402);
                                        DeleteItemID(10087402, 1, true);
                                        item.CritEnhance++;
                                        break;

                                }
                            }
                            if (item.BaseData.itemType == ItemType.SHIELD)
                            {
                                switch (p.Material)
                                {
                                    //一般水晶
                                    case 90000044:
                                        item.Def += FindEnhancementValue2(item, 90000044);
                                        DeleteItemID(90000044, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 90000045:
                                        item.MDef += FindEnhancementValue2(item, 90000045);
                                        DeleteItemID(90000045, 1, true);
                                        item.MagEnhance++;
                                        break;

                                    //強化水晶
                                    case 90000054:
                                        item.Def += FindEnhancementValue2(item, 90000054);
                                        DeleteItemID(90000054, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 90000055:
                                        item.MDef += FindEnhancementValue2(item, 90000055);
                                        DeleteItemID(90000055, 1, true);
                                        item.MagEnhance++;
                                        break;
                                        ;

                                    //超強化水晶
                                    case 16004700:
                                        item.Def += FindEnhancementValue2(item, 16004700);
                                        DeleteItemID(16004700, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 16004800:
                                        item.MDef += FindEnhancementValue2(item, 16004800);
                                        DeleteItemID(16004800, 1, true);
                                        item.MagEnhance++;
                                        break;

                                    //強化王
                                    case 10087401:
                                        item.Def += FindEnhancementValue2(item, 10087401);
                                        DeleteItemID(10087401, 1, true);
                                        item.PowerEnhance++;
                                        break;
                                    case 10087403:
                                        item.MDef += FindEnhancementValue2(item, 10087403);
                                        DeleteItemID(10087403, 1, true);
                                        item.MagEnhance++;
                                        break;
                                }
                            }
                            if (item.BaseData.itemType == ItemType.ACCESORY_NECK)
                            {
                                switch (p.Material)
                                {
                                    case 90000045:
                                        item.MDef += FindEnhancementValue2(item, 90000045);
                                        DeleteItemID(90000045, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 90000055:
                                        item.MDef += FindEnhancementValue2(item, 90000055);
                                        DeleteItemID(90000055, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 16004800:
                                        item.MDef += FindEnhancementValue2(item, 16004800);
                                        DeleteItemID(16004800, 1, true);
                                        item.MagEnhance++;
                                        break;
                                    case 10087403:
                                        item.MDef += FindEnhancementValue2(item, 10087403);
                                        DeleteItemID(10087403, 1, true);
                                        item.MagEnhance++;
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
                          where (((enhanctitem.IsArmor && ((CountItem(90000044) > 0 || CountItem(90000045) > 0 || CountItem(90000046) > 0 || CountItem(90000043) > 0 || CountItem(10087400) > 0 || CountItem(10087401) > 0) || CountItem(10087402) > 0 || CountItem(10087403) > 0))
            || (enhanctitem.IsWeapon && (CountItem(90000044) > 0 || CountItem(90000045) > 0 || CountItem(90000046) > 0 || CountItem(10087401) > 0) || CountItem(10087402) > 0 || CountItem(10087403) > 0))
            || (enhanctitem.BaseData.itemType == ItemType.SHIELD && (CountItem(90000044) > 0 || CountItem(90000045) > 0 || CountItem(10087401) > 0 || CountItem(10087403) > 0))
            || (enhanctitem.BaseData.itemType == ItemType.ACCESORY_NECK && (CountItem(90000044) > 0 || CountItem(10087403) > 0)) && item.Refine < 30 && this.Character.Gold >= 5000)
                          select enhanctitem;
                List<SagaDB.Item.Item> items = res.ToList();

                foreach (var itemsitem in res.ToList())
                {
                    if (itemsitem.IsArmor)
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
                    if (itemsitem.IsWeapon)
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
                    if (itemsitem.BaseData.itemType == ItemType.SHIELD)
                    {

                        //力量结晶
                        if (CountItem(90000044) > 0)
                            if (!items.Exists(x => x.ItemID == 90000044))
                                items.AddRange(GetItem(90000044));

                        //魔力结晶
                        if (CountItem(90000045) > 0)
                            if (!items.Exists(x => x.ItemID == 90000045))
                                items.AddRange(GetItem(90000045));


                        //力量強化结晶
                        if (CountItem(90000054) > 0)
                            if (!items.Exists(x => x.ItemID == 90000054))

                                //魔力強化结晶
                                if (CountItem(90000055) > 0)
                                    if (!items.Exists(x => x.ItemID == 90000055))
                                        items.AddRange(GetItem(90000055));



                        //力量超強化结晶
                        if (CountItem(16004700) > 0)
                            if (!items.Exists(x => x.ItemID == 16004700))
                                items.AddRange(GetItem(16004700));

                        //魔力超強化结晶
                        if (CountItem(16004800) > 0)
                            if (!items.Exists(x => x.ItemID == 16004800))
                                items.AddRange(GetItem(16004800));



                        //强化王的力量
                        if (CountItem(10087401) > 0)
                            if (!items.Exists(x => x.ItemID == 10087401))
                                items.AddRange(GetItem(10087401));

                        //强化王的魔力
                        if (CountItem(10087403) > 0)
                            if (!items.Exists(x => x.ItemID == 10087403))
                                items.AddRange(GetItem(10087403));




                    }
                    if (itemsitem.BaseData.itemType == ItemType.ACCESORY_NECK)
                    {

                        //魔力结晶
                        if (CountItem(90000045) > 0)
                            if (!items.Exists(x => x.ItemID == 90000045))
                                items.AddRange(GetItem(90000045));


                        //魔力強化结晶
                        if (CountItem(90000055) > 0)
                            if (!items.Exists(x => x.ItemID == 90000055))
                                items.AddRange(GetItem(90000055));



                        //魔力超強化结晶
                        if (CountItem(16004800) > 0)
                            if (!items.Exists(x => x.ItemID == 16004800))
                                items.AddRange(GetItem(16004800));


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
                    Packets.Server.SSMG_ITEM_ENHANCE_LIST p2 = new SagaMap.Packets.Server.SSMG_ITEM_ENHANCE_LIST();
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
                          where (((enhanctitem.IsArmor && ((CountItem(90000044) > 0 || CountItem(90000045) > 0 || CountItem(90000046) > 0 || CountItem(90000043) > 0 || CountItem(10087400) > 0 || CountItem(10087401) > 0) || CountItem(10087402) > 0 || CountItem(10087403) > 0))
            || (enhanctitem.IsWeapon && (CountItem(90000044) > 0 || CountItem(90000045) > 0 || CountItem(90000046) > 0 || CountItem(10087401) > 0) || CountItem(10087402) > 0 || CountItem(10087403) > 0))
            || (enhanctitem.BaseData.itemType == ItemType.SHIELD && (CountItem(90000044) > 0 || CountItem(90000045) > 0 || CountItem(10087401) > 0 || CountItem(10087403) > 0))
            || (enhanctitem.BaseData.itemType == ItemType.ACCESORY_NECK && (CountItem(90000044) > 0 || CountItem(10087403) > 0)) && item.Refine < 30 && this.Character.Gold >= 5000)
                          select enhanctitem;
                List<SagaDB.Item.Item> items = res.ToList();

                foreach (var itemsitem in res.ToList())
                {
                    if (itemsitem.IsArmor)
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
                    if (itemsitem.IsWeapon)
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
                    if (itemsitem.BaseData.itemType == ItemType.SHIELD)
                    {

                        //力量结晶
                        if (CountItem(90000044) > 0)
                            if (!items.Exists(x => x.ItemID == 90000044))
                                items.AddRange(GetItem(90000044));

                        //魔力结晶
                        if (CountItem(90000045) > 0)
                            if (!items.Exists(x => x.ItemID == 90000045))
                                items.AddRange(GetItem(90000045));


                        //力量強化结晶
                        if (CountItem(90000054) > 0)
                            if (!items.Exists(x => x.ItemID == 90000054))

                                //魔力強化结晶
                                if (CountItem(90000055) > 0)
                                    if (!items.Exists(x => x.ItemID == 90000055))
                                        items.AddRange(GetItem(90000055));



                        //力量超強化结晶
                        if (CountItem(16004700) > 0)
                            if (!items.Exists(x => x.ItemID == 16004700))
                                items.AddRange(GetItem(16004700));

                        //魔力超強化结晶
                        if (CountItem(16004800) > 0)
                            if (!items.Exists(x => x.ItemID == 16004800))
                                items.AddRange(GetItem(16004800));



                        //强化王的力量
                        if (CountItem(10087401) > 0)
                            if (!items.Exists(x => x.ItemID == 10087401))
                                items.AddRange(GetItem(10087401));

                        //强化王的魔力
                        if (CountItem(10087403) > 0)
                            if (!items.Exists(x => x.ItemID == 10087403))
                                items.AddRange(GetItem(10087403));




                    }
                    if (itemsitem.BaseData.itemType == ItemType.ACCESORY_NECK)
                    {

                        //魔力结晶
                        if (CountItem(90000045) > 0)
                            if (!items.Exists(x => x.ItemID == 90000045))
                                items.AddRange(GetItem(90000045));


                        //魔力強化结晶
                        if (CountItem(90000055) > 0)
                            if (!items.Exists(x => x.ItemID == 90000055))
                                items.AddRange(GetItem(90000055));



                        //魔力超強化结晶
                        if (CountItem(16004800) > 0)
                            if (!items.Exists(x => x.ItemID == 16004800))
                                items.AddRange(GetItem(16004800));


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
                    Packets.Server.SSMG_ITEM_ENHANCE_LIST p2 = new SagaMap.Packets.Server.SSMG_ITEM_ENHANCE_LIST();
                    p2.Items = items;
                    this.netIO.SendPacket(p2);
                }
                else
                {
                    this.itemEnhance = false;
                    p1 = new SagaMap.Packets.Server.SSMG_ITEM_ENHANCE_RESULT();
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


        public void OnItemUse(Packets.Client.CSMG_ITEM_USE p)
        {
            if (this.Character.Status.Additions.ContainsKey("Meditatioon"))
            {
                this.Character.Status.Additions["Meditatioon"].AdditionEnd();
                this.Character.Status.Additions.Remove("Meditatioon");
            }
            if (this.Character.Status.Additions.ContainsKey("Hiding"))
            {
                this.Character.Status.Additions["Hiding"].AdditionEnd();
                this.Character.Status.Additions.Remove("Hiding");
            }
            if (this.Character.Status.Additions.ContainsKey("fish"))
            {
                this.Character.Status.Additions["fish"].AdditionEnd();
                this.Character.Status.Additions.Remove("fish");
            }
            if (this.Character.Status.Additions.ContainsKey("Cloaking"))
            {
                this.Character.Status.Additions["Cloaking"].AdditionEnd();
                this.Character.Status.Additions.Remove("Cloaking");
            }
            if (this.Character.Status.Additions.ContainsKey("IAmTree"))
            {
                this.Character.Status.Additions["IAmTree"].AdditionEnd();
                this.Character.Status.Additions.Remove("IAmTree");
            }
            if (this.Character.Status.Additions.ContainsKey("Invisible"))
            {
                this.Character.Status.Additions["Invisible"].AdditionEnd();
                this.Character.Status.Additions.Remove("Invisible");
            }

            if (this.Character.PossessionTarget != 0)
            {
                var act = Map.GetActor(this.Character.PossessionTarget);
                if (act is ActorPC)
                {
                    act = act as ActorPC;
                    if (act.Status.Additions.ContainsKey("Meditatioon"))
                    {
                        act.Status.Additions["Meditatioon"].AdditionEnd();
                        act.Status.Additions.Remove("Meditatioon");
                    }
                    if (act.Status.Additions.ContainsKey("Hiding"))
                    {
                        act.Status.Additions["Hiding"].AdditionEnd();
                        act.Status.Additions.Remove("Hiding");
                    }
                    if (act.Status.Additions.ContainsKey("fish"))
                    {
                        act.Status.Additions["fish"].AdditionEnd();
                        act.Status.Additions.Remove("fish");
                    }
                    if (act.Status.Additions.ContainsKey("Cloaking"))
                    {
                        act.Status.Additions["Cloaking"].AdditionEnd();
                        act.Status.Additions.Remove("Cloaking");
                    }
                    if (act.Status.Additions.ContainsKey("IAmTree"))
                    {
                        act.Status.Additions["IAmTree"].AdditionEnd();
                        act.Status.Additions.Remove("IAmTree");
                    }
                    if (act.Status.Additions.ContainsKey("Invisible"))
                    {
                        act.Status.Additions["Invisible"].AdditionEnd();
                        act.Status.Additions.Remove("Invisible");
                    }
                }
            }

            Item item = this.Character.Inventory.GetItem(p.InventorySlot);
            SkillArg arg = new SkillArg();
            arg.sActor = this.Character.ActorID;
            arg.dActor = p.ActorID;
            arg.item = item;
            arg.x = p.X;
            arg.y = p.Y;
            arg.argType = SkillArg.ArgType.Item_Cast;
            arg.inventorySlot = p.InventorySlot;
            if (item == null)
                return;
            Actor dActor = this.map.GetActor(p.ActorID);

            if (Character.Account.GMLevel > 0)
                FromActorPC(Character).SendSystemMessage("道具ID：" + item.ItemID.ToString());

            if (item.BaseData.itemType == ItemType.POTION || item.BaseData.itemType == ItemType.FOOD)
            {
                if (Character.Status.Additions.ContainsKey("FOODCD"))
                {
                    FromActorPC(Character).SendSystemMessage("暂时吃不下食物了哦...(30秒CD)");
                    arg.result = -21;
                }
                else
                {
                    //Skill.Additions.Global.DefaultBuff cd = new Skill.Additions.Global.DefaultBuff(Character, "FOODCD", 30000);
                    //SkillHandler.ApplyAddition(Character, cd);
                }

            }
            if (item.BaseData.itemType == ItemType.PETFOOD)
            {
                arg.result = -21;
            }

            if (item.BaseData.itemType == ItemType.GOLEM && (item.Durability <= 0 || this.Character.Inventory.Payload[ContainerType.BODY] >= this.Character.Inventory.MaxPayload[ContainerType.BODY]))
            {
                arg.result = -21;
            }

            //ECOKEY 機器人
            /*if ((item.BaseData.itemType == ItemType.ROBOT_GROW_ATKHIT || 
                item.BaseData.itemType == ItemType.ROBOT_GROW_MAGSKLHIT ||
                item.BaseData.itemType == ItemType.ROBOT_GROW_ATKDAM ||
                item.BaseData.itemType == ItemType.ROBOT_GROW_MAGSKLDAM ||
                item.BaseData.itemType == ItemType.ROBOT_GROW_CRIHIT ||
                item.BaseData.itemType == ItemType.ROBOT_GROW_MEAL))*/
            if (item.ItemID >= 10023001 && item.ItemID <= 10023007)
            {
                if (this.Character.Pet == null || this.Character.Pet.PetID != 10840000)
                {
                    arg.result = -32;
                }
                if (!RobotGrowthLimits(item.ItemID))//ECOKEY 機器人上限判斷
                {
                    FromActorPC(Character).SendSystemMessage("強化已達到上限");
                    return;
                }
            }
            //ECOKEY 蔬菜棒
            if (item.ItemID >= 10034401 && item.ItemID <= 10034407)
            {
                if (this.Character.Partner == null)
                {
                    FromActorPC(Character).SendSystemMessage("請召喚夥伴後再使用");
                    return;
                }
                else
                {
                    arg.dActor = this.Character.Partner.ActorID;
                }
            }

            if (this.Character.PossessionTarget != 0)
            {
                Actor posse = this.Map.GetActor(this.Character.PossessionTarget);
                if (posse != null)
                {
                    if (posse.type == ActorType.PC)
                    {
                        if (arg.dActor == this.Character.ActorID)
                            arg.dActor = posse.ActorID;
                    }
                    else
                    {
                        arg.result = -21;
                    }
                }
            }
            if (item.BaseData.itemType == ItemType.MARIONETTE && arg.result == 0)
            {
                //ECOKEY 大逃殺禁止活動木偶
                if (this.Character.Mode == PlayerMode.BATTLE_NORTH ||
                    this.Character.Mode == PlayerMode.BATTLE_SOUTH ||
                    this.Character.MapID == 20080011)
                {
                    arg.result = -16;
                }
                if (this.Character.Marionette == null)
                {
                    if (DateTime.Now < this.Character.NextMarionetteTime)
                        arg.result = -18;
                }
                if (this.Character.Pet != null)
                {
                    if (this.Character.Pet.Ride)
                        arg.result = -32;
                }
                if (this.Character.PossessionTarget != 0 || this.Character.PossesionedActors.Count > 0)
                {
                    arg.result = -16;
                }
                if (this.chara.Race == PC_RACE.DEM)
                {
                    arg.result = -33;
                }
            }

            if (this.GetPossessionTarget() != null && arg.result == 0)
            {
                if (this.GetPossessionTarget().Buff.Dead && !(item.ItemID == 10000604 || item.ItemID == 10034104))
                    arg.result = -27;
                if (arg.result == 0)
                {
                    if (item.ItemID == 10022900)
                        arg.result = -3;
                }
            }
            if (dActor != null && arg.result == 0)
            {
                if (!dActor.Buff.Dead && (item.ItemID == 10000604 || item.ItemID == 10034104))
                {
                    arg.result = -23;
                }
            }
            if (this.scriptThread != null && arg.result == 0)
            {
                arg.result = -7;
            }

            if (this.Character.Buff.Dead && arg.result == 0)
            {
                arg.result = -9;
            }
            if (this.Character.Buff.GetReadyPossession && arg.result == 0)
                arg.result = -3;

            if (arg.result == 0)
            {
                if (this.Character.Tasks.ContainsKey("ItemCast"))
                    arg.result = -19;
            }
            //ECOKEY 冷卻禁止施放
            if (DateTime.Now < skillDelay)
            {
                arg.result = -38;
            }
            if (arg.result == 0)
            {
                if (item.BaseData.itemType == ItemType.UNION_FOOD)
                {
                    //if (!OnPartnerFeed(item.Slot)) return;//ECOKEY 關閉此處的棒棒糖
                }
                this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, arg, this.Character, true);
                uint casttime = item.BaseData.cast;
                if (item.BaseData.itemType == ItemType.POTION || item.BaseData.itemType == ItemType.FOOD)
                    casttime = 2000;
                if (item.BaseData.cast > 0)
                {
                    Tasks.PC.SkillCast task = new SagaMap.Tasks.PC.SkillCast(this, arg);
                    this.Character.Tasks.Add("ItemCast", task);
                    task.Activate();
                }
                else
                {
                    OnItemCastComplete(arg);
                }

                //Cancel Cloak
                if (this.Character.Status.Additions.ContainsKey("Cloaking"))
                    SagaMap.Skill.SkillHandler.RemoveAddition(this.Character, "Cloaking");



                if (this.Character.PossessionTarget != 0)
                {
                    Map map = Manager.MapManager.Instance.GetMap(this.Character.MapID);
                    Actor TargetPossessionActor = map.GetActor(this.Character.PossessionTarget);

                    if (TargetPossessionActor.Status.Additions.ContainsKey("Cloaking"))
                        SagaMap.Skill.SkillHandler.RemoveAddition(TargetPossessionActor, "Cloaking");
                }
            }
            else
            {
                this.Character.e.OnActorSkillUse(this.Character, arg);
            }
        }

        public void OnItemCastComplete(SkillArg skill)
        {
            if (this.Character.Status.Additions.ContainsKey("Meditatioon"))
            {
                this.Character.Status.Additions["Meditatioon"].AdditionEnd();
                this.Character.Status.Additions.Remove("Meditatioon");
            }
            if (this.Character.Status.Additions.ContainsKey("Hiding"))
            {
                this.Character.Status.Additions["Hiding"].AdditionEnd();
                this.Character.Status.Additions.Remove("Hiding");
            }
            if (this.Character.Status.Additions.ContainsKey("fish"))
            {
                this.Character.Status.Additions["fish"].AdditionEnd();
                this.Character.Status.Additions.Remove("fish");
            }
            if (this.Character.Status.Additions.ContainsKey("IAmTree"))
            {
                this.Character.Status.Additions["IAmTree"].AdditionEnd();
                this.Character.Status.Additions.Remove("IAmTree");
            }
            if (this.Character.Status.Additions.ContainsKey("Cloaking"))
            {
                this.Character.Status.Additions["Cloaking"].AdditionEnd();
                this.Character.Status.Additions.Remove("Cloaking");
            }
            if (this.Character.Status.Additions.ContainsKey("Invisible"))
            {
                this.Character.Status.Additions["Invisible"].AdditionEnd();
                this.Character.Status.Additions.Remove("Invisible");
            }

            if (this.Character.PossessionTarget != 0)
            {
                var act = Map.GetActor(this.Character.PossessionTarget);
                if (act is ActorPC)
                {
                    act = act as ActorPC;
                    if (act.Status.Additions.ContainsKey("Meditatioon"))
                    {
                        act.Status.Additions["Meditatioon"].AdditionEnd();
                        act.Status.Additions.Remove("Meditatioon");
                    }
                    if (act.Status.Additions.ContainsKey("Hiding"))
                    {
                        act.Status.Additions["Hiding"].AdditionEnd();
                        act.Status.Additions.Remove("Hiding");
                    }
                    if (act.Status.Additions.ContainsKey("fish"))
                    {
                        act.Status.Additions["fish"].AdditionEnd();
                        act.Status.Additions.Remove("fish");
                    }
                    if (act.Status.Additions.ContainsKey("Cloaking"))
                    {
                        act.Status.Additions["Cloaking"].AdditionEnd();
                        act.Status.Additions.Remove("Cloaking");
                    }
                    if (act.Status.Additions.ContainsKey("IAmTree"))
                    {
                        act.Status.Additions["IAmTree"].AdditionEnd();
                        act.Status.Additions.Remove("IAmTree");
                    }
                    if (act.Status.Additions.ContainsKey("Invisible"))
                    {
                        act.Status.Additions["Invisible"].AdditionEnd();
                        act.Status.Additions.Remove("Invisible");
                    }
                }
            }

            if (skill.dActor != 0xFFFFFFFF)
            {
                Actor dActor = this.Map.GetActor(skill.dActor);
                this.Character.Tasks.Remove("ItemCast");
                skill.argType = SkillArg.ArgType.Item_Active;
                SkillHandler.Instance.ItemUse(this.Character, dActor, skill);
            }
            else
            {
                this.Character.Tasks.Remove("ItemCast");
                skill.argType = SkillArg.ArgType.Item_Active;
            }

            if (skill.item.BaseData.usable || skill.item.BaseData.itemType == ItemType.POTION ||
                skill.item.BaseData.itemType == ItemType.SCROLL ||
                skill.item.BaseData.itemType == ItemType.FREESCROLL)
            {
                if (skill.item.Durability > 0)
                    skill.item.Durability--;
                SendItemInfo(skill.item);
                if (skill.item.Durability == 0)
                {
                    Logger.LogItemLost(Logger.EventType.ItemUseLost, this.Character.Name + "(" + this.Character.CharID + ")", skill.item.BaseData.name + "(" + skill.item.ItemID + ")",
                           string.Format("ItemUse Count:{0}", 1), false);
                    DeleteItem(skill.inventorySlot, 1, true);
                }
            }
            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SKILL, skill, this.Character, true);
            if (skill.item.BaseData.effectID != 0)
            {
                EffectArg eff = new EffectArg();
                eff.actorID = skill.dActor;
                eff.effectID = skill.item.BaseData.effectID;
                this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, eff, this.Character, true);
            }
            //ECOKEY 機器人
            if (skill.item.ItemID >= 10023001 && skill.item.ItemID <= 10023007)
            {
                RobotGrowth(skill.item.ItemID);
            }
            if (skill.item.ItemID >= 10047800 && skill.item.ItemID <= 10047852)
            {
                OnItemRepair(skill.item);
            }
            else if (skill.item.ActiveSkillID != 0)
            {
                Packets.Client.CSMG_SKILL_CAST p1 = new SagaMap.Packets.Client.CSMG_SKILL_CAST();
                p1.ActorID = skill.dActor;
                p1.SkillID = skill.item.ActiveSkillID;
                p1.SkillLv = 1;
                p1.X = skill.x;
                p1.Y = skill.y;
                p1.Random = (short)Global.Random.Next();
                //ECOKEY 道具使用不須MPSP
                OnSkillCast(p1, false, true);
                // OnSkillCast(p1, true, true);
            }
          /*  //萬能藥
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Poisen))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Poisen] == 100)
                {
                    if (this.Character.Status.Additions.ContainsKey("Poison"))
                    {
                        this.Character.Status.Additions["Poison"].AdditionEnd();
                        this.Character.Status.Additions.Remove("Poison");
                    }
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Stone))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Stone] == 100)
                {
                    if (this.Character.Status.Additions.ContainsKey("Stone"))
                    {
                        this.Character.Status.Additions["Stone"].AdditionEnd();
                        this.Character.Status.Additions.Remove("Stone");
                    }
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Paralyse))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Paralyse] == 100)
                {
                    if (this.Character.Status.Additions.ContainsKey("Paralyse"))
                    {
                        this.Character.Status.Additions["Paralyse"].AdditionEnd();
                        this.Character.Status.Additions.Remove("Paralyse");
                    }
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Sleep))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Sleep] == 100)
                {
                    if (this.Character.Status.Additions.ContainsKey("Sleep"))
                    {
                        this.Character.Status.Additions["Sleep"].AdditionEnd();
                        this.Character.Status.Additions.Remove("Sleep");
                    }
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Silence))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Silence] == 100)
                {
                    if (this.Character.Status.Additions.ContainsKey("Silence"))
                    {
                        this.Character.Status.Additions["Silence"].AdditionEnd();
                        this.Character.Status.Additions.Remove("Silence");
                    }
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.MoveSpeedDown))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.MoveSpeedDown] == 100)
                {
                    if (this.Character.Status.Additions.ContainsKey("MoveSpeedDown"))
                    {
                        this.Character.Status.Additions["MoveSpeedDown"].AdditionEnd();
                        this.Character.Status.Additions.Remove("MoveSpeedDown");
                    }
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Confused))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Confused] == 100)
                {
                    if (this.Character.Status.Additions.ContainsKey("Confused"))
                    {
                        this.Character.Status.Additions["Confused"].AdditionEnd();
                        this.Character.Status.Additions.Remove("Confused");
                    }
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Frosen))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Frosen] == 100)
                {
                    if (this.Character.Status.Additions.ContainsKey("Frosen"))
                    {
                        this.Character.Status.Additions["Frosen"].AdditionEnd();
                        this.Character.Status.Additions.Remove("Frosen");
                    }
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.Stun))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.Stun] == 100)
                {
                    if (this.Character.Status.Additions.ContainsKey("Stun"))
                    {
                        this.Character.Status.Additions["Stun"].AdditionEnd();
                        this.Character.Status.Additions.Remove("Stun");
                    }
                }
            }
            if (skill.item.BaseData.abnormalStatus.ContainsKey(AbnormalStatus.SkillForbid))
            {
                if (skill.item.BaseData.abnormalStatus[AbnormalStatus.SkillForbid] == 100)
                {
                    if (this.Character.Status.Additions.ContainsKey("SkillForbid"))
                    {
                        this.Character.Status.Additions["SkillForbid"].AdditionEnd();
                        this.Character.Status.Additions.Remove("SkillForbid");
                    }
                }
            }*/
            //
            if (skill.item.BaseData.itemType == ItemType.MARIONETTE)
            {
                if (this.Character.Marionette == null)
                {
                    MarionetteActivate(skill.item.BaseData.marionetteID);
                }
                else
                {
                    if (!this.Character.Status.Additions.ContainsKey("ChangeMarionette"))
                        MarionetteDeactivate();
                    else
                        MarionetteActivate(skill.item.BaseData.marionetteID, false, false);
                    return;
                }
            }
            if (skill.item.BaseData.eventID != 0)
            {
                if (skill.item.BaseData.eventID == 90000529)
                    Character.TInt["技能块ItemID"] = (int)skill.item.ItemID;
                EventActivate(skill.item.BaseData.eventID);
            }

            if (skill.item.ItemID > kujiboxID0 && skill.item.ItemID <= kujiboxID0 + kujinum_max)
            {
                DeleteItem(skill.inventorySlot, 1, false);
                OnKujiBoxOpen(skill.item);
            }
            if (skill.item.BaseData.itemType == ItemType.GOLEM)
            {
                if (skill.item.Durability <= 0)
                    return;


                if (this.Character.Golem == null)
                    this.Character.Golem = new ActorGolem();
                this.Character.Golem.Item = skill.item;
                EventActivate(0xFFFFFF33);
            }
        }

        //ECOKEY 機器人上限判斷
        bool RobotGrowthLimits(uint itemid)
        {
            if (this.Character.Pet == null)
                return false;
            ActorPet pet = (ActorPet)this.Character.Pet;
            if (pet.Owner.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.PET))
            {
                SagaDB.Item.Item item = pet.Owner.Inventory.Equipments[EnumEquipSlot.PET];
                switch (itemid)
                {
                    //力量回路
                    case 10023001:
                        if (pet.Limits.atk_max <= item.Atk1 && pet.Limits.hit_melee <= item.HitMelee && pet.Limits.hit_ranged <= item.HitRanged)
                            return false;
                        break;
                    //魔法回路
                    case 10023002:
                        if (pet.Limits.matk_max <= item.MAtk && pet.Limits.hit_magic <= item.HitMagic)
                            return false;
                        break;
                    //防御回路
                    case 10023003:
                        if (pet.Limits.def_add <= item.Def && pet.Limits.avoid_melee <= item.AvoidMelee && pet.Limits.avoid_ranged <= item.AvoidRanged && pet.Limits.hp <= item.HP)
                            return false;
                        break;
                    //魔法防御回路
                    case 10023004:
                        if (pet.Limits.mdef_add <= item.MDef && pet.Limits.avoid_magic <= item.AvoidMagic)
                            return false;
                        break;
                    //會心一擊迴路
                    case 10023005:
                        if (pet.Limits.cri <= item.HitCritical && pet.Limits.criavd <= item.AvoidCritical)
                            return false;
                        break;
                    //修復迴路
                    case 10023007:
                        Logger.ShowInfo("判斷中" + pet.Limits.physicreduce.ToString() + "/" + item.HPRecover.ToString());
                        Logger.ShowInfo("判斷中" + pet.Limits.magicreduce.ToString() + "/" + item.MPRecover.ToString());
                        if (pet.Limits.physicreduce <= item.HPRecover && pet.Limits.magicreduce <= item.MPRecover)
                        {
                            Logger.ShowInfo("禁止");
                            return false;
                        }

                        break;
                }
                return true;
            }
            return false;
        }

        //ECOKEY 機器人
        void RobotGrowth(uint itemid)
        {
            if (this.Character.Pet == null)
                return;
            ActorPet pet = (ActorPet)this.Character.Pet;
            if (pet.Owner.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.PET))
            {
                SagaDB.Item.Item item = pet.Owner.Inventory.Equipments[EnumEquipSlot.PET];
                item.Refine = 1;
                switch (itemid)
                {
                    //力量回路
                    case 10023001:
                        //物理攻擊力+2
                        if (pet.Limits.atk_max > item.Atk1)
                        {
                            item.Atk1 += 2;
                            item.Atk2 += 2;
                            item.Atk3 += 2;

                            pet.Status.min_atk1 += 2;
                            pet.Status.max_atk1 += 2;
                            pet.Status.min_atk2 += 2;
                            pet.Status.max_atk2 += 2;
                            pet.Status.min_atk3 += 2;
                            pet.Status.max_atk3 += 2;
                        }
                        //近距離命中+1
                        if (pet.Limits.hit_melee > item.HitMelee)
                        {
                            item.HitMelee++;
                            pet.Status.hit_melee++;
                        }
                        //遠距離命中+1
                        if (pet.Limits.hit_ranged > item.HitRanged)
                        {
                            item.HitRanged++;
                            pet.Status.hit_ranged++;
                        }

                        break;
                    //魔法回路
                    case 10023002:
                        //魔法攻擊力+2
                        if (pet.Limits.matk_max > item.MAtk)
                        {
                            item.MAtk += 2;
                            pet.Status.min_matk += 2;
                            pet.Status.max_matk += 2;
                        }
                        //魔法命中+2
                        if (pet.Limits.hit_magic > item.HitMagic)
                        {
                            item.HitMagic += 2;
                            pet.Status.hit_magic += 2;
                        }
                        break;
                    //防御回路
                    case 10023003:
                        //防禦力+2
                        if (pet.Limits.def_add > item.Def)
                        {
                            item.Def += 2;
                            pet.Status.def_add += 2;
                        }
                        //近距離迴避+1
                        if (pet.Limits.avoid_melee > item.AvoidMelee)
                        {
                            item.AvoidMelee++;
                            pet.Status.avoid_melee++;
                        }
                        //遠距離迴避+1
                        if (pet.Limits.avoid_ranged > item.AvoidRanged)
                        {
                            item.AvoidRanged++;
                            pet.Status.avoid_ranged++;
                        }
                        //HP+2
                        if (pet.Limits.hp > item.HP)
                        {
                            item.HP += 2;
                            pet.MaxHP += 2;
                        }
                        break;
                    //魔法防御回路
                    case 10023004:
                        //魔法防禦+2
                        if (pet.Limits.mdef_add > item.MDef)
                        {
                            item.MDef += 2;
                            pet.Status.mdef_add += 2;
                        }
                        //魔法抵抗+2
                        if (pet.Limits.avoid_magic > item.AvoidMagic)
                        {
                            item.AvoidMagic += 2;
                            pet.Status.avoid_magic += 2;
                        }
                        break;
                    //會心一擊迴路
                    case 10023005:
                        //會心一擊+2
                        if (pet.Limits.cri > item.HitCritical)
                        {
                            item.HitCritical += 2;
                            pet.Status.hit_critical += 2;
                        }
                        //會心一擊迴避+2
                        if (pet.Limits.criavd > item.AvoidCritical)
                        {
                            item.AvoidCritical += 2;
                            pet.Status.avoid_critical += 2;
                        }
                        break;
                    //修復迴路
                    case 10023007:
                        //回復力+2
                        if (pet.Limits.physicreduce > item.HPRecover)
                        {
                            item.HPRecover += 2;
                            pet.Status.hp_recover += 2;
                        }
                        //魔法回復力+2
                        if (pet.Limits.magicreduce > item.MPRecover)
                        {
                            item.MPRecover += 2;
                            pet.Status.mp_recover += 2;
                        }
                        break;
                }
                if (pet.Owner.Online)
                {
                    if (pet.Ride)
                    {
                        SagaMap.PC.StatusFactory.Instance.CalcStatus(pet.Owner);
                        MapClient.FromActorPC(pet.Owner).SendPlayerInfo();
                    }
                    MapClient.FromActorPC(pet.Owner).SendItemInfo(item);
                }
            }
        }

        int GetKujiRare(List<Kuji> kuji)
        {//
            int min = int.MaxValue;
            for (int i = 0; i < kuji.Count; i++)
            {
                min = Math.Min(min, kuji[0].rank);
            }
            return min;
        }
        private void OnKujiBoxOpen(Item box)
        {
            uint kujiID = box.ItemID - kujiboxID0;

            if (KujiListFactory.Instance.KujiList.ContainsKey(kujiID))
            {
                List<Kuji> kujis = KujiListFactory.Instance.KujiList[kujiID];
                if (kujis.Count == 0) return;
                int rare = GetKujiRare(KujiListFactory.Instance.KujiList[kujiID]);

                List<int> rates = new List<int>();
                int r = 0;
                for (int i = 0; i < kujis.Count; i++)
                {
                    r = r + kujis[i].rate;
                    rates.Add(r);
                }
                SkillHandler.Instance.ShowEffectOnActor(Character, 8056);
                int ratemin = 0;
                int ratemax = rates[rates.Count - 1];
                int ran = Global.Random.Next(ratemin, ratemax);
                for (int i = 0; i < kujis.Count; i++)
                {
                    if (ran <= rates[i])
                    {
                        switch (kujis[i].rank)
                        {
                            case 1:
                                Character.AInt["SSS保底次数"] = 0;
                                SendSystemMessage("啧，可恶的欧洲人");
                                break;
                            case 2:
                            case 3:
                                Character.AInt["SS保底次数"] = 0;
                                SendSystemMessage("嘁，可恶的欧洲人");
                                break;
                            default:
                                switch (rare)
                                {
                                    case 1:
                                        Character.AInt["SSS保底次数"]++;
                                        SendSystemMessage("如果连续200次未抽到SSS级头赏，将获得彩虹钥匙。当前次数：" + Character.AInt["SSS保底次数"].ToString() + "/200");
                                        if (Character.AInt["SSS保底次数"] >= 200)
                                        {
                                            Character.AInt["SSS保底次数累计"]++;
                                            Character.AInt["SSS保底次数"] = 0;
                                            SendSystemMessage("由于您连续200次未抽到SSS级头赏，获得了彩虹钥匙。");
                                            Item item = ItemFactory.Instance.GetItem(950000032);
                                            AddItem(item, true);
                                        }
                                        break;
                                    case 2:
                                    case 3:
                                        Character.AInt["SS保底次数"]++;
                                        SendSystemMessage("如果连续200次未抽到SS/S级头赏，将获得金钥匙。当前次数：" + Character.AInt["SS保底次数"].ToString() + "/200");
                                        if (Character.AInt["SS保底次数"] >= 200)
                                        {
                                            Character.AInt["SS保底次数累计"]++;
                                            Character.AInt["SS保底次数"] = 0;
                                            SendSystemMessage("由于您连续200次未抽到SS/S级头赏，获得了金钥匙。");
                                            Item item = ItemFactory.Instance.GetItem(950000031);
                                            AddItem(item, true);
                                            TitleProccess(Character, 8, 1);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                        }
                        Item kuji = ItemFactory.Instance.GetItem(kujis[i].itemid);
                        AddItem(kuji, true);
                        break;
                    }
                }
            }
        }

        public void OnItemRepair(SagaDB.Item.Item item)
        {
            List<Item> RepairItems = new List<Item>();
            foreach (var i in this.Character.Inventory.Items)
            {
                foreach (Item items in i.Value)
                {
                    if (items.BaseData.repairItem == item.BaseData.id)
                    {
                        RepairItems.Add(items);
                    }
                }
            }
            Packets.Server.SSMG_ITEM_EQUIP_REPAIR_LIST p = new Packets.Server.SSMG_ITEM_EQUIP_REPAIR_LIST();
            p.Items = RepairItems;
            this.netIO.SendPacket(p);
        }

        public void OnItemDrop(Packets.Client.CSMG_ITEM_DROP p)
        {
            if (this.Character.Status.Additions.ContainsKey("Meditatioon"))
            {
                this.Character.Status.Additions["Meditatioon"].AdditionEnd();
                this.Character.Status.Additions.Remove("Meditatioon");
            }
            if (this.Character.Status.Additions.ContainsKey("Hiding"))
            {
                this.Character.Status.Additions["Hiding"].AdditionEnd();
            }
            if (this.Character.Status.Additions.ContainsKey("Cloaking"))
            {
                this.Character.Status.Additions["Cloaking"].AdditionEnd();
            }
            if (this.Character.Status.Additions.ContainsKey("fish"))
            {
                this.Character.Status.Additions["fish"].AdditionEnd();
                this.Character.Status.Additions.Remove("fish");
            }
            if (this.Character.Status.Additions.ContainsKey("IAmTree"))
            {
                this.Character.Status.Additions["IAmTree"].AdditionEnd();
                this.Character.Status.Additions.Remove("IAmTree");
            }
            if (this.Character.Status.Additions.ContainsKey("Invisible"))
            {
                this.Character.Status.Additions["Invisible"].AdditionEnd();
                this.Character.Status.Additions.Remove("Invisible");
            }
            Item itemDroped = this.Character.Inventory.GetItem(p.InventorySlot);
            ushort count = p.Count;

            if (count > itemDroped.Stack)
                count = itemDroped.Stack;

            Packets.Server.SSMG_ITEM_PUT_ERROR p1 = new SagaMap.Packets.Server.SSMG_ITEM_PUT_ERROR();

            if (itemDroped.BaseData.events == 1)
            {
                p1.ErrorID = -3;
                this.netIO.SendPacket(p1);
                return;
            }
            if (this.trading == true)
            {
                p1.ErrorID = -8;
                this.netIO.SendPacket(p1);
                return;
            }

            if (this.Character.Golem != null)
            {
                if (this.Character.Golem.GolemType == GolemType.Sell && this.Character.Golem.SellShop.Count != 0)
                {
                    if (this.Character.Golem.SellShop.ContainsKey(itemDroped.Slot))
                    {
                        p1.ErrorID = -6;
                        this.netIO.SendPacket(p1);
                        return;
                    }
                }
            }

            if (itemDroped.BaseData.noDrop && TradeConditionFactory.Instance.Items[itemDroped.ItemID].GMLevelToIgnoreCondition > this.Character.Account.GMLevel)
            {
                p1.ErrorID = -16;
                this.netIO.SendPacket(p1);
                return;
            }

            if (itemDroped.BaseData.itemType == ItemType.DEMIC_CHIP)
            {
                p1.ErrorID = -18;
                this.netIO.SendPacket(p1);
                return;
            }


            //ECOKEY 重製寵物
            if (itemDroped.BaseData.itemType == ItemType.PARTNER || itemDroped.BaseData.itemType == ItemType.RIDE_PARTNER)
            {
                if (this.Character.CInt["partnerwarning"] == 0)
                {
                    MapClient client1 = MapClient.FromActorPC(this.Character);
                    uint Event = 90000530;
                    client1.EventActivate(Event);
                    this.Character.CInt["partnerwarning"] = 1;
                    return;
                }
                ActorPartner partner = MapServer.charDB.GetActorPartner(itemDroped.ActorPartnerID, itemDroped);
                ResetPetQualities(partner);
            }

            if (itemDroped.Stack > 0)
            {
                Logger.LogItemLost(Logger.EventType.ItemDropLost, this.Character.Name + "(" + this.Character.CharID + ")", itemDroped.BaseData.name + "(" + itemDroped.ItemID + ")",
                    string.Format("Drop Count:{0}", count), false);
            }

            InventoryDeleteResult result = this.Character.Inventory.DeleteItem(p.InventorySlot, count);
            switch (result)
            {
                case InventoryDeleteResult.STACK_UPDATED:
                    Packets.Server.SSMG_ITEM_COUNT_UPDATE p2 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                    Item item = this.Character.Inventory.GetItem(p.InventorySlot);
                    itemDroped = item.Clone();
                    itemDroped.Stack = count;
                    p2.InventorySlot = p.InventorySlot;
                    p2.Stack = item.Stack;
                    this.netIO.SendPacket(p2);
                    break;
                case InventoryDeleteResult.ALL_DELETED:
                    Packets.Server.SSMG_ITEM_DELETE p3 = new SagaMap.Packets.Server.SSMG_ITEM_DELETE();
                    p3.InventorySlot = p.InventorySlot;
                    this.netIO.SendPacket(p3);
                    break;
            }
            ActorItem actor = new ActorItem(itemDroped);
            actor.e = new ActorEventHandlers.ItemEventHandler(actor);
            actor.MapID = this.Character.MapID;
            actor.X = this.Character.X;
            actor.Y = this.Character.Y;
            if (!itemDroped.BaseData.noTrade) //7月27日更新，取消交易
            {
                actor.Owner = chara;
                actor.CreateTime = DateTime.Now;
            }
            this.map.RegisterActor(actor);
            actor.invisble = false;
            this.map.OnActorVisibilityChange(actor);
            Tasks.Item.DeleteItem task = new SagaMap.Tasks.Item.DeleteItem(actor);
            task.Activate();
            actor.Tasks.Add("DeleteItem", task);

            this.Character.Inventory.CalcPayloadVolume();
            this.SendCapacity();

            this.SendSystemMessage(string.Format(LocalManager.Instance.Strings.ITEM_DELETED, itemDroped.BaseData.name, itemDroped.Stack));

        }

        //ECOKEY 重製寵物
        private void ResetPetQualities(ActorPartner partner)
        {
            if (partner != null)
            {
                // 重置寵物相關素質
                //Rank跟稀有度
                partner.rank = 0;
                //信賴度等級
                partner.reliability = 0;
                //信賴度exp
                partner.reliabilityexp = 0;
                //成長點數
                partner.perkpoint = 0;
                //已加的成長點數
                partner.perk0 = 0;
                partner.perk1 = 0;
                partner.perk2 = 0;
                partner.perk3 = 0;
                partner.perk4 = 0;
                partner.perk5 = 0;
                // 重置寵物技能塊
                //新增避免出生技能被刪除
                for (int i = 0; i <= partner.equipcubes_activeskill.Count; i++)
                {
                    foreach (ushort nowList2 in partner.equipcubes_activeskill)
                    {
                        var result = partner.BaseData.born_active_cubes.Exists(t => t == nowList2);
                        if (result == false)
                        {
                            partner.equipcubes_activeskill.Remove(nowList2);
                            break;
                        }
                    }
                }
                for (int i = 0; i <= partner.equipcubes_action.Count; i++)
                {
                    foreach (ushort nowList in partner.equipcubes_action)
                    {
                        var result = partner.BaseData.born_skills.Exists(t => t == nowList);
                        if (result == false)
                        {
                            partner.equipcubes_action.Remove(nowList);
                        }
                    }
                }

                //if (partner.equipcubes_activeskill.Count != 0) partner.equipcubes_activeskill.Clear();
                if (partner.equipcubes_passiveskill.Count != 0) partner.equipcubes_passiveskill.Clear();
                //if (partner.equipcubes_action.Count != 0)  partner.equipcubes_action.Clear();
                if (partner.equipcubes_condition.Count != 0) partner.equipcubes_condition.Clear();
                //清除自定義
                if (partner.ai_conditions.Count != 0) partner.ai_conditions.Clear();
                if (partner.ai_intervals.Count != 0) partner.ai_intervals.Clear();
                if (partner.ai_reactions.Count != 0) partner.ai_reactions.Clear();
                if (partner.ai_states.Count != 0) partner.ai_states.Clear();

                // 保存寵物狀態
                MapServer.charDB.SavePartnerAI(partner);
                MapServer.charDB.SavePartnerCube(partner);
                MapServer.charDB.SavePartner(partner);
            }
        }

        public void OnItemGet(Packets.Client.CSMG_ITEM_GET p)
        {
            if (this.Character.Status.Additions.ContainsKey("Meditatioon"))
            {
                this.Character.Status.Additions["Meditatioon"].AdditionEnd();
                this.Character.Status.Additions.Remove("Meditatioon");
            }
            if (this.Character.Status.Additions.ContainsKey("Hiding"))
            {
                this.Character.Status.Additions["Hiding"].AdditionEnd();
            }
            /*
            if (this.Character.Status.Additions.ContainsKey("Cloaking"))
            {
                this.Character.Status.Additions["Cloaking"].AdditionEnd();
            }
            */
            if (this.Character.Status.Additions.ContainsKey("IAmTree"))
            {
                this.Character.Status.Additions["IAmTree"].AdditionEnd();
                this.Character.Status.Additions.Remove("IAmTree");
            }
            if (this.Character.Status.Additions.ContainsKey("Invisible"))
            {
                this.Character.Status.Additions["Invisible"].AdditionEnd();
                this.Character.Status.Additions.Remove("Invisible");
            }
            ActorItem item = (ActorItem)this.map.GetActor(p.ActorID);
            if (item == null)
                return;

            //SendSystemMessage("目前玩家持有數: " + (this.Character.Inventory.Items[ContainerType.BODY].Count + this.Character.Inventory.Equipments.Count));
            if (this.Character.Inventory.Items[ContainerType.BODY].Count + this.Character.Inventory.Equipments.Count >= 100)
            {
                SagaDB.Item.Item temp_item = this.chara.Inventory.GetItem(item.Item.BaseData.id, Inventory.SearchType.ITEM_ID);
                if (!temp_item.Stackable)
                {
                    Packets.Server.SSMG_ITEM_GET_ERROR p1 = new SagaMap.Packets.Server.SSMG_ITEM_GET_ERROR();
                    p1.ActorID = item.ActorID;
                    p1.ErrorID = -11;
                    this.netIO.SendPacket(p1);
                    return;
                }
            }

            if (item.Owner != null)
            {
                if (item.Owner.type == ActorType.PC)
                {
                    ActorPC pc = (ActorPC)item.Owner;
                    if (pc != Character && !item.Roll)
                    {
                        if (pc.Party != null && !item.Party)
                        {
                            if (!pc.Party.IsMember(this.Character) && !item.Party)
                            {
                                if ((DateTime.Now - item.CreateTime).TotalMinutes < 1)
                                {
                                    Packets.Server.SSMG_ITEM_GET_ERROR p1 = new SagaMap.Packets.Server.SSMG_ITEM_GET_ERROR();
                                    p1.ActorID = item.ActorID;
                                    p1.ErrorID = -10;
                                    this.netIO.SendPacket(p1);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if ((DateTime.Now - item.CreateTime).TotalSeconds < 30 || item.Party)
                            {
                                Packets.Server.SSMG_ITEM_GET_ERROR p1 = new SagaMap.Packets.Server.SSMG_ITEM_GET_ERROR();
                                p1.ActorID = item.ActorID;
                                p1.ErrorID = -10;
                                this.netIO.SendPacket(p1);
                                return;
                            }
                        }
                    }
                }
            }
            if (!item.PossessionItem)
            {
                if (Character.Party != null)
                {
                    if ((item.Roll) || (Character.Party.Roll == 0 && Character.Party != null))
                    {
                        bool mes = true;
                        if (Character.Party.Roll == 0) mes = false;
                        if (item.Roll) mes = true;
                        ActorPC winner = Character;
                        int MaxRate = 0;
                        foreach (var it in Character.Party.Members.Values)
                        {
                            if (it.MapID == Character.MapID && it.Online)
                            {
                                int rate = Global.Random.Next(0, 100);
                                if (rate > MaxRate)
                                {
                                    winner = it;
                                    MaxRate = rate;
                                }
                                /*
                                foreach (var item2 in Character.Party.Members.Values)
                                    if (mes && item2.Online)
                                        FromActorPC(item2).SendSystemMessage(it.Name + " 的拾取点数为:" + rate.ToString());
                                */
                            }
                        }
                        //string a = "";
                        //if (mes)
                        //a = "的点数最大，";
                        /*
                        foreach (var item2 in Character.Party.Members.Values)
                            if (item2.Online)
                                FromActorPC(item2).SendSystemMessage(winner.Name + a + " 获得了物品[" + item.Name + "]" + item.Item.Stack.ToString() + "个。");
                        */
                        item.LootedBy = winner.ActorID;
                        this.map.DeleteActor(item);
                        Logger.LogItemGet(Logger.EventType.ItemLootGet, this.Character.Name + "(" + this.Character.CharID + ")", item.Item.BaseData.name + "(" + item.Item.ItemID + ")",
                            string.Format("ItemLoot Count:{0}", item.Item.Stack), false);
                        FromActorPC(winner).AddItem(item.Item, true);

                        item.Tasks["DeleteItem"].Deactivate();
                        item.Tasks.Remove("DeleteItem");
                    }
                    else
                    {
                        item.LootedBy = this.Character.ActorID;
                        this.map.DeleteActor(item);
                        Logger.LogItemGet(Logger.EventType.ItemLootGet, this.Character.Name + "(" + this.Character.CharID + ")", item.Item.BaseData.name + "(" + item.Item.ItemID + ")",
                            string.Format("ItemLoot Count:{0}", item.Item.Stack), false);
                        AddItem(item.Item, true);

                        item.Tasks["DeleteItem"].Deactivate();
                        item.Tasks.Remove("DeleteItem");
                    }
                }
                else
                {
                    item.LootedBy = this.Character.ActorID;
                    this.map.DeleteActor(item);
                    Logger.LogItemGet(Logger.EventType.ItemLootGet, this.Character.Name + "(" + this.Character.CharID + ")", item.Item.BaseData.name + "(" + item.Item.ItemID + ")",
                        string.Format("ItemLoot Count:{0}", item.Item.Stack), false);
                    AddItem(item.Item, true);

                    item.Tasks["DeleteItem"].Deactivate();
                    item.Tasks.Remove("DeleteItem");
                }
            }
            else
            {
                foreach (EnumEquipSlot i in item.Item.EquipSlot)
                {
                    if (this.Character.Inventory.Equipments.ContainsKey(i))
                    {
                        Packets.Server.SSMG_ITEM_GET_ERROR p1 = new SagaMap.Packets.Server.SSMG_ITEM_GET_ERROR();
                        p1.ActorID = item.ActorID;
                        p1.ErrorID = -5;
                        this.netIO.SendPacket(p1);
                        return;
                    }
                }
                if (this.chara.Race == PC_RACE.DEM && this.chara.Form == DEM_FORM.MACHINA_FORM)
                {
                    Packets.Server.SSMG_ITEM_GET_ERROR p1 = new SagaMap.Packets.Server.SSMG_ITEM_GET_ERROR();
                    p1.ActorID = item.ActorID;
                    p1.ErrorID = -16;
                    this.netIO.SendPacket(p1);
                    return;

                }
                //if (Math.Abs(this.Character.Level - item.Item.PossessionedActor.Level) > 30)
                //{
                //    Packets.Server.SSMG_ITEM_GET_ERROR p1 = new SagaMap.Packets.Server.SSMG_ITEM_GET_ERROR();
                //    p1.ActorID = item.ActorID;
                //    p1.ErrorID = -4;
                //    this.netIO.SendPacket(p1);
                //    return;
                //}
                int result = CheckEquipRequirement(item.Item);
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
                //临时处理手段
                //SendSystemMessage("自凭依道具暂时无法捡起来");

                item.LootedBy = this.Character.ActorID;
                this.map.DeleteActor(item);
                Item addItem = item.Item.Clone();
                AddItem(addItem, false);
                Packets.Client.CSMG_ITEM_EQUIPT p2 = new SagaMap.Packets.Client.CSMG_ITEM_EQUIPT();
                p2.InventoryID = addItem.Slot;
                p2.EquipSlot = (byte)addItem.EquipSlot[0];
                OnItemEquipt(p2);
            }

            //this.SendItems();

            this.SendPlayerInfo();
        }

        public int CheckEquipRequirement(Item item)
        {
            if (this.Character.Account.GMLevel >= 200) return 0;

            //ECOKEY 融合後裝備判斷
            /* if (item.PictID != 0)
             {
                 item = ItemFactory.Instance.GetItem(item.PictID);
             }*/

            //ECOKEY 融合後裝備判斷
            if (item.BaseData.itemType == ItemType.PARTNER ||
                item.BaseData.itemType == ItemType.RIDE_PARTNER ||
                item.BaseData.itemType == ItemType.RIDE_PET ||
                item.BaseData.itemType == ItemType.RIDE_PET_ROBOT ||
                item.BaseData.itemType == ItemType.PET)
            {

            }
            else
            {
                if (item.PictID2 != 0)
                {
                    item = ItemFactory.Instance.GetItem(item.PictID2);
                }
                else
                {
                    if (item.PictID != 0)
                    {
                        item = ItemFactory.Instance.GetItem(item.PictID);
                    }
                }
            }

            if (this.Character.Buff.Dead || this.Character.Buff.Confused || this.Character.Buff.Frosen || this.Character.Buff.Paralysis || this.Character.Buff.Sleep || this.Character.Buff.Stone || this.Character.Buff.Stun)
                return -3;
            switch (item.BaseData.itemType)
            {
                case ItemType.ARROW:
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                    {
                        if (this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType != ItemType.BOW)
                            return -6;
                    }
                    else
                    {
                        return -6;
                    }
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                    {
                        Item oriItem = this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND];
                        if (oriItem.PossessionedActor != null)
                        {
                            return -10;
                        }
                    }
                    break;
                case ItemType.BULLET:
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                    {
                        if (this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType != ItemType.GUN &&
                            this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType != ItemType.RIFLE &&
                            this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType != ItemType.DUALGUN)
                        {
                            return -7;
                        }
                    }
                    else
                    {
                        return -7;
                    }
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                    {
                        Item oriItem = this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND];
                        if (oriItem.PossessionedActor != null)
                        {
                            return -10;
                        }
                    }
                    break;
                //
                case ItemType.SHIELD:
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                    {
                        if (this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.BOW ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.GUN ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.RIFLE ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.DUALGUN)
                        {
                            Item oriItem = this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND];
                            if (oriItem.PossessionedActor != null)
                            {
                                return -2;
                            }
                            return -2;
                        }
                    }
                    break;

                case ItemType.ACCESORY_FINGER:
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                    {
                        if (this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.BOW ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.GUN ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.RIFLE ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.DUALGUN)
                        {
                            Item oriItem = this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND];
                            if (oriItem.PossessionedActor != null)
                            {
                                return -2;
                            }
                            return -2;
                        }
                    }

                    break;
                case ItemType.LEFT_HANDBAG:
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                    {
                        if (this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.BOW ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.GUN ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.RIFLE ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.DUALGUN)
                        {
                            Item oriItem = this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND];
                            if (oriItem.PossessionedActor != null)
                            {
                                return -2;
                            }
                            return -2;
                        }
                    }
                    break;
                case ItemType.BOW:
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                    {
                        if (this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.SHIELD ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.ACCESORY_FINGER ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.LEFT_HANDBAG)
                        {
                            Item oriItem = this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND];
                            if (oriItem.PossessionedActor != null)
                            {
                                return -10;
                            }
                            return -2;
                        }
                    }
                    break;
                case ItemType.GUN:
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                    {
                        if (this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.SHIELD ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.ACCESORY_FINGER ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.LEFT_HANDBAG)
                        {
                            Item oriItem = this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND];
                            if (oriItem.PossessionedActor != null)
                            {
                                return -10;
                            }
                            return -2;
                        }
                    }
                    break;
                case ItemType.RIFLE:
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                    {
                        if (this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.SHIELD ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.ACCESORY_FINGER ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.LEFT_HANDBAG)
                        {
                            Item oriItem = this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND];
                            if (oriItem.PossessionedActor != null)
                            {
                                return -10;
                            }
                            return -2;
                        }
                    }
                    break;
                case ItemType.DUALGUN:
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                    {
                        if (this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.SHIELD ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.ACCESORY_FINGER ||
                            this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.LEFT_HANDBAG)
                        {
                            Item oriItem = this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND];
                            if (oriItem.PossessionedActor != null)
                            {
                                return -10;
                            }
                            return -2;
                        }
                    }
                    break;
            }
         

            if (item.Durability < 1 && item.maxDurability >= 1)
                return -12;
            if (this.Character.Str < item.BaseData.possibleStr)
                return -16;
            if (this.Character.Dex < item.BaseData.possibleDex)
                return -19;
            if (this.Character.Agi < item.BaseData.possibleAgi)
                return -20;
            if (this.Character.Vit < item.BaseData.possibleVit)
                return -18;
            if (this.Character.Int < item.BaseData.possibleInt)
                return -21;
            if (this.Character.Mag < item.BaseData.possibleMag)
                return -17;
            if (!item.BaseData.possibleRace[this.Character.Race])
                return -13;
            if (!item.BaseData.possibleGender[this.Character.Gender])
                return -14;
            byte lv = this.Character.Level;
            if (this.Character.Rebirth)
            {
                if (lv < (item.BaseData.possibleLv - 10))
                    return -15;
            }
            else if (lv < item.BaseData.possibleLv)
                return -15;
            if ((item.BaseData.itemType == ItemType.RIDE_PET || item.BaseData.itemType == ItemType.RIDE_PARTNER) && this.Character.Marionette != null)
                return -2;
            if (item.EquipSlot.Contains(EnumEquipSlot.RIGHT_HAND) || item.EquipSlot.Contains(EnumEquipSlot.LEFT_HAND))
            {
                if (Character.Skills3.ContainsKey(990))
                {
                    return 0;
                }
            }
            else
            {
                if (Character.Skills3.ContainsKey(991))
                {
                    return 0;
                }
            }
            if (!item.IsParts && this.chara.Race != PC_RACE.DEM)
            {
                if (this.Character.JobJoint == PC_JOB.NONE)
                {


                    if (this.Character.DualJobID != 0)
                    {
                        var dualjobinfo = DualJobInfoFactory.Instance.items[this.Character.DualJobID];
                        if (!item.BaseData.possibleJob[this.Character.Job])
                            if (!item.BaseData.possibleJob[(PC_JOB)dualjobinfo.BaseJobID])
                                if (!item.BaseData.possibleJob[(PC_JOB)dualjobinfo.ExperJobID])
                                    if (!item.BaseData.possibleJob[(PC_JOB)dualjobinfo.TechnicalJobID])
                                        if (!item.BaseData.possibleJob[(PC_JOB)dualjobinfo.ChronicleJobID])
                                            return -2;
                    }
                    else
                    {
                        if (!item.BaseData.possibleJob[this.Character.Job])
                            return -2;
                    }
                }
                else
                {
                    //ECOKEY 修復馴獸項能穿其他職業問題
                    if (!item.BaseData.possibleJob[this.Character.JobJoint])
                    {
                        if (this.Character.DualJobID != 0)
                        {
                            var dualjobinfo = DualJobInfoFactory.Instance.items[this.Character.DualJobID];
                            if (!item.BaseData.possibleJob[(PC_JOB)dualjobinfo.BaseJobID])
                                if (!item.BaseData.possibleJob[(PC_JOB)dualjobinfo.ExperJobID])
                                    if (!item.BaseData.possibleJob[(PC_JOB)dualjobinfo.TechnicalJobID])
                                        if (!item.BaseData.possibleJob[(PC_JOB)dualjobinfo.ChronicleJobID])
                                            return -2;
                        }
                        else
                        {
                            return -2;
                        }
                    }
                }
            }
            if (this.Character.Race == PC_RACE.DEM && this.Character.Form == DEM_FORM.MACHINA_FORM)//DEM的机械形态不能装备
                return -29;
            if (item.BaseData.possibleRebirth)
                if (!this.Character.Rebirth || this.Character.Job != this.Character.Job3)
                    return -31;
            return 0;
        }
        public void OnItemEquiptRepair(Packets.Client.CSMG_ITEM_EQUIPT_REPAIR p)
        {
            Item item = this.Character.Inventory.GetItem(p.InventoryID);
            if (CountItem(item.BaseData.repairItem) > 0)
            {
                if (item.maxDurability > item.Durability)
                {
                    item.Durability = (ushort)(item.maxDurability - 1);
                    item.maxDurability--;
                    EffectArg arg = new EffectArg();
                    arg.actorID = this.Character.ActorID;
                    arg.effectID = 8043;
                    this.Character.e.OnShowEffect(this.Character, arg);
                    this.SendItemInfo(item);
                    DeleteItemID(item.BaseData.repairItem, 1, true);
                }
            }
        }
        /// <summary>
        /// 装备卸下过程，卸下该格子里的装备对应的所有格子里的道具，并移除道具附加的技能
        /// </summary>
        public void OnItemUnequipt(EnumEquipSlot eqslot)
        {
            if (!this.Character.Inventory.Equipments.ContainsKey(eqslot))
                return;
            Item oriItem = this.Character.Inventory.Equipments[eqslot];
            CleanItemSkills(oriItem);
            foreach (EnumEquipSlot i in oriItem.EquipSlot)
            {
                if (this.Character.Inventory.Equipments.ContainsKey(i))
                {
                    if (this.Character.Inventory.Equipments[i].Stack == 0)
                    {
                        this.Character.Inventory.Equipments.Remove(i);
                    }
                    else
                    {
                        ItemMoveSub(this.Character.Inventory.Equipments[i], ContainerType.BODY, this.Character.Inventory.Equipments[i].Stack);
                    }
                }
            }
        }

        //围观梦美卖萌0.0
        //从头写！
        //重写&简化逻辑结构
        public void OnItemEquipt(Packets.Client.CSMG_ITEM_EQUIPT p)
        {
            OnItemEquipt(p.InventoryID, p.EquipSlot);
        }
        public void OnItemEquipt(uint InventoryID, byte EquipSlot)
        {
            //特殊状态解除
            if (this.Character.Status.Additions.ContainsKey("Meditatioon"))
            {
                this.Character.Status.Additions["Meditatioon"].AdditionEnd();
                this.Character.Status.Additions.Remove("Meditatioon");
            }
            if (this.Character.Status.Additions.ContainsKey("Hiding"))
            {
                this.Character.Status.Additions["Hiding"].AdditionEnd();
            }
            if (this.Character.Status.Additions.ContainsKey("Cloaking"))
            {
                this.Character.Status.Additions["Cloaking"].AdditionEnd();
            }
            if (this.Character.Status.Additions.ContainsKey("fish"))
            {
                this.Character.Status.Additions["fish"].AdditionEnd();
                this.Character.Status.Additions.Remove("fish");
            }
            if (this.Character.Status.Additions.ContainsKey("IAmTree"))
            {
                this.Character.Status.Additions["IAmTree"].AdditionEnd();
                this.Character.Status.Additions.Remove("IAmTree");
            }
            if (this.Character.Status.Additions.ContainsKey("Invisible"))
            {
                this.Character.Status.Additions["Invisible"].AdditionEnd();
                this.Character.Status.Additions.Remove("Invisible");
            }
            if (this.Character.Tasks.ContainsKey("Regeneration"))
            {
                this.Character.Tasks["Regeneration"].Deactivate();
                this.Character.Tasks.Remove("Regeneration");
            }
            if (this.Character.Tasks.ContainsKey("Scorponok"))
            {
                this.Character.Tasks["Scorponok"].Deactivate();
                this.Character.Tasks.Remove("Scorponok");
            }
            if (this.Character.Tasks.ContainsKey("自由射击"))
            {
                this.Character.Tasks["自由射击"].Deactivate();
                this.Character.Tasks.Remove("自由射击");
            }
            if (this.Character.Tasks.ContainsKey("Possession"))
            {
                this.Character.Tasks["Possession"].Deactivate();
                this.Character.Tasks.Remove("Possession");
                if (this.chara.Buff.GetReadyPossession)
                {
                    this.chara.Buff.GetReadyPossession = false;
                    this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, this.Character, true);

                }
            }
            /*  if (this.Character.Tasks.ContainsKey("Possession"))
              {
                  //ECOKEY PE進度條消失修復
                  this.PossessionCancel();
                  this.Character.Tasks["Possession"].Deactivate();
                  this.Character.Tasks.Remove("Possession");
                  if (this.chara.Buff.GetReadyPossession)
                  {
                      this.chara.Buff.GetReadyPossession = false;
                      this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, this.Character, true);

                  }
              }*/

            Item item = this.Character.Inventory.GetItem(InventoryID);

            ushort count = item.Stack;//count是实际移动数量，考虑弹药
            if (item == null)//不存在？卡住或者用外挂了？
            {
                return;
            }
            int result;//返回不能装备的类型
            result = CheckEquipRequirement(item);//检查装备条件

            //寵物親密度歸0
            if (item.BaseData.itemType == ItemType.RIDE_PET || item.BaseData.itemType == ItemType.RIDE_PARTNER || item.BaseData.itemType == ItemType.PARTNER || item.BaseData.itemType == ItemType.PET)
            {
                //ECOKEY  騎士團和大逃殺禁止召喚寵物
                List<uint> map = new List<uint>() { 10023001, 10032001, 10034001, 10042001, 10056001, 10064001, 20020001, 20080007, 20080008, 20080009, 20080010, 20080011 };
                if (item.BaseData.itemType == ItemType.PARTNER || item.BaseData.itemType == ItemType.PET)
                {
                    if (map.Contains(this.Character.MapID))
                    {
                        SendSystemMessage("此地圖禁止召喚寵物");
                        return;
                    }
                }
                //ECOKEY  騎士團騎寵召喚限制
                if (item.BaseData.itemType == ItemType.RIDE_PET || item.BaseData.itemType == ItemType.RIDE_PARTNER)
                {
                    if (map.Contains(this.Character.MapID))
                    {
                        //ECOKEY 冷卻30秒
                        if (this.Character.TTime["PartnerCDTime"] >= DateTime.Now)
                        {
                            SagaMap.Network.Client.MapClient.FromActorPC(this.Character).SendSystemMessage("裝備夥伴後30秒內無法再次更換夥伴");
                            return;
                        }
                        else
                        {
                            this.Character.TTime["PartnerCDTime"] = DateTime.Now.AddSeconds(30);
                        }
                    }
                }
                if (item.Durability < 1 && item.maxDurability >= 1)
                {
                    SendSystemMessage("您的夥伴在默默哭泣");
                    return;
                }

            }
            //ECOKEY 冷卻30秒舊版本
            /* if (this.Character.TTime["PartnerCDTime"] >= DateTime.Now)
             {
                 SagaMap.Network.Client.MapClient.FromActorPC(this.Character).SendSystemMessage("裝備夥伴後30秒內無法再次更換夥伴");
                 return;
             }*/
            //ECOKEY 冷卻30秒新版
            /*     if (this.Character.TTime["PartnerCDTime"] >= DateTime.Now)
                 {
                     SagaMap.Network.Client.MapClient.FromActorPC(this.Character).SendSystemMessage("裝備夥伴後30秒內無法再次更換夥伴");
                     return;
                 }
                 else
                 //ECOKEY 冷卻30秒
                 {
                     this.Character.TTime["PartnerCDTime"] = DateTime.Now.AddSeconds(30);
                 }*/
        

            if (result < 0)//不能装备
            {
                Packets.Server.SSMG_ITEM_EQUIP p4 = new SagaMap.Packets.Server.SSMG_ITEM_EQUIP();
                p4.InventorySlot = 0xffffffff;
                p4.Target = ContainerType.NONE;
                p4.Result = result;
                p4.Range = this.Character.Range;
                this.netIO.SendPacket(p4);
                return;
            }
            uint oldPetHP = 0;//原宠物HP，这次不想改

            List<EnumEquipSlot> targetslots = new List<EnumEquipSlot>(); //EquipSlot involved in this item target slots
            foreach (EnumEquipSlot i in item.EquipSlot)
            {
                if (!targetslots.Contains(i))
                    targetslots.Add(i);
            }

            //ECOKEY
            //卸下
            try
            {
                foreach (EnumEquipSlot i in targetslots) //解決當我要換裝備 點擊之後不會馬上穿上 而是會先脫掉 之後再裝備
                {
                    //检查
                    if (!this.Character.Inventory.Equipments.ContainsKey(i))
                    {
                        //该格子原来就是空的 直接下一个格子 特殊检查在循环外写
                        continue;
                    }

                    foreach (EnumEquipSlot j in this.Character.Inventory.Equipments[i].EquipSlot) //检查对应位置的之前穿的装备是否可脱下
                    {
                        Item oriItem = this.Character.Inventory.Equipments[j]; //???

                        if (!CheckPossessionForEquipMove(oriItem))
                        {
                            //装备被PY状态中不能移动,不能填装弹药
                            return;
                        }

                        /*   if (item.BaseData.itemType == ItemType.BOW)
                           {
                               Item oriItem1 = this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND];
                               if (!CheckPossessionForEquipMove(oriItem1))
                               {
                                   //装备被PY状态中不能移动,不能填装弹药
                                   return;
                               }
                           }*/

                        if (oriItem.NeedAmmo) //取下射击类装备前检查左手 如果左手有装备必然是弹药 需取下
                        {
                            if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                            {
                                Item ammo = this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND];
                                if (!CheckPossessionForEquipMove(ammo))
                                {
                                    //装备被PY状态中不能移动
                                    return;
                                }
                            }
                        }
                        //ECOKEY 裝著弓 帶著箭矢 然後換武器就可以持續裝著箭矢**
                        // 射擊類武器的特殊處理
                        /*  if (oriItem.NeedAmmo) //取下射击类装备前检查左手 如果左手有装备必然是弹药 需取下
                          {
                              if ((j == EnumEquipSlot.RIGHT_HAND) ||
                              (item.BaseData.itemType == ItemType.BOW || item.BaseData.itemType == ItemType.GUN ||
                               item.BaseData.itemType == ItemType.DUALGUN || item.BaseData.itemType == ItemType.RIFLE))
                          {
                              // 若將要換裝的是弓或槍類武器，需檢查是否有箭裝備在左手
                              if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND) &&
                                  this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.ARROW)
                              { }
                              else
                              {
                                  // 左手有箭，需先脫下
                                  ItemMoveSub(this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND], ContainerType.BODY, this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Stack);
                              }
                              }
                          }*/
                        //


                        if (i == EnumEquipSlot.UPPER_BODY)
                        {
                            Character.PossessionPartnerSlotIDinClothes = 0;
                            Character.Status.hp_petpy = 0;

                        }
                        if (i == EnumEquipSlot.RIGHT_HAND)
                        {
                            Character.PossessionPartnerSlotIDinRightHand = 0;
                            Character.Status.max_atk1_petpy = 0;
                            Character.Status.min_atk1_petpy = 0;
                            Character.Status.max_matk_petpy = 0;
                            Character.Status.min_matk_petpy = 0;
                        }
                        if (i == EnumEquipSlot.LEFT_HAND)
                        {
                            Character.PossessionPartnerSlotIDinLeftHand = 0;
                            Character.Status.def_add_petpy = 0;
                            Character.Status.mdef_add_petpy = 0;
                        }
                        if (i == EnumEquipSlot.CHEST_ACCE)
                        {
                            Character.PossessionPartnerSlotIDinAccesory = 0;
                            Character.Status.aspd_petpy = 0;
                            Character.Status.cspd_petpy = 0;
                        }

                        if (item.IsAmmo) //填装弹药，检查原左手道具是否是同种(之前检查过故左手必然是弹药)，若是，则不需取下，后面直接填装补充，否则直接卸下
                        {
                            if (this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].ItemID != item.ItemID)
                            {
                                //不是同种弹药 卸下
                                ItemMoveSub(this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND], ContainerType.BODY, this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Stack);
                            }
                            else //999检查
                            {
                                if (this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Stack + count > 999)
                                {
                                    count = (ushort)(999 - this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Stack);
                                }
                            }
                        }
                        else if (item.BaseData.itemType == ItemType.CARD || item.BaseData.itemType == ItemType.THROW) //填装投掷武器，检查原右手道具是否是同种，若是，则不需取下，后面直接填装补充，否则直接卸下
                        {
                            //若是双手的投掷类？？？ 以后再说。。。
                            if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                            {
                                if (this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].ItemID != item.ItemID)
                                {
                                    OnItemUnequipt(EnumEquipSlot.RIGHT_HAND);
                                }
                                else //999检查
                                {
                                    if (this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].Stack + count > 999)
                                    {
                                        count = (ushort)(999 - this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].Stack);
                                    }
                                }
                            }
                        }

                        else //将要装备的装备既不是射击武器也不是弹药也不是投掷武器
                        {
                            if ((j == EnumEquipSlot.RIGHT_HAND) || (j == EnumEquipSlot.LEFT_HAND))//手部装备需要卸下，需特别检查射击类装备相关
                            {
                                //包里东西出来
                                if (oriItem.BaseData.itemType == ItemType.HANDBAG)
                                {
                                    while (this.Character.Inventory.GetContainer(ContainerType.RIGHT_BAG).Count > 0)
                                    {
                                        Item content = this.Character.Inventory.GetContainer(ContainerType.RIGHT_BAG).First();
                                        ItemMoveSub(content, ContainerType.BODY, content.Stack);
                                    }
                                }
                                if (oriItem.BaseData.itemType == ItemType.LEFT_HANDBAG)
                                {
                                    while (this.Character.Inventory.GetContainer(ContainerType.LEFT_BAG).Count > 0)
                                    {
                                        Item content = this.Character.Inventory.GetContainer(ContainerType.LEFT_BAG).First();
                                        ItemMoveSub(content, ContainerType.BODY, content.Stack);
                                    }
                                }

                                //射击类相关左手判定：原来装备射击武器且将要装备的新道具（含左手）不是对应的弹药，需卸下右手的射击武器
                                if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND) && item.EquipSlot.Contains(EnumEquipSlot.LEFT_HAND))
                                {
                                    switch (this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType)
                                    {
                                        case ItemType.DUALGUN:
                                        case ItemType.GUN:
                                        case ItemType.RIFLE:
                                            if (item.BaseData.itemType != ItemType.BULLET)
                                            {
                                                //取下射击武器
                                                ItemMoveSub(this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND], ContainerType.BODY, this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Stack);
                                            }
                                            break;
                                        case ItemType.BOW:
                                            if (item.BaseData.itemType != ItemType.ARROW)
                                            {
                                                //取下射击武器
                                                ItemMoveSub(this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND], ContainerType.BODY, this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Stack);
                                            }
                                            break;
                                    }
                                }
                                //ECOKEY裝弓跟箭，換其他武器 箭會脫下
                                else if ((this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND)) &&
             (this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.ARROW ||
              this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.BULLET))
                                {
                                    // 如果右手不是射击武器但左手裝備箭或子彈，脫下箭或子彈
                                    ItemMoveSub(this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND], ContainerType.BODY, this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Stack);
                                }
                                //
                            }
                            else//非手部装备需要卸下
                            {
                                //宠物类装备卸下过程
                                if (j == EnumEquipSlot.PET)
                                {
                                    if (this.Character.Pet != null)
                                    {
                                        //ECOKEY 清空 
                                        /*if (this.Character.Pet.Ride)
                                        {
                                            //oldPetHP = this.Character.Pet.HP;
                                            //this.Character.HP = oldPetHP;
                                            //this.Character.Speed = Configuration.Instance.Speed;
                                            //this.Character.Pet = null;
                                        }*/
                                        DeletePet();
                                    }
                                    if (Character.Partner != null)
                                    {
                                        DeletePartner();
                                    }
                                }


                                //检查副职业切换
                                if (oriItem.BaseData.jointJob != PC_JOB.NONE)
                                {
                                    this.Character.JobJoint = PC_JOB.NONE;
                                }
                                //推进器效果？待检查
                                if (oriItem.BaseData.itemType == ItemType.BACK_DEMON)
                                {
                                    SkillHandler.RemoveAddition(this.chara, "MoveUp2");
                                    SkillHandler.RemoveAddition(this.chara, "MoveUp3");
                                }
                                //包里东西出来
                                if (oriItem.BaseData.itemType == ItemType.BACKPACK)
                                {
                                    while (this.Character.Inventory.GetContainer(ContainerType.BACK_BAG).Count > 0)
                                    {
                                        Item content = this.Character.Inventory.GetContainer(ContainerType.BACK_BAG).First();
                                        ItemMoveSub(content, ContainerType.BODY, content.Stack);
                                    }
                                }
                            }
                            //j位置的装备正式卸下
                            if (this.Character.Inventory.Equipments.Values.Contains(oriItem))//检查以防之前过程中已经卸下了
                            {
                                ItemMoveSub(oriItem, ContainerType.BODY, oriItem.Stack);
                            }
                        }
                    }
                    //卸下

                }

                //道具对应格子本来就是空着时却需要检查别的格子的特殊卸下
                if (item.NeedAmmo) //将要装备射击类武器，需额外检查左手，左手只能装备对应的弹药种类，否则都卸下左手装备
                {
                    //判定左手裝備是否處於裝備被PY狀態
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                    {
                        Item ammo = this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND];
                        if (!CheckPossessionForEquipMove(ammo))
                        {
                            //装备被PY状态中不能移动
                            return;
                        }
                    }

                    //弓装备前判定左手
                    if (item.BaseData.itemType == ItemType.BOW)
                    {
                        if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                        {
                            if (this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType != ItemType.ARROW)
                            {
                                ItemMoveSub(this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND], ContainerType.BODY, this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Stack);
                            }
                        }
                    }
                    //枪类装备前判定左手
                    if (item.BaseData.itemType == ItemType.GUN || item.BaseData.itemType == ItemType.DUALGUN || item.BaseData.itemType == ItemType.RIFLE)
                    {
                        if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                        {
                            if (this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.itemType != ItemType.BULLET)
                            {
                                ItemMoveSub(this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND], ContainerType.BODY, this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Stack);
                            }
                        }
                    }
                }
                else if (targetslots.Contains(EnumEquipSlot.LEFT_HAND) && (!item.IsAmmo)) //包含左手的非弹药道具(弹药与武器的匹配最先就检查过了)需要额外检查右手是不是射击武器，是否对应
                {
                    if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                    {
                        if (this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].NeedAmmo)
                        {
                            ItemMoveSub(this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND], ContainerType.BODY, this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Stack);
                        }

                    }
                }


                if (count == 0) return;
                if (this.Character.Inventory.MoveItem(this.Character.Inventory.GetContainerType(item.Slot), (int)item.Slot, (ContainerType)Enum.Parse(typeof(ContainerType), item.EquipSlot[0].ToString()), count))
                {
                    if (item.Stack == 0)
                    {
                        Packets.Server.SSMG_ITEM_EQUIP p4 = new SagaMap.Packets.Server.SSMG_ITEM_EQUIP();
                        p4.Target = (ContainerType)Enum.Parse(typeof(ContainerType), item.EquipSlot[0].ToString());
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
                    if (item.BaseData.itemType == ItemType.ARROW || item.BaseData.itemType == ItemType.BULLET)
                    {
                        if (item.Stack == 0)
                        {
                            this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Slot = item.Slot;
                        }
                        Packets.Server.SSMG_ITEM_COUNT_UPDATE p5 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                        p5.InventorySlot = this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Slot;
                        p5.Stack = this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Stack;
                        this.netIO.SendPacket(p5);
                    }
                    if (item.BaseData.itemType == ItemType.CARD || item.BaseData.itemType == ItemType.THROW)
                    {
                        if (item.Stack == 0)
                        {
                            this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].Slot = item.Slot;
                        }
                        Packets.Server.SSMG_ITEM_COUNT_UPDATE p5 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                        p5.InventorySlot = this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].Slot;
                        p5.Stack = this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].Stack;
                        this.netIO.SendPacket(p5);
                    }
                }
                //create dummy item to take the slots
                List<EnumEquipSlot> slots = item.EquipSlot;
                if (slots.Count > 1)
                {
                    for (int i = 1; i < slots.Count; i++)
                    {
                        Item dummy = item.Clone();
                        dummy.Stack = 0;
                        this.Character.Inventory.AddItem((ContainerType)Enum.Parse(typeof(ContainerType), slots[i].ToString()), dummy);
                    }
                }
                //renew stauts
                if (item.EquipSlot.Contains(EnumEquipSlot.RIGHT_HAND))
                {
                    SendAttackType();
                    SkillHandler.Instance.CastPassiveSkills(this.Character, false);
                }
                if (item.EquipSlot.Contains(EnumEquipSlot.LEFT_HAND))
                {
                    SkillHandler.Instance.CastPassiveSkills(this.Character, false);
                }
                SkillHandler.Instance.CheckBuffValid(this.Character);
                if (item.BaseData.itemType == ItemType.PET || item.BaseData.itemType == ItemType.PET_NEKOMATA)
                {
                    this.SendPet(item);
                }
                if (item.BaseData.itemType == ItemType.PARTNER)
                {
                    this.SendPartner(item);
                    Character.Inventory.Equipments[EnumEquipSlot.PET].ActorPartnerID = item.ActorPartnerID;
                    PartnerTalking(Character.Partner, TALK_EVENT.SUMMONED, 100, 0);
                }

                if (item.BaseData.itemType == ItemType.RIDE_PET || item.BaseData.itemType == ItemType.RIDE_PET_ROBOT)
                {
                    //ECOKEY 機器人（0310補上這裡！！）
                    SendPetRobot(item);

                }
                /*  if (item.BaseData.itemType == ItemType.RIDE_PARTNER)
                  {
                      ActorPet pet = new ActorPet(item.BaseData.petID, item);
                      pet.Owner = this.Character;
                      Character.Pet = pet;
                      pet.Ride = true;

                      if (!pet.Owner.CInt.ContainsKey("PC_HUNMAN_HP"))
                      {
                          pet.Owner.CInt["PC_HUNMAN_HP"] = (int)Character.HP;
                      }
                      //pet.MaxHP = 2000;
                      /*if (oldPetHP == 0)
                          pet.HP = this.Character.HP;
                      else
                          pet.HP = oldPetHP;
                      pet.HP = 99;
                      Character.HP = pet.MaxHP;*/
                //Character.Speed = 600;
                /*    Character.Speed = Configuration.Instance.Speed;

                    //SendSystemMessage("[提示]在骑宠时移动速度上升33%，受到的伤害提升100%，治愈量下降70%。");
                    SendPetBasicInfo();
                    SendPetDetailInfo();
                }*/
                //ECOKEY 騎寵
                if (item.BaseData.itemType == ItemType.RIDE_PARTNER)
                {
                    ActorPet pet = new ActorPet(item.BaseData.petID, item);
                    pet.Owner = this.Character;
                    Character.Pet = pet;
                    pet.Ride = true;

                    this.SendPartner(item);
                    Character.Inventory.Equipments[EnumEquipSlot.PET].ActorPartnerID = item.ActorPartnerID;

                    SendPetBasicInfo();
                    SendPetDetailInfo();
                }
                if (item.BaseData.jointJob != PC_JOB.NONE)
                {
                    this.Character.JobJoint = item.BaseData.jointJob;
                }
                //凭依，跟我没关系
                if (item.PossessionedActor != null)
                {
                    PossessionArg arg = new PossessionArg();
                    arg.fromID = item.PossessionedActor.ActorID;
                    arg.toID = this.Character.ActorID;
                    arg.result = (int)item.PossessionedActor.PossessionPosition;
                    item.PossessionedActor.PossessionTarget = this.Character.ActorID;
                    MapServer.charDB.SaveChar(item.PossessionedActor, false, false);
                    MapServer.accountDB.WriteUser(item.PossessionedActor.Account);
                    string pos = "";
                    switch (item.PossessionedActor.PossessionPosition)
                    {
                        case PossessionPosition.RIGHT_HAND:
                            pos = LocalManager.Instance.Strings.POSSESSION_RIGHT;
                            break;
                        case PossessionPosition.LEFT_HAND:
                            pos = LocalManager.Instance.Strings.POSSESSION_LEFT;
                            break;
                        case PossessionPosition.NECK:
                            pos = LocalManager.Instance.Strings.POSSESSION_NECK;
                            break;
                        case PossessionPosition.CHEST:
                            pos = LocalManager.Instance.Strings.POSSESSION_ARMOR;
                            break;
                    }
                    this.SendSystemMessage(string.Format(LocalManager.Instance.Strings.POSSESSION_DONE, pos));
                    if (item.PossessionedActor.Online)
                    {
                        MapClient.FromActorPC(item.PossessionedActor).SendSystemMessage(string.Format(LocalManager.Instance.Strings.POSSESSION_DONE, pos));
                    }
                    this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.POSSESSION, arg, this.Character, true);
                }
                AddItemSkills(item);
                //重新计算状态值
                PC.StatusFactory.Instance.CalcStatus(this.Character);
                this.SendPlayerInfo();
                //broadcast
                this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHANGE_EQUIP, null, this.Character, true);
                List<Item> list = new List<Item>();
                foreach (Item i in this.chara.Inventory.Equipments.Values)
                {
                    if (i.Stack == 0)
                        continue;
                    if (CheckEquipRequirement(i) != 0)
                    {
                        list.Add(i);
                    }
                }
                foreach (Item i in list)
                {
                    Packets.Client.CSMG_ITEM_MOVE p2 = new SagaMap.Packets.Client.CSMG_ITEM_MOVE();
                    p2.data = new byte[9];
                    p2.Count = 1;
                    p2.InventoryID = i.Slot;
                    p2.Target = ContainerType.BODY;
                    OnItemMove(p2);
                }
            }
            catch (Exception)
            {
            }
        }

        public void OnItemMove(Packets.Client.CSMG_ITEM_MOVE p)
        {
            OnItemMove(p, false);
        }

        public void OnItemMove(Packets.Client.CSMG_ITEM_MOVE p, bool possessionRemove)
        {
            OnItemMove(p.InventoryID, p.Target, p.Count, possessionRemove);
        }
        public void OnItemMove(uint InventoryID, ContainerType Target, ushort Count, bool possessionRemove)
        {
            if (this.Character.Status.Additions.ContainsKey("Meditatioon"))
            {
                this.Character.Status.Additions["Meditatioon"].AdditionEnd();
                this.Character.Status.Additions.Remove("Meditatioon");
            }
            if (this.Character.Status.Additions.ContainsKey("Hiding"))
            {
                this.Character.Status.Additions["Hiding"].AdditionEnd();
                this.Character.Status.Additions.Remove("Hiding");
            }
            if (this.Character.Status.Additions.ContainsKey("Cloaking"))
            {
                this.Character.Status.Additions["Cloaking"].AdditionEnd();
                this.Character.Status.Additions.Remove("Cloaking");
            }
            if (this.Character.Status.Additions.ContainsKey("IAmTree"))
            {
                this.Character.Status.Additions["IAmTree"].AdditionEnd();
                this.Character.Status.Additions.Remove("IAmTree");
            }
            if (this.Character.Status.Additions.ContainsKey("Invisible"))
            {
                this.Character.Status.Additions["Invisible"].AdditionEnd();
                this.Character.Status.Additions.Remove("Invisible");
            }

            if (this.Character.PossessionTarget != 0)
            {
                var act = Map.GetActor(this.Character.PossessionTarget);
                if (act is ActorPC)
                {
                    act = act as ActorPC;
                    if (act.Status.Additions.ContainsKey("Meditatioon"))
                    {
                        act.Status.Additions["Meditatioon"].AdditionEnd();
                        act.Status.Additions.Remove("Meditatioon");
                    }
                    if (act.Status.Additions.ContainsKey("Hiding"))
                    {
                        act.Status.Additions["Hiding"].AdditionEnd();
                        act.Status.Additions.Remove("Hiding");
                    }
                    if (act.Status.Additions.ContainsKey("fish"))
                    {
                        act.Status.Additions["fish"].AdditionEnd();
                        act.Status.Additions.Remove("fish");
                    }
                    if (act.Status.Additions.ContainsKey("Cloaking"))
                    {
                        act.Status.Additions["Cloaking"].AdditionEnd();
                        act.Status.Additions.Remove("Cloaking");
                    }
                    if (act.Status.Additions.ContainsKey("IAmTree"))
                    {
                        act.Status.Additions["IAmTree"].AdditionEnd();
                        act.Status.Additions.Remove("IAmTree");
                    }
                    if (act.Status.Additions.ContainsKey("Invisible"))
                    {
                        act.Status.Additions["Invisible"].AdditionEnd();
                        act.Status.Additions.Remove("Invisible");
                    }
                }
            }

            Item item = this.Character.Inventory.GetItem(InventoryID);
            if (Target >= ContainerType.HEAD) //移动目标错误
            {
                Packets.Server.SSMG_ITEM_CONTAINER_CHANGE p1 = new SagaMap.Packets.Server.SSMG_ITEM_CONTAINER_CHANGE();
                p1.InventorySlot = item.Slot;
                p1.Result = -3;
                p1.Target = (ContainerType)(-1);
                this.netIO.SendPacket(p1);
                return;
            }
            bool ifUnequip = this.Character.Inventory.IsContainerEquip(this.Character.Inventory.GetContainerType(item.Slot));
            //ifUnequip &= p.Count == item.Stack;
            if (ifUnequip) //如果是卸下装备而不是在不同容器中移动
            {
                //检查
                if (item.PossessionedActor != null && !possessionRemove)
                {
                    Packets.Server.SSMG_ITEM_CONTAINER_CHANGE p1 = new SagaMap.Packets.Server.SSMG_ITEM_CONTAINER_CHANGE();
                    p1.InventorySlot = item.Slot;
                    p1.Result = -4;
                    p1.Target = (ContainerType)(-1);
                    this.netIO.SendPacket(p1);
                    return;
                }
                if (this.chara.Race == PC_RACE.DEM && this.chara.Form == DEM_FORM.MACHINA_FORM)
                {
                    Packets.Server.SSMG_ITEM_CONTAINER_CHANGE p1 = new SagaMap.Packets.Server.SSMG_ITEM_CONTAINER_CHANGE();
                    p1.InventorySlot = item.Slot;
                    p1.Result = -10;
                    p1.Target = (ContainerType)(-1);
                    this.netIO.SendPacket(p1);
                    return;
                }
                if (possessionRemove)
                    return;


                //卸下相关的额外格子
                List<EnumEquipSlot> slots = item.EquipSlot;
                if (slots.Count > 1)
                {
                    for (int i = 0; i < slots.Count; i++)
                    {
                        if (this.Character.Inventory.Equipments[slots[i]].Stack == 0)
                        {
                            this.Character.Inventory.Equipments.Remove(slots[i]);
                        }
                        else
                        {
                            ItemMoveSub(this.Character.Inventory.Equipments[slots[i]], ContainerType.BODY, this.Character.Inventory.Equipments[slots[i]].Stack);
                        }
                    }
                }
                else
                {
                    if (slots[0] == EnumEquipSlot.PET)
                    {
                        if (this.Character.Pet != null)
                            DeletePet();
                        if (this.Character.Partner != null)
                        {
                            DeletePartner();
                            //   PC.StatusFactory.Instance.CalcStatus(Character);
                        }
                    }
                    //箱包类装备移动时内容物进入body
                    if (item.BaseData.itemType == ItemType.BACKPACK)
                    {
                        while (this.Character.Inventory.GetContainer(ContainerType.BACK_BAG).Count > 0)
                        {
                            Item content = this.Character.Inventory.GetContainer(ContainerType.BACK_BAG).First();
                            ItemMoveSub(content, ContainerType.BODY, content.Stack);
                        }
                    }
                    if (item.BaseData.itemType == ItemType.HANDBAG)
                    {
                        while (this.Character.Inventory.GetContainer(ContainerType.RIGHT_BAG).Count > 0)
                        {
                            Item content = this.Character.Inventory.GetContainer(ContainerType.RIGHT_BAG).First();
                            ItemMoveSub(content, ContainerType.BODY, content.Stack);
                        }
                    }
                    if (item.BaseData.itemType == ItemType.LEFT_HANDBAG)
                    {
                        while (this.Character.Inventory.GetContainer(ContainerType.LEFT_BAG).Count > 0)
                        {
                            Item content = this.Character.Inventory.GetContainer(ContainerType.LEFT_BAG).First();
                            ItemMoveSub(content, ContainerType.BODY, content.Stack);
                        }
                    }
                    //卸下射击武器时自动卸下弹药
                    if (item.NeedAmmo)
                    {
                        if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                        {
                            ItemMoveSub(this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND], ContainerType.BODY, this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].Stack);
                        }
                    }
                    //解除宠物py 这里暂且不启用 ECOKEY
                    if (false && slots[0] == EnumEquipSlot.LEFT_HAND)
                    {
                        Character.PossessionPartnerSlotIDinLeftHand = 0;
                        Character.Status.def_add_petpy = 0;
                        Character.Status.mdef_add_petpy = 0;
                        Packets.Server.SSMG_PARTNER_PY_CANCEL p2 = new SagaMap.Packets.Server.SSMG_PARTNER_PY_CANCEL();
                        p2.Type = 1;
                        netIO.SendPacket(p2);
                    }
                }
                //删除装备附带技能
                if (item.BaseData.jointJob != PC_JOB.NONE)
                {
                    this.Character.JobJoint = PC_JOB.NONE;
                }
                if (item.BaseData.itemType == ItemType.BACK_DEMON)
                {
                    SkillHandler.RemoveAddition(this.chara, "MoveUp2");
                    SkillHandler.RemoveAddition(this.chara, "MoveUp3");
                    SkillHandler.RemoveAddition(this.chara, "MoveUp4");
                    SkillHandler.RemoveAddition(this.chara, "MoveUp5");
                }
            }
            //无体积装备时不能放入物品
            if (Target == ContainerType.BACK_BAG)
            {
                if (!this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.BACK))
                {
                    return;
                }
                else
                {
                    if (this.Character.Inventory.Equipments[EnumEquipSlot.BACK].BaseData.volumeUp == 0)
                    {
                        return;
                    }
                }
            }
            if (Target == ContainerType.RIGHT_BAG)
            {
                if (!this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                {
                    return;
                }
                else
                {
                    if (this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.volumeUp == 0)
                    {
                        return;
                    }
                }
            }
            if (Target == ContainerType.LEFT_BAG)
            {
                if (!this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                {
                    return;
                }
                else
                {
                    if (this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].BaseData.volumeUp == 0)
                    {
                        return;
                    }
                }
            }
            /*双持以后再说
            //双持时若卸下右手则同时卸下左手
            if (item.EquipSlot.Contains(EnumEquipSlot.RIGHT_HAND)
                && item.EquipSlot.Count == 1
                && this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND)
                && this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND]==item
                && this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND)
                && this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND].EquipSlot.Contains(EnumEquipSlot.RIGHT_HAND))
            {
                ItemMoveSub(this.Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND], ContainerType.BODY, 1);
            }*/
            //正式移动道具
            ItemMoveSub(item, Target, Count);
            //CleanItemSkills(item);
            //PC.StatusFactory.Instance.CalcStatus(this.Character);
            //SendPlayerInfo();
            //map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHANGE_EQUIP, null, this.Character, true);
        }

        /// <summary>
        /// 道具移动，只移动对应的真实格子的道具，不影响伪道具
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <param name="count"></param>
        public void ItemMoveSub(Item item, ContainerType container, ushort count)
        {
            Packets.Server.SSMG_ITEM_DELETE p2;
            Packets.Server.SSMG_ITEM_ADD p3;

            CleanItemSkills(item);
            if (item.BaseData.itemType == ItemType.RIDE_PET || item.BaseData.itemType == ItemType.RIDE_PARTNER)
            {
                PC.StatusFactory.Instance.CalcStatus(this.Character);
                //ECOKEY 刪除
                /*if (!Character.CInt.ContainsKey("PC_HUNMAN_HP"))
                {
                    Character.CInt["PC_HUNMAN_HP"] = 99;//防止变量更改前就骑着骑宠的人上线后寻找不到PC_HUNMAN_HP值导致0HP,理论上不可能发生,以防万一
                }
                Character.HP = (uint)Character.CInt["PC_HUNMAN_HP"];
                Character.CInt.Remove("PC_HUNMAN_HP");*/

            }
            bool ifUnequip = this.Character.Inventory.IsContainerEquip(this.Character.Inventory.GetContainerType(item.Slot));
            uint slot = item.Slot;
            //Logger.ShowError(this.Character.Inventory.GetContainerType(item.Slot).ToString());
            if (this.Character.Inventory.MoveItem(this.Character.Inventory.GetContainerType(item.Slot), (int)item.Slot, container, count))
            {
                //Logger.ShowError(this.Character.Inventory.GetContainerType(item.Slot).ToString());
                if (item.Stack == 0)
                {
                    if (slot == this.Character.Inventory.LastItem.Slot)
                    {
                        if (!ifUnequip)
                        {
                            p2 = new SagaMap.Packets.Server.SSMG_ITEM_DELETE();
                            p2.InventorySlot = item.Slot;
                            this.netIO.SendPacket(p2);
                            p3 = new SagaMap.Packets.Server.SSMG_ITEM_ADD();
                            p3.Container = container;
                            p3.InventorySlot = item.Slot;
                            item.Stack = count;
                            p3.Item = item;
                            this.netIO.SendPacket(p3);
                            Packets.Server.SSMG_ITEM_CONTAINER_CHANGE p1 = new SagaMap.Packets.Server.SSMG_ITEM_CONTAINER_CHANGE();
                            p1.InventorySlot = item.Slot;
                            p1.Target = container;
                            this.netIO.SendPacket(p1);
                        }
                        else
                        {
                            Packets.Server.SSMG_ITEM_CONTAINER_CHANGE p1 = new SagaMap.Packets.Server.SSMG_ITEM_CONTAINER_CHANGE();
                            p1.InventorySlot = item.Slot;
                            p1.Target = container;
                            this.netIO.SendPacket(p1);
                            Packets.Server.SSMG_ITEM_EQUIP p4 = new SagaMap.Packets.Server.SSMG_ITEM_EQUIP();
                            p4.InventorySlot = 0xffffffff;
                            p4.Target = ContainerType.NONE;
                            p4.Result = 1;
                            PC.StatusFactory.Instance.CalcRange(this.Character);
                            if (item.EquipSlot.Contains(EnumEquipSlot.RIGHT_HAND))
                            {
                                SendAttackType();
                                SkillHandler.Instance.CastPassiveSkills(this.Character);
                            }
                            p4.Range = this.Character.Range;
                            this.netIO.SendPacket(p4);
                            this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHANGE_EQUIP, null, this.Character, true);

                            if (item.EquipSlot[0] == EnumEquipSlot.PET)
                            {
                                if (this.Character.Pet != null)
                                {
                                    if (this.Character.Pet.Ride)
                                    {
                                        //this.Character.Speed = Configuration.Instance.Speed;
                                        //this.Character.HP = this.Character.Pet.HP;
                                        this.Character.Pet = null;
                                    }
                                }
                            }
                            // PC.StatusFactory.Instance.CalcStatus(this.Character);

                            this.SendPlayerInfo();
                        }
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
                            p3.Container = container;
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
                    if (this.Character.Inventory.LastItem.Stack == count)
                    {
                        p3 = new SagaMap.Packets.Server.SSMG_ITEM_ADD();
                        p3.Container = container;
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
            this.Character.Inventory.Items[ContainerType.BODY].RemoveAll(x => x.Stack == 0);
            this.Character.Inventory.CalcPayloadVolume();
            this.SendCapacity();
            //ECOKEY 修正裝備脫下屬性沒變問題，包括貓靈體積
            PC.StatusFactory.Instance.CalcStatus(this.Character);
            SendPlayerInfo();
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHANGE_EQUIP, null, this.Character, true);

        }

        public bool CheckPossessionForEquipMove(Item item)
        {
            if (item.PossessionedActor != null)
            {
                Packets.Server.SSMG_ITEM_EQUIP p4 = new SagaMap.Packets.Server.SSMG_ITEM_EQUIP();
                p4.InventorySlot = 0xffffffff;
                p4.Target = ContainerType.NONE;
                p4.Result = -10;
                p4.Range = this.Character.Range;
                this.netIO.SendPacket(p4);
                return false;
            }
            else
            {
                return true;
            }
        }

        public void AddItemSkills(Item item)
        {
            if (item.BaseData.itemType == ItemType.PARTNER)
            {
                ActorPartner partner = MapServer.charDB.GetActorPartner(item.ActorPartnerID, item);
                if (partner.rebirth)
                {
                    SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(2443, 1);
                    if (skill != null)
                        if (!Character.Skills.ContainsKey(2443))
                            Character.Skills.Add(2443, skill);
                }
            }

            if (item.BaseData.possibleSkill != 0)//装备附带主动技能
            {
                SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(item.BaseData.possibleSkill, 1);
                if (skill != null)
                {
                    if (!this.Character.Skills.ContainsKey(item.BaseData.possibleSkill))
                    {
                        this.Character.Skills.Add(item.BaseData.possibleSkill, skill);
                    }
                }
            }

            if (item.BaseData.passiveSkill != 0)//装备附带被动技能
            {
                ushort skillID = item.BaseData.passiveSkill;
                byte lv = 0;
                foreach (var eq in Character.Inventory.Equipments)
                {
                    if (eq.Value.BaseData.passiveSkill == skillID && eq.Value.EquipSlot[0] == eq.Key)
                        lv++;
                }
                if (lv > 5) lv = 5;
                SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(item.BaseData.passiveSkill, lv);
                if (skill != null)
                {
                    if (Character.Skills.ContainsKey(skillID))
                        Character.Skills.Remove(skillID);
                    if (lv > 0)
                    {
                        Character.Skills.Add(skillID, skill);
                        if (!skill.BaseData.active)
                        {
                            SkillArg arg = new SkillArg();
                            arg.skill = skill;
                            SkillHandler.Instance.SkillCast(this.Character, this.Character, arg);
                        }
                    }
                }
                SkillHandler.Instance.CastPassiveSkills(Character);
            }
        }

        public void SendTimerItem()
        {
            uint i = 0;
            foreach (var item in this.Character.TimerItems)
            {
                Packets.Server.SSMG_ITEM_SEND_TIMER p = new Packets.Server.SSMG_ITEM_SEND_TIMER();
                p.Slot = i;
                p.ItemID = this.Character.TimerItems[item.Key].ItemID;
                p.StartTime = this.Character.TimerItems[item.Key].StartTime;
                p.EndTime = this.Character.TimerItems[item.Key].EndTime;
                p.Unknow = 256;
                this.netIO.SendPacket(p);
                i++;
            }

        }
        public void CleanItemSkills(Item item)
        {
            if (item.BaseData.itemType == ItemType.PARTNER)
            {
                ActorPartner partner = MapServer.charDB.GetActorPartner(item.ActorPartnerID, item);
                if (partner != null)
                {
                    if (partner.rebirth)
                    {
                        SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(2443, 1);
                        if (skill != null)
                            if (Character.Skills.ContainsKey(2443))
                                Character.Skills.Remove(2443);
                    }
                }
            }
            if (item.BaseData.possibleSkill != 0)//装备附带主动技能
            {
                SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(item.BaseData.possibleSkill, 1);
                if (skill != null)
                {
                    if (this.Character.Skills.ContainsKey(item.BaseData.possibleSkill))
                    {
                        this.Character.Skills.Remove(item.BaseData.possibleSkill);
                    }
                }
            }

            if (item.BaseData.passiveSkill != 0)//装备附带被动技能
            {
                ushort skillID = item.BaseData.passiveSkill;
                //byte lv = 0;
                //foreach (var eq in Character.Inventory.Equipments)
                //    if (eq.Value.BaseData.passiveSkill == skillID && eq.Value.EquipSlot[0] == eq.Key)
                //        lv++;
                //if (lv > 5) lv = 5;
                SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(item.BaseData.passiveSkill, 1);
                if (skill != null)
                {
                    if (Character.Skills.ContainsKey(skillID))
                        Character.Skills.Remove(skillID);
                    //if (lv > 0)
                    //{
                    //    Character.Skills.Add(skillID, skill);
                    //    if (!skill.BaseData.active)
                    //    {
                    //        SkillArg arg = new SkillArg();
                    //        arg.skill = skill;
                    //        SkillHandler.Instance.SkillCast(this.Character, this.Character, arg);
                    //    }
                    //}
                }
                SkillHandler.Instance.CastPassiveSkills(Character);
            }
        }

        public void SendItemAdd(Item item, ContainerType container, InventoryAddResult result, int count, bool sendMessage)
        {
            switch (result)
            {
                case InventoryAddResult.NEW_INDEX:
                    Packets.Server.SSMG_ITEM_ADD p = new SagaMap.Packets.Server.SSMG_ITEM_ADD();
                    p.Container = container;
                    p.Item = item;
                    p.InventorySlot = item.Slot;
                    this.netIO.SendPacket(p);
                    break;
                case InventoryAddResult.STACKED:
                    {
                        Packets.Server.SSMG_ITEM_COUNT_UPDATE p1 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                        p1.InventorySlot = item.Slot;
                        p1.Stack = item.Stack;
                        this.netIO.SendPacket(p1);
                    }
                    break;
                case InventoryAddResult.MIXED:
                    {
                        Packets.Server.SSMG_ITEM_COUNT_UPDATE p1 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                        p1.InventorySlot = item.Slot;
                        p1.Stack = item.Stack;
                        this.netIO.SendPacket(p1);
                        Packets.Server.SSMG_ITEM_ADD p2 = new SagaMap.Packets.Server.SSMG_ITEM_ADD();
                        p2.Container = container;
                        p2.Item = this.Character.Inventory.LastItem;
                        p2.InventorySlot = this.Character.Inventory.LastItem.Slot;
                        this.netIO.SendPacket(p2);
                    }
                    break;
                case InventoryAddResult.GOWARE:
                    SendSystemMessage("背包已满，物品『" + item.BaseData.name + "』存入了仓库");
                    break;
            }

            this.Character.Inventory.CalcPayloadVolume();
            this.SendCapacity();

            if (sendMessage)
            {
                if (item.Identified)
                    this.SendSystemMessage(string.Format(LocalManager.Instance.Strings.ITEM_ADDED, item.BaseData.name, count));
                else
                    this.SendSystemMessage(string.Format(LocalManager.Instance.Strings.ITEM_ADDED, Scripting.Event.GetItemNameByType(item.BaseData.itemType), count));
            }
        }

        public void SendItems()
        {
            string[] names = Enum.GetNames(typeof(ContainerType));
            foreach (string i in names)
            {
                ContainerType container = (ContainerType)Enum.Parse(typeof(ContainerType), i);
                List<Item> items = this.Character.Inventory.GetContainer(container);
                List<Item> trashItem = new List<Item>();
                if (container == ContainerType.BODY)//扫描并删除身上的垃圾数据
                {
                    foreach (Item j in items)
                    {
                        if (j.Stack == 0)
                            trashItem.Add(j);
                    }
                    if (trashItem.Count > 0)
                    {
                        for (int y = 0; y < trashItem.Count; y++)
                        {
                            Character.Inventory.Items[ContainerType.BODY].Remove(trashItem[y]);
                        }
                    }
                }
                foreach (Item j in items)
                {
                    if (j.Stack == 0)
                        continue;
                    //if (j.Refine == 0)
                    //    j.Clear();
                    Packets.Server.SSMG_ITEM_INFO p = new SagaMap.Packets.Server.SSMG_ITEM_INFO();
                    p.Item = j;
                    p.InventorySlot = j.Slot;
                    p.Container = container;
                    this.netIO.SendPacket(p);
                }
            }
        }

        public void SendItemInfo(uint slot)
        {
            Item item = this.Character.Inventory.GetItem(slot);
            if (item == null)
                return;
            Packets.Server.SSMG_ITEM_INFO p = new SagaMap.Packets.Server.SSMG_ITEM_INFO();
            p.Item = item;
            p.InventorySlot = item.Slot;
            p.Container = this.Character.Inventory.GetContainerType(slot);
            this.netIO.SendPacket(p);
        }

        public void SendItemInfo(Item item)
        {
            if (item == null)
                return;

            Packets.Server.SSMG_ITEM_INFO p = new SagaMap.Packets.Server.SSMG_ITEM_INFO();
            p.Item = item;
            p.InventorySlot = item.Slot;
            p.Container = this.Character.Inventory.GetContainerType(item.Slot);
            this.netIO.SendPacket(p);

            Packet packet = new Packet();
            packet.data = new byte[3];
            packet.ID = 0x0203;
            packet.offset = 2;
            packet.PutByte(02);
            this.netIO.SendPacket(packet);
        }

        public void SendItemIdentify(uint slot)
        {
            Item item = this.Character.Inventory.GetItem(slot);
            if (item == null)
                return;
            Packets.Server.SSMG_ITEM_IDENTIFY p = new SagaMap.Packets.Server.SSMG_ITEM_IDENTIFY();
            p.InventorySlot = item.Slot;
            p.Identify = item.Identified;
            p.Lock = item.Locked;
            this.netIO.SendPacket(p);
        }

        public void SendEquip()
        {
            Packets.Server.SSMG_ITEM_ACTOR_EQUIP_UPDATE p = new SagaMap.Packets.Server.SSMG_ITEM_ACTOR_EQUIP_UPDATE();
            p.Player = this.Character;
            this.netIO.SendPacket(p);
        }

        public void AddItem(Item item, bool sendMessage)
        {
            AddItem(item, sendMessage, true);
        }

        public void CleanItem()
        {
            Character.Inventory.Items[ContainerType.BODY].Clear();
            this.Character.Inventory.CalcPayloadVolume();
            this.SendCapacity();
        }

        public void AddItem(Item item, bool sendMessage, bool fullgoware)
        {
            ushort stack = item.Stack;
            //SagaLib.Logger.ShowWarning("1"+item.Stack.ToString()+item.BaseData.name);
            //if (this.Character.Inventory.Items.Count < 1000 || this.Character.Account.GMLevel > 10)
            //{
            //临时解决方案↓↓↓↓↓
            //if (this.Character.Inventory.Items[ContainerType.BODY].Count + this.Character.Inventory.Equipments.Count > 100 && fullgoware)
            //{
            //    string[] names = Enum.GetNames(typeof(ContainerType));
            //    foreach (string i in names)
            //    {
            //        ContainerType container = (ContainerType)Enum.Parse(typeof(ContainerType), i);
            //        List<Item> items = this.Character.Inventory.GetContainer(container);
            //        List<Item> trashItem = new List<Item>();
            //        if (container == ContainerType.BODY)//扫描并删除身上的垃圾数据
            //        {
            //            foreach (Item j in items)
            //            {
            //                if (j.Stack == 0)
            //                    trashItem.Add(j);
            //            }
            //            if (trashItem.Count > 0)
            //            {
            //                for (int y = 0; y < trashItem.Count; y++)
            //                {
            //                    Character.Inventory.Items[ContainerType.BODY].Remove(trashItem[y]);
            //                }
            //            }
            //        }
            //    }
            //}
            //临时解决方案↑↑↑↑↑


            /*
                if (this.Character.Inventory.Items[ContainerType.BODY].Count + this.Character.Inventory.Equipments.Count > 100 && fullgoware)
                {
                    if (Character.CInt["背包满后仓库"] == 1)
                    {
                        Character.Inventory.AddWareItem(WarehousePlace.Acropolis, item);
                        SendSystemMessage("背包已满，物品『" + item.BaseData.name + "』存入了第一页仓库。");
                    }
                    else if (Character.CInt["背包满后仓库"] == 2)
                    {
                        Character.Inventory.AddWareItem(WarehousePlace.FarEast, item);
                        SendSystemMessage("背包已满，物品『" + item.BaseData.name + "』存入了第二页仓库。");
                    }
                    else if (Character.CInt["背包满后仓库"] == 3)
                    {
                        Character.Inventory.AddWareItem(WarehousePlace.IronSouth, item);
                        SendSystemMessage("背包已满，物品『" + item.BaseData.name + "』存入了第三页仓库。");
                    }
                    else if (Character.CInt["背包满后仓库"] == 4)
                    {
                        Character.Inventory.AddWareItem(WarehousePlace.Northan, item);
                        SendSystemMessage("背包已满，物品『" + item.BaseData.name + "』存入了第四页仓库。");
                    }
                    else
                    {
                        Character.Inventory.AddWareItem(WarehousePlace.Northan, item);
                        SendSystemMessage("背包已满，物品『" + item.BaseData.name + "』存入了第四页仓库。");
                    }
                }
                else
                {
                */
            InventoryAddResult result = this.Character.Inventory.AddItem(ContainerType.BODY, item);
            this.SendItemAdd(item, ContainerType.BODY, result, stack, sendMessage);

            this.Character.Inventory.CalcPayloadVolume();
            this.SendCapacity();
            //this.SendItems();
            //}

            /*}
            else
            {
                this.SendSystemMessage("道具栏已满，无法获得道具。");
                /*this.SendSystemMessage("（本次获得的道具可以向 吉田佳美 领取，临时道具只能保存3个，请及时处理道具栏并领取。）");
                if (this.Character.CInt["临时道具1"] == 0)
                    this.Character.CInt["临时道具1"] = (int)item.ItemID;
                else if (this.Character.CInt["临时道具2"] == 0)
                    this.Character.CInt["临时道具2"] = (int)item.ItemID;
                else if (this.Character.CInt["临时道具3"] == 0)
                    this.Character.CInt["临时道具3"] = (int)item.ItemID;*/

        }

        int CountItem(uint itemID)
        {
            SagaDB.Item.Item item = this.chara.Inventory.GetItem(itemID, Inventory.SearchType.ITEM_ID);
            if (item != null)
            {
                return item.Stack;
            }
            else
            {
                return 0;
            }
        }

        public Item DeleteItemID(uint itemID, ushort count, bool message)
        {
            Item item = this.Character.Inventory.GetItem(itemID, Inventory.SearchType.ITEM_ID);
            if (item == null) return null;
            uint slot = item.Slot;
            InventoryDeleteResult result = this.Character.Inventory.DeleteItem(item.Slot, count);
            if (item.IsEquipt)
            {
                SendEquip();
                // PC.StatusFactory.Instance.CalcStatus(this.Character);
                SendPlayerInfo();
            }
            switch (result)
            {
                case InventoryDeleteResult.STACK_UPDATED:
                    Packets.Server.SSMG_ITEM_COUNT_UPDATE p1 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                    item = this.Character.Inventory.GetItem(slot);
                    p1.InventorySlot = slot;
                    p1.Stack = item.Stack;
                    this.netIO.SendPacket(p1);
                    break;
                case InventoryDeleteResult.ALL_DELETED:
                    Packets.Server.SSMG_ITEM_DELETE p2 = new SagaMap.Packets.Server.SSMG_ITEM_DELETE();
                    p2.InventorySlot = slot;
                    this.netIO.SendPacket(p2);
                    if (item.IsEquipt)
                    {
                        SendAttackType();
                        Packets.Server.SSMG_ITEM_EQUIP p4 = new SagaMap.Packets.Server.SSMG_ITEM_EQUIP();
                        p4.InventorySlot = 0xffffffff;
                        p4.Target = ContainerType.NONE;
                        p4.Result = 1;
                        p4.Range = this.Character.Range;
                        this.netIO.SendPacket(p4);
                    }
                    break;
            }
            this.Character.Inventory.CalcPayloadVolume();
            this.SendCapacity();
            if (message)
                this.SendSystemMessage(string.Format(LocalManager.Instance.Strings.ITEM_DELETED, item.BaseData.name, count));
            return item;
        }

        public void DeleteItem(uint slot, ushort count, bool message)
        {
            Item item = this.Character.Inventory.GetItem(slot);
            ContainerType container = this.Character.Inventory.GetContainerType(item.Slot);
            bool equiped = false;
            if (container >= ContainerType.HEAD && container <= ContainerType.PET)
                equiped = true;
            InventoryDeleteResult result = this.Character.Inventory.DeleteItem(slot, count);
            if (equiped)
            {
                SendEquip();
                // PC.StatusFactory.Instance.CalcStatus(this.Character);
                SendPlayerInfo();
            }
            switch (result)
            {
                case InventoryDeleteResult.STACK_UPDATED:
                    Packets.Server.SSMG_ITEM_COUNT_UPDATE p1 = new SagaMap.Packets.Server.SSMG_ITEM_COUNT_UPDATE();
                    item = this.Character.Inventory.GetItem(slot);
                    p1.InventorySlot = slot;
                    p1.Stack = item.Stack;
                    this.netIO.SendPacket(p1);
                    break;
                case InventoryDeleteResult.ALL_DELETED:
                    Packets.Server.SSMG_ITEM_DELETE p2 = new SagaMap.Packets.Server.SSMG_ITEM_DELETE();
                    p2.InventorySlot = slot;
                    this.netIO.SendPacket(p2);
                    this.Character.Inventory.GetContainerType(slot);
                    if (equiped)
                    {
                        SendAttackType();
                        Packets.Server.SSMG_ITEM_EQUIP p4 = new SagaMap.Packets.Server.SSMG_ITEM_EQUIP();
                        p4.InventorySlot = 0xffffffff;
                        p4.Target = ContainerType.NONE;
                        p4.Result = 1;
                        p4.Range = this.Character.Range;
                        this.netIO.SendPacket(p4);
                        if (item.BaseData.itemType == ItemType.ARROW || item.BaseData.itemType == ItemType.BULLET)
                        {
                            Item dummy = this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].Clone();
                            dummy.Stack = 0;
                            this.Character.Inventory.AddItem(ContainerType.LEFT_HAND, dummy);
                        }
                    }
                    break;
            }
            this.Character.Inventory.CalcPayloadVolume();
            this.SendCapacity();
            if (message)
                this.SendSystemMessage(string.Format(LocalManager.Instance.Strings.ITEM_DELETED, item.BaseData.name, count));
        }

        public void SendPet(Item item)
        {
            if (item.BaseData.itemType != ItemType.BACK_DEMON && item.BaseData.itemType != ItemType.RIDE_PET && item.BaseData.itemType != ItemType.RIDE_PARTNER)
            {
                ActorPet pet = new ActorPet(item.BaseData.petID, item);
                this.Character.Pet = pet;
                //砍掉PET
                /*
                pet.MapID = this.Character.MapID;
                pet.X = this.Character.X;
                pet.Y = this.Character.Y;
                pet.Owner = this.Character;
                ActorEventHandlers.PetEventHandler eh = new ActorEventHandlers.PetEventHandler(pet);
                pet.e = eh;
                if (Mob.MobAIFactory.Instance.Items.ContainsKey(item.BaseData.petID))
                    eh.AI.Mode = Mob.MobAIFactory.Instance.Items[item.BaseData.petID];
                else
                    eh.AI.Mode = new SagaMap.Mob.AIMode(0);
                eh.AI.Start();
                //Mob.AIThread.Instance.RegisterAI(eh.AI);

                this.map.RegisterActor(pet);
                pet.invisble = false;
                this.map.OnActorVisibilityChange(pet);
                this.map.SendVisibleActorsToActor(pet);//*/
            }
        }

        //ECOKEY 拿下騎寵
        public void DeletePet()
        {
            if (this.Character.Pet.Ride)
            {
                //ECOKEY 機器人（0310補上這裡！！）
                if (this.Character.Partner != null)
                {
                    if (this.Character.Partner.Tasks.ContainsKey("Feed"))
                    {
                        this.Character.Partner.Tasks["Feed"].Deactivate();
                        this.Character.Partner.Tasks.Remove("Feed");
                    }
                    if (this.Character.Partner.Tasks.ContainsKey("ReliabilityGrow"))
                    {
                        this.Character.Partner.Tasks["ReliabilityGrow"].Deactivate();
                        this.Character.Partner.Tasks.Remove("ReliabilityGrow");
                    }
                    if (this.Character.Partner.Tasks.ContainsKey("TalkAtFreeTime"))
                    {
                        this.Character.Partner.Tasks["TalkAtFreeTime"].Deactivate();
                        this.Character.Partner.Tasks.Remove("TalkAtFreeTime");
                    }
                    MapServer.charDB.SavePartner(this.Character.Partner);
                    this.Character.Partner = null;
                    this.Character.Pet = null;
                    PC.StatusFactory.Instance.CalcStatus(Character);
                    return;
                }
                else
                {
                    this.Character.Pet = null;
                    PC.StatusFactory.Instance.CalcStatus(Character);
                    MapClient.FromActorPC(this.Character).SendPlayerInfo();
                    return;
                }
            }

            if (this.Character.Partner != null)
            {

                Manager.MapManager.Instance.GetMap(this.Character.Partner.MapID).DeleteActor(this.Character.Partner);
                this.Character.Partner = null;
                return;
            }


            if (this.Character.Pet != null)
            {
                return;
            }
            else
            {
                //AI被砍掉！
                //參考SendPet()

                ActorEventHandlers.PetEventHandler eh = (ActorEventHandlers.PetEventHandler)this.Character.Pet.e;
                eh.AI.Pause();
                eh.AI.Activated = false;
                Manager.MapManager.Instance.GetMap(this.Character.Pet.MapID).DeleteActor(this.Character.Pet);
                this.Character.Pet = null;
            }

            //Ride 沒有被定義，請考慮Default 宣告false！
            //if (this.Character.Pet.Ride)
            //return;
        }

        //ECOKEY 機器人
        public void SendPetRobot(Item item)
        {
            if (!this.Character.Skills2.ContainsKey(132) && !this.Character.DualJobSkill.Exists(x => x.ID == 132))
            {
                if (this.Character.Inventory.Equipments[EnumEquipSlot.PET].ItemID == 10052310)
                {
                }
                else
                {
                    SagaMap.Network.Client.MapClient.FromActorPC(this.Character).SendSystemMessage("未學習“機械駕駛專精”，無法使用");
                ItemMoveSub(this.Character.Inventory.Equipments[EnumEquipSlot.PET], ContainerType.BODY, this.Character.Inventory.Equipments[EnumEquipSlot.PET].Stack);
                return;
                }
            }
            else if (this.Character.Marionette != null)
            {
                SagaMap.Network.Client.MapClient.FromActorPC(this.Character).SendSystemMessage("狀態異常，無法使用");
                ItemMoveSub(this.Character.Inventory.Equipments[EnumEquipSlot.PET], ContainerType.BODY, this.Character.Inventory.Equipments[EnumEquipSlot.PET].Stack);
                return;
            }
            else if (this.Character.Inventory.Equipments[EnumEquipSlot.PET].ItemID == 10052309)
            {
            if (this.Character.Inventory.Equipments[EnumEquipSlot.CHEST_ACCE].ItemID == 50057300)
            {
                SagaMap.Network.Client.MapClient.FromActorPC(this.Character).SendSystemMessage("你沒裝備馴獸項，無法使用");
                ItemMoveSub(this.Character.Inventory.Equipments[EnumEquipSlot.CHEST_ACCE], ContainerType.BODY, this.Character.Inventory.Equipments[EnumEquipSlot.CHEST_ACCE].Stack);
                return;
            }
            }

            ActorPet pet = new ActorPet(item.BaseData.petID, item);
            pet.Owner = this.Character;
            Character.Pet = pet;
            #region MA"匠师"模块1
            List<string> adds = new List<string>();
            foreach (System.Collections.Generic.KeyValuePair<string, SagaDB.Actor.Addition> ad in this.Character.Status.Additions)
            {
                if (!(ad.Value is DefaultPassiveSkill))
                    adds.Add(ad.Value.Name);
            }
            foreach (string adn in adds)
            {
                SkillHandler.RemoveAddition(this.Character, adn);
            }
            #endregion
            pet.Ride = true;


            #region 双足步行机器人两装被动模块
            if (Character.Skills2_2.ContainsKey(964) || Character.DualJobSkill.Exists(x => x.ID == 964))//生命强化
            {
                var duallv = 0;
                if (Character.DualJobSkill.Exists(x => x.ID == 964))
                    duallv = Character.DualJobSkill.FirstOrDefault(x => x.ID == 964).Level;

                var mainlv = 0;
                if (Character.Skills2_2.ContainsKey(964))
                    mainlv = Character.Skills2_2[964].Level;

                float hpaddrate = 0.07f + 0.03f * Math.Max(duallv, mainlv);
                pet.MaxHP = (uint)(pet.MaxHP * (1 + hpaddrate));
            }

            if (Character.Skills2_2.ContainsKey(965) || Character.DualJobSkill.Exists(x => x.ID == 965))//攻击强化
            {
                var duallv = 0;
                if (Character.DualJobSkill.Exists(x => x.ID == 965))
                    duallv = Character.DualJobSkill.FirstOrDefault(x => x.ID == 965).Level;

                var mainlv = 0;
                if (Character.Skills2_2.ContainsKey(965))
                    mainlv = Character.Skills2_2[965].Level;

                float atkaddrate = 0.08f + 0.02f * Math.Max(duallv, mainlv);
                pet.Status.min_atk1 = (ushort)(pet.Status.min_atk1 * (1 + atkaddrate));
                pet.Status.min_atk2 = (ushort)(pet.Status.min_atk2 * (1 + atkaddrate));
                pet.Status.min_atk3 = (ushort)(pet.Status.min_atk3 * (1 + atkaddrate));
                pet.Status.max_atk1 = (ushort)(pet.Status.max_atk1 * (1 + atkaddrate));
                pet.Status.max_atk2 = (ushort)(pet.Status.max_atk2 * (1 + atkaddrate));
                pet.Status.max_atk3 = (ushort)(pet.Status.max_atk3 * (1 + atkaddrate));
                pet.Status.min_matk = (ushort)(pet.Status.min_matk * (1 + atkaddrate));
                pet.Status.max_matk = (ushort)(pet.Status.max_matk * (1 + atkaddrate));
            }

            if (Character.Skills2_2.ContainsKey(966) || Character.DualJobSkill.Exists(x => x.ID == 966))//防御强化
            {
                var duallv = 0;
                if (Character.DualJobSkill.Exists(x => x.ID == 966))
                    duallv = Character.DualJobSkill.FirstOrDefault(x => x.ID == 966).Level;

                var mainlv = 0;
                if (Character.Skills2_2.ContainsKey(966))
                    mainlv = Character.Skills2_2[966].Level;

                float defaddrate = 0.07f + 0.03f * Math.Max(duallv, mainlv);
                pet.Status.def_add = (short)(pet.Status.def_add * (1 + defaddrate));
            }
            if (Character.Skills2_2.ContainsKey(968) || Character.DualJobSkill.Exists(x => x.ID == 968))//命中强化
            {
                var duallv = 0;
                if (Character.DualJobSkill.Exists(x => x.ID == 968))
                    duallv = Character.DualJobSkill.FirstOrDefault(x => x.ID == 968).Level;

                var mainlv = 0;
                if (Character.Skills2_2.ContainsKey(968))
                    mainlv = Character.Skills2_2[968].Level;

                float hitaddrate = 0.08f + 0.02f * Math.Max(duallv, mainlv);
                pet.Status.hit_melee = (ushort)(pet.Status.hit_melee * (1 + hitaddrate));
                pet.Status.hit_ranged = (ushort)(pet.Status.hit_ranged * (1 + hitaddrate));
            }
            if (Character.Skills2_2.ContainsKey(969) || Character.DualJobSkill.Exists(x => x.ID == 969))//回避强化
            {
                var duallv = 0;
                if (Character.DualJobSkill.Exists(x => x.ID == 969))
                    duallv = Character.DualJobSkill.FirstOrDefault(x => x.ID == 969).Level;

                var mainlv = 0;
                if (Character.Skills2_2.ContainsKey(969))
                    mainlv = Character.Skills2_2[969].Level;

                float avoidaddrate = 0.07f + 0.03f * Math.Max(duallv, mainlv);
                pet.Status.avoid_melee = (ushort)(pet.Status.avoid_melee * (1 + avoidaddrate));
                pet.Status.avoid_ranged = (ushort)(pet.Status.avoid_ranged * (1 + avoidaddrate));
            }
            if (Character.Skills2_2.ContainsKey(970) || Character.DualJobSkill.Exists(x => x.ID == 970))//回复强化
            {
                var duallv = 0;
                if (Character.DualJobSkill.Exists(x => x.ID == 970))
                    duallv = Character.DualJobSkill.FirstOrDefault(x => x.ID == 970).Level;

                var mainlv = 0;
                if (Character.Skills2_2.ContainsKey(970))
                    mainlv = Character.Skills2_2[970].Level;

                float hpreaddrate = new float[] { 0, 0.05f, 0.06f, 0.08f, 0.1f, 0.12f }[Math.Max(duallv, mainlv)];
                pet.Status.hp_recover = (short)(pet.Status.hp_recover * (1 + hpreaddrate));
            }
            #endregion

            #region MA"匠师"模块2
            float OnDir = 1.0f;
            if (Character is ActorPC)
            {
                ActorPC pc = Character as ActorPC;

                if (pc.Skills3.ContainsKey(987) || pc.DualJobSkill.Exists(x => x.ID == 987))
                {
                    var duallv = 0;
                    if (pc.DualJobSkill.Exists(x => x.ID == 987))
                        duallv = pc.DualJobSkill.FirstOrDefault(x => x.ID == 987).Level;

                    var mainlv = 0;
                    if (pc.Skills3.ContainsKey(987))
                        mainlv = pc.Skills3[987].Level;

                    //OnDir = OnDir + (float)(((Math.Max(duallv, mainlv)) - 1) * 0.05f);
                    int maxlv = (Math.Max(duallv, mainlv));
                    if (SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE_ROBOT"))
                    {
                        OnDir = OnDir + (float)(maxlv - 1) * 0.05f;
                        pet.MaxHP += (ushort)(Character.MaxHP * OnDir);
                        pet.MaxMP += (ushort)(Character.MaxMP * OnDir);
                        pet.MaxSP += (ushort)(Character.MaxSP * OnDir);
                        pet.Status.min_atk1 += (ushort)(Character.Status.min_atk1 * OnDir);
                        pet.Status.min_atk2 += (ushort)(Character.Status.min_atk2 * OnDir);
                        pet.Status.min_atk3 += (ushort)(Character.Status.min_atk3 * OnDir);
                        pet.Status.max_atk1 += (ushort)(Character.Status.max_atk1 * OnDir);
                        pet.Status.max_atk2 += (ushort)(Character.Status.max_atk2 * OnDir);
                        pet.Status.max_atk3 += (ushort)(Character.Status.max_atk3 * OnDir);
                        pet.Status.min_matk += (ushort)(Character.Status.min_matk * OnDir);
                        pet.Status.max_matk += (ushort)(Character.Status.max_matk * OnDir);
                        pet.Status.hit_melee += (ushort)(Character.Status.hit_melee * OnDir);
                        pet.Status.hit_ranged += (ushort)(Character.Status.hit_ranged * OnDir);
                        if ((pet.Status.def + Character.Status.def * OnDir) > 90)
                            pet.Status.def = 90;
                        else
                            pet.Status.def += (ushort)(Character.Status.def * OnDir);

                        pet.Status.def_add += (short)(Character.Status.def_add * OnDir);
                        if ((pet.Status.mdef + Character.Status.mdef * OnDir) > 90)
                            pet.Status.mdef = 90;
                        else
                            pet.Status.mdef += (ushort)(Character.Status.mdef * OnDir);

                        pet.Status.mdef_add += (short)(Character.Status.mdef_add * OnDir);
                        pet.Status.avoid_melee += (ushort)(Character.Status.avoid_melee * OnDir);
                        pet.Status.avoid_ranged += (ushort)(Character.Status.avoid_ranged * OnDir);

                        if ((pet.Status.aspd + Character.Status.aspd * OnDir) > 800)
                            pet.Status.aspd = 800;
                        else
                            pet.Status.aspd += (short)(Character.Status.aspd * OnDir);
                        if ((pet.Status.cspd + Character.Status.cspd * OnDir) > 800)
                            pet.Status.cspd = 800;
                        else
                            pet.Status.cspd += (short)(Character.Status.cspd * OnDir);
                    }
                    else if (SkillHandler.Instance.CheckMobType(pet, "MACHINE_RIDE"))
                    {
                        OnDir = (float)(0.25 + (float)(maxlv * 0.01f));
                        pet.MaxHP += (ushort)(Character.MaxHP * OnDir);
                        pet.MaxMP += (ushort)(Character.MaxMP * OnDir);
                        pet.MaxSP += (ushort)(Character.MaxSP * OnDir);
                        pet.Status.min_atk1 += (ushort)(Character.Status.min_atk1 * OnDir);
                        pet.Status.min_atk2 += (ushort)(Character.Status.min_atk2 * OnDir);
                        pet.Status.min_atk3 += (ushort)(Character.Status.min_atk3 * OnDir);
                        pet.Status.max_atk1 += (ushort)(Character.Status.max_atk1 * OnDir);
                        pet.Status.max_atk2 += (ushort)(Character.Status.max_atk2 * OnDir);
                        pet.Status.max_atk3 += (ushort)(Character.Status.max_atk3 * OnDir);
                        pet.Status.min_matk += (ushort)(Character.Status.min_matk * OnDir);
                        pet.Status.max_matk += (ushort)(Character.Status.max_matk * OnDir);
                        pet.Status.hit_melee += (ushort)(Character.Status.hit_melee * OnDir);
                        pet.Status.hit_ranged += (ushort)(Character.Status.hit_ranged * OnDir);

                        if ((pet.Status.def + Character.Status.def * OnDir) > 90)
                            pet.Status.def = 90;
                        else
                            pet.Status.def += (ushort)(Character.Status.def * OnDir);

                        pet.Status.def_add += (short)(Character.Status.def_add * OnDir);

                        if ((pet.Status.mdef + Character.Status.mdef * OnDir) > 90)
                            pet.Status.mdef = 90;
                        else
                            pet.Status.mdef += (ushort)(Character.Status.mdef * OnDir);

                        pet.Status.mdef_add += (short)(Character.Status.mdef_add * OnDir);
                        pet.Status.avoid_melee += (ushort)(Character.Status.avoid_melee * OnDir);
                        pet.Status.avoid_ranged += (ushort)(Character.Status.avoid_ranged * OnDir);

                        if ((pet.Status.aspd + Character.Status.aspd * OnDir) > 800)
                            pet.Status.aspd = 800;
                        else
                            pet.Status.aspd += (short)(Character.Status.aspd * OnDir);

                        if ((pet.Status.cspd + Character.Status.cspd * OnDir) > 800)
                            pet.Status.cspd = 800;
                        else
                            pet.Status.cspd += (short)(Character.Status.cspd * OnDir);
                    }

                }

            }
            pet.Status.min_atk_bs = Math.Max(Math.Max(pet.Status.min_atk1, pet.Status.min_atk2), pet.Status.min_atk3);
            pet.Status.max_atk_bs = Math.Max(Math.Max(pet.Status.max_atk1, pet.Status.max_atk2), pet.Status.max_atk3);
            pet.Status.min_matk_bs = pet.Status.min_matk;
            pet.Status.max_matk_bs = pet.Status.max_matk;
            #endregion
            Character.Speed = Configuration.Instance.Speed;
            SendPetBasicInfo();
            SendPetDetailInfo();
        }

        public void OnItemChangeSlot(Packets.Client.CSMG_ITEM_CHANGE_SLOT p)
        {

        }

    }
}