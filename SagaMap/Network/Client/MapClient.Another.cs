using SagaDB.AnotherBook;
using SagaDB.Item;
using SagaDB.Actor;
using SagaLib;
using System;
using System.Collections.Generic;
using SagaMap.Manager;
using System.Linq;
using SagaMap.PC;

namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        public void SendAnotherButton()
        {
            Packets.Server.SSMG_ANOTHERBOOK_WINDOW_UNLOCK_STATE p = new Packets.Server.SSMG_ANOTHERBOOK_WINDOW_UNLOCK_STATE();
            p.Type = 1;
            p.UsingPageID = this.Character.UsingPaperID;
            this.netIO.SendPacket(p);
        }

        public void OnAnotherBookPaperEquip(Packets.Client.CSMG_ANOTHERBOOK_PAPER_EQUIP p)
        {
            if (Character.AnotherPapers.ContainsKey(p.paperID))
            {
                Character.UsingPaperID = p.paperID;
                StatusFactory.Instance.CalcStatus(Character);
                Packets.Server.SSMG_ANOTHERBOOK_PAPER_EQUIP_RESULT p1 = new Packets.Server.SSMG_ANOTHERBOOK_PAPER_EQUIP_RESULT();
                p1.Result = 0;
                p1.PaperID = Character.UsingPaperID;
                SendPlayerInfo();
                netIO.SendPacket(p1);
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.PAPER_CHANGE, null, Character, true);
            }
        }

        public void OnAnotherBookPaperUnequip(Packets.Client.CSMG_ANOTHERBOOK_PAPER_UNEQUIP p)
        {
            Character.UsingPaperID = 0;
            StatusFactory.Instance.CalcStatus(Character);
            Packets.Server.SSMG_ANOTHERBOOK_UNEQUIP_RESULT p1 = new Packets.Server.SSMG_ANOTHERBOOK_UNEQUIP_RESULT();
            p1.PaperID = Character.UsingPaperID;
            netIO.SendPacket(p1);
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.PAPER_CHANGE, null, Character, true);
            SendPlayerInfo();
        }

        public void CreateAnotherPaper(uint paperID)
        {
            AnotherDetail detail = new AnotherDetail();
            detail.PaperValue = new BitMask_Long();
            detail.Level = 0;
            foreach (var item in AnotherFactory.Instance.Items[paperID].Keys)
            {
                if (!detail.Skills.ContainsKey(item))
                    detail.Skills.Add(item, 0);
            }
            if (!this.Character.AnotherPapers.ContainsKey(paperID))
                this.Character.AnotherPapers.Add(paperID, detail);
        }

        public void OnAnotherBookPaperCompound(Packets.Client.CSMG_ANOTHERBOOK_PAPER_COMPOUND p)
        {
            Item penItem = this.Character.Inventory.GetItem(p.SlotID);
            byte paperID = p.paperID;
            byte lv = (byte)(this.Character.AnotherPapers[paperID].Level + 1);
            ulong value = (ulong)(0xff << 8 * (lv - 1));
            if (lv == 1) value = 0xff;
            if (this.Character.AnotherPapers[paperID].PaperValue.Test(value))
            {
                if (lv > 1)
                {
                    uint penID = 0;
                    if (AnotherFactory.Instance.Items[paperID][lv].ReqeustItem2 == penItem.ItemID)
                        penID = penItem.ItemID;
                    else if (AnotherFactory.Instance.Items[paperID][lv].RequestItem1 == penItem.ItemID)
                        penID = penItem.ItemID;
                    else return;
                    if (CountItem(penID) < 1) return;
                    DeleteItemID(penID, 1, true);
                }
                Character.AnotherPapers[paperID].Level = lv;
                Packets.Server.SSMG_ANOTHERBOOK_PAPER_COMPOUND_RESULT p2 = new Packets.Server.SSMG_ANOTHERBOOK_PAPER_COMPOUND_RESULT();
                p2.lv = lv;
                p2.paperID = paperID;
                this.netIO.SendPacket(p2);
            }
            else return;
        }

        public void OnAnotherBookPaperUse(Packets.Client.CSMG_ANOTHERBOOK_PAPER_USE p)
        {
            Item paperItem = this.Character.Inventory.GetItem(p.slotID);
            if (paperItem != null)
            {
                byte paperID = p.paperID;
                if (AnotherFactory.Instance.Items[paperID][1].PaperItems1.Contains(paperItem.ItemID))
                {
                    byte lv = AnotherFactory.Instance.GetPaperLv(this.Character.AnotherPapers[paperID].PaperValue.Value);
                    ulong value = GetPaperValue(paperID, (byte)(lv + 1), paperItem.ItemID);
                    if (value == 0) return;
                    if (!this.Character.AnotherPapers[paperID].PaperValue.Test(value))
                        this.Character.AnotherPapers[paperID].PaperValue.SetValue(value, true);
                    else return;
                    this.DeleteItem(p.slotID, 1, true);
                    Packets.Server.SSMG_ANOTHERBOOK_PAPER_USE_RESULT p2 = new Packets.Server.SSMG_ANOTHERBOOK_PAPER_USE_RESULT();
                    p2.PageValue = this.Character.AnotherPapers[paperID].PaperValue.Value;
                    p2.PaperID = paperID;
                    this.netIO.SendPacket(p2);
                    MapServer.charDB.SavePaper(Character);
                }
                else if (AnotherFactory.Instance.Items[paperID][1].PaperItems2.Contains(paperItem.ItemID))
                {
                    byte lv = AnotherFactory.Instance.GetPaperLv(this.Character.AnotherPapers[paperID].PaperValue.Value);
                    ulong value = GetPaperValue(paperID, (byte)(lv + 1), paperItem.ItemID);
                    if (value == 0) return;
                    if (!this.Character.AnotherPapers[paperID].PaperValue.Test(value))
                        this.Character.AnotherPapers[paperID].PaperValue.SetValue(value, true);
                    else return;
                    this.DeleteItem(p.slotID, 1, true);
                    Packets.Server.SSMG_ANOTHERBOOK_PAPER_USE_RESULT p2 = new Packets.Server.SSMG_ANOTHERBOOK_PAPER_USE_RESULT();
                    p2.PageValue = this.Character.AnotherPapers[paperID].PaperValue.Value;
                    p2.PaperID = paperID;
                    this.netIO.SendPacket(p2);
                    MapServer.charDB.SavePaper(Character);
                }
            }
        }

        public ulong GetPaperValue(byte paperID, byte lv, uint ItemID)
        {
            ulong value = 0;
            if (!AnotherFactory.Instance.Items.ContainsKey(paperID)) return 0;
            if (!AnotherFactory.Instance.Items[paperID].ContainsKey(lv)) return 0;
            if (!AnotherFactory.Instance.Items[paperID][lv].PaperItems1.Contains(ItemID))
                if (!AnotherFactory.Instance.Items[paperID][lv].PaperItems2.Contains(ItemID))
                    return 0;
            int index = AnotherFactory.Instance.Items[paperID][lv].PaperItems1.IndexOf(ItemID);
            if(index == -1)
                index = AnotherFactory.Instance.Items[paperID][lv].PaperItems2.IndexOf(ItemID);
            switch (index)
            {
                case 0:
                    value = 0x1;
                    break;
                case 1:
                    value = 0x2;
                    break;
                case 2:
                    value = 0x4;
                    break;
                case 3:
                    value = 0x8;
                    break;
                case 4:
                    value = 0x10;
                    break;
                case 5:
                    value = 0x20;
                    break;
                case 6:
                    value = 0x40;
                    break;
                case 7:
                    value = 0x80;
                    break;
            }
            value = value << 8 * (lv - 1);
            return value;
        }

        public void OnAnotherBookUIOpen(Packets.Client.CSMG_ANOTHERBOOK_UI_OPEN_REQEUST p)
        {
            try
            {
                CreateAnotherPaper(1);
                CreateAnotherPaper(2);
                CreateAnotherPaper(3);
                CreateAnotherPaper(4);
                CreateAnotherPaper(5);
                CreateAnotherPaper(6);
                CreateAnotherPaper(7);
                CreateAnotherPaper(8);
                CreateAnotherPaper(9);
                CreateAnotherPaper(10);
                CreateAnotherPaper(11);
                CreateAnotherPaper(12);
                CreateAnotherPaper(13);
                List<ushort> AnotherPaperClass = new List<ushort>();
                List<ulong> AnotherPaperValues = new List<ulong>();
                List<byte> AnotherPaperLevels = new List<byte>();
                Packets.Server.SSMG_ANOTHERBOOK_SHOW_WINDOW p2 = new Packets.Server.SSMG_ANOTHERBOOK_SHOW_WINDOW();
                p2.PageIndex = p.index;
                p2.CEXP = this.Character.CEXP;
                p2.UsingPaperID = this.Character.UsingPaperID;
                foreach (var item in this.Character.AnotherPapers.Keys)
                {
                    if (AnotherFactory.Instance.Items.ContainsKey(item))
                    {
                        if (AnotherFactory.Instance.Items[item][1].Type == p.index)
                        {
                            AnotherPaperClass.Add((ushort)item);
                        }
                    }
                }
                p2.AnotherPaperClassIDs = AnotherPaperClass;
                if (this.Character.UsingPaperID != 0)
                    p2.UsingPaperValue = this.Character.AnotherPapers[this.Character.UsingPaperID].PaperValue.Value;

                for (int i = 0; i < AnotherPaperClass.Count; i++)
                {
                    AnotherPaperValues.Add(this.Character.AnotherPapers[AnotherPaperClass[i]].PaperValue.Value);
                }

                p2.AnotherPaperValues = AnotherPaperValues;

                if (this.Character.UsingPaperID != 0)
                    p2.UsingPaperLevel = Character.AnotherPapers[Character.UsingPaperID].Level;//AnotherFactory.Instance.GetPaperLv(this.Character.AnotherPapers[this.Character.UsingPaperID].value.Value);

                for (int i = 0; i < AnotherPaperClass.Count; i++)
                {
                    AnotherPaperLevels.Add(Character.AnotherPapers[AnotherPaperClass[i]].Level);//AnotherFactory.Instance.GetPaperLv(this.Character.AnotherPapers[List1[i]].value.Value));
                }

                p2.AnotherPaperLevels = AnotherPaperLevels;
                /*if (this.Character.UsingPaperID != 0)
                    p2.usingSkillEXP_1 = this.Character.AnotherPapers[this.Character.UsingPaperID].skills[0];
                List2 = new List<ulong>();
                for (int i = 0; i < List1.Count; i++)
                {
                    List2.Add(this.Character.AnotherPapers[List1[i]].skills[1]);
                }
                p2.paperSkillsEXP_1 = List2;
                if (this.Character.UsingPaperID != 0)
                    p2.usingSkillEXP_2 = this.Character.AnotherPapers[this.Character.UsingPaperID].skills[1];
                List2 = new List<ulong>();
                for (int i = 0; i < List1.Count; i++)
                {
                    List2.Add(this.Character.AnotherPapers[List1[i]].skills[2]);
                }
                p2.paperSkillsEXP_2 = List2;
                if (this.Character.UsingPaperID != 0)
                    p2.usingSkillEXP_3 = this.Character.AnotherPapers[this.Character.UsingPaperID].skills[2];
                List2 = new List<ulong>();
                for (int i = 0; i < List1.Count; i++)
                {
                    List2.Add(this.Character.AnotherPapers[List1[i]].skills[3]);
                }
                p2.paperSkillsEXP_3 = List2;
                if (this.Character.UsingPaperID != 0)
                    p2.usingSkillEXP_4 = this.Character.AnotherPapers[this.Character.UsingPaperID].skills[3];
                List2 = new List<ulong>();
                for (int i = 0; i < List1.Count; i++)
                {
                    List2.Add(this.Character.AnotherPapers[List1[i]].skills[4]);
                }
                p2.paperSkillsEXP_4 = List2;*/


                this.netIO.SendPacket(p2);
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }
        }

        public void OnAnotherBookPaperSkillGainExp(Packets.Client.CSMG_ANOTHERBOOK_PAPER_SKILL_GAIN_EXP p)
        {
            var skillcount = p.SkillCount;
            var skilllist = p.SkillList;
            var explist = p.ExpList;

            Packets.Server.SSMG_ANOTHERBOOK_PAPER_SKILL_GAIN_EXP_RESULT p1 = new Packets.Server.SSMG_ANOTHERBOOK_PAPER_SKILL_GAIN_EXP_RESULT();
            p1.Result = 0;
            p1.SkillIDList = skilllist;

            //for (int i = 0; i < skilllist.Count; i++)
            //{
            //    this.Character.AnotherPapers[this.Character.UsingPaperID].Skills[skilllist[i]] = (ulong)(this.Character.AnotherPapers[this.Character.UsingPaperID].Skills[skilllist[i]] + explist[i]);
            //}
            //explist = this.Character.AnotherPapers[this.Character.UsingPaperID].Skills.Values.ToList();
            p1.SkillExpList = explist;

            ulong sum = 0;
            foreach (var item in explist)
                sum += item;

            p1.PageExpRemain = (ulong)(this.Character.CEXP - sum);
            this.netIO.SendPacket(p1);
        }
    }
}
