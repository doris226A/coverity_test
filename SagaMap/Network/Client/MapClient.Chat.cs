using System;
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


namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        public void OnChat(Packets.Client.CSMG_CHAT_PUBLIC p)
        {
            if (!AtCommand.Instance.ProcessCommand(this, p.Content))
            {
                if (p.Content.Substring(0, 1) == "!")
                {
                    if (Character.Account.GMLevel > 100)
                        SendSystemMessage("Command error。");
                    return;
                }
                ChatArg arg = new ChatArg();
                arg.content = p.Content;
                Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAT, arg, this.Character, true);
                if (this.Map.IsAncientArk)//ECOKEY 圖書館任務新增
                {
                    SagaMap.Manager.AncientArkManager.Instance.任務_回答問題(p.Content, this.Map);
                }
            }
        }

        public void OnTakeGift(Packets.Client.CSMG_CHAT_GIFT_TAKE p)
        {
            MapServer.charDB.GetGifts(Character);
            uint GiftID = p.GiftID;
            byte Type = p.type;
            if (Type == 0)
            {
                var gift = from G in Character.Gifts
                           where GiftID == G.MailID
                           select G;
                if (gift == null)
                {
                    SendSystemMessage("unexpected command");
                    return;
                }
                else
                {
                    if (gift.Count() == 0)
                    {
                        SendSystemMessage("unexpected command");
                        return;
                    }
                    SagaDB.BBS.Gift Gift = gift.First();
                    if(Gift.AccountID != Character.Account.AccountID)
                    {
                        SendSystemMessage("unexpected command");
                        return;
                    }
                    if (!MapServer.charDB.DeleteGift(Gift))
                    {
                        SendSystemMessage("unexpected command");
                        return;
                    }
                    foreach (var i in Gift.Items.Keys)
                    {
                        uint ItemID = i;
                        ushort Count = Gift.Items[i];
                        Item item = ItemFactory.Instance.GetItem(ItemID);
                        item.Stack = Count;
                        AddItem(item, true);

                    }
                    Character.Gifts.Remove(Gift);
                }
            }
            else
            {
                var gift = from G in Character.Gifts
                           where GiftID == G.MailID
                           select G;
                if (gift == null)
                {
                    SendSystemMessage("unexpected command");
                    return;
                }
                if (gift.Count() == 0)
                {
                    SendSystemMessage("unexpected command");
                    return;
                }
                SagaDB.BBS.Gift Gift = gift.First();
                if (!MapServer.charDB.DeleteGift(Gift))
                {
                    SendSystemMessage("unexpected command");
                    return;
                }
                Character.Gifts.Remove(Gift);
            }

            Packets.Server.SSMG_GIFT_TAKERECIPT p3 = new Packets.Server.SSMG_GIFT_TAKERECIPT();
            p3.type = Type;
            p3.MailID = GiftID;
            netIO.SendPacket(p3);
            return;
        }

        public void OnChatParty(Packets.Client.CSMG_CHAT_PARTY p)
        {
            if (this.Character != null)
            {
                if (p.Content.Substring(0, 1) == "!")
                    return;
                PartyManager.Instance.PartyChat(this.Character.Party, this.Character, p.Content);
            }
            Logger logger = new Logger("玩家隊伍聊天.txt");
            string log = "\r\n玩家" + this.Character.Name + " ：" + p.Content + "";
            logger.WriteLog(log);
        }
        public void OnExpression(Packets.Client.CSMG_CHAT_EXPRESSION p)
        {
            ChatArg arg = new ChatArg();
            arg.expression = p.Motion;
            if (p.Loop == 0)
                Character.EMotionLoop = false;
            else
                Character.EMotionLoop = true;
            if (p.Motion <= 4)
                Character.EMotion = p.Motion;
            Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.MOTION, arg, this.Character, true);
        }
        public void OnWaitType(Packets.Client.CSMG_CHAT_WAITTYPE p)
        {
            this.Character.WaitType = p.type;
            Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.WAITTYPE, null, this.Character, true);
        }
        public void OnMotion(Packets.Client.CSMG_CHAT_MOTION p)
        {

            //Cancel Cloak
            if (this.Character.Status.Additions.ContainsKey("Cloaking"))
                SagaMap.Skill.SkillHandler.RemoveAddition(this.Character, "Cloaking");

            ChatArg arg = new ChatArg();
            arg.motion = p.Motion;
            arg.loop = p.Loop;
            Character.Motion = arg.motion;
            if (arg.loop == 1)
                Character.MotionLoop = true;
            else
                Character.MotionLoop = false;
            Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.MOTION, arg, this.Character, true);

            if ((int)p.Motion == 140 || (int)p.Motion == 141 || (int)p.Motion == 159 || (int)p.Motion == 113 || (int)p.Motion == 210
                || (int)p.Motion == 555 || (int)p.Motion == 556 || (int)p.Motion == 557 || (int)p.Motion == 558 || (int)p.Motion == 559
                || (int)p.Motion == 400)
            {
                if (Character.Partner != null)
                {
                    ActorPartner partner = Character.Partner;
                    ChatArg parg = new ChatArg();
                    parg.motion = p.Motion;
                    parg.loop = 1;
                    partner.Motion = parg.motion;
                    partner.MotionLoop = true;
                    Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.MOTION, parg, partner, true);
                }
            }
        }

        public void OnEmotion(Packets.Client.CSMG_CHAT_EMOTION p)
        {
            ChatArg arg = new ChatArg();
            arg.emotion = p.Emotion;
            Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.EMOTION, arg, this.Character, true);
        }

        public void OnSit(Packets.Client.CSMG_CHAT_SIT p)
        {
            if (this.Character.PossessionTarget != 0)
            {
                return;
            }
            else
            {
                ChatArg arg = new ChatArg();
            ChatArg parg = new ChatArg();
            if (this.Character.Motion != MotionType.SIT)
            {
                arg.motion = MotionType.SIT;
                arg.loop = 1;
                this.Character.Motion = MotionType.SIT;
                this.Character.MotionLoop = true;
                this.Character.Buff.Sit = true;
                PartnerTalking(Character.Partner, TALK_EVENT.MASTERSIT, 50, 5000);

                if (Character.Partner != null)
                {
                    ActorPartner partner = Character.Partner;
                    //ECOKEY 紀錄寵物坐下位置，TInt是臨時變量，下線後就沒了
                    this.Character.TInt["PartnerX"] = partner.X;
                    this.Character.TInt["PartnerY"] = partner.Y;
                    parg.motion = (MotionType)135;
                    parg.loop = 1;
                    partner.Motion = parg.motion;
                    partner.MotionLoop = true;
                 //   this.Character.Partner.Buff.Sit = true;
                   // Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, arg, this.Character.Partner, true);
                    Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.MOTION, parg, partner, true);
                }

                if (!this.Character.Tasks.ContainsKey("Regeneration"))
                {
                    Tasks.PC.Regeneration reg = new SagaMap.Tasks.PC.Regeneration(this);
                    this.Character.Tasks.Add("Regeneration", reg);
                    reg.Activate();
                }
            }
            else
            {
                if (this.Character.Tasks.ContainsKey("Regeneration"))
                {
                    this.Character.Tasks["Regeneration"].Deactivate();
                    this.Character.Tasks.Remove("Regeneration");
                    this.Character.Buff.Sit = false;
                    Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, arg, this.Character, true);
                }
                arg.motion = MotionType.STAND;
                arg.loop = 0;
                this.Character.Motion = MotionType.NONE;
                this.Character.MotionLoop = false;
                if (Character.Partner != null)
                {
                    ActorPartner partner = Character.Partner;
                    parg.motion = (MotionType)111;
                    parg.loop = 0;
                    partner.Motion = parg.motion;
                    partner.MotionLoop = false;
                    this.Character.Partner.Buff.Sit = false;
             //       Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, arg, this.Character.Partner, true);
                    Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.MOTION, parg, partner, true);
                }
            }
            Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, arg, this.Character, true);
            Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.MOTION, arg, this.Character, true);
          //  Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, this.Character.Partner, true);
        }
        }
        public void OnSign(Packets.Client.CSMG_CHAT_SIGN p)
        {
            this.Character.Sign = p.Content;
            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SIGN_UPDATE, null, this.Character, true);
        }

        public void SendMotion(MotionType motion, byte loop)
        {
            ChatArg arg = new ChatArg();
            arg.motion = motion;
            arg.loop = loop;
            this.Character.Motion = arg.motion;
            if (arg.loop == 1)
                this.Character.MotionLoop = true;
            else
                this.Character.MotionLoop = false;
            Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.MOTION, arg, this.Character, true);

            if (arg.loop == 0)
                Character.Motion =  (MotionType)111;

            if ((int)arg.motion == 140 || (int)arg.motion == 141 || (int)arg.motion == 159 || (int)arg.motion == 113 || (int)arg.motion == 210
    || (int)arg.motion == 555 || (int)arg.motion == 556 || (int)arg.motion == 557 || (int)arg.motion == 558 || (int)arg.motion == 559
    || (int)arg.motion == 400)
            {
                if (Character.Partner != null)
                {
                    ActorPartner partner = Character.Partner;
                    ChatArg parg = new ChatArg();
                    parg.motion = arg.motion;
                    parg.loop = loop;
                    partner.Motion = parg.motion;
                    partner.MotionLoop = true;
                    Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.MOTION, parg, partner, true);
                }
            }
        }

        public void SendSystemMessage(string content)
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_CHAT_PUBLIC p = new SagaMap.Packets.Server.SSMG_CHAT_PUBLIC();
                p.ActorID = 0xFFFFFFFF;
                p.Message = content;
                this.netIO.SendPacket(p);
            }
        }

        public void SendSystemMessage(Packets.Server.SSMG_SYSTEM_MESSAGE.Messages message)
        {
            Packets.Server.SSMG_SYSTEM_MESSAGE p = new SagaMap.Packets.Server.SSMG_SYSTEM_MESSAGE();
            p.Message = message;
            this.netIO.SendPacket(p);
        }

        public void SendChatParty(string sender, string content)
        {
            Packets.Server.SSMG_CHAT_PARTY p = new SagaMap.Packets.Server.SSMG_CHAT_PARTY();
            p.Sender = sender;
            p.Content = content;
            this.netIO.SendPacket(p);
        }

        /// <summary>
        /// 查看目标装备
        /// </summary>
        public void OnPlayerEquipOpen(uint charID)
        {
            ActorPC pc = this.map.GetPC(charID);
            if(pc.Fictitious)
            {
                SendSystemMessage("無法查看已融合的裝備列表。");
                return;
            }
            if (!pc.canShowEquipment)
            {
                SendSystemMessage("對方已設置不允許查看裝備列表。");
                return;
            }
            MapClient.FromActorPC(pc).SendSystemMessage(this.Character.Name + "正在查看你的裝備列表");
            Packets.Server.SSMG_PLAYER_EQUIP_START p1 = new SagaMap.Packets.Server.SSMG_PLAYER_EQUIP_START();
            this.netIO.SendPacket(p1);
            Packets.Server.SSMG_PLAYER_EQUIP_NAME p2 = new SagaMap.Packets.Server.SSMG_PLAYER_EQUIP_NAME();
            p2.ActorName = pc.Name;
            this.netIO.SendPacket(p2);
            foreach (KeyValuePair<EnumEquipSlot, Item> i in pc.Inventory.Equipments)
            {
                Item item = i.Value;
                if (item == null)
                    continue;
                Packets.Server.SSMG_PLAYER_EQUIP_INFO p3 = new SagaMap.Packets.Server.SSMG_PLAYER_EQUIP_INFO();
                p3.InventorySlot = item.Slot;
                p3.Container = pc.Inventory.GetContainerType(item.Slot);
                p3.Item = item;
                this.netIO.SendPacket(p3);
            }
            Packets.Server.SSMG_PLAYER_EQUIP_END p4 = new SagaMap.Packets.Server.SSMG_PLAYER_EQUIP_END();
            this.netIO.SendPacket(p4);
        }

        public void OnPlayerFurnitureSit(Packets.Client.CSMG_PLAYER_FURNITURE_SIT p)
        {
            if (p.unknown != -1)
            {
                this.chara.FurnitureID = p.FurnitureID;
                this.chara.FurnitureID_old = (uint)p.unknown;
            }
            else
            {
                this.chara.FurnitureID_old = 255;
                this.chara.FurnitureID = 255;
            }

            Packets.Server.SSMG_PLAYER_FURNITURE_SIT p1 = new SagaMap.Packets.Server.SSMG_PLAYER_FURNITURE_SIT();
            p1.FurnitureID = p.FurnitureID;
            p1.unknown = p.unknown;
            this.netIO.SendPacket(p1);

            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.FURNITURE_SIT, null, this.Character, true);
        }

    }
}
