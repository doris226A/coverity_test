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
        public void OnPossessionRequest(Packets.Client.CSMG_POSSESSION_REQUEST p)
        {
            ActorPC target = (ActorPC)this.Map.GetActor(p.ActorID);
            PossessionPosition pos = p.PossessionPosition;
            int result = TestPossesionPosition(target, pos);
            if (result >= 0)
            {
                this.Character.Buff.GetReadyPossession = true;
                this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, this.Character, true);
                int reduce = 0;
                if (this.Character.Status.Additions.ContainsKey("TranceSpdUp"))
                {
                    Skill.Additions.Global.DefaultPassiveSkill passive = (Skill.Additions.Global.DefaultPassiveSkill)this.Character.Status.Additions["TranceSpdUp"];
                    reduce = passive["TranceSpdUp"];
                }
                if (this.Character.Status.reance_speed_iris > 0)
                {
                    reduce += this.Character.Status.reance_speed_iris;
                }
                //Tasks.PC.Possession task = new SagaMap.Tasks.PC.Possession(this, target, pos, p.Comment, reduce);
                // this.Character.Tasks.Add("Possession", task);

                // task.Activate();
                string taskKey = "Possession";

                // 檢查是否已經存在相同索引鍵的任務
                if (this.Character.Tasks.ContainsKey(taskKey))
                {
                    return;
                }
                else
                {
                    // 如果不存在相同索引鍵的任務，才進行添加
                    Tasks.PC.Possession task = new SagaMap.Tasks.PC.Possession(this, target, pos, p.Comment, reduce);
                    this.Character.Tasks.Add(taskKey, task);

                    task.Activate();
                }
            }
            else
            {
                Packets.Server.SSMG_POSSESSION_RESULT p1 = new SagaMap.Packets.Server.SSMG_POSSESSION_RESULT();
                p1.FromID = this.Character.ActorID;
                p1.ToID = 0xFFFFFFFF;
                p1.Result = result;
                this.netIO.SendPacket(p1);
            }
            /* ActorPC target = (ActorPC)this.Map.GetActor(p.ActorID);
             PossessionPosition pos = p.PossessionPosition;
             int result = TestPossesionPosition(target, pos);
             if (result >= 0)
             {
                 //ECOKEY PE進度條修復
                 this.Character.Buff.GetReadyPossession = true;
                 this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, this.Character, true);
                 int reduce = 0;
                 if (this.Character.Status.Additions.ContainsKey("TranceSpdUp"))
                 {
                     Skill.Additions.Global.DefaultPassiveSkill passive = (Skill.Additions.Global.DefaultPassiveSkill)this.Character.Status.Additions["TranceSpdUp"];
                     reduce = passive["TranceSpdUp"];
                 }
                 if (this.Character.Status.reance_speed_iris > 0)
                 {
                     reduce += this.Character.Status.reance_speed_iris;
                 }
                 Tasks.PC.Possession task = new SagaMap.Tasks.PC.Possession(this, target, pos, p.Comment, reduce);
                 this.Character.Tasks.Add("Possession", task);
                 task.Activate();


                 reduce = Configuration.Instance.PossessionRequireTime - reduce * 1000;

                 Item item = SagaDB.Item.ItemFactory.Instance.GetItem(10023500);
                 item.BaseData.cast = (uint)reduce;
                 Packets.Server.SSMG_ITEM_USE p2 = new SagaMap.Packets.Server.SSMG_ITEM_USE();
                 p2.ItemID = item.ItemID;
                 p2.Form_ActorId = this.Character.ActorID;
                 p2.result = 0;
                 p2.To_ActorID = this.Character.ActorID;
                 p2.SkillID = 0;
                 p2.Cast = (uint)reduce;
                 p2.SkillLV = 1;
                 p2.X = SagaLib.Global.PosX16to8(this.Character.X, map.Width);
                 p2.Y = SagaLib.Global.PosY16to8(this.Character.Y, map.Height);
                 MapClient.FromActorPC((ActorPC)this.Character).netIO.SendPacket(p2);
                 ////ECOKEY PE進度條修復到這裡
             }
             else
             {
                 Packets.Server.SSMG_POSSESSION_RESULT p1 = new SagaMap.Packets.Server.SSMG_POSSESSION_RESULT();
                 p1.FromID = this.Character.ActorID;
                 p1.ToID = 0xFFFFFFFF;
                 p1.Result = result;
                 this.netIO.SendPacket(p1);
             }*/
        }

        public void OnPossessionCancel(Packets.Client.CSMG_POSSESSION_CANCEL p)
        {
            PossessionPosition pos = p.PossessionPosition;
            uint posID = 0;//ECOKEY 自P掉地上優化
            Logger.ShowInfo(pos.ToString());
            switch (pos)
            {
                case PossessionPosition.NONE:
                    Actor actor = this.Map.GetActor(this.Character.PossessionTarget);
                    if (actor == null)
                        return;
                    PossessionArg arg = new PossessionArg();
                    arg.fromID = this.Character.ActorID;
                    arg.cancel = true;
                    arg.result = (int)this.Character.PossessionPosition;
                    arg.x = Global.PosX16to8(this.Character.X, Map.Width);
                    arg.y = Global.PosY16to8(this.Character.Y, Map.Height);
                    arg.dir = (byte)(this.Character.Dir / 45);
                    if (actor.type == ActorType.ITEM)
                    {
                        Item item = GetPossessionItem(this.Character, this.Character.PossessionPosition);
                        item.PossessionedActor = null;
                        item.PossessionOwner = null;
                        this.Character.PossessionTarget = 0;
                        this.Character.PossessionPosition = PossessionPosition.NONE;
                        arg.toID = 0xFFFFFFFF;
                        this.Map.DeleteActor(actor);
                        //ECOKEY 聯合職業判斷，並更新資料
                        if (item.BaseData.jointJob != PC_JOB.NONE) this.Character.JobJoint = PC_JOB.NONE;
                        PC.StatusFactory.Instance.CalcStatus(this.Character);
                        SendPlayerInfo();
                    }
                    else if (actor.type == ActorType.PC)
                    {
                        ActorPC pc = (ActorPC)actor;
                        arg.toID = pc.ActorID;
                        Item item = GetPossessionItem(pc, this.Character.PossessionPosition);
                        if (item.PossessionedActor != null && item.PossessionOwner != null && item.PossessionedActor == item.PossessionOwner)//ECOKEY 自P掉地上優化
                        {
                            posID = item.PossessionedActor.ActorID;
                            pos = item.PossessionedActor.PossessionPosition;
                        }
                        if (item.PossessionOwner != this.Character)
                        {
                            item.PossessionedActor = null;
                            this.Character.PossessionTarget = 0;
                            this.Character.PossessionPosition = PossessionPosition.NONE;
                        }
                        else
                        {
                            Item item2 = GetPossessionItem(this.Character, this.Character.PossessionPosition);
                            item2.PossessionedActor = null;
                            item2.PossessionOwner = null;
                            this.Character.PossessionTarget = 0;
                            this.Character.PossessionPosition = PossessionPosition.NONE;
                            Packets.Client.CSMG_ITEM_MOVE p3 = new SagaMap.Packets.Client.CSMG_ITEM_MOVE();
                            p3.data = new byte[9];
                            p3.InventoryID = item.Slot;
                            p3.Target = ContainerType.BODY;
                            p3.Count = 1;
                            MapClient.FromActorPC(pc).OnItemMove(p3, true);
                            pc.Inventory.DeleteItem(item.Slot, 1);

                            Packets.Server.SSMG_ITEM_DELETE p2 = new SagaMap.Packets.Server.SSMG_ITEM_DELETE();
                            p2.InventorySlot = item.Slot;
                            ((ActorEventHandlers.PCEventHandler)pc.e).Client.netIO.SendPacket(p2);
                            this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHANGE_EQUIP, null, pc, true);
                        }
                        item.PossessionedActor = null;
                        item.PossessionOwner = null;

                        PC.StatusFactory.Instance.CalcStatus(this.Character);
                        SendPlayerInfo();
                        PC.StatusFactory.Instance.CalcStatus(pc);
                        ((ActorEventHandlers.PCEventHandler)pc.e).Client.SendPlayerInfo();
                    }
                    this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.POSSESSION, arg, this.Character, true);
                    if (posID != 0)//ECOKEY 自P掉地上優化
                    {
                        ActorPC posActor = (ActorPC)this.Map.GetActor(posID);
                        MapClient.FromActorPC(posActor).PossessionPerform(posActor, pos, posActor.CStr["PEcomment"]);
                    }
                    break;
                default:
                    Item item3 = GetPossessionItem(this.Character, pos);
                    if (item3 == null)
                        return;
                    if (item3.PossessionedActor == null)
                        return;
                    PossessionArg arg2 = new PossessionArg();
                    arg2.fromID = item3.PossessionedActor.ActorID;
                    arg2.toID = this.Character.ActorID;
                    arg2.cancel = true;
                    arg2.result = (int)item3.PossessionedActor.PossessionPosition;
                    arg2.x = Global.PosX16to8(this.Character.X, Map.Width);
                    arg2.y = Global.PosY16to8(this.Character.Y, Map.Height);
                    arg2.dir = (byte)(this.Character.Dir / 45);


                    if (item3.PossessionOwner != this.Character && item3.PossessionOwner != null)
                    {
                        Item item4 = GetPossessionItem(item3.PossessionedActor, item3.PossessionedActor.PossessionPosition);
                        if (item4 != null)
                        {
                            item4.PossessionedActor = null;
                            item4.PossessionOwner = null;
                        }

                        Packets.Client.CSMG_ITEM_MOVE p3 = new SagaMap.Packets.Client.CSMG_ITEM_MOVE();
                        p3.data = new byte[9];
                        p3.InventoryID = item3.Slot;
                        p3.Target = ContainerType.BODY;
                        p3.Count = 1;
                        OnItemMove(p3, true);
                        this.Character.Inventory.DeleteItem(item3.Slot, 1);

                        Packets.Server.SSMG_ITEM_DELETE p2 = new SagaMap.Packets.Server.SSMG_ITEM_DELETE();
                        p2.InventorySlot = item3.Slot;
                        this.netIO.SendPacket(p2);
                        this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHANGE_EQUIP, null, this.Character, true);

                        this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.POSSESSION, arg2, this.Character, true);
                        if (((ActorEventHandlers.PCEventHandler)item3.PossessionedActor.e).Client.state == SESSION_STATE.DISCONNECTED)
                        {
                            //ECOKEY PE文字
                            string text = "";
                            if (item3.PossessionedActor.CStr.ContainsKey("PEcomment"))
                                text = item3.PossessionedActor.CStr["PEcomment"];

                            ActorItem itemactor = PossessionItemAdd(item3.PossessionedActor, item3.PossessionedActor.PossessionPosition, text);
                            item3.PossessionedActor.PossessionTarget = itemactor.ActorID;
                            MapServer.charDB.SaveChar(item3.PossessionedActor, false, false);
                            MapServer.accountDB.WriteUser(item3.PossessionedActor.Account);
                            if (item3.BaseData.jointJob != PC_JOB.NONE) this.Character.JobJoint = PC_JOB.NONE;//ECOKEY 聯合職業判斷
                            //ECOKEY 修正脫下裝備時不會更新素質
                            PC.StatusFactory.Instance.CalcStatus(this.Character);
                            SendPlayerInfo();
                            return;
                        }

                        /*this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.POSSESSION, arg2, this.Character, true);
                        if (((ActorEventHandlers.PCEventHandler)item3.PossessionedActor.e).Client.state == SESSION_STATE.DISCONNECTED)
                        {
                            ActorItem itemactor = PossessionItemAdd(item3.PossessionedActor, item3.PossessionedActor.PossessionPosition, "");
                            item3.PossessionedActor.PossessionTarget = itemactor.ActorID;
                            MapServer.charDB.SaveChar(item3.PossessionedActor, false, false);
                            MapServer.accountDB.WriteUser(item3.PossessionedActor.Account);
                            return;
                        }*/
                    }
                    else
                    {
                        Actor actor2 = map.GetActor(this.chara.PossessionTarget);
                        if (actor2 != null)
                        {
                            if (actor2.type == ActorType.ITEM)
                                this.map.DeleteActor(actor2);
                            if (!item3.PossessionedActor.Online)
                            {
                                arg2.fromID = 0xFFFFFFFF;
                            }
                        }
                        this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.POSSESSION, arg2, this.Character, true);
                    }
                    //ECOKEY 自P掉地上優化
                    uint posID2 = 0;
                    if (item3.PossessionedActor != null && item3.PossessionOwner != null && item3.PossessionedActor == item3.PossessionOwner)
                    {
                        posID2 = item3.PossessionedActor.ActorID;
                    }
                    item3.PossessionedActor.PossessionTarget = 0;
                    item3.PossessionedActor.PossessionPosition = PossessionPosition.NONE;
                    item3.PossessionedActor = null;
                    item3.PossessionOwner = null;
                    if (item3.BaseData.jointJob != PC_JOB.NONE) this.Character.JobJoint = PC_JOB.NONE;//ECOKEY 聯合職業判斷
                    PC.StatusFactory.Instance.CalcStatus(this.Character);
                    SendPlayerInfo();
                    if (posID2 != 0)//ECOKEY 自P掉地上優化
                    {
                        ActorPC posActor = (ActorPC)this.Map.GetActor(posID2);
                        MapClient.FromActorPC(posActor).PossessionPerform(posActor, pos, posActor.CStr["PEcomment"]);
                    }
                    break;
                    /*  item3.PossessionedActor.PossessionTarget = 0;
                      item3.PossessionedActor.PossessionPosition = PossessionPosition.NONE;
                      ActorPC posActor = (ActorPC)this.Map.GetActor(item3.PossessionedActor.ActorID);//ECOKEY 自P掉地上，code順序不能錯
                      item3.PossessionedActor = null;
                      item3.PossessionOwner = null;
                      if (item3.BaseData.jointJob != PC_JOB.NONE) this.Character.JobJoint = PC_JOB.NONE;//ECOKEY 聯合職業判斷
                      PC.StatusFactory.Instance.CalcStatus(this.Character);
                      SendPlayerInfo();
                      MapClient.FromActorPC(posActor).PossessionPerform(posActor, pos, posActor.CStr["PEcomment"]);//ECOKEY 自P掉地上，code順序不能錯
                      break;*/
            }
        }

        public void PossessionPerform(ActorPC target, PossessionPosition position, string comment)
        {
            int result = TestPossesionPosition(target, position);
            if (result >= 0)
            {
                PossessionArg arg = new PossessionArg();
                arg.fromID = this.Character.ActorID;
                arg.toID = target.ActorID;
                arg.result = result;
                arg.comment = comment;
                this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.POSSESSION, arg, this.Character, true);

                string pos = "";
                switch (position)
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
                if (target == this.Character)
                {
                    //ECOKEY PE文字
                    this.Character.CStr["PEcomment"] = comment;
                    this.Character.PossessionTarget = PossessionItemAdd(this.Character, position, comment).ActorID;
                    this.Character.PossessionPosition = position;
                }
                else
                {
                    MapClient.FromActorPC(target).SendSystemMessage(string.Format(LocalManager.Instance.Strings.POSSESSION_DONE, pos));
                    this.Character.PossessionTarget = target.ActorID;
                    this.Character.PossessionPosition = position;
                    Item item = GetPossessionItem(target, position);
                    item.PossessionedActor = this.Character;
                }

                if (!this.Character.Tasks.ContainsKey("PossessionRecover"))
                {
                    Tasks.PC.PossessionRecover task = new SagaMap.Tasks.PC.PossessionRecover(this);
                    this.Character.Tasks.Add("PossessionRecover", task);
                    task.Activate();
                }
                Skill.SkillHandler.Instance.CastPassiveSkills(this.Character);
                PC.StatusFactory.Instance.CalcStatus(this.Character);
                SendPlayerInfo();
                PC.StatusFactory.Instance.CalcStatus(target);
                ((ActorEventHandlers.PCEventHandler)target.e).Client.SendPlayerInfo();

                this.BingoCheck(2, true);//BINGO任務2
            }
            else
            {
                Packets.Server.SSMG_POSSESSION_RESULT p1 = new SagaMap.Packets.Server.SSMG_POSSESSION_RESULT();
                p1.FromID = this.Character.ActorID;
                p1.ToID = 0xFFFFFFFF;
                p1.Result = result;
                this.netIO.SendPacket(p1);
            }
        }

        int TestPossesionPosition(ActorPC target, PossessionPosition pos)
        {
            Item item = null;
            if (this.Character.PossessionTarget != 0)
                return -1; //憑依失敗 : 憑依中です
            if (this.Character.PossesionedPartner.Count != 0)//ECOKEY 新增寵物憑依判斷
                return -2; //憑依失敗 : 宿主です
            if (this.Character.PossesionedActors.Count != 0)
                return -2; //憑依失敗 : 宿主です
            if (target.type != ActorType.PC)
                return -3; //憑依失敗 : プレイヤーのみ憑依可能です
            ActorPC targetPC = (ActorPC)target;
            //if (Math.Abs(target.Level - this.Character.Level) > 30)
            //    return -4; //憑依失敗 : レベルが離れすぎです
            //ECOKEY DEM下面移到這邊，禁止DEM憑依，除了指定裝備
            if (targetPC.Race == PC_RACE.DEM && targetPC.Form == DEM_FORM.MACHINA_FORM)
            {
                if (targetPC.Inventory.Parts.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                {
                    if (targetPC.Inventory.Parts[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.SETPARTS_BLOW ||
                    targetPC.Inventory.Parts[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.SETPARTS_SLASH ||
                    targetPC.Inventory.Parts[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.SETPARTS_STAB ||
                    targetPC.Inventory.Parts[EnumEquipSlot.RIGHT_HAND].BaseData.itemType == ItemType.SETPARTS_LONGRANGE)
                    {

                    }
                    else
                    {
                        return -31; //憑依失敗 : マシナフォームのＤＥＭキャラクターに憑依することはできません
                    }
                }
                else
                {
                    return -31; //憑依失敗 : マシナフォームのＤＥＭキャラクターに憑依することはできません
                }
            }
            switch (pos)
            {
                case PossessionPosition.CHEST:
                    if (targetPC.PossessionPartnerSlotIDinClothes != 0)//ECOKEY 新增寵物憑依判斷
                        return -6;
                    if (targetPC.Inventory.Equipments.ContainsKey(EnumEquipSlot.UPPER_BODY))
                        item = targetPC.Inventory.Equipments[EnumEquipSlot.UPPER_BODY];
                    else
                        return -5; //憑依失敗 : 装備がありません
                    break;
                case PossessionPosition.LEFT_HAND:
                    if (targetPC.PossessionPartnerSlotIDinLeftHand != 0)//ECOKEY 新增寵物憑依判斷
                        return -6;
                    if (targetPC.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                        item = targetPC.Inventory.Equipments[EnumEquipSlot.LEFT_HAND];
                    else
                        return -5; //憑依失敗 : 装備がありません
                    break;
                case PossessionPosition.NECK:
                    if (targetPC.PossessionPartnerSlotIDinAccesory != 0)//ECOKEY 新增寵物憑依判斷
                        return -6;
                    if (targetPC.Inventory.Equipments.ContainsKey(EnumEquipSlot.CHEST_ACCE))
                        item = targetPC.Inventory.Equipments[EnumEquipSlot.CHEST_ACCE];
                    else
                        return -5; //憑依失敗 : 装備がありません
                    break;
                case PossessionPosition.RIGHT_HAND:
                    if (targetPC.PossessionPartnerSlotIDinRightHand != 0)//ECOKEY 新增寵物憑依判斷
                        return -6;
                    if (targetPC.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                    {
                        if (targetPC.Buff.FishingState == true)
                        {
                            return -15;
                        }
                        item = targetPC.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND];
                    }
                    else
                    {
                        return -5; //憑依失敗 : 装備がありません
                    }
                    break;
            }
            if (item == null)
                return -5; //憑依失敗 : 装備がありません
            if (item.Stack == 0)
                return -5; ////憑依失敗 : 装備がありません
            if (item.PossessionedActor != null)
                return -6; //憑依失敗 : 誰かが憑依しています
            if (item.BaseData.itemType == ItemType.CARD || item.BaseData.itemType == ItemType.THROW || item.BaseData.itemType == ItemType.ARROW || item.BaseData.itemType == ItemType.BULLET)
                return -7; //憑依失敗 : 憑依不可能なアイテムです
                           //if (targetPC.PossesionedActors.Count >= 3)
                           //    return -8; //憑依失敗 : 満員宿主です

            if (targetPC.PossesionedActors.Count + targetPC.PossesionedPartner.Count >= 3)//ECOKEY 新增寵物憑依判斷
                return -8; //憑依失敗 : 満員宿主です
            if (this.Character.Marionette != null || targetPC.Marionette != null
                || this.Character.Buff.Confused || this.Character.Buff.Frosen || this.Character.Buff.Paralysis
                || this.Character.Buff.Sleep || this.Character.Buff.Stone || this.Character.Buff.Stun)
                return -15; //憑依失敗 : 状態異常中です
            if (targetPC.PossessionTarget != 0)
                return -16; //憑依失敗 : 相手は憑依中です
            if (target.Buff.GetReadyPossession)
                return -17; //憑依失敗 : 相手はGetReadyPossession中です
            if (target.Buff.Dead)
                return -18; //憑依失敗 : 相手は行動不能状態です
            if (this.scriptThread != null || ((ActorEventHandlers.PCEventHandler)target.e).Client.scriptThread != null || target.Buff.FishingState)
                return -19; //憑依失敗 : イベント中です
            if (this.Character.Tasks.ContainsKey("ItemCast"))
                return -19; //憑依失敗 : イベント中です
            if (this.Character.MapID != target.MapID)
                return -20; //憑依失敗 : 相手とマップが違います
            if (Math.Abs(target.X - this.Character.X) > 300 || Math.Abs(target.Y - this.Character.Y) > 300)
                return -21; //憑依失敗 : 相手と離れすぎています
            if (!target.canPossession)
                return -22; //憑依失敗 : 相手が憑依不許可設定中です
                            //ECOKEY 騎士團相關判斷 ~453
                            //if(this.Character.Mode != target.Mode)
                            //    return -25; //憑依失敗:與指定的對像不屬於同一軍團
            if (this.Character.Mode == PlayerMode.KNIGHT_EAST_HERO ||
                this.Character.Mode == PlayerMode.KNIGHT_WEST_HERO ||
                this.Character.Mode == PlayerMode.KNIGHT_SOUTH_HERO ||
                this.Character.Mode == PlayerMode.KNIGHT_NORTH_HERO)
            {
                return -24;//憑依失敗:你處於英雄狀態
            }
            if (this.Character.Mode != target.Mode)
            {
                if ((this.Character.Mode == PlayerMode.KNIGHT_EAST && target.Mode == PlayerMode.KNIGHT_EAST_HERO) ||
                (this.Character.Mode == PlayerMode.KNIGHT_WEST && target.Mode == PlayerMode.KNIGHT_WEST_HERO) ||
                (this.Character.Mode == PlayerMode.KNIGHT_SOUTH && target.Mode == PlayerMode.KNIGHT_SOUTH_HERO) ||
                (this.Character.Mode == PlayerMode.KNIGHT_NORTH && target.Mode == PlayerMode.KNIGHT_NORTH_HERO))
                {

                }
                else
                {
                    return -25; //憑依失敗:與指定的對像不屬於同一軍團
                }
            }
            if (this.Character.Pet != null)
            {
                if (this.Character.Pet.Ride)
                    return -27; //憑依失敗 : 騎乗中です
            }
            if (this.Character.Buff.Dead)
                return -29; //憑依失敗: 憑依できない状態です
            if (this.chara.Race == PC_RACE.DEM)
                return -29; //憑依失敗 : 憑依できない状態です
                            //ECOKEY DEM這段移到上面
                            //if (targetPC.Race == PC_RACE.DEM && targetPC.Form == DEM_FORM.MACHINA_FORM)
                            //    return -31; //憑依失敗 : マシナフォームのＤＥＭキャラクターに憑依することはできません

            //ECOKEY憑依失敗 : 大逃殺和騎士團準備室
            if (this.Character.Mode == PlayerMode.BATTLE_SOUTH ||
                    this.Character.Mode == PlayerMode.BATTLE_NORTH ||
                    this.Character.MapID == 20080011 ||
                    this.Character.MapID == 20080007 ||
                    this.Character.MapID == 20080008 ||
                    this.Character.MapID == 20080009 ||
                    this.Character.MapID == 20080010)
                return -28;
            /*
            if (this.Character.Buff.GetReadyPossession == true || this.Character.PossessionTarget != 0)
                return -100; //憑依失敗 : 何らかの原因で出来ませんでした
            */
            return (int)pos;
        }

        Item GetPossessionItem(ActorPC target, PossessionPosition pos)
        {
            Item item = null;
            switch (pos)
            {
                case PossessionPosition.CHEST:
                    if (target.Inventory.Equipments.ContainsKey(EnumEquipSlot.UPPER_BODY))
                        item = target.Inventory.Equipments[EnumEquipSlot.UPPER_BODY];
                    break;
                case PossessionPosition.LEFT_HAND:
                    if (target.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                        item = target.Inventory.Equipments[EnumEquipSlot.LEFT_HAND];
                    break;
                case PossessionPosition.NECK:
                    if (target.Inventory.Equipments.ContainsKey(EnumEquipSlot.CHEST_ACCE))
                        item = target.Inventory.Equipments[EnumEquipSlot.CHEST_ACCE];
                    break;
                case PossessionPosition.RIGHT_HAND:
                    if (target.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                        item = target.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND];
                    break;
            }
            return item;
        }

        ActorItem PossessionItemAdd(ActorPC target, PossessionPosition position, string comment)
        {
            Item itemDroped = GetPossessionItem(target, position);
            if (itemDroped == null) return null;
            itemDroped.PossessionedActor = target;
            itemDroped.PossessionOwner = target;
            ActorItem actor = new ActorItem(itemDroped);
            actor.e = new ActorEventHandlers.ItemEventHandler(actor);
            actor.MapID = target.MapID;
            actor.X = target.X;
            actor.Y = target.Y;
            actor.Comment = comment;
            actor.Mode = target.Mode;//新增自P道具的mode
            this.Map.RegisterActor(actor);
            actor.invisble = false;
            this.Map.OnActorVisibilityChange(actor);
            return actor;
        }

        ActorPC GetPossessionTarget()
        {
            if (this.Character.PossessionTarget == 0)
                return null;
            Actor actor = this.Map.GetActor(this.Character.PossessionTarget);
            if (actor == null)
                return null;
            if (actor.type != ActorType.PC)
                return null;
            return (ActorPC)actor;
        }

        void PossessionPrepareCancel()
        {
            if (this.Character.Buff.GetReadyPossession)
            {
                this.Character.Buff.GetReadyPossession = false;
                this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, this.Character, true);
                if (this.Character.Tasks.ContainsKey("Possession"))
                {
                    this.Character.Tasks["Possession"].Deactivate();
                    this.Character.Tasks.Remove("Possession");
                }
            }
        }
        //ECOKEY PE進度條消失修復
        /*  public void PossessionCancel()
          {
              SagaDB.Item.Item item = SagaDB.Item.ItemFactory.Instance.GetItem(10023500);
              SkillArg arg = new SkillArg();
              arg.sActor = this.Character.ActorID;
              arg.dActor = this.Character.ActorID;
              arg.item = item;
              arg.x = (byte)this.Character.X;
              arg.y = (byte)this.Character.Y;
              arg.argType = SkillArg.ArgType.Item_Cast;

              Packets.Server.SSMG_ITEM_ACTIVE_SELF p3 = new SagaMap.Packets.Server.SSMG_ITEM_ACTIVE_SELF((byte)arg.affectedActors.Count);
              p3.ActorID = this.Character.ActorID;
              p3.AffectedID = arg.affectedActors;
              p3.AttackFlag(arg.flag);
              p3.ItemID = arg.item.ItemID;
              p3.SetHP(arg.hp);
              p3.SetMP(arg.mp);
              p3.SetSP(arg.sp);
              MapClient.FromActorPC((ActorPC)this.Character).netIO.SendPacket(p3);
          }*/

        public void OnPossessionCatalogRequest(Packets.Client.CSMG_POSSESSION_CATALOG_REQUEST p)
        {
            List<ActorItem> list = new List<ActorItem>();
            foreach (Actor actor in this.map.Actors.Values)
            {
                if (actor is ActorItem)
                {
                    ActorItem item = (ActorItem)actor;
                    if (item.Item.PossessionedActor.PossessionPosition == p.Position)
                        list.Add(item);
                }
            }
            int pageSize = 5;
            int skip = pageSize * (p.Page - 1);
            var items = list.Select(x => x)
                         .Skip(skip)
                         .Take(pageSize)
                         .ToArray();
            for (int i = 0; i < items.Length; i++)
            {
                Packets.Server.SSMG_POSSESSION_CATALOG p1 = new Packets.Server.SSMG_POSSESSION_CATALOG();
                p1.ActorID = items[i].ActorID;
                p1.comment = items[i].Comment;
                p1.Index = (uint)i + 1;
                p1.Item = items[i].Item;
                this.netIO.SendPacket(p1);
            }
            Packets.Server.SSMG_POSSESSION_CATALOG_END p2 = new Packets.Server.SSMG_POSSESSION_CATALOG_END();
            p2.Page = p.Page;
            this.netIO.SendPacket(p2);
        }
        public void OnPossessionCatalogItemInfoRequest(Packets.Client.CSMG_POSSESSION_CATALOG_ITEM_INFO_REQUEST p)
        {
            Actor target = this.map.GetActor(p.ActorID);
            if (target != null)
            {
                if (target is ActorItem)
                {
                    ActorItem item = (ActorItem)target;
                    Packets.Server.SSMG_POSSESSION_CATALOG_ITEM_INFO p2 = new Packets.Server.SSMG_POSSESSION_CATALOG_ITEM_INFO();
                    p2.ActorID = item.ActorID;
                    p2.ItemID = item.Item.ItemID;
                    p2.Level = item.Item.BaseData.possibleLv;
                    p2.X = Global.PosX16to8(item.X, this.map.Width);
                    p2.Y = Global.PosY16to8(item.Y, this.map.Height);
                    this.netIO.SendPacket(p2);
                }
            }
        }
        //ECOKEY 寵物PE修復
        public void OnPartnerPossessionRequest(Packets.Client.CSMG_POSSESSION_PARTNER_REQUEST p)
        {
            Item partneritem = Character.Inventory.GetItem(p.InventorySlot);
            int res = TestPossesionPartner(p.PossessionPosition);
            if (res < 0)
            {
                Packets.Server.SSMG_POSSESSION_RESULT p1 = new SagaMap.Packets.Server.SSMG_POSSESSION_RESULT();
                p1.FromID = this.Character.ActorID;
                p1.ToID = 0xFFFFFFFF;
                p1.Result = res;
                this.netIO.SendPacket(p1);
                return;
            }
            ActorPartner partner = MapServer.charDB.GetActorPartner(partneritem.ActorPartnerID, partneritem);
            if (partner == null)
                return;
            uint Pict = partneritem.BaseData.petID;
            if (partner.PictID != 0)
            {
                Pict = partner.PictID;
            }
            //uint Pict = partneritem.BaseData.petID;
            //if (Pict == partner.BaseData.pictid) return;
            if (partneritem != null)
            {
                switch (p.PossessionPosition)
                {
                    case PossessionPosition.RIGHT_HAND:
                        if (Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                        {
                            Character.PossessionPartnerSlotIDinRightHand = Pict;
                            Character.PartnerSlotRightHand = p.InventorySlot;
                            if (Character.PossesionedPartner.ContainsKey(PossessionPosition.RIGHT_HAND))
                            {
                                MapServer.charDB.SavePartner(Character.PossesionedPartner[PossessionPosition.RIGHT_HAND]);
                                Character.PossesionedPartner[PossessionPosition.RIGHT_HAND] = partner;
                            }
                            else
                                Character.PossesionedPartner.Add(PossessionPosition.RIGHT_HAND, partner);
                        }
                        break;
                    case PossessionPosition.LEFT_HAND:
                        if (Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                        {
                            Character.PossessionPartnerSlotIDinLeftHand = Pict;
                            Character.PartnerSlotLeftHand = p.InventorySlot;
                            if (Character.PossesionedPartner.ContainsKey(PossessionPosition.LEFT_HAND))
                            {
                                MapServer.charDB.SavePartner(Character.PossesionedPartner[PossessionPosition.LEFT_HAND]);
                                Character.PossesionedPartner[PossessionPosition.LEFT_HAND] = partner;
                            }
                            else
                                Character.PossesionedPartner.Add(PossessionPosition.LEFT_HAND, partner);
                        }
                        break;
                    case PossessionPosition.NECK:
                        if (Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.CHEST_ACCE))
                        {
                            Character.PossessionPartnerSlotIDinAccesory = Pict;
                            Character.PartnerSlotAccesory = p.InventorySlot;
                            if (Character.PossesionedPartner.ContainsKey(PossessionPosition.NECK))
                            {
                                MapServer.charDB.SavePartner(Character.PossesionedPartner[PossessionPosition.NECK]);
                                Character.PossesionedPartner[PossessionPosition.NECK] = partner;
                            }
                            else
                                Character.PossesionedPartner.Add(PossessionPosition.NECK, partner);
                        }
                        break;
                    case PossessionPosition.CHEST:
                        if (Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.UPPER_BODY))
                        {
                            Character.PossessionPartnerSlotIDinClothes = Pict;
                            Character.PartnerSlotClothes = p.InventorySlot;
                            if (Character.PossesionedPartner.ContainsKey(PossessionPosition.CHEST))
                            {
                                MapServer.charDB.SavePartner(Character.PossesionedPartner[PossessionPosition.CHEST]);
                                Character.PossesionedPartner[PossessionPosition.CHEST] = partner;
                            }
                            else
                                Character.PossesionedPartner.Add(PossessionPosition.CHEST, partner);
                        }
                        break;
                }
                Packets.Server.SSMG_POSSESSION_PARTNER_RESULT p1 = new Packets.Server.SSMG_POSSESSION_PARTNER_RESULT();
                p1.InventorySlot = p.InventorySlot;
                p1.Pos = p.PossessionPosition;
                this.netIO.SendPacket(p1);
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, Character, true);
                PartnerPossessionStatus();
                SagaMap.PC.StatusFactory.Instance.CalcStatus(Character);
                SendPlayerInfo();
                SagaMap.Network.Client.MapClient.FromActorPC((ActorPC)Character).BingoCheck(3, true);//BINGO任務3

            }
        }
        //ECOKEY 寵物取消PE修復
        public void OnPartnerPossessionCancel(Packets.Client.CSMG_POSSESSION_PARTNER_CANCEL p)
        {
            Packets.Server.SSMG_POSSESSION_PARTNER_CANCEL p1 = new Packets.Server.SSMG_POSSESSION_PARTNER_CANCEL();
            switch (p.PossessionPosition)
            {
                case PossessionPosition.RIGHT_HAND:
                    Character.PossessionPartnerSlotIDinRightHand = 0;
                    Character.PartnerSlotRightHand = 0;
                    if (Character.PossesionedPartner.ContainsKey(PossessionPosition.RIGHT_HAND))
                    {
                        MapServer.charDB.SavePartner(Character.PossesionedPartner[PossessionPosition.RIGHT_HAND]);
                        Character.PossesionedPartner.Remove(PossessionPosition.RIGHT_HAND);
                    }
                    break;
                case PossessionPosition.LEFT_HAND:
                    Character.PossessionPartnerSlotIDinLeftHand = 0;
                    Character.PartnerSlotLeftHand = 0;
                    if (Character.PossesionedPartner.ContainsKey(PossessionPosition.LEFT_HAND))
                    {
                        MapServer.charDB.SavePartner(Character.PossesionedPartner[PossessionPosition.LEFT_HAND]);
                        Character.PossesionedPartner.Remove(PossessionPosition.LEFT_HAND);
                    }
                    break;
                case PossessionPosition.NECK:
                    Character.PossessionPartnerSlotIDinAccesory = 0;
                    Character.PartnerSlotAccesory = 0;
                    if (Character.PossesionedPartner.ContainsKey(PossessionPosition.NECK))
                    {
                        MapServer.charDB.SavePartner(Character.PossesionedPartner[PossessionPosition.NECK]);
                        Character.PossesionedPartner.Remove(PossessionPosition.NECK);
                    }
                    break;
                case PossessionPosition.CHEST:
                    Character.PossessionPartnerSlotIDinClothes = 0;
                    Character.PartnerSlotClothes = 0;
                    if (Character.PossesionedPartner.ContainsKey(PossessionPosition.CHEST))
                    {
                        MapServer.charDB.SavePartner(Character.PossesionedPartner[PossessionPosition.CHEST]);
                        Character.PossesionedPartner.Remove(PossessionPosition.CHEST);
                    }
                    break;
            }
            p1.Pos = p.PossessionPosition;
            this.netIO.SendPacket(p1);
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, Character, true);
            PartnerPossessionStatus();
            SagaMap.PC.StatusFactory.Instance.CalcStatus(Character);
            SendPlayerInfo();
        }
        //ECOKEY 寵物PE專用判斷
        int TestPossesionPartner(PossessionPosition pos)
        {
            Item item = null;
            if (this.Character.PossessionTarget != 0)
                return -1; //憑依失敗 : 憑依中です
            switch (pos)
            {
                case PossessionPosition.CHEST:
                    if (Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.UPPER_BODY))
                        item = Character.Inventory.Equipments[EnumEquipSlot.UPPER_BODY];
                    else
                        return -5; //憑依失敗 : 装備がありません
                    break;
                case PossessionPosition.LEFT_HAND:
                    if (Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                        item = Character.Inventory.Equipments[EnumEquipSlot.LEFT_HAND];
                    else
                        return -5; //憑依失敗 : 装備がありません
                    break;
                case PossessionPosition.NECK:
                    if (Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.CHEST_ACCE))
                        item = Character.Inventory.Equipments[EnumEquipSlot.CHEST_ACCE];
                    else
                        return -5; //憑依失敗 : 装備がありません
                    break;
                case PossessionPosition.RIGHT_HAND:
                    if (Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                    {
                        if (Character.Buff.FishingState == true)
                        {
                            return -15;
                        }
                        item = Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND];
                    }
                    else
                    {
                        return -5; //憑依失敗 : 装備がありません
                    }
                    break;
            }
            if (item == null)
                return -5; //憑依失敗 : 装備がありません
            if (item.Stack == 0)
                return -5; ////憑依失敗 : 装備がありません
            if (item.PossessionedActor != null)
                return -6; //憑依失敗 : 誰かが憑依しています
            if (item.BaseData.itemType == ItemType.CARD || item.BaseData.itemType == ItemType.THROW || item.BaseData.itemType == ItemType.ARROW || item.BaseData.itemType == ItemType.BULLET)
                return -7; //憑依失敗 : 憑依不可能なアイテムです


            if (Character.PossesionedActors.Count + Character.PossesionedPartner.Count >= 3)
                return -8; //憑依失敗 : 満員宿主です


            if (this.Character.Buff.Confused || this.Character.Buff.Frosen || this.Character.Buff.Paralysis
                || this.Character.Buff.Sleep || this.Character.Buff.Stone || this.Character.Buff.Stun)
                return -15; //憑依失敗 : 状態異常中です
            if (Character.PossessionTarget != 0)
                return -16; //憑依失敗 : 相手は憑依中です
            if (Character.Buff.GetReadyPossession)
                return -17; //憑依失敗 : 相手はGetReadyPossession中です
            if (Character.Buff.Dead)
                return -18; //憑依失敗 : 相手は行動不能状態です
            if (this.scriptThread != null || Character.Buff.FishingState)
                return -19; //憑依失敗 : イベント中です
            if (this.Character.Tasks.ContainsKey("ItemCast"))
                return -19; //憑依失敗 : イベント中です
            if (this.Character.MapID != Character.MapID)
                return -20; //憑依失敗 : 相手とマップが違います
            if (this.chara.Race == PC_RACE.DEM)
                return -29; //憑依失敗 : 憑依できない状態です
            if (Character.Race == PC_RACE.DEM && Character.Form == DEM_FORM.MACHINA_FORM)
                return -31; //憑依失敗 : マシナフォームのＤＥＭキャラクターに憑依することはできません

            //ECOKEY憑依失敗 : 大逃殺和騎士團準備室
            if (this.Character.Mode == PlayerMode.BATTLE_SOUTH ||
                    this.Character.Mode == PlayerMode.BATTLE_NORTH ||
                    this.Character.MapID == 20080011 ||
                    this.Character.MapID == 20080007 ||
                    this.Character.MapID == 20080008 ||
                    this.Character.MapID == 20080009 ||
                    this.Character.MapID == 20080010)
                return -28;
            /*
            if (this.Character.Buff.GetReadyPossession == true || this.Character.PossessionTarget != 0)
                return -100; //憑依失敗 : 何らかの原因で出来ませんでした
            */
            return (int)pos;
        }

        //ECOKEY 新增寵物憑依專用等差計算
        float CalcPetLevelDiff(int lvDelta)
        {
            if (lvDelta >= -30 && lvDelta < 0)
                return 1f;
            if (lvDelta >= -60 && lvDelta < -31)
                return 1.2f;
            if (lvDelta >= -90 && lvDelta < -61)
                return 1.4f;
            if (lvDelta >= -99 && lvDelta < -91)
                return 1.6f;
            if (lvDelta < -100)
                return 1.8f;
            return 1f;
        }

        //ECOKEY 新增寵物憑依專用計算12/16
        void PartnerPossessionStatus()
        {
            Character.Status.max_atk1_petpy = 0;
            Character.Status.max_atk2_petpy = 0;
            Character.Status.max_atk3_petpy = 0;
            Character.Status.min_atk1_petpy = 0;
            Character.Status.min_atk2_petpy = 0;
            Character.Status.min_atk3_petpy = 0;
            Character.Status.max_matk_petpy = 0;
            Character.Status.min_matk_petpy = 0;
            Character.Status.hit_melee_petpy = 0;
            Character.Status.hit_ranged_petpy = 0;
            Character.Status.def_add_petpy = 0;
            Character.Status.mdef_add_petpy = 0;
            Character.Status.avoid_melee_petpy = 0;
            Character.Status.avoid_ranged_petpy = 0;
            Character.Status.hp_petpy = 0;
            Character.Status.mp_petpy = 0;
            Character.Status.sp_petpy = 0;

            foreach (PossessionPosition i in Character.PossesionedPartner.Keys)
            {
                ActorPartner partner = Character.PossesionedPartner[i];
                partner.Owner = this.Character;
                SagaMap.Partner.StatusFactory.Instance.CalcPartnerStatus(partner);
                float diff = CalcPetLevelDiff(partner.Level - Character.Level);
                switch (i)
                {
                    case PossessionPosition.RIGHT_HAND:
                        if (Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                        {
                            Character.Status.max_atk1_petpy += (short)Math.Min(partner.Status.max_atk1 * 0.035f * diff, 100);
                            Character.Status.max_atk2_petpy += (short)Math.Min(partner.Status.max_atk2 * 0.035f * diff, 100);
                            Character.Status.max_atk3_petpy += (short)Math.Min(partner.Status.max_atk3 * 0.035f * diff, 100);
                            Character.Status.min_atk1_petpy += (short)Math.Min(partner.Status.min_atk1 * 0.035f * diff, 100);
                            Character.Status.min_atk2_petpy += (short)Math.Min(partner.Status.min_atk2 * 0.035f * diff, 100);
                            Character.Status.min_atk3_petpy += (short)Math.Min(partner.Status.min_atk3 * 0.035f * diff, 100);
                            Character.Status.max_matk_petpy += (short)Math.Min(partner.Status.max_matk * 0.035f * diff, 80);
                            Character.Status.min_matk_petpy += (short)Math.Min(partner.Status.min_matk * 0.035f * diff, 80);
                            Character.Status.hit_melee_petpy += (short)Math.Min(partner.Status.hit_melee * 0.035f * diff, 28);
                            Character.Status.hit_ranged_petpy += (short)Math.Min(partner.Status.hit_ranged * 0.035f * diff, 31);
                            Character.Status.def_add_petpy += (short)Math.Min(partner.Status.def_add * 0.045f * diff, 2);
                            Character.Status.mdef_add_petpy += (short)Math.Min(partner.Status.mdef_add * 0.035f * diff, 2);
                            Character.Status.avoid_melee_petpy += (short)Math.Min(partner.Status.hit_melee * 0.016f * diff, 8);
                            Character.Status.avoid_ranged_petpy += (short)Math.Min(partner.Status.hit_ranged * 0.016f * diff, 8);
                            Character.Status.hp_petpy += (short)Math.Min(partner.MaxHP * 0.006f * diff, 300);
                            Character.Status.mp_petpy += (short)Math.Min(partner.MaxMP * 0.012f * diff, 63);
                            Character.Status.sp_petpy += (short)Math.Min(partner.MaxSP * 0.012f * diff, 41);
                        }
                        break;
                    case PossessionPosition.LEFT_HAND:
                        if (Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.LEFT_HAND))
                        {
                            Character.Status.max_atk1_petpy += (short)Math.Min(partner.Status.max_atk1 * 0.019f * diff, 50);
                            Character.Status.max_atk2_petpy += (short)Math.Min(partner.Status.max_atk2 * 0.019f * diff, 50);
                            Character.Status.max_atk3_petpy += (short)Math.Min(partner.Status.max_atk3 * 0.019f * diff, 50);
                            Character.Status.min_atk1_petpy += (short)Math.Min(partner.Status.min_atk1 * 0.019f * diff, 50);
                            Character.Status.min_atk2_petpy += (short)Math.Min(partner.Status.min_atk2 * 0.019f * diff, 50);
                            Character.Status.min_atk3_petpy += (short)Math.Min(partner.Status.min_atk3 * 0.019f * diff, 50);
                            Character.Status.max_matk_petpy += (short)Math.Min(partner.Status.max_matk * 0.019f * diff, 40);
                            Character.Status.min_matk_petpy += (short)Math.Min(partner.Status.min_matk * 0.019f * diff, 40);
                            Character.Status.hit_melee_petpy += (short)Math.Min(partner.Status.hit_melee * 0.015f * diff, 12);
                            Character.Status.hit_ranged_petpy += (short)Math.Min(partner.Status.hit_ranged * 0.015f * diff, 13);
                            Character.Status.def_add_petpy += (short)Math.Min(partner.Status.def_add * 0.14f * diff, 7);
                            Character.Status.mdef_add_petpy += (short)Math.Min(partner.Status.mdef_add * 0.14f * diff, 7);
                            Character.Status.avoid_melee_petpy += (short)Math.Min(partner.Status.hit_melee * 0.012f * diff, 4);
                            Character.Status.avoid_ranged_petpy += (short)Math.Min(partner.Status.hit_ranged * 0.035f * diff, 5);
                            Character.Status.hp_petpy += (short)Math.Min(partner.MaxHP * 0.009f * diff, 450);
                            Character.Status.mp_petpy += (short)Math.Min(partner.MaxMP * 0.019f * diff, 87);
                            Character.Status.sp_petpy += (short)Math.Min(partner.MaxSP * 0.019f * diff, 66);
                        }
                        break;
                    case PossessionPosition.NECK:
                        if (Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.CHEST_ACCE))
                        {
                            Character.Status.max_atk1_petpy += (short)Math.Min(partner.Status.max_atk1 * 0.02f * diff, 55);
                            Character.Status.max_atk2_petpy += (short)Math.Min(partner.Status.max_atk2 * 0.02f * diff, 55);
                            Character.Status.max_atk3_petpy += (short)Math.Min(partner.Status.max_atk3 * 0.02f * diff, 55);
                            Character.Status.min_atk1_petpy += (short)Math.Min(partner.Status.min_atk1 * 0.02f * diff, 55);
                            Character.Status.min_atk2_petpy += (short)Math.Min(partner.Status.min_atk2 * 0.02f * diff, 55);
                            Character.Status.min_atk3_petpy += (short)Math.Min(partner.Status.min_atk3 * 0.02f * diff, 55);
                            Character.Status.max_matk_petpy += (short)Math.Min(partner.Status.max_matk * 0.02f * diff, 45);
                            Character.Status.min_matk_petpy += (short)Math.Min(partner.Status.min_matk * 0.02f * diff, 45);
                            Character.Status.hit_melee_petpy += (short)Math.Min(partner.Status.hit_melee * 0.022f * diff, 18);
                            Character.Status.hit_ranged_petpy += (short)Math.Min(partner.Status.hit_ranged * 0.022f * diff, 19);
                            Character.Status.def_add_petpy += (short)Math.Min(partner.Status.def_add * 0.8f * diff, 4);
                            Character.Status.mdef_add_petpy += (short)Math.Min(partner.Status.mdef_add * 0.9f * diff, 4);
                            Character.Status.avoid_melee_petpy += (short)Math.Min(partner.Status.hit_melee * 0.035f * diff, 6);
                            Character.Status.avoid_ranged_petpy += (short)Math.Min(partner.Status.hit_ranged * 0.018f * diff, 7);
                            Character.Status.hp_petpy += (short)Math.Min(partner.MaxHP * 0.009f * diff, 450);
                            Character.Status.mp_petpy += (short)Math.Min(partner.MaxMP * 0.019f * diff, 87);
                            Character.Status.sp_petpy += (short)Math.Min(partner.MaxSP * 0.019f * diff, 66);
                        }
                        break;
                    case PossessionPosition.CHEST:
                        if (Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.UPPER_BODY))
                        {
                            Character.Status.max_atk1_petpy += (short)Math.Min(partner.Status.max_atk1 * 0.012f * diff, 35);
                            Character.Status.max_atk2_petpy += (short)Math.Min(partner.Status.max_atk2 * 0.012f * diff, 35);
                            Character.Status.max_atk3_petpy += (short)Math.Min(partner.Status.max_atk3 * 0.012f * diff, 35);
                            Character.Status.min_atk1_petpy += (short)Math.Min(partner.Status.min_atk1 * 0.012f * diff, 35);
                            Character.Status.min_atk2_petpy += (short)Math.Min(partner.Status.min_atk2 * 0.012f * diff, 35);
                            Character.Status.min_atk3_petpy += (short)Math.Min(partner.Status.min_atk3 * 0.012f * diff, 35);
                            Character.Status.max_matk_petpy += (short)Math.Min(partner.Status.max_matk * 0.012f * diff, 30);
                            Character.Status.min_matk_petpy += (short)Math.Min(partner.Status.min_matk * 0.012f * diff, 30);
                            Character.Status.hit_melee_petpy += (short)Math.Min(partner.Status.hit_melee * 0.012f * diff, 9);
                            Character.Status.hit_ranged_petpy += (short)Math.Min(partner.Status.hit_ranged * 0.012f * diff, 10);
                            Character.Status.def_add_petpy += (short)Math.Min(partner.Status.def_add * 0.075f * diff, 3);
                            Character.Status.mdef_add_petpy += (short)Math.Min(partner.Status.mdef_add * 0.075f * diff, 3);
                            Character.Status.avoid_melee_petpy += (short)Math.Min(partner.Status.hit_melee * 0.021f * diff, 3);
                            Character.Status.avoid_ranged_petpy += (short)Math.Min(partner.Status.hit_ranged * 0.016f * diff, 4);
                            Character.Status.hp_petpy += (short)Math.Min(partner.MaxHP * 0.017f * diff, 850);
                            Character.Status.mp_petpy += (short)Math.Min(partner.MaxMP * 0.035f * diff, 161);
                            Character.Status.sp_petpy += (short)Math.Min(partner.MaxSP * 0.035f * diff, 122);
                        }
                        break;
                }
            }
        }

        //ECOKEY 讀取寵物PE
        public void SendPossessionPartner()
        {
            foreach (PossessionPosition i in this.Character.PossesionedPartner.Keys)
            {
                Packets.Server.SSMG_POSSESSION_PARTNER_RESULT PartnerPos = new Packets.Server.SSMG_POSSESSION_PARTNER_RESULT();
                switch (i)
                {
                    case PossessionPosition.RIGHT_HAND:
                        PartnerPos.InventorySlot = Character.PartnerSlotRightHand;
                        PartnerPos.Pos = PossessionPosition.RIGHT_HAND;
                        break;
                    case PossessionPosition.LEFT_HAND:
                        PartnerPos.InventorySlot = Character.PartnerSlotLeftHand;
                        PartnerPos.Pos = PossessionPosition.LEFT_HAND;
                        break;
                    case PossessionPosition.NECK:
                        PartnerPos.InventorySlot = Character.PartnerSlotAccesory;
                        PartnerPos.Pos = PossessionPosition.NECK;
                        break;
                    case PossessionPosition.CHEST:
                        PartnerPos.InventorySlot = Character.PartnerSlotClothes;
                        PartnerPos.Pos = PossessionPosition.CHEST;
                        break;
                }
                this.netIO.SendPacket(PartnerPos);
            }
            map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, Character, true);
        }

    }
}
