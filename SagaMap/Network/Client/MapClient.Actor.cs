using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using SagaDB;
using SagaDB.Item;
using SagaDB.Actor;
using SagaDB.DefWar;
using SagaDB.Map;
using SagaDB.Title;
using SagaDB.Skill;
using SagaLib;
using SagaMap;
using SagaMap.Manager;
using SagaMap.PC;
using SagaMap.Packets.Server;

namespace SagaMap.Network.Client
{
    public partial class MapClient
    {
        public DateTime moveStamp = DateTime.Now;
        public DateTime hpmpspStamp = DateTime.Now;
        public DateTime moveCheckStamp = DateTime.Now;

        //发送虚拟actor

        public void TitleProccess(ActorPC pc, uint ID, uint value)
        {
            if (CheckTitle((int)ID)) return;
            if (TitleFactory.Instance.Items.ContainsKey(ID))
            {
                Title t = TitleFactory.Instance.Items[ID];
                //應該逐項條件檢查 先放著
                /*
                string name = "称号" + ID.ToString() + "完成度";
                pc.AInt[name] += (int)value;
                if (pc.ALong[name] >= (long)t.PrerequisiteCount)
                    UnlockTitle(pc, ID);
                */
            }
        }

        public void UnlockTitle(ActorPC pc, uint ID)
        {
            if (TitleFactory.Instance.Items.ContainsKey(ID))
            {
                Title t = TitleFactory.Instance.Items[ID];
                string name = "称号" + ID.ToString() + "完成度";
                if (Character.ALong[name] >= (long)t.PrerequisiteCount)
                {
                    //應該逐項條件檢查 先放著
                    /*
                    if (!CheckTitle((int)ID))
                    {
                        SetTitle((int)ID, true);
                        SendSystemMessage("恭喜你！解锁了『" + t.name + "』称号！");
                        Skill.SkillHandler.Instance.ShowEffectOnActor(pc, 5420);
                    }
                    */
                }
            }
        }
        public void OnPlayerSetOption(Packets.Client.CSMG_PLAYER_SETOPTION p)
        {
            if (Character == null)
                return;


            //SendSystemMessage("OPTION Result:" +  (Packets.Server.SSMG_ACTOR_OPTION.Options)p.GetOption);
            //SendSystemMessage("PACKET: " + p.DumpData());
            //SendSystemMessage("-----------");

            var unk = (Packets.Server.SSMG_ACTOR_OPTION.Options)p.GetOption;

            foreach (var item in Enum.GetValues(typeof(Packets.Server.SSMG_ACTOR_OPTION.Options)).Cast<Enum>().Where(item => unk.HasFlag(item)))
            {
                switch ((Packets.Server.SSMG_ACTOR_OPTION.Options)item)
                {
                    case Packets.Server.SSMG_ACTOR_OPTION.Options.NONE:
                        ResetOption();
                        break;
                    case Packets.Server.SSMG_ACTOR_OPTION.Options.NO_TRADE:
                        Character.canTrade = false;
                        Character.CInt["canTrade"] = 0;
                        break;
                    case Packets.Server.SSMG_ACTOR_OPTION.Options.NO_EQUPMENT:
                        Character.canShowEquipment = false;
                        Character.CInt["canShowEquipment"] = 0;
                        break;
                    case Packets.Server.SSMG_ACTOR_OPTION.Options.NO_FRIEND:
                        Character.canFriend = false;
                        Character.CInt["canFriend"] = 0;
                        break;
                    case Packets.Server.SSMG_ACTOR_OPTION.Options.NO_BOND:
                        Character.canMentor = false;
                        Character.CInt["canMentor"] = 0;
                        break;
                    case Packets.Server.SSMG_ACTOR_OPTION.Options.NO_PARTY:
                        Character.canParty = false;
                        Character.CInt["canParty"] = 0;
                        break;
                    case Packets.Server.SSMG_ACTOR_OPTION.Options.NO_PATNER:
                        Character.canChangePartnerDisplay = false;
                        Character.CInt["canChangePartnerDisplay"] = 0;
                        break;
                    case Packets.Server.SSMG_ACTOR_OPTION.Options.NO_REVIVE_MESSAGE:
                        Character.showRevive = false;
                        Character.CInt["showRevive"] = 0;
                        break;
                    case Packets.Server.SSMG_ACTOR_OPTION.Options.NO_RING:
                        Character.canRing = false;
                        Character.CInt["canRing"] = 0;
                        break;
                    case Packets.Server.SSMG_ACTOR_OPTION.Options.NO_SKILL:
                        Character.canWork = false;
                        Character.CInt["canWork"] = 0;
                        break;
                    case Packets.Server.SSMG_ACTOR_OPTION.Options.NO_SPIRIT_POSSESSION:
                        Character.canPossession = false;
                        Character.CInt["canPossession"] = 0;
                        break;
                }
            }

        }
        public void ResetOption()
        {
            if (Character == null)
                return;

            Character.canTrade = true;
            Character.canParty = true;
            Character.canWork = true;
            Character.canRing = true;
            Character.canPossession = true;
            Character.canFriend = true;
            Character.canShowEquipment = true;
            Character.showRevive = true;
            Character.canMentor = true;
            Character.canChangePartnerDisplay = true;
            Character.CInt["canTrade"] = 1;
            Character.CInt["canParty"] = 1;
            Character.CInt["canPossession"] = 1;
            Character.CInt["canRing"] = 1;
            Character.CInt["showRevive"] = 1;
            Character.CInt["canWork"] = 1;
            Character.CInt["canMentor"] = 1;
            Character.CInt["canShowEquipment"] = 1;
            Character.CInt["canChangePartnerDisplay"] = 1;
            Character.CInt["canFriend"] = 1;

        }

        //ECOKEY 角色各項設定
        public void SetOption()
        {
            if (Character == null)
                return;

            Character.canTrade = Convert.ToBoolean(Character.CInt["canTrade"]);
            Character.canParty = Convert.ToBoolean(Character.CInt["canParty"]);
            Character.canWork = Convert.ToBoolean(Character.CInt["canWork"]);
            Character.canRing = Convert.ToBoolean(Character.CInt["canRing"]);
            Character.canPossession = Convert.ToBoolean(Character.CInt["canPossession"]);
            Character.canFriend = Convert.ToBoolean(Character.CInt["canFriend"]);
            Character.canShowEquipment = Convert.ToBoolean(Character.CInt["canShowEquipment"]);
            Character.showRevive = Convert.ToBoolean(Character.CInt["showRevive"]);
            Character.canMentor = Convert.ToBoolean(Character.CInt["canMentor"]);
            Character.canChangePartnerDisplay = Convert.ToBoolean(Character.CInt["canChangePartnerDisplay"]);
        }
        public void OnPlayerSetTitle(Packets.Client.CSMG_PLAYER_SETTITLE p)
        {
            if ((p.GetTSubID < 100000 || CheckTitle((int)p.GetTSubID)) && (p.GetTPredID < 100000 || CheckTitle((int)p.GetTPredID)) && (p.GetTBattleID < 100000 || CheckTitle((int)p.GetTBattleID)))
            {
                Character.AInt["称号_主语"] = (int)p.GetTSubID;
                Character.AInt["称号_连词"] = (int)p.GetTConjID;
                Character.AInt["称号_谓语"] = (int)p.GetTPredID;
                Character.AInt["称号_战斗"] = (int)p.GetTBattleID;
                StatusFactory.Instance.CalcStatus(Character);
                SendPCTitleInfo();
            }
            else
                SendSystemMessage("非法的称号ID");
        }

        public void OnPlayerOpenDailyStamp(Packets.Client.CSMG_DAILY_STAMP_OPEN p2)
        {
            this.SendNPCPlaySound(3501, 0, 100, 50);
            //Hide Daily Stamp Icon
            Packets.Server.SSMG_PLAYER_SHOW_DAILYSTAMP ds = new Packets.Server.SSMG_PLAYER_SHOW_DAILYSTAMP();
            ds.Type = 0;
            this.netIO.SendPacket(ds);

            int days = Character.AInt["每日盖章"];
            DateTime thisDay = DateTime.Today;
            if (Character.AInt["每日盖章"] == 10 && Character.AInt["每日盖章10印獎品"] == 0)
            {
                EventActivate(19230003);
                return;
            }
            if (Character.AStr["DailyStamp_DAY"] != thisDay.ToString("d"))
            {
                if (Character.AInt["每日盖章"] == 10)
                {
                    Character.AInt["每日盖章"] = 0;
                }

                Character.AStr["DailyStamp_DAY"] = thisDay.ToString("d");
                Character.AInt["每日盖章"] += 1;

                Packets.Server.SSMG_NPC_DAILY_STAMP p = new Packets.Server.SSMG_NPC_DAILY_STAMP();
                p.StampCount = (byte)Character.AInt["每日盖章"];
                p.Type = 2;
                netIO.SendPacket(p);
                //Normal Stamp
                //EventActivate(19230001);//每日送禮放在前面
                uint itemID = 0;
                switch (Character.AInt["每日盖章"])
                {
                    case 1:
                    case 2:
                    case 3:
                        switch (Global.Random.Next(1, 5))
                        {
                            case 1:
                                itemID = 20050073;
                                break;
                            case 2:
                                itemID = 10022900;
                                break;
                            case 3:
                                itemID = 20050056;
                                break;
                            case 4:
                                itemID = 16000300;
                                break;
                            case 5:
                                itemID = 16001120;
                                break;
                        }
                        break;
                    case 4:
                    case 5:
                        switch (Global.Random.Next(1, 10))
                        {
                            case 1:
                                itemID = 20050097;
                                break;
                            case 2:
                                itemID = 20050105;
                                break;
                            case 3:
                                itemID = 20050074;
                                break;
                            case 4:
                                itemID = 20050018;
                                break;
                            case 5:
                                itemID = 20050068;
                                break;
                            case 6:
                                itemID = 10000609;
                                break;
                            case 7:
                                itemID = 10000600; ;
                                break;
                            case 8:
                                itemID = 10000602;
                                break;
                            case 9:
                                itemID = 10034104;
                                break;
                            case 10:
                                itemID = 10014651;
                                break;
                        }
                        break;
                    case 6:
                    case 7:
                        switch (Global.Random.Next(1, 7))
                        {
                            case 1:
                                itemID = 16002210;
                                break;
                            case 2:
                                itemID = 16002310;
                                break;
                            case 3:
                                itemID = 16002410;
                                break;
                            case 4:
                                itemID = 90000043;
                                break;
                            case 5:
                                itemID = 90000044;
                                break;
                            case 6:
                                itemID = 90000045;
                                break;
                            case 7:
                                itemID = 90000046;
                                break;
                        }
                        break;
                    case 8:
                    case 9:
                        switch (Global.Random.Next(1, 6))
                        {
                            case 1:
                                itemID = 16001210;
                                break;
                            case 2:
                                itemID = 16006203;
                                break;
                            case 3:
                                itemID = 16000200;
                                break;
                            case 4:
                                itemID = 16000100;
                                break;
                            case 5:
                                itemID = 16001000;
                                break;
                            case 6:
                                itemID = 16002110;
                                break;
                        }
                        break;
                    case 10:
                        switch (Global.Random.Next(1, 4))
                        {
                            case 1:
                                itemID = 22000471;
                                break;
                            case 2:
                                itemID = 16000000;
                                break;
                            case 3:
                                itemID = 16013510;
                                break;
                            case 4:
                                itemID = 10048005;
                                break;
                        }
                        break;
                }

                Item item = ItemFactory.Instance.GetItem(itemID);
                item.Stack = 1;
                item.Identified = true;//免鉴定
                Logger.LogItemGet(Logger.EventType.ItemNPCGet, Character.Name + "(" + Character.CharID + ")", item.BaseData.name + "(" + item.ItemID + ")",
                        string.Format("ScriptGive Count:{0}", item.Stack), true);
                this.AddItem(item, true);

                if (Character.AInt["每日盖章"] == 5)
                {
                    EventActivate(19230002);
                    return;
                }

                if (Character.AInt["每日盖章"] == 10)
                {
                    Character.AInt["每日盖章10印獎品"] = 0;
                    //Character.AInt["每日盖章"] = 0;
                    EventActivate(19230003);
                    return;
                }
            }
            else
            {
                Packets.Server.SSMG_NPC_DAILY_STAMP p = new Packets.Server.SSMG_NPC_DAILY_STAMP();
                p.StampCount = (byte)Character.AInt["每日盖章"];
                p.Type = 1;
                netIO.SendPacket(p);
            }
            //EventActivate(19230001);
        }
        public void OnPlayerTitleRequire(Packets.Client.CSMG_PLAYER_TITLE_REQUIRE p)
        {
            if (p.tID == 9 && Character.Gold >= 10000000)
                TitleProccess(Character, 9, 1);

            /*
            p2.tID = p.tID;
            p2.mark = 1;
            if (Character.AInt["称号" + p.tID.ToString() + "完成度"] != 0)
                p2.task = (ulong)Character.AInt["称号" + p.tID.ToString() + "完成度"];
            else p2.task = 0;
            */
            uint id = p.ID;
            if (TitleFactory.Instance.Items.ContainsKey(id))
            {
                Packets.Server.SSMG_PLAYER_TITLE_REQ p2 = new Packets.Server.SSMG_PLAYER_TITLE_REQ();
                p2.tID = TitleFactory.Instance.Items[id].ID;
                p2.PutPrerequisite(TitleFactory.Instance.Items[id].Prerequisites.Values.ToList<ulong>());
                netIO.SendPacket(p2);
            }
        }

        uint masterpartner = 0;
        uint pupilinpartner = 0;

        public void OnBondRequestFromMaster(Packets.Client.CSMG_BOND_REQUEST_FROM_MASTER p)
        {
            MapClient pupilin = MapClientManager.Instance.FindClient(p.CharID);
            SendSystemMessage("师徒系统尚未实装。");

            int result = CheckMasterToPupilinInvitation(pupilin);
            Packets.Server.SSMG_BOND_INVITE_MASTER_RESULT p1 = new Packets.Server.SSMG_BOND_INVITE_MASTER_RESULT();
            p1.Result = result;
            this.netIO.SendPacket(p1);
        }

        public int CheckMasterToPupilinInvitation(MapClient pupilin)
        {
            if (pupilin == null)
                return -2; //相手が見つかりませんでした
            if (!pupilin.Character.canMentor)
                return -10; //相手の師弟関係設定が不許可になっています
            if (pupilin.trading || pupilin.scriptThread != null)
                return -9; //相手と師弟関係を結べる状態ではありません
            if (this.Character.Pupilins.Count >= this.Character.PupilinLimit)
                return -7; //これ以上弟子を取ることが出来ません
            if (pupilin.Character.Level >= 50 || pupilin.Character.JobLevel2T != 0 || pupilin.Character.JobLevel2X != 0 || pupilin.Character.Rebirth)
                return -6; //そのキャラクターは弟子になれません
            if (pupilin.masterpartner != 0)
                return -5; //他の人から招待を受けています
            if (this.Character.Pupilins.Contains(pupilin.Character.CharID))
                return -3; //既に師匠がいます
            try
            {
                Packets.Server.SSMG_BOND_INVITE_TO_PUPILIN p2 = new Packets.Server.SSMG_BOND_INVITE_TO_PUPILIN();
                p2.MasterID = this.Character.CharID;
                pupilin.netIO.SendPacket(p2);
                this.pupilinpartner = pupilin.Character.CharID;
                pupilin.masterpartner = this.Character.CharID;
            }
            catch
            {
                return -1; //何らかの原因で失敗しました
            }
            return -11; //申請中です
        }

        public void OnBondRequestFromPupilin(Packets.Client.CSMG_BOND_REQUEST_FROM_PUPILIN p)
        {
            MapClient master = MapClientManager.Instance.FindClient(p.CharID);
            SendSystemMessage("师徒系统尚未实装。");

            int result = CheckPupilinToMasterInvitation(master);
            Packets.Server.SSMG_BOND_INVITE_PUPILIN_RESULT p1 = new Packets.Server.SSMG_BOND_INVITE_PUPILIN_RESULT();
            p1.Result = result;
            this.netIO.SendPacket(p1);
        }

        public int CheckPupilinToMasterInvitation(MapClient master)
        {
            if (master == null)
                return -2; //相手が見つかりませんでした
            if (!master.Character.canMentor)
                return -10; //相手の師弟関係設定が不許可になっています
            if (master.trading || master.scriptThread != null)
                return -9; //相手と師弟関係を結べる状態ではありません
            if (this.Character.Master == master.Character.CharID)
                return -7; //既に師匠がいます
            if (master.Character.Level <= 55 || !master.Character.Rebirth)
                return -6; //そのキャラクターは師匠になれません
            if (master.pupilinpartner != 0)
                return -5; //他の人から招待を受けています
            if (this.Character.Master != 0)
                return -3; //既に違う人の弟子になっています
            try
            {
                Packets.Server.SSMG_BOND_INVITE_TO_MASTER p2 = new Packets.Server.SSMG_BOND_INVITE_TO_MASTER();
                p2.PupilinID = this.Character.CharID;
                master.netIO.SendPacket(p2);
                this.masterpartner = master.Character.CharID;
                master.pupilinpartner = this.Character.CharID;
            }
            catch
            {
                return -1; //何らかの原因で失敗しました
            }
            return -11; //申請中です
        }

        public void OnBondPupilinAnswer(Packets.Client.CSMG_BOND_REQUEST_PUPILIN_ANSWER p)
        {
            MapClient master = MapClientManager.Instance.FindClient(p.MasterCharID);
            int result = CheckPupilinToMasterAnswer(p.Rejected, master);
            Packets.Server.SSMG_BOND_INVITE_MASTER_RESULT p2 = new Packets.Server.SSMG_BOND_INVITE_MASTER_RESULT();
            p2.Result = result;
            master.netIO.SendPacket(p2);
        }

        public int CheckPupilinToMasterAnswer(bool rejected, MapClient master)
        {
            if (rejected)
                return -4; //拒否されました
            if (master == null)
                return -2; //相手が見つかりませんでした
            if (!master.Character.canMentor)
                return -10; //相手の師弟関係設定が不許可になっています
            if (master.trading || master.scriptThread != null)
                return -9; //相手と師弟関係を結べる状態ではありません
            if (this.Character.Master == master.Character.CharID)
                return -7; //既に師匠がいます
            if (master.Character.Level <= 55 || !master.Character.Rebirth)
                return -6; //そのキャラクターは師匠になれません
            if (master.pupilinpartner != 0)
                return -5; //他の人から招待を受けています
            if (this.Character.Master != 0)
                return -3; //既に違う人の弟子になっています
            try
            {
                this.Character.Master = master.Character.CharID;
                master.Character.Pupilins.Add(this.Character.CharID);
                this.masterpartner = 0;
                master.pupilinpartner = 0;
            }
            catch
            {
                return -1; //何らかの原因で失敗しました
            }
            return 0; //師匠になりました
        }

        public void OnBondMasterAnswer(Packets.Client.CSMG_BOND_REQUEST_MASTER_ANSWER p)
        {
            MapClient pupilin = MapClientManager.Instance.FindClient(p.PupilinCharID);
            int result = CheckMasterToPupilinAnswer(p.Rejected, pupilin);
            Packets.Server.SSMG_BOND_INVITE_MASTER_RESULT p2 = new Packets.Server.SSMG_BOND_INVITE_MASTER_RESULT();
            p2.Result = result;
            pupilin.netIO.SendPacket(p2);
        }

        public int CheckMasterToPupilinAnswer(bool rejected, MapClient pupilin)
        {
            if (rejected)
                return -4; //拒否されました
            if (pupilin == null)
                return -2; //相手が見つかりませんでした
            if (!pupilin.Character.canMentor)
                return -10; //相手の師弟関係設定が不許可になっています
            if (pupilin.trading || pupilin.scriptThread != null)
                return -9; //相手と師弟関係を結べる状態ではありません
            if (this.Character.Pupilins.Count >= this.Character.PupilinLimit)
                return -7; //これ以上弟子を取ることが出来ません
            if (pupilin.Character.Level <= 55 || !pupilin.Character.Rebirth)
                return -6; //そのキャラクターは師匠になれません
            if (pupilin.pupilinpartner != 0)
                return -5; //他の人から招待を受けています
            if (this.Character.Pupilins.Contains(pupilin.Character.CharID))
                return -3; //既に師匠がいます
            try
            {
                this.Character.Pupilins.Add(pupilin.Character.CharID);
                pupilin.Character.Master = this.Character.CharID;
                this.pupilinpartner = 0;
                pupilin.masterpartner = 0;
            }
            catch
            {
                return -1; //何らかの原因で失敗しました
            }
            return 0; //師匠になりました
        }

        public void OnBondBreak(Packets.Client.CSMG_BOND_CANCEL p)
        {
            MapClient target = MapClientManager.Instance.FindClient(p.TargetCharID);
            SendSystemMessage("師徒系統尚未實裝。");
            try
            {
                if (this.Character.Pupilins.Contains(target.Character.CharID))
                    this.Character.Pupilins.Remove(target.Character.CharID);
                if (this.Character.Master == target.Character.CharID)
                    this.Character.Master = 0;
                if (target.Character.Pupilins.Contains(this.Character.CharID))
                    target.Character.Pupilins.Remove(this.Character.CharID);
                if (target.Character.Master == this.Character.CharID)
                    target.Character.Master = 0;
            }
            catch
            {

            }
            Packets.Server.SSMG_BOND_BREAK_RESULT p1 = new Packets.Server.SSMG_BOND_BREAK_RESULT();
            this.netIO.SendPacket(p1);
            Packets.Server.SSMG_BOND_BREAK_RESULT p2 = new Packets.Server.SSMG_BOND_BREAK_RESULT();
            target.netIO.SendPacket(p2);
        }

        public void OnPlayerCancleTitleNew(Packets.Client.CSMG_PLAYER_TITLE_CANCLENEW p)
        {
            int index = (int)p.tID;
            byte page = 1;
            bool bounsflag = false;
            if (index > 64)
            {
                page += (byte)(index / 64);
                index -= 64 * (page - 1);
            }
            BitMask_Long value = new BitMask_Long();
            string name = "N称号记录" + page;
            if (Character.AStr[name] == "")
                value.Value = 0;
            else
            {
                value.Value = ulong.Parse(Character.AStr[name]);
                if (value.Test((ulong)Math.Pow(2, (index - 1))))
                    bounsflag = true;
                value.SetValueForNum(index, false);
            }
            Character.AStr[name] = value.Value.ToString();

            if (bounsflag)
            {
                Title t = TitleFactory.Instance.Items[p.tID];
                foreach (var item in t.Bonus.Keys)
                {
                    Item it = ItemFactory.Instance.GetItem(item);
                    it.Stack = t.Bonus[item];
                    AddItem(it, true);
                }
                if (t.Bonus.Count > 0)
                    SendSystemMessage("获得了称号『" + t.name + "』的奖励！");
                else
                    SendSystemMessage("称号『" + t.name + "』没有物品奖励。");
            }
        }

        public bool CheckTitle(int ID)
        {
            if (ID > 100000) return true;
            int index = ID;
            byte page = 1;
            if (index > 64)
            {
                page += (byte)(index / 64);
                index -= 64 * (page - 1);
            }
            BitMask_Long value = new BitMask_Long();
            string name = "称号记录" + page;
            if (Character.AStr[name] == "")
                value.Value = 0;
            else
                value.Value = ulong.Parse(Character.AStr[name]);
            ulong mark = (ulong)Math.Pow(2, (index - 1));
            return value.Test(mark);
        }
        public void SendPCTitleInfo()
        {
            if (Character == null)
                return;
            Packets.Server.SSMG_PLAYER_TITLE p = new Packets.Server.SSMG_PLAYER_TITLE();
            List<uint> titles = new List<uint>();
            titles.Add((uint)Character.AInt["称号_主语"]);
            titles.Add((uint)Character.AInt["称号_连词"]);
            titles.Add((uint)Character.AInt["称号_谓语"]);
            titles.Add((uint)Character.AInt["称号_战斗"]);

            netIO.SendPacket(p);
            StatusFactory.Instance.CalcStatus(Character);
        }
        public void SendTitleList()
        {
            if (Character == null)
                return;
            Packets.Server.SSMG_PLAYER_TITLE_LIST p = new Packets.Server.SSMG_PLAYER_TITLE_LIST();
            List<ulong> unknown1 = new List<ulong>();
            unknown1.Add(0);
            unknown1.Add(0);
            unknown1.Add(0);
            p.PutUnknown1(unknown1);

            List<ulong> unknown2 = new List<ulong>();
            unknown2.Add(0);
            unknown2.Add(0);
            unknown2.Add(0);
            p.PutUnknown2(unknown2);

            List<ulong> titles = new List<ulong>();
            if (Character.AStr["称号记录1"] != "")
                titles.Add(ulong.Parse(Character.AStr["称号记录1"]));
            else
                titles.Add(0);
            if (Character.AStr["称号记录2"] != "")
                titles.Add(ulong.Parse(Character.AStr["称号记录2"]));
            else
                titles.Add(0);
            if (Character.AStr["称号记录3"] != "")
                titles.Add(ulong.Parse(Character.AStr["称号记录3"]));
            else
                titles.Add(0);
            if (Character.AStr["称号记录4"] != "")
                titles.Add(ulong.Parse(Character.AStr["称号记录4"]));
            else
                titles.Add(0);
            if (Character.AStr["称号记录5"] != "")
                titles.Add(ulong.Parse(Character.AStr["称号记录5"]));
            else
                titles.Add(0);
            p.PutTitles(titles);

            List<ulong> ntitles = new List<ulong>();
            if (Character.AStr["N称号记录1"] != "")
                ntitles.Add(ulong.Parse(Character.AStr["N称号记录1"]));
            else
                ntitles.Add(0);
            if (Character.AStr["N称号记录2"] != "")
                ntitles.Add(ulong.Parse(Character.AStr["N称号记录2"]));
            else
                ntitles.Add(0);
            if (Character.AStr["N称号记录3"] != "")
                ntitles.Add(ulong.Parse(Character.AStr["N称号记录3"]));
            else
                ntitles.Add(0);
            if (Character.AStr["N称号记录4"] != "")
                ntitles.Add(ulong.Parse(Character.AStr["N称号记录4"]));
            else
                ntitles.Add(0);
            if (Character.AStr["N称号记录5"] != "")
                ntitles.Add(ulong.Parse(Character.AStr["N称号记录5"]));
            else
                ntitles.Add(0);
            p.PutTitles(ntitles);

            netIO.SendPacket(p);
        }

        public void SetTitle(int n, bool v)
        {
            int index = n;
            BitMask_Long value = new BitMask_Long();
            byte page = 1;
            if (index > 64)
            {
                page += (byte)(index / 64);
                index -= 64 * (page - 1);
            }
            string name = "称号记录" + page;
            if (Character.AStr[name] == "")
                value.Value = 0;
            else
                value.Value = ulong.Parse(Character.AStr[name]);
            value.SetValueForNum(index, v);
            Character.AStr[name] = value.Value.ToString();


            name = "N" + name;
            value = new BitMask_Long();
            if (Character.AStr[name] == "")
                value.Value = 0;
            else
                value.Value = ulong.Parse(Character.AStr[name]);
            value.SetValueForNum(index, v);
            Character.AStr[name] = value.Value.ToString();

            SendTitleList();
        }

        public void SendPetInfo()
        {
            if (Character.Partner == null)
                return;
            Partner.StatusFactory.Instance.CalcPartnerStatus(Character.Partner);
            SendPetDetailInfo();
            SendPetBasicInfo();
        }
        public void SendPetDetailInfo()
        {
            if (this.Character.Partner != null)
            {
                Packets.Server.SSMG_PARTNER_INFO_DETAIL p = new Packets.Server.SSMG_PARTNER_INFO_DETAIL();
                ActorPartner pet = this.Character.Partner;
                p.InventorySlot = this.Character.Inventory.Equipments[EnumEquipSlot.PET].Slot;
                p.MaxHP = pet.MaxHP;
                p.MaxMP = pet.MaxMP;
                p.MaxSP = pet.MaxSP;
                p.MoveSpeed = pet.Speed;
                p.MinPhyATK = pet.Status.min_atk1;
                p.MaxPhyATK = pet.Status.max_atk1;
                p.MinMAGATK = pet.Status.min_matk;
                p.MaxMAGATK = pet.Status.max_matk;
                p.DEF = pet.Status.def;
                p.DEFAdd = pet.Status.def_add;
                p.MDEF = pet.Status.mdef;
                p.MDEFAdd = pet.Status.mdef_add;
                p.ShortHit = pet.Status.hit_melee;
                p.LongHit = pet.Status.hit_ranged;
                p.ShortAvoid = pet.Status.avoid_melee;
                p.LongAvoid = pet.Status.avoid_ranged;
                p.ASPD = pet.Status.aspd;
                p.CSPD = pet.Status.cspd;
                this.netIO.SendPacket(p);
            }
        }
        public void SendPetBasicInfo()
        {
            if (this.Character.Partner != null)// || this.Character.Pet != null)//ECOKEY 機器人（修正這句）
            {
                Packets.Server.SSMG_PARTNER_INFO_BASIC p = new Packets.Server.SSMG_PARTNER_INFO_BASIC();
                ActorPartner pet = this.Character.Partner;
                p.InventorySlot = this.Character.Inventory.Equipments[EnumEquipSlot.PET].Slot;
                p.Level = pet.Level;

                //處理寵物經驗爆掉
                if (pet.exp >= ExperienceManager.Instance.GetExpForLevel(110, Scripting.LevelType.PLEVEL))
                    pet.exp = ExperienceManager.Instance.GetExpForLevel(110, Scripting.LevelType.PLEVEL) + 1;

                ulong bexp = ExperienceManager.Instance.GetExpForLevel(pet.Level, Scripting.LevelType.PLEVEL); //ECOKEY 寵物經驗更改
                ulong nextExp = ExperienceManager.Instance.GetExpForLevel((uint)(pet.Level + 1), Scripting.LevelType.PLEVEL);//ECOKEY 寵物經驗更改

                //uint cexp = (uint)((float)(pet.exp - bexp) / (nextExp - bexp) * 1000);
                //不知道為什麼上面的cexp怎麼算算出來都是0，把以下兩個數值另外拿出來計算就ok
                float A = pet.exp;
                float B = bexp;
                uint cexp2 = (uint)((float)(A - B) / (nextExp - bexp) * 1000f);
                //Logger.ShowInfo(pet.exp.ToString());
                //Logger.ShowInfo(bexp.ToString());
                //Logger.ShowInfo((pet.exp - bexp).ToString());
                //顯示經驗%數 cexp2通常是0~999
                p.EXPPercentage = (uint)cexp2;// p.EXPPercentage = ((uint)cexp >= 1000 ? 999 : (uint)cexp);
                p.Rebirth = 0;
                if (pet.rebirth)
                    p.Rebirth = 1;
                byte rank = (byte)(pet.rank);
                p.Rank = rank;
                p.ReliabilityColor = pet.reliability;
                p.ReliabilityUpRate = pet.reliabilityuprate;

                if (pet.nextfeedtime > DateTime.Now)
                    p.NextFeedTime = (uint)(pet.nextfeedtime - DateTime.Now).TotalSeconds;
                else
                    p.NextFeedTime = 0;

                p.AIMode = pet.ai_mode;
                //p.MaxNextFeedTime = no data

                //ECOKEY 自定義AI2開啟判斷
                if (pet.CustomAI2)
                {
                    p.CustomAISheet = 1;
                    //ECOKEY 自定義AI數量追加
                    if (pet.PlusAI == 0)
                    {
                        p.AICommandCount1 = 5;
                        p.AICommandCount2 = 5;
                    }
                    else if (pet.PlusAI == 1)
                    {
                        p.AICommandCount1 = 10;
                        p.AICommandCount2 = 5;
                    }
                    else if (pet.PlusAI == 2)
                    {
                        p.AICommandCount1 = 10;
                        p.AICommandCount2 = 10;
                    }
                }
                else
                {
                    p.CustomAISheet = 0;
                    //ECOKEY 自定義AI數量追加
                    if (pet.PlusAI >= 1)
                    {
                        p.AICommandCount1 = 10;
                    }
                    else
                    {
                        p.AICommandCount1 = 5;
                    }
                }

                p.PerkPoint = pet.perkpoint;
                //p.PerkListCount = no data
                p.Perk0 = pet.perk0;
                p.Perk1 = pet.perk1;
                p.Perk2 = pet.perk2;
                p.Perk3 = pet.perk3;
                p.Perk4 = pet.perk4;
                p.Perk5 = pet.perk5;
                if (pet.equipments.ContainsKey(SagaDB.Partner.EnumPartnerEquipSlot.WEAPON))
                    p.WeaponID = pet.equipments[SagaDB.Partner.EnumPartnerEquipSlot.WEAPON].ItemID;
                if (pet.equipments.ContainsKey(SagaDB.Partner.EnumPartnerEquipSlot.COSTUME))
                    p.ArmorID = pet.equipments[SagaDB.Partner.EnumPartnerEquipSlot.COSTUME].ItemID;
                this.netIO.SendPacket(p);
            }
        }

        public void OnMirrorOpenRequire(Packets.Client.CSMG_PLAYER_MIRROR_OPEN p)
        {
            Packets.Server.SSMG_PLAYER_MIRROR_WINDOW_OPEN p1 = new SSMG_PLAYER_MIRROR_WINDOW_OPEN();
            p1.SetFace = this.Character.MirrorFaceInfo;
            p1.SetHairStyle = this.Character.MirrorHairInfo;
            p1.SetWigStyle = this.Character.MirrorWigInfo;
            p1.SetHairLimit = this.Character.MirrorHairLimit;
            p1.SetHairColor = this.Character.MirrorHairColor;
            this.netIO.SendPacket(p1);
        }

        public void OnMirrorChangeSettingConfirm(Packets.Client.CSMG_PLAYER_MIRROR_CHANGE_SETTING_CONFIRM p)
        {
            var hairindex = p.HairStyleIndex;
            var haircolorindex = p.HairColorIndex;
            var faceindex = p.FaceIndex;

            SSMG_PLAYER_MIRROR_CHANGE_SETTING_RESULT p1 = new SSMG_PLAYER_MIRROR_CHANGE_SETTING_RESULT();

            if (this.currentEvent != null)
            {
                p1.Result = 0xFE;
                this.netIO.SendPacket(p1);
                return;
            }

            if (hairindex != 0xFF)
            {
                var nowhair = this.Character.HairStyle;
                this.Character.HairStyle = this.Character.MirrorHairInfo[hairindex];
                this.Character.MirrorHairInfo[hairindex] = nowhair;

                var nowig = this.Character.Wig;
                this.Character.Wig = this.Character.MirrorWigInfo[hairindex];
                this.Character.MirrorWigInfo[hairindex] = nowig;
            }

            if (haircolorindex != 0xFF)
            {
                var nowhc = this.Character.HairColor;
                this.Character.HairColor = this.Character.MirrorHairColor[haircolorindex];
                this.Character.MirrorHairColor[haircolorindex] = nowhc;
            }

            if (faceindex != 0xFF)
            {
                var nowface = this.Character.Face;
                this.Character.Face = this.Character.MirrorFaceInfo[faceindex];
                this.Character.MirrorFaceInfo[faceindex] = nowface;
            }

            p1.Result = 0;
            this.netIO.SendPacket(p1);

            SSMG_ACTOR_LOOK_UPDATE p2 = new SSMG_ACTOR_LOOK_UPDATE();
            p2.HairStyle = this.Character.HairStyle;
            p2.HairColor = this.Character.HairColor;
            p2.Wig = this.Character.Wig;
            p2.Face = this.Character.Face;
            this.netIO.SendPacket(p2);

            this.SendCharInfoUpdate();
        }

        public void OnMirrorAddFaceSlot(Packets.Client.CSMG_PLAYER_MIRROR_ADD_FACE_SLOT p)
        {
            SSMG_PLAYER_MIRROR_ADD_FACE_SLOT_RESULT p1 = new SSMG_PLAYER_MIRROR_ADD_FACE_SLOT_RESULT();

            if (this.currentEvent != null)
            {
                p1.Result = 0xFE;
                this.netIO.SendPacket(p1);
                return;
            }

            var item = this.Character.Inventory.GetItem(16017200, Inventory.SearchType.ITEM_ID);
            if (item == null || item.ItemID == 10000000)
            {
                p1.Result = 0xFC;
                this.netIO.SendPacket(p1);
                return;
            }

            var facelist = this.Character.MirrorFaceInfo;
            var idx = facelist.IndexOf(0xFFFF);
            if (idx == -1)
            {
                p1.Result = 0xFD;
                this.netIO.SendPacket(p1);
                return;
            }

            this.Character.MirrorFaceInfo[idx] = this.Character.BaseFace;

            DeleteItemID(16017200, 1, true);

            p1.Result = 0;
            this.netIO.SendPacket(p1);

            Packets.Client.CSMG_PLAYER_MIRROR_OPEN p3 = new Packets.Client.CSMG_PLAYER_MIRROR_OPEN();
            OnMirrorOpenRequire(p3);
        }

        public void OnMirrorAddHairSlot(Packets.Client.CSMG_PLAYER_MIRROR_ADD_HAIR_SLOT p)
        {
            SSMG_PLAYER_MIRROR_ADD_HAIR_SLOT_RESULT p1 = new SSMG_PLAYER_MIRROR_ADD_HAIR_SLOT_RESULT();

            if (this.currentEvent != null)
            {
                p1.Result = 0xFD;
                this.netIO.SendPacket(p1);
                return;
            }

            var item = this.Character.Inventory.GetItem(16004200, Inventory.SearchType.ITEM_ID);
            if (item == null || item.ItemID == 10000000)
            {
                p1.Result = 0xFC;
                this.netIO.SendPacket(p1);
                return;
            }

            var hairlist = this.Character.MirrorHairInfo;
            var idx = hairlist.IndexOf(0xFFFF);

            if (idx == -1)
            {
                p1.Result = 0xFD;
                this.netIO.SendPacket(p1);
                return;
            }

            if (this.Character.BaseHairStyle != 0)
            {
                this.Character.MirrorHairInfo[idx] = this.Character.BaseHairStyle;
                this.Character.MirrorWigInfo[idx] = this.Character.BaseWig;
            }
            else
            {
                this.Character.MirrorHairInfo[idx] = 7;
                this.Character.MirrorWigInfo[idx] = 0xFF;
            }


            this.Character.MirrorHairLimit[idx] = 0xFFFFFFFF;

            DeleteItemID(16004200, 1, true);

            p1.Result = 0;
            this.netIO.SendPacket(p1);

            Packets.Client.CSMG_PLAYER_MIRROR_OPEN p3 = new Packets.Client.CSMG_PLAYER_MIRROR_OPEN();
            OnMirrorOpenRequire(p3);
        }

        public void OnMirrorAddHairColorSlot(Packets.Client.CSMG_PLAYER_MIRROR_ADD_HAIR_COLOR_SLOT p)
        {
            SSMG_PLAYER_MIRROR_ADD_HAIR_COLOR_SLOT_RESULT p1 = new SSMG_PLAYER_MIRROR_ADD_HAIR_COLOR_SLOT_RESULT();

            if (this.currentEvent != null)
            {
                p1.Result = 0xFE;
                this.netIO.SendPacket(p1);
                return;
            }

            var item = this.Character.Inventory.GetItem(16004201, Inventory.SearchType.ITEM_ID);
            if (item == null || item.ItemID == 10000000)
            {
                p1.Result = 0xFC;
                this.netIO.SendPacket(p1);
                return;
            }

            var haircolorlist = this.Character.MirrorHairColor;
            var idx = haircolorlist.IndexOf(0xFF);

            if (idx == -1)
            {
                p1.Result = 0xFD;
                this.netIO.SendPacket(p1);
                return;
            }

            if (this.Character.BaseHairColor != 0)
                this.Character.MirrorHairColor[idx] = this.Character.BaseHairColor;
            else
            {
                byte race = (byte)this.Character.Race;
                if (race > 3)
                {
                    p1.Result = 0xFF;
                    this.netIO.SendPacket(p1);
                    return;
                }
                var hcs = new byte[4] { 50, 60, 70, 80 };
                this.Character.MirrorHairColor[idx] = hcs[race];
            }

            DeleteItemID(16004201, 1, true);

            p1.Result = 0;
            this.netIO.SendPacket(p1);

            Packets.Client.CSMG_PLAYER_MIRROR_OPEN p3 = new Packets.Client.CSMG_PLAYER_MIRROR_OPEN();
            OnMirrorOpenRequire(p3);
        }

        public void OnRequireRebirthReward(Packets.Client.CSMG_PLAYER_REQUIRE_REBIRTHREWARD p)
        {
            Packets.Server.SSMG_PLAYER_OPEN_REBIRTHREWARD_WINDOW p1 = new Packets.Server.SSMG_PLAYER_OPEN_REBIRTHREWARD_WINDOW();
            p1.SetOpen = 0x0A;
            this.netIO.SendPacket(p1);
        }

        public void OnCharFormChange(Packets.Client.CSMG_CHAR_FORM p)
        {
            this.Character.TailStyle = p.tailstyle;
            this.Character.WingStyle = p.wingstyle;
            this.Character.WingColor = p.wingcolor;
            this.Character.e.PropertyUpdate(UpdateEvent.CHAR_INFO, 0);
        }
        public void OnPlayerFaceView(Packets.Client.CSMG_ITEM_FACEVIEW p)
        {
            Packet p2 = new Packet(3);
            p2.ID = 0x1CF3;
            this.netIO.SendPacket(p2);
        }
        public void OnPlayerFaceChange(Packets.Client.CSMG_ITEM_FACECHANGE p)
        {
            uint itemID = this.Character.Inventory.GetItem(p.SlotID).ItemID;
            if (itemID == FaceFactory.Instance.Faces[p.FaceID])
            {
                this.DeleteItem(p.SlotID, 1, true);
                this.Character.Face = p.FaceID;
                this.SendPlayerInfo();
            }
        }

        public void OnHairChangeConfirm(Packets.Client.CSMG_NPC_HAIR_CHANGE_CONFIRM p)
        {
            var slotID = p.InventoryID;
            var hairID = p.HairStyle;
            var wigID = p.WigStyle;
            SSMG_NPC_CHANGE_HAIR_RESULT p1 = new SSMG_NPC_CHANGE_HAIR_RESULT();

            if (slotID == 0xFFFFFFFF && hairID == Convert.ToInt16("FFFF", 16) && wigID == Convert.ToInt16("FFFF", 16))
            {
                p1.Result = 0;
                p1.Type = -1;
                p1.ItemID = -1;
                p1.HairStyle = -1;
                p1.WigStyle = -1;
                this.netIO.SendPacket(p1);
                return;
            }

            var hairinfo = HairFactory.Instance.Hairs.First(x => x.HairStyle == hairID && x.HairWig == wigID);

            if (hairinfo == null)
            {
                p1.Result = -1;
                p1.Type = -1;
                p1.ItemID = -1;
                p1.HairStyle = -1;
                p1.WigStyle = -1;
                this.netIO.SendPacket(p1);
                return;
            }

            if (slotID != 0xFFFFFFFF)
            {
                var keyitem = this.Character.Inventory.GetItem(slotID, Inventory.SearchType.SLOT_ID);
                if (keyitem == null || keyitem.ItemID == 10000000)
                {
                    p1.Result = -1;
                    p1.Type = -1;
                    p1.ItemID = -1;
                    p1.HairStyle = -1;
                    p1.WigStyle = -1;
                    this.netIO.SendPacket(p1);
                    return;
                }
                this.DeleteItem(slotID, 1, true);
            }
            this.Character.HairStyle = (ushort)hairID;
            this.Character.Wig = (ushort)wigID;
            this.SendPlayerInfo();

            p1.Result = 0;
            p1.Type = 0;
            p1.ItemID = (int)hairinfo.ItemID;
            p1.HairStyle = hairID;
            p1.WigStyle = wigID;
            this.netIO.SendPacket(p1);
        }

        /*public void SendNaviList(Packets.Client.CSMG_NAVI_OPEN p)
        {
            Packets.Server.SSMG_NAVI_LIST p1 = new Packets.Server.SSMG_NAVI_LIST();
            p1.CategoryId = p.CategoryId;
            p1.Count = 18;
            p1.Navi = this.Character.Navi;
            this.netIO.SendPacket(p1);
        }*/

        public void SendRingFF()
        {
            MapServer.charDB.GetFF(this.Character);
            if (this.Character.Ring != null)
            {
                if (this.Character.Ring.FFarden != null)
                {
                    this.SendRingFFObtainMode();
                    this.SendRingFFHealthMode();
                    this.SendRingFFIsLock();
                    this.SendRingFFName();
                    this.SendRingFFMaterialPoint();
                    this.SendRingFFMaterialConsume();
                    this.SendRingFFLevel();
                    this.SendRingFFNextFeeTime();
                }
            }
        }
        void SendRingFFObtainMode()
        {
            Packets.Server.SSMG_FF_OBTAIN_MODE p = new SagaMap.Packets.Server.SSMG_FF_OBTAIN_MODE();
            p.value = this.Character.Ring.FFarden.ObMode;
            this.netIO.SendPacket(p);
        }
        void SendRingFFHealthMode()
        {
            Packets.Server.SSMG_FF_HEALTH_MODE p = new Packets.Server.SSMG_FF_HEALTH_MODE();
            p.value = this.Character.Ring.FFarden.HealthMode;
            this.netIO.SendPacket(p);
        }
        void SendRingFFIsLock()
        {
            Packets.Server.SSMG_FF_ISLOCK p = new Packets.Server.SSMG_FF_ISLOCK();
            if (this.Character.Ring.FFarden.IsLock)
                p.value = 1;
            else
                p.value = 0;
            this.netIO.SendPacket(p);
        }
        void SendRingFFName()
        {
            Packets.Server.SSMG_FF_RINGSELF p = new Packets.Server.SSMG_FF_RINGSELF();
            p.name = this.Character.Ring.FFarden.Name;
            this.netIO.SendPacket(p);
        }
        void SendRingFFMaterialPoint()
        {
            Packets.Server.SSMG_FF_MATERIAL_POINT p = new Packets.Server.SSMG_FF_MATERIAL_POINT();
            p.value = this.Character.Ring.FFarden.MaterialPoint;
            this.netIO.SendPacket(p);
        }
        void SendRingFFMaterialConsume()
        {
            Packets.Server.SSMG_FF_MATERIAL_CONSUME p = new Packets.Server.SSMG_FF_MATERIAL_CONSUME();
            p.value = this.Character.Ring.FFarden.MaterialConsume;
            this.netIO.SendPacket(p);
        }
        void SendRingFFLevel()
        {
            Packets.Server.SSMG_FF_LEVEL p = new Packets.Server.SSMG_FF_LEVEL();
            p.level = this.Character.Ring.FFarden.Level;
            p.value = this.Character.Ring.FFarden.FFexp;
            this.netIO.SendPacket(p);
            Packets.Server.SSMG_FF_F_LEVEL p1 = new Packets.Server.SSMG_FF_F_LEVEL();
            p1.level = this.Character.Ring.FFarden.FLevel;
            p1.value = this.Character.Ring.FFarden.FFFexp;
            this.netIO.SendPacket(p1);
            Packets.Server.SSMG_FF_SU_LEVEL p2 = new Packets.Server.SSMG_FF_SU_LEVEL();
            p2.level = this.Character.Ring.FFarden.SULevel;
            p2.value = this.Character.Ring.FFarden.FFSUexp;
            this.netIO.SendPacket(p2);
            Packets.Server.SSMG_FF_BP_LEVEL p3 = new Packets.Server.SSMG_FF_BP_LEVEL();
            p3.level = this.Character.Ring.FFarden.BPLevel;
            p3.value = this.Character.Ring.FFarden.FFBPexp;
            this.netIO.SendPacket(p3);
            Packets.Server.SSMG_FF_DEM_LEVEL p4 = new Packets.Server.SSMG_FF_DEM_LEVEL();
            p4.level = this.Character.Ring.FFarden.DEMLevel;
            p4.value = this.Character.Ring.FFarden.FFDEMexp;
            this.netIO.SendPacket(p4);
        }
        void SendRingFFNextFeeTime()
        {
            Packets.Server.SSMG_FF_NEXTFEE_DATE p = new Packets.Server.SSMG_FF_NEXTFEE_DATE();
            p.UpdateTime = DateTime.Now;
            this.netIO.SendPacket(p);
        }


        void SendEffect(uint effect)
        {
            EffectArg arg = new EffectArg();
            arg.actorID = this.Character.ActorID;
            arg.effectID = effect;
            this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.SHOW_EFFECT, arg, this.chara, true);
        }

        public void ResetStatusPoint()
        {
            SagaLogin.Configurations.StartupSetting setting = Configuration.Instance.StartupSetting[this.Character.Race];
            this.Character.StatsPoint += PC.StatusFactory.Instance.GetTotalBonusPointForStats(setting.Str, this.Character.Str);
            this.Character.StatsPoint += PC.StatusFactory.Instance.GetTotalBonusPointForStats(setting.Dex, this.Character.Dex);
            this.Character.StatsPoint += PC.StatusFactory.Instance.GetTotalBonusPointForStats(setting.Int, this.Character.Int);
            this.Character.StatsPoint += PC.StatusFactory.Instance.GetTotalBonusPointForStats(setting.Vit, this.Character.Vit);
            this.Character.StatsPoint += PC.StatusFactory.Instance.GetTotalBonusPointForStats(setting.Agi, this.Character.Agi);
            this.Character.StatsPoint += PC.StatusFactory.Instance.GetTotalBonusPointForStats(setting.Mag, this.Character.Mag);

            this.Character.Str = setting.Str;
            this.Character.Dex = setting.Dex;
            this.Character.Int = setting.Int;
            this.Character.Vit = setting.Vit;
            this.Character.Agi = setting.Agi;
            this.Character.Mag = setting.Mag;

            PC.StatusFactory.Instance.CalcStatus(this.Character);
            SendPlayerInfo();
        }

        public void SendRange()
        {

            Packets.Server.SSMG_ITEM_EQUIP p = new SagaMap.Packets.Server.SSMG_ITEM_EQUIP();
            p.InventorySlot = 0xFFFFFFFF;
            p.Target = ContainerType.NONE;
            p.Result = 1;
            p.Range = this.chara.Range;
            this.netIO.SendPacket(p);
        }

        public void SendActorID()
        {
            Packets.Server.SSMG_ACTOR_SPEED p2 = new SagaMap.Packets.Server.SSMG_ACTOR_SPEED();
            p2.ActorID = this.Character.ActorID;
            p2.Speed = this.Character.Speed;
            //p2.Speed = 96;
            this.netIO.SendPacket(p2);
        }

        public void SendStamp()
        {
            Packets.Server.SSMG_STAMP_INFO p1 = new SagaMap.Packets.Server.SSMG_STAMP_INFO();
            p1.Page = 0;
            p1.Stamp = this.Character.Stamp;
            this.netIO.SendPacket(p1);
            Packets.Server.SSMG_STAMP_INFO p2 = new SagaMap.Packets.Server.SSMG_STAMP_INFO();
            p2.Page = 1;
            p2.Stamp = this.Character.Stamp;
            this.netIO.SendPacket(p2);
        }

        public void SendActorMode()
        {
            this.Character.e.OnPlayerMode(this.Character);
        }

        public void SendCharOption()
        {
            int sum = 0;
            if (Character.CInt["canTrade"] == 0) sum += 1;
            if (Character.CInt["canParty"] == 0) sum += 2;
            if (Character.CInt["canPossession"] == 0) sum += 4;
            if (Character.CInt["canRing"] == 0) sum += 8;
            if (Character.CInt["showRevive"] == 0) sum += 16;
            if (Character.CInt["canWork"] == 0) sum += 32;
            if (Character.CInt["canMentor"] == 0) sum += 256;
            if (Character.CInt["showEquipment"] == 0) sum += 512;
            if (Character.CInt["canChangePartnerDisplay"] == 0) sum += 1024;
            if (Character.CInt["canFriend"] == 0) sum += 2048;

            if (sum == 0)
            {
                Packets.Server.SSMG_ACTOR_OPTION p4 = new SagaMap.Packets.Server.SSMG_ACTOR_OPTION();
                p4.Option = SagaMap.Packets.Server.SSMG_ACTOR_OPTION.Options.NONE;
                this.netIO.SendPacket(p4);
            }
            else
            {

                Packets.Server.SSMG_ACTOR_OPTION p4 = new SagaMap.Packets.Server.SSMG_ACTOR_OPTION();
                p4.RawOption = sum;
                this.netIO.SendPacket(p4);
            }
        }

        public void SendCharInfo()
        {
            if (this.Character.Online)
            {
                Skill.SkillHandler.Instance.CastPassiveSkills(this.Character);

                SendAttackType();
                Packets.Server.SSMG_PLAYER_INFO p1 = new SagaMap.Packets.Server.SSMG_PLAYER_INFO();
                p1.Player = this.Character;
                this.netIO.SendPacket(p1);

                SendPlayerInfo();
            }
        }

        public void SendPlayerInfo()
        {
            if (this.Character.Online)
            {
                SendPlayerStatsBreak(this.Character);
                SendGoldUpdate();
                SendActorHPMPSP(this.Character);
                SendStatus();
                SendRange();
                SendStatusExtend();
                SendCapacity();
                //SendMaxCapacity();
                SendPlayerJob();
                SendSkillList();
                SendAnotherSkillList();
                SendPlayerLevel();
                SendEXP();
                SendActorMode();
                SendCL();
                SendMotionList();
                SendPlayerEXPoints(this.Character);

                SendPlayerDualJobInfo();
                SendPlayerDualJobSkillList();
            }
        }

        private void SendPlayerStatsBreak(ActorPC actor)
        {
            Packets.Server.SSMG_PLAYER_STATS_BREAK p = new Packets.Server.SSMG_PLAYER_STATS_BREAK();
            p.STATS = (byte)(StatsBreakType.Str | StatsBreakType.Agi | StatsBreakType.Vit | StatsBreakType.Int | StatsBreakType.Dex | StatsBreakType.Mag);
            this.netIO.SendPacket(p);
        }

        private void SendPlayerEXPoints(ActorPC actor)
        {
            Packets.Server.SSMG_PLAYER_EXPOINT p = new Packets.Server.SSMG_PLAYER_EXPOINT();
            p.EXStatPoint = actor.EXStatPoint;
            p.CanUseStatPoint = (ushort)(actor.StatsPoint);
            p.EXSkillPoint = actor.EXSkillPoint;
            this.netIO.SendPacket(p);
        }

        public void SendMotionList()
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_CHAT_EXPRESSION_UNLOCK p = new SagaMap.Packets.Server.SSMG_CHAT_EXPRESSION_UNLOCK();
                p.unlock = 0xffffffff;
                this.netIO.SendPacket(p);
                Packets.Server.SSMG_CHAT_EXEMOTION_UNLOCK p2 = new SagaMap.Packets.Server.SSMG_CHAT_EXEMOTION_UNLOCK();
                p2.List1 = 0xffffffff;
                p2.List2 = 0xffffffff;
                p2.List3 = 0xffffffff;
                p2.List4 = 0xffffffff;
                p2.List5 = 0xffffffff;
                this.netIO.SendPacket(p2);
            }
        }
        public void SendAttackType()
        {
            if (this.Character.Online)
            {
                //去掉攻击类型消息显示
                Dictionary<EnumEquipSlot, Item> equips;
                if (this.chara.Form == DEM_FORM.NORMAL_FORM)
                    equips = this.chara.Inventory.Equipments;
                else
                    equips = this.chara.Inventory.Parts;
                if (equips.ContainsKey(EnumEquipSlot.RIGHT_HAND) || (equips.ContainsKey(EnumEquipSlot.LEFT_HAND) && equips[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.BOW))
                {
                    Item item = new Item();
                    if ((equips.ContainsKey(EnumEquipSlot.LEFT_HAND) && equips[EnumEquipSlot.LEFT_HAND].BaseData.itemType == ItemType.BOW))
                        item = equips[EnumEquipSlot.LEFT_HAND];
                    else
                        item = equips[EnumEquipSlot.RIGHT_HAND];
                    this.Character.Status.attackType = item.AttackType;
                    //switch (item.AttackType)
                    //{
                    //    case ATTACK_TYPE.BLOW:
                    //        SendSystemMessage(SagaMap.Packets.Server.SSMG_SYSTEM_MESSAGE.Messages.GAME_SMSG_RECV_POSTUREBLOW_TEXT);
                    //        break;
                    //    case ATTACK_TYPE.STAB:
                    //        SendSystemMessage(SagaMap.Packets.Server.SSMG_SYSTEM_MESSAGE.Messages.GAME_SMSG_RECV_POSTURESTAB_TEXT);
                    //        break;
                    //    case ATTACK_TYPE.SLASH:
                    //        SendSystemMessage(SagaMap.Packets.Server.SSMG_SYSTEM_MESSAGE.Messages.GAME_SMSG_RECV_POSTURESLASH_TEXT);
                    //        break;
                    //    default:
                    //        SendSystemMessage(SagaMap.Packets.Server.SSMG_SYSTEM_MESSAGE.Messages.GAME_SMSG_RECV_POSTUREERROR_TEXT);
                    //        break;
                    //}
                }
                else
                {
                    this.Character.Status.attackType = ATTACK_TYPE.BLOW;
                    //SendSystemMessage(SagaMap.Packets.Server.SSMG_SYSTEM_MESSAGE.Messages.GAME_SMSG_RECV_POSTUREBLOW_TEXT);
                }
                map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.ATTACK_TYPE_CHANGE, null, this.Character, true);
                小丑專用判斷();//ECOKEY 小丑J50專用判斷
            }
        }
        void 小丑專用判斷()//ECOKEY 小丑J50專用判斷
        {
            //2560空手
            //2561一般武器 鞭子（片手剣;　片手斧;　片手槌;　細剣;　爪;　鞭）
            //2562遠距離
            //2563雙手系列
            //2564法系
            //1048576 空手
            //2013265920 雙手系列
            if (this.Character.Job == PC_JOB.JOKER && this.Character.Skills3.ContainsKey(2566))
            {
                byte lv = this.Character.Skills3[2566].Level;
                if (this.Character.Inventory.Equipments.ContainsKey(EnumEquipSlot.RIGHT_HAND))
                {
                    uint skillid = 2560;
                    switch (this.Character.Inventory.Equipments[EnumEquipSlot.RIGHT_HAND].BaseData.itemType)
                    {
                        case ItemType.HANDBAG:
                        case ItemType.RAPIER:
                        case ItemType.SHORT_SWORD:
                        case ItemType.ROPE:
                        case ItemType.CLAW:
                        case ItemType.ETC_WEAPON:
                            skillid = 2561;
                            break;
                        case ItemType.HAMMER:
                        case ItemType.AXE:
                        case ItemType.SPEAR:
                        case ItemType.SWORD:
                            if (this.Character.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.RIGHT_HAND) && this.Character.Inventory.Equipments.ContainsKey(SagaDB.Item.EnumEquipSlot.LEFT_HAND))
                            {
                                if (this.Character.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.RIGHT_HAND].BaseData.itemType ==
                                    this.Character.Inventory.Equipments[SagaDB.Item.EnumEquipSlot.LEFT_HAND].BaseData.itemType)
                                {
                                    skillid = 2563;
                                }
                                else
                                {
                                    skillid = 2561;
                                }
                            }
                            else
                            {
                                skillid = 2561;
                            }
                            break;
                        case ItemType.CARD:
                        case ItemType.THROW:
                        case ItemType.RIFLE:
                        case ItemType.BOW:
                        case ItemType.DUALGUN:
                        case ItemType.GUN:
                            skillid = 2562;
                            break;
                        case ItemType.STRINGS:
                        case ItemType.BOOK:
                        case ItemType.STAFF:
                            skillid = 2564;
                            break;
                        default:
                            skillid = 2560;
                            break;
                    }
                    SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(skillid, lv);
                    this.Character.Skills3[2566] = skill;
                }
                else
                {
                    SagaDB.Skill.Skill skill = SkillFactory.Instance.GetSkill(2560, lv);
                    this.Character.Skills3[2566] = skill;
                }
                SendSkillList();
            }

        }

        public void SendStatus()
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_PLAYER_STATUS p = new SagaMap.Packets.Server.SSMG_PLAYER_STATUS();
                if (this.chara.Form == DEM_FORM.MACHINA_FORM || this.chara.Race != PC_RACE.DEM)
                {
                    p.AgiBase = (ushort)(this.Character.Agi + this.chara.Status.m_agi_chip);
                    p.AgiRevide = (short)(this.Character.Status.agi_rev + this.Character.Status.agi_item + this.Character.Status.agi_mario + this.Character.Status.agi_skill + this.Character.Status.agi_iris);
                    p.AgiBonus = StatusFactory.Instance.RequiredBonusPoint(this.Character.Agi);
                    p.DexBase = (ushort)(this.Character.Dex + this.chara.Status.m_dex_chip);
                    p.DexRevide = (short)(this.Character.Status.dex_rev + this.Character.Status.dex_item + this.Character.Status.dex_mario + this.Character.Status.dex_skill + this.Character.Status.dex_iris);
                    p.DexBonus = StatusFactory.Instance.RequiredBonusPoint(this.Character.Dex);
                    p.IntBase = (ushort)(this.Character.Int + this.chara.Status.m_int_chip);
                    p.IntRevide = (short)(this.Character.Status.int_rev + this.Character.Status.int_item + this.Character.Status.int_mario + this.Character.Status.int_skill + this.Character.Status.int_iris);
                    p.IntBonus = StatusFactory.Instance.RequiredBonusPoint(this.Character.Int);
                    p.VitBase = (ushort)(this.Character.Vit + this.chara.Status.m_vit_chip);
                    p.VitRevide = (short)(this.Character.Status.vit_rev + this.Character.Status.vit_item + this.Character.Status.vit_mario + this.Character.Status.vit_skill + this.Character.Status.vit_iris);
                    p.VitBonus = StatusFactory.Instance.RequiredBonusPoint(this.Character.Vit);
                    p.StrBase = (ushort)(this.Character.Str + this.chara.Status.m_str_chip);
                    p.StrRevide = (short)(this.Character.Status.str_rev + this.Character.Status.str_item + this.Character.Status.str_mario + this.Character.Status.str_skill + this.Character.Status.str_iris);
                    p.StrBonus = StatusFactory.Instance.RequiredBonusPoint(this.Character.Str);
                    p.MagBase = (ushort)(this.Character.Mag + this.chara.Status.m_mag_chip);
                    p.MagRevide = (short)(this.Character.Status.mag_rev + this.Character.Status.mag_item + this.Character.Status.mag_mario + this.Character.Status.mag_skill + this.Character.Status.mag_iris);
                    p.MagBonus = StatusFactory.Instance.RequiredBonusPoint(this.Character.Mag);
                    this.netIO.SendPacket(p);
                }
                else
                {
                    p.AgiBase = (ushort)(this.Character.Agi + this.chara.Status.m_agi_chip);
                    p.AgiRevide = (short)(this.Character.Status.agi_rev - this.chara.Status.m_agi_chip + this.Character.Status.agi_item + this.Character.Status.agi_mario + this.Character.Status.agi_skill + this.Character.Status.agi_iris);
                    p.AgiBonus = StatusFactory.Instance.RequiredBonusPoint(this.Character.Agi);
                    p.DexBase = (ushort)(this.Character.Dex + this.chara.Status.m_dex_chip);
                    p.DexRevide = (short)(this.Character.Status.dex_rev - this.chara.Status.m_dex_chip + this.Character.Status.dex_item + this.Character.Status.dex_mario + this.Character.Status.dex_skill + this.Character.Status.dex_iris);
                    p.DexBonus = StatusFactory.Instance.RequiredBonusPoint(this.Character.Dex);
                    p.IntBase = (ushort)(this.Character.Int + this.chara.Status.m_int_chip);
                    p.IntRevide = (short)(this.Character.Status.int_rev - this.chara.Status.m_int_chip + this.Character.Status.int_item + this.Character.Status.int_mario + this.Character.Status.int_skill + this.Character.Status.int_iris);
                    p.IntBonus = StatusFactory.Instance.RequiredBonusPoint(this.Character.Int);
                    p.VitBase = (ushort)(this.Character.Vit + this.chara.Status.m_vit_chip);
                    p.VitRevide = (short)(this.Character.Status.vit_rev - this.chara.Status.m_vit_chip + this.Character.Status.vit_item + this.Character.Status.vit_mario + this.Character.Status.vit_skill + this.Character.Status.vit_iris);
                    p.VitBonus = StatusFactory.Instance.RequiredBonusPoint(this.Character.Vit);
                    p.StrBase = (ushort)(this.Character.Str + this.chara.Status.m_str_chip);
                    p.StrRevide = (short)(this.Character.Status.str_rev - this.chara.Status.m_str_chip + this.Character.Status.str_item + this.Character.Status.str_mario + this.Character.Status.str_skill + this.Character.Status.str_iris);
                    p.StrBonus = StatusFactory.Instance.RequiredBonusPoint(this.Character.Str);
                    p.MagBase = (ushort)(this.Character.Mag + this.chara.Status.m_mag_chip);
                    p.MagRevide = (short)(this.Character.Status.mag_rev - this.chara.Status.m_mag_chip + this.Character.Status.mag_item + this.Character.Status.mag_mario + this.Character.Status.mag_skill + this.Character.Status.mag_iris);
                    p.MagBonus = StatusFactory.Instance.RequiredBonusPoint(this.Character.Mag);

                    this.netIO.SendPacket(p);
                }
            }
        }

        public void SendStatusExtend()
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_PLAYER_STATUS_EXTEND p = new SagaMap.Packets.Server.SSMG_PLAYER_STATUS_EXTEND();

                switch (this.chara.Status.attackType)
                {
                    case ATTACK_TYPE.BLOW:
                        p.ATK1Max = this.Character.Status.max_atk1;
                        p.ATK1Min = this.Character.Status.min_atk1;
                        break;
                    case ATTACK_TYPE.SLASH:
                        p.ATK1Max = this.Character.Status.max_atk2;
                        p.ATK1Min = this.Character.Status.min_atk2;
                        break;
                    case ATTACK_TYPE.STAB:
                        p.ATK1Max = this.Character.Status.max_atk3;
                        p.ATK1Min = this.Character.Status.min_atk3;
                        break;
                }
                p.ATK2Max = this.Character.Status.max_atk2;
                p.ATK2Min = this.Character.Status.min_atk2;
                p.ATK3Max = this.Character.Status.max_atk3;
                p.ATK3Min = this.Character.Status.min_atk3;
                p.MATKMax = this.Character.Status.max_matk;
                p.MATKMin = this.Character.Status.min_matk;

                p.ASPD = (short)(this.Character.Status.aspd);// + this.Character.Status.aspd_skill);
                p.CSPD = (short)(this.Character.Status.cspd);// + this.Character.Status.cspd_skill);

                p.AvoidCritical = this.Character.Status.avoid_critical;
                p.AvoidMagic = this.Character.Status.avoid_magic;
                p.AvoidMelee = this.Character.Status.avoid_melee;
                p.AvoidRanged = this.Character.Status.avoid_ranged;

                p.DefAddition = (ushort)this.Character.Status.def_add;
                p.DefBase = this.Character.Status.def;
                p.MDefAddition = (ushort)this.Character.Status.mdef_add;
                p.MDefBase = this.Character.Status.mdef;

                p.HitCritical = this.Character.Status.hit_critical;
                p.HitMagic = this.Character.Status.hit_magic;
                p.HitMelee = this.Character.Status.hit_melee;
                p.HitRanged = this.Character.Status.hit_ranged;

                p.Speed = this.Character.Speed;

                this.netIO.SendPacket(p);
            }
        }

        public void SendCapacity()
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_PLAYER_CAPACITY p = new SagaMap.Packets.Server.SSMG_PLAYER_CAPACITY();
                /*p.CapacityBack = this.Character.Inventory.Volume[ContainerType.BACK_BAG];
                p.CapacityBody = this.Character.Inventory.Volume[ContainerType.BODY];
                p.CapacityLeft = this.Character.Inventory.Volume[ContainerType.LEFT_BAG];
                p.CapacityRight = this.Character.Inventory.Volume[ContainerType.RIGHT_BAG];
                p.PayloadBack = this.Character.Inventory.Payload[ContainerType.BACK_BAG];
                p.PayloadBody = this.Character.Inventory.Payload[ContainerType.BODY];
                p.PayloadLeft = this.Character.Inventory.Payload[ContainerType.LEFT_BAG];
                p.PayloadRight = this.Character.Inventory.Payload[ContainerType.RIGHT_BAG];*/
                p.Payload = this.Character.Inventory.Payload[ContainerType.BODY];
                p.Volume = this.Character.Inventory.Volume[ContainerType.BODY];
                p.MaxPayload = this.Character.Inventory.MaxPayload[ContainerType.BODY];
                p.MaxVolume = this.Character.Inventory.MaxVolume[ContainerType.BODY];
                this.netIO.SendPacket(p);
            }
        }

        public void SendMaxCapacity()
        {
            /*if (this.Character.Online)
            {
                Packets.Server.SSMG_PLAYER_MAX_CAPACITY p = new SagaMap.Packets.Server.SSMG_PLAYER_MAX_CAPACITY();
                /*p.CapacityBack = this.Character.Inventory.MaxVolume[ContainerType.BACK_BAG];
                p.CapacityBody = this.Character.Inventory.MaxVolume[ContainerType.BODY];
                p.CapacityLeft = this.Character.Inventory.MaxVolume[ContainerType.LEFT_BAG];
                p.CapacityRight = this.Character.Inventory.MaxVolume[ContainerType.RIGHT_BAG];
                p.PayloadBack = this.Character.Inventory.MaxPayload[ContainerType.BACK_BAG];
                p.PayloadBody = this.Character.Inventory.MaxPayload[ContainerType.BODY];
                p.PayloadLeft = this.Character.Inventory.MaxPayload[ContainerType.LEFT_BAG];
                p.PayloadRight = this.Character.Inventory.MaxPayload[ContainerType.RIGHT_BAG];
                p.Payload = this.Character.Inventory.MaxPayload[ContainerType.BODY];
                p.Volume = this.Character.Inventory.MaxVolume[ContainerType.BODY]; 
                this.netIO.SendPacket(p);
            }*/
        }

        public void SendChangeMap()
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_PLAYER_CHANGE_MAP p = new SagaMap.Packets.Server.SSMG_PLAYER_CHANGE_MAP();
                if (this.map.returnori)
                    p.MapID = this.map.OriID;
                else
                    p.MapID = this.Character.MapID;
                p.X = Global.PosX16to8(this.Character.X, this.map.Width);
                p.Y = Global.PosY16to8(this.Character.Y, this.map.Height);
                p.Dir = (byte)(this.Character.Dir / 45);
                if (this.map.IsDungeon)
                {
                    p.DungeonDir = this.map.DungeonMap.Dir;
                    p.DungeonX = this.map.DungeonMap.X;
                    p.DungeonY = this.map.DungeonMap.Y;
                    this.Character.Speed = Configuration.Instance.Speed;
                }
                if (this.fgTakeOff)
                {
                    p.FGTakeOff = fgTakeOff;
                    fgTakeOff = false;
                }
                else
                {
                    //this.Character.Speed = Configuration.Instance.Speed;
                }
                this.netIO.SendPacket(p);
            }
        }

        public void SendGotoFG()
        {
            if (this.Character.Online)
            {
                Map fgMap = MapManager.Instance.GetMap(this.Character.MapID);
                if (!fgMap.IsMapInstance)
                {
                    Logger.ShowDebug(string.Format("MapID:{0} isn't a valid flying garden!"), Logger.defaultlogger);
                }
                ActorPC owner = fgMap.Creator;
                Packets.Server.SSMG_PLAYER_GOTO_FG p = new SagaMap.Packets.Server.SSMG_PLAYER_GOTO_FG();
                p.MapID = this.Character.MapID;
                p.X = Global.PosX16to8(this.Character.X, this.map.Width);
                p.Y = Global.PosY16to8(this.Character.Y, this.map.Height);
                p.Dir = (byte)(this.Character.Dir / 45);
                p.Equiptments = owner.FGarden.FGardenEquipments;
                this.netIO.SendPacket(p);
            }
        }

        public void SendGotoFF()
        {
            if (this.Character.Online)
            {
                Map fgMap = MapManager.Instance.GetMap(this.Character.MapID);
                if (fgMap.ID == 90001999)//铲除一个神经病逻辑
                {
                    CustomMapManager.Instance.SendGotoSerFFMap(this);
                    return;
                }
                if (!fgMap.IsMapInstance)
                {
                    Logger.ShowDebug(string.Format("MapID:{0} isn't a valid flying garden!", fgMap.ID), Logger.defaultlogger);
                }
                ActorPC owner = fgMap.Creator;
                Packets.Server.SSMG_FF_ENTER p = new Packets.Server.SSMG_FF_ENTER();
                p.MapID = this.Character.MapID;
                p.X = Global.PosX16to8(this.Character.X, this.map.Width);
                p.Y = Global.PosY16to8(this.Character.Y, this.map.Height);
                p.Dir = (byte)(this.Character.Dir / 45);
                p.RingID = this.Character.Ring.ID;
                p.RingHouseID = 30250000;
                this.netIO.SendPacket(p);
            }
        }

        public void SendDungeonEvent()
        {
            if (this.Character.Online)
            {
                if (!this.map.IsMapInstance || !this.map.IsDungeon)
                    return;
                foreach (Dungeon.GateType i in this.map.DungeonMap.Gates.Keys)
                {
                    if (this.map.DungeonMap.Gates[i].NPCID != 0)
                    {
                        Packets.Server.SSMG_NPC_SHOW p = new SagaMap.Packets.Server.SSMG_NPC_SHOW();
                        p.NPCID = this.map.DungeonMap.Gates[i].NPCID;
                        this.netIO.SendPacket(p);
                    }
                    if (this.map.DungeonMap.Gates[i].ConnectedMap != null)
                    {
                        if (i != SagaMap.Dungeon.GateType.Central && i != SagaMap.Dungeon.GateType.Exit)
                        {
                            Packets.Server.SSMG_NPC_SET_EVENT_AREA p1 = new SagaMap.Packets.Server.SSMG_NPC_SET_EVENT_AREA();
                            p1.StartX = this.map.DungeonMap.Gates[i].X;
                            p1.EndX = this.map.DungeonMap.Gates[i].X;
                            p1.StartY = this.map.DungeonMap.Gates[i].Y;
                            p1.EndY = this.map.DungeonMap.Gates[i].Y;

                            switch (i)
                            {
                                case SagaMap.Dungeon.GateType.North:
                                    p1.EventID = 12001501;
                                    break;
                                case SagaMap.Dungeon.GateType.East:
                                    p1.EventID = 12001502;
                                    break;
                                case SagaMap.Dungeon.GateType.South:
                                    p1.EventID = 12001503;
                                    break;
                                case SagaMap.Dungeon.GateType.West:
                                    p1.EventID = 12001504;
                                    break;

                            }
                            switch (this.map.DungeonMap.Gates[i].Direction)
                            {
                                case SagaMap.Dungeon.Direction.In:
                                    p1.EffectID = 9002;
                                    break;
                                case SagaMap.Dungeon.Direction.Out:
                                    p1.EffectID = 9005;
                                    break;
                            }
                            this.netIO.SendPacket(p1);
                        }
                        else
                        {
                            Packets.Server.SSMG_NPC_SET_EVENT_AREA p1 = new SagaMap.Packets.Server.SSMG_NPC_SET_EVENT_AREA();
                            p1.StartX = this.map.DungeonMap.Gates[i].X;
                            p1.EndX = this.map.DungeonMap.Gates[i].X;
                            p1.StartY = this.map.DungeonMap.Gates[i].Y;
                            p1.EndY = this.map.DungeonMap.Gates[i].Y;
                            p1.EventID = 12001505;
                            p1.EffectID = 9005;
                            this.netIO.SendPacket(p1);
                        }

                        if (this.map.DungeonMap.Gates[i].NPCID != 0)
                        {
                            Packets.Server.SSMG_CHAT_MOTION p = new SagaMap.Packets.Server.SSMG_CHAT_MOTION();
                            p.ActorID = this.map.DungeonMap.Gates[i].NPCID;
                            p.Motion = (MotionType)621;
                            this.netIO.SendPacket(p);
                        }
                    }
                    else
                    {
                        if (i == SagaMap.Dungeon.GateType.Entrance)
                        {
                            Packets.Server.SSMG_NPC_SET_EVENT_AREA p1 = new SagaMap.Packets.Server.SSMG_NPC_SET_EVENT_AREA();
                            p1.StartX = this.map.DungeonMap.Gates[i].X;
                            p1.EndX = this.map.DungeonMap.Gates[i].X;
                            p1.StartY = this.map.DungeonMap.Gates[i].Y;
                            p1.EndY = this.map.DungeonMap.Gates[i].Y;
                            p1.EventID = 12001505;
                            p1.EffectID = 9003;
                            this.netIO.SendPacket(p1);
                        }
                    }
                }

            }
        }

        public void SendFGEvent()
        {
            if (this.Character.Online)
            {
                Map fgMap = MapManager.Instance.GetMap(this.Character.MapID);
                if (!fgMap.IsMapInstance)
                    return;
                if ((this.map.ID / 10) == 7000000)
                {
                    if (this.map.Creator.FGarden.Sound != 0) //ECOKEY 飛空庭音樂撥放
                    {
                        MapClient.FromActorPC((ActorPC)this.Character).SendChangeBGM(this.map.Creator.FGarden.Sound, 1, 100, 50);
                    }
                    if (this.map.Creator.FGarden.FGardenEquipments[SagaDB.FGarden.FGardenSlot.GARDEN_MODELHOUSE] != 0)
                    {
                        Packets.Server.SSMG_NPC_SET_EVENT_AREA p1 = new SagaMap.Packets.Server.SSMG_NPC_SET_EVENT_AREA();
                        p1.EventID = 10000315;
                        p1.StartX = 6;
                        p1.StartY = 7;
                        p1.EndX = 6;
                        p1.EndY = 7;
                        this.netIO.SendPacket(p1);
                    }
                    //ECOKEY 重新讀取飛空庭裝備（主要應對有染色家具）
                    foreach (SagaDB.FGarden.FGardenSlot i in this.map.Creator.FGarden.FGardenEquipments.Keys)
                    {
                        if (this.map.Creator.FGarden.FGardenEquipments[i] != 0 && this.map.Creator.AInt[i.ToString()] != 0)
                        {
                            Packets.Server.SSMG_FG_EQUIPT p1 = new SagaMap.Packets.Server.SSMG_FG_EQUIPT();
                            p1.ItemID = this.map.Creator.FGarden.FGardenEquipments[i];
                            p1.Place = i;
                            p1.Color = (byte)this.map.Creator.AInt[i.ToString()];//ECOKEY 染色新增
                            this.netIO.SendPacket(p1);
                        }
                    }
                    if (this.map.Creator.AInt["FGarden_Heavenly"] != 0)//飛空天空
                    {
                        Packets.Server.SSMG_FG_CHANGE_SKY p = new SagaMap.Packets.Server.SSMG_FG_CHANGE_SKY();
                        p.Sky = (byte)this.map.Creator.AInt["FGarden_Heavenly"];
                        MapClient.FromActorPC(this.Character).netIO.SendPacket(p);
                    }
                    if (this.map.Creator.AInt["FGarden_Weather"] != 0)//飛空天氣
                    {
                        Packets.Server.SSMG_FG_CHANGE_WEATHER p = new SagaMap.Packets.Server.SSMG_FG_CHANGE_WEATHER();
                        p.Weather = (byte)this.map.Creator.AInt["FGarden_Weather"];
                        MapClient.FromActorPC(this.Character).netIO.SendPacket(p);
                    }
                }
                if ((this.map.ID / 10) == 7500000)
                {
                    if (this.map.Creator.FGarden.Sound != 0) //ECOKEY 飛空庭音樂撥放
                    {
                        MapClient.FromActorPC((ActorPC)this.Character).SendChangeBGM(this.map.Creator.FGarden.Sound, 1, 100, 50);
                    }
                    //ECOKEY 重新讀取飛空庭裝備（主要應對有染色家具）
                    foreach (SagaDB.FGarden.FGardenSlot i in this.map.Creator.FGarden.FGardenEquipments.Keys)
                    {
                        if (this.map.Creator.FGarden.FGardenEquipments[i] != 0 && this.map.Creator.AInt[i.ToString()] != 0)
                        {
                            Packets.Server.SSMG_FG_EQUIPT p1 = new SagaMap.Packets.Server.SSMG_FG_EQUIPT();
                            p1.ItemID = this.map.Creator.FGarden.FGardenEquipments[i];
                            p1.Place = i;
                            p1.Color = (byte)this.map.Creator.AInt[i.ToString()];//ECOKEY 染色新增
                            this.netIO.SendPacket(p1);
                        }
                    }
                }
            }
        }

        public void SendGoldUpdate()
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_PLAYER_GOLD_UPDATE p = new SagaMap.Packets.Server.SSMG_PLAYER_GOLD_UPDATE();
                p.Gold = this.Character.Gold;
                this.netIO.SendPacket(p);
            }

        }
        public void SendActorHPMPSP(Actor actor)
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_PLAYER_MAX_HPMPSP p = new SagaMap.Packets.Server.SSMG_PLAYER_MAX_HPMPSP();
                p.ActorID = actor.ActorID;
                p.MaxHP = actor.MaxHP;
                p.MaxMP = actor.MaxMP;
                p.MaxSP = actor.MaxSP;
                p.MaxEP = actor.MaxEP;
                this.netIO.SendPacket(p);
                Packets.Server.SSMG_PLAYER_HPMPSP p10 = new SagaMap.Packets.Server.SSMG_PLAYER_HPMPSP();
                p10.ActorID = actor.ActorID;
                p10.HP = actor.HP;
                p10.MP = actor.MP;
                p10.SP = actor.SP;
                p10.EP = actor.EP;
                this.netIO.SendPacket(p10);
                if (actor == this.Character)
                {

                    if (this.Character.Party != null)
                    {
                        PartyManager.Instance.UpdateMemberHPMPSP(this.Character.Party, this.Character);
                    }

                    //if ((DateTime.Now - hpmpspStamp).TotalSeconds >= 2)
                    //{
                    //    hpmpspStamp = DateTime.Now;
                    //}
                }
            }
        }

        public void SendCharXY()
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_ACTOR_MOVE p = new SagaMap.Packets.Server.SSMG_ACTOR_MOVE();
                p.ActorID = this.Character.ActorID;
                p.Dir = this.Character.Dir;
                p.X = this.Character.X;
                p.Y = this.Character.Y;
                p.MoveType = MoveType.WARP;
                this.netIO.SendPacket(p);
            }

        }

        public void SendPlayerLevel()
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_PLAYER_LEVEL p = new SagaMap.Packets.Server.SSMG_PLAYER_LEVEL();
                p.Level = this.Character.Level;
                p.JobLevel = this.Character.JobLevel1;
                p.JobLevel2T = this.Character.JobLevel2T;
                p.JobLevel2X = this.Character.JobLevel2X;
                p.JobLevel3 = this.Character.JobLevel3;
                p.IsDualJob = this.Character.JointJobLevel;//ECOKEY 馴獸師等級顯示
                //p.IsDualJob = (this.Character.DualJobID != 0) ? (byte)1 : (byte)0;
                //if (p.IsDualJob == 0x1)
                if (this.Character.DualJobID != 0)//ECOKEY 隨著上面那些的更改，這邊也要改
                    p.DualjobLevel = this.Character.PlayerDualJobList[this.Character.DualJobID].DualJobLevel;

                p.UseableStatPoint = (ushort)(this.Character.StatsPoint);
                p.SkillPoint = this.Character.SkillPoint;
                p.Skill2XPoint = this.Character.SkillPoint2X;
                p.Skill2TPoint = this.Character.SkillPoint2T;
                p.Skill3Point = this.Character.SkillPoint3;
                this.netIO.SendPacket(p);
            }
        }

        public void SendPlayerJob()
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_PLAYER_JOB p = new SagaMap.Packets.Server.SSMG_PLAYER_JOB();
                p.Job = this.Character.Job;
                if (this.Character.JobJoint != PC_JOB.NONE)
                    p.JointJob = this.Character.JobJoint;
                if (this.Character.DualJobID != 0)
                    p.DualJob = this.Character.DualJobID;
                this.netIO.SendPacket(p);
            }
        }


        public void SendCharInfoUpdate()
        {
            if (this.Character.Online)
            {
                this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, this.Character, true);
            }
        }

        public void SendAnnounce(string text)
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_CHAT_PUBLIC p11 = new SagaMap.Packets.Server.SSMG_CHAT_PUBLIC();
                p11.ActorID = 0;
                p11.Message = text;
                this.netIO.SendPacket(p11);
            }
        }

        public void SendPkMode()
        {
            if (this.Character.Online)
            {
                this.Character.Mode = PlayerMode.COLISEUM_MODE;
                this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.PLAYER_MODE, null, this.Character, true);
            }
        }

        public void SendNormalMode()
        {
            if (this.Character.Online)
            {
                this.Character.Mode = PlayerMode.NORMAL;
                this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.PLAYER_MODE, null, this.Character, true);
            }
        }

        public void SendPlayerSizeUpdate()
        {
            if (this.Character.Online)
            {
                this.map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.PLAYER_SIZE_UPDATE, null, this.Character, true);
            }

        }

        public void OnMove(Packets.Client.CSMG_PLAYER_MOVE p)
        {
            if (this.Character.Online)
            {
                if (this.state != SESSION_STATE.LOADED)
                    return;
                switch (p.MoveType)
                {
                    case MoveType.RUN:
                        this.Map.MoveActor(Map.MOVE_TYPE.START, this.Character, new short[2] { p.X, p.Y }, p.Dir, this.Character.Speed);
                        moveCheckStamp = DateTime.Now;
                        break;
                    case MoveType.CHANGE_DIR:
                        this.Map.MoveActor(Map.MOVE_TYPE.STOP, this.Character, new short[2] { p.X, p.Y }, p.Dir, this.Character.Speed);
                        break;
                    case MoveType.WALK:
                        this.Map.MoveActor(Map.MOVE_TYPE.START, this.Character, new short[2] { p.X, p.Y }, p.Dir, this.Character.Speed, false, MoveType.WALK);
                        moveCheckStamp = DateTime.Now;
                        break;
                }
                if (this.Character.CInt["NextMoveEventID"] != 0)
                {
                    this.EventActivate((uint)this.Character.CInt["NextMoveEventID"]);
                    this.Character.CInt["NextMoveEventID"] = 0;
                    this.Character.CInt.Remove("NextMoveEventID");
                }
                if (this.Character.TTime["特殊刀攻击间隔"] != DateTime.Now)
                    this.Character.TTime["特殊刀攻击间隔"] = DateTime.Now;
            }
        }

        public void SendActorSpeed(Actor actor, ushort speed)
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_ACTOR_SPEED p = new SagaMap.Packets.Server.SSMG_ACTOR_SPEED();
                p.ActorID = actor.ActorID;
                p.Speed = speed;
                this.netIO.SendPacket(p);
            }
        }

        public void SendEXP()
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_PLAYER_EXP p = new SagaMap.Packets.Server.SSMG_PLAYER_EXP();
                ulong cexp, jexp;
                ulong bexp = 0, nextExp = 0;
                if (!this.Character.Rebirth || this.Character.Job != this.Character.Job3)
                {
                    bexp = ExperienceManager.Instance.GetExpForLevel(this.Character.Level, Scripting.LevelType.CLEVEL);
                    nextExp = ExperienceManager.Instance.GetExpForLevel((uint)(this.Character.Level + 1), Scripting.LevelType.CLEVEL);
                }
                else
                {
                    bexp = ExperienceManager.Instance.GetExpForLevel(this.Character.Level, Scripting.LevelType.CLEVEL2);
                    nextExp = ExperienceManager.Instance.GetExpForLevel((uint)(this.Character.Level + 1), Scripting.LevelType.CLEVEL2);
                }
                cexp = (uint)((float)(this.Character.CEXP - bexp) / (nextExp - bexp) * 1000);
                if (this.Character.JobJoint == PC_JOB.NONE)
                {
                    if (this.Character.DualJobID != 0)
                    {
                        bexp = ExperienceManager.Instance.GetExpForLevel(this.Character.PlayerDualJobList[this.Character.DualJobID].DualJobLevel, Scripting.LevelType.DUALJ);
                        nextExp = ExperienceManager.Instance.GetExpForLevel((uint)(this.Character.PlayerDualJobList[this.Character.DualJobID].DualJobLevel + 1), Scripting.LevelType.DUALJ);
                    }
                    else if (this.Character.Job == this.Character.JobBasic)
                    {
                        bexp = ExperienceManager.Instance.GetExpForLevel(this.Character.JobLevel1, Scripting.LevelType.JLEVEL);
                        nextExp = ExperienceManager.Instance.GetExpForLevel((uint)(this.Character.JobLevel1 + 1), Scripting.LevelType.JLEVEL);
                    }
                    else if (this.Character.Job == this.Character.Job2X)
                    {
                        bexp = ExperienceManager.Instance.GetExpForLevel(this.Character.JobLevel2X, Scripting.LevelType.JLEVEL2);
                        nextExp = ExperienceManager.Instance.GetExpForLevel((uint)(this.Character.JobLevel2X + 1), Scripting.LevelType.JLEVEL2);
                    }
                    else if (this.Character.Job == this.Character.Job2T)
                    {
                        bexp = ExperienceManager.Instance.GetExpForLevel(this.Character.JobLevel2T, Scripting.LevelType.JLEVEL2);
                        nextExp = ExperienceManager.Instance.GetExpForLevel((uint)(this.Character.JobLevel2T + 1), Scripting.LevelType.JLEVEL2);
                    }
                    else
                    {
                        bexp = ExperienceManager.Instance.GetExpForLevel(this.Character.JobLevel3, Scripting.LevelType.JLEVEL3);
                        nextExp = ExperienceManager.Instance.GetExpForLevel((uint)(this.Character.JobLevel3 + 1), Scripting.LevelType.JLEVEL3);
                    }
                    if (this.Character.DualJobID != 0)
                        jexp = (uint)((float)(this.Character.PlayerDualJobList[this.Character.DualJobID].DualJobExp - bexp) / (nextExp - bexp) * 1000);
                    else
                        jexp = (uint)((float)(this.Character.JEXP - bexp) / (nextExp - bexp) * 1000);
                }
                else
                {
                    bexp = ExperienceManager.Instance.GetExpForLevel(this.Character.JointJobLevel, Scripting.LevelType.JLEVEL2);
                    nextExp = ExperienceManager.Instance.GetExpForLevel((uint)(this.Character.JointJobLevel + 1), Scripting.LevelType.JLEVEL2);

                    jexp = (uint)((float)(this.Character.JointJEXP - bexp) / (nextExp - bexp) * 1000);
                }
                p.EXPPercentage = ((uint)cexp >= 1000 ? 999 : (uint)cexp);
                p.JEXPPercentage = ((uint)jexp >= 1000 ? 999 : (uint)jexp);
                p.WRP = this.Character.WRP;
                p.ECoin = this.Character.ECoin;
                if (map.Info.Flag.Test(MapFlags.Dominion))
                {
                    p.Exp = (uint)this.Character.DominionCEXP;
                    p.JExp = (uint)this.Character.DominionJEXP;
                }
                else
                {
                    p.Exp = (long)this.Character.CEXP;
                    if (this.Character.DualJobID != 0)
                        p.JExp = (long)this.Character.PlayerDualJobList[this.Character.DualJobID].DualJobExp;
                    else
                        p.JExp = (long)this.Character.JEXP;
                }
                //ECOKEY DEM修復
                if (this.Character.Race == PC_RACE.DEM)
                {
                    Packets.Server.SSMG_PLAYER_EXP_DEM p2 = new SagaMap.Packets.Server.SSMG_PLAYER_EXP_DEM();
                    p2.EXPPercentage = (uint)this.Character.CEXP;
                    p2.JEXPPercentage = (uint)this.Character.JEXP;
                    p2.WRP = this.Character.WRP;
                    p2.ECoin = this.Character.ECoin;
                    p2.Exp = (long)this.Character.CEXP;
                    p2.JExp = (long)this.Character.JEXP;
                    this.netIO.SendPacket(p2);
                    return;
                }
                this.netIO.SendPacket(p);
            }
        }

        public void SendEXPMessage(long exp, long jexp, long pexp, SagaMap.Packets.Server.SSMG_PLAYER_EXP_MESSAGE.EXP_MESSAGE_TYPE type)
        {
            SagaMap.Packets.Server.SSMG_PLAYER_EXP_MESSAGE p = new SagaMap.Packets.Server.SSMG_PLAYER_EXP_MESSAGE();
            p.EXP = exp;
            p.JEXP = jexp;
            p.PEXP = pexp;
            p.Type = type;
            this.netIO.SendPacket(p);
        }

        public void SendLvUP(Actor pc, byte type)
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_ACTOR_LEVEL_UP p = new SagaMap.Packets.Server.SSMG_ACTOR_LEVEL_UP();
                p.ActorID = pc.ActorID;
                p.Level = pc.Level;
                p.LvType = type;

                this.netIO.SendPacket(p);
            }
        }

        public void OnPlayerGreetings(Packets.Client.CSMG_PLAYER_GREETINGS p)
        {
            if (chara.TTime["打招呼时间"] + new TimeSpan(0, 0, 10) > DateTime.Now)
            {
                SendSystemMessage("不可以頻繁打招呼哦。");
                return;
            }
            //ECOKEY 騎士團禁止打招呼
            if (chara.Mode == PlayerMode.KNIGHT_EAST ||
                chara.Mode == PlayerMode.KNIGHT_WEST ||
                chara.Mode == PlayerMode.KNIGHT_SOUTH ||
                chara.Mode == PlayerMode.KNIGHT_NORTH)
            {
                SendSystemMessage("騎士團中禁止打招呼。");
                return;
            }
            this.BingoCheck(6, 1);//BINGO任務6
            Actor actor = map.GetActor(p.ActorID);
            if (actor.Buff.FishingState)
            {
                SendSystemMessage("对方正在钓鱼，不要打扰人家哦。");
                return;
            }
            if (actor != null)
            {
                if (actor.type == ActorType.PC)
                {
                    ActorPC target = (ActorPC)actor;
                    if (target.Online && this.chara.Online)
                    {
                        ushort dir = map.CalcDir(chara.X, chara.Y, target.X, target.Y);
                        ushort dir2 = map.CalcDir(target.X, target.Y, chara.X, chara.Y);

                        short[] ys = new short[2];
                        ys[0] = chara.X;
                        ys[1] = chara.Y;
                        map.MoveActor(Map.MOVE_TYPE.START, chara, ys, dir, 500, true, MoveType.CHANGE_DIR);
                        if (chara.Partner != null)
                        {
                            ys = new short[2];
                            ys[0] = chara.Partner.X;
                            ys[1] = chara.Partner.Y;
                            map.MoveActor(Map.MOVE_TYPE.START, chara.Partner, ys, dir, 500, true, MoveType.CHANGE_DIR);
                        }
                        ys = new short[2];
                        ys[0] = target.X;
                        ys[1] = target.Y;
                        map.MoveActor(Map.MOVE_TYPE.START, target, ys, dir2, 500, true, MoveType.CHANGE_DIR);
                        if (target.Partner != null)
                        {
                            ys = new short[2];
                            ys[0] = target.Partner.X;
                            ys[1] = target.Partner.Y;
                            map.MoveActor(Map.MOVE_TYPE.START, target.Partner, ys, dir, 500, true, MoveType.CHANGE_DIR);
                        }

                        ushort motionid = 163;
                        byte loop = 0;
                        switch (Global.Random.Next(0, 31))
                        {
                            case 0:
                                motionid = 113;
                                loop = 1;
                                break;
                            case 1:
                                motionid = 163;
                                break;
                            case 2:
                                motionid = 509;
                                break;
                            case 3:
                                motionid = 159;
                                break;
                            case 4:
                                motionid = 210;
                                break;
                            case 5:
                                motionid = 509;
                                break;
                            case 6:
                                motionid = 300;
                                break;
                            case 7:
                                motionid = 2035;
                                break;
                            case 8:
                                motionid = 2040;
                                break;
                            case 9:
                                motionid = 1520;
                                break;
                            case 10:
                                motionid = 1521;
                                break;
                            case 11:
                                motionid = 2020;
                                break;
                            case 12:
                                motionid = 2020;
                                break;
                            case 13:
                                motionid = 2064;
                                break;
                            case 14:
                                motionid = 2065;
                                break;
                            case 15:
                                motionid = 2066;
                                break;
                            case 16:
                                motionid = 2067;
                                break;
                            case 17:
                                motionid = 2069;
                                break;
                            case 18:
                                motionid = 2070;
                                break;
                            case 19:
                                motionid = 1524;
                                break;
                            case 20:
                                motionid = 2084;
                                break;
                            case 21:
                                motionid = 2095;
                                break;
                            case 22:
                                motionid = 2091;
                                break;
                            case 23:
                                motionid = 2085;
                                break;
                            case 24:
                                motionid = 2109;
                                break;
                            case 25:
                                motionid = 2125;
                                break;
                            case 26:
                                motionid = 2098;
                                break;
                            case 27:
                                motionid = 2079;
                                loop = 1;
                                break;
                            case 28:
                                motionid = 1523;
                                break;
                            case 29:
                                motionid = 2080;
                                break;
                            case 30:
                                motionid = 2138;
                                break;
                            case 31:
                                motionid = 2139;
                                break;
                        }

                        MapClient tclient = FromActorPC(target);
                        SendMotion((MotionType)motionid, loop);
                        tclient.SendMotion((MotionType)motionid, loop);
                        SendSystemMessage("你問候了 " + target.Name);
                        tclient.SendSystemMessage(chara.Name + "跟你說哈囉^_^ ");
                        SendEPGreetingTime((ActorPC)actor);//ECOKEY DEM打招呼EP
                        /*if (target.AStr["打招呼每日重置2"] != DateTime.Now.ToString("yyyy-MM-dd"))
                        {
                            target.AStr["打招呼每日重置2"] = DateTime.Now.ToString("yyyy-MM-dd");
                            if (target.CIDict["打招呼的玩家"].Count > 0)
                                target.CIDict["打招呼的玩家"] = new VariableHolderA<int, int>();
                            target.AInt["今日被打招呼次数"] = 0;
                        }
                        if (!target.CIDict["打招呼的玩家"].ContainsKey(Character.Account.AccountID))
                        {
                            target.CIDict["打招呼的玩家"][Character.Account.AccountID] = 0;
                            tclient.TitleProccess(target, 62, 1);
                            if (target.AInt["今日被打招呼次数"] < 1)
                            {
                                target.AInt["今日被打招呼次数"]++;
                                //int cp = Global.Random.Next(100, 280);
                                // int ep = 10;
                                // target.EP = Math.Min((target.EP + (uint)ep), target.MaxEP);
                                //tclient.SendSystemMessage("被亲切地问候了！获得" + cp + "CP。");
                            }
                        }*/


                        chara.TTime["打招呼时间"] = DateTime.Now;
                    }
                }
            }
        }

        public void OnPlayerElements(Packets.Client.CSMG_PLAYER_ELEMENTS p)
        {
            Packets.Server.SSMG_PLAYER_ELEMENTS p1 = new SagaMap.Packets.Server.SSMG_PLAYER_ELEMENTS();
            Dictionary<Elements, int> elements = new Dictionary<Elements, int>();
            foreach (Elements i in this.chara.AttackElements.Keys)
            {
                elements.Add(i, this.chara.AttackElements[i] + this.chara.Status.attackElements_item[i] + this.chara.Status.attackelements_iris[i] + this.chara.Status.attackElements_skill[i]);
            }
            p1.AttackElements = elements;
            elements.Clear();
            foreach (Elements i in this.chara.Elements.Keys)
            {
                elements.Add(i, this.chara.Elements[i] + this.chara.Status.elements_item[i] + this.chara.Status.elements_iris[i] + this.chara.Status.elements_skill[i]);
            }
            p1.DefenceElements = elements;
            this.netIO.SendPacket(p1);
        }
        public void OnPlayerElements()
        {
            Packets.Server.SSMG_PLAYER_ELEMENTS p1 = new SagaMap.Packets.Server.SSMG_PLAYER_ELEMENTS();
            Dictionary<Elements, int> elements = new Dictionary<Elements, int>();
            foreach (Elements i in this.chara.AttackElements.Keys)
            {
                elements.Add(i, this.chara.AttackElements[i] + this.chara.Status.attackElements_item[i] + this.chara.Status.attackelements_iris[i] + this.chara.Status.attackElements_skill[i]);
            }
            p1.AttackElements = elements;
            elements.Clear();
            foreach (Elements i in this.chara.Elements.Keys)
            {
                elements.Add(i, this.chara.Elements[i] + this.chara.Status.elements_item[i] + this.chara.Status.elements_iris[i] + this.chara.Status.elements_skill[i]);
            }
            p1.DefenceElements = elements;
            this.netIO.SendPacket(p1);
        }
        public void OnRequestPCInfo(Packets.Client.CSMG_ACTOR_REQUEST_PC_INFO p)
        {
            Packets.Server.SSMG_ACTOR_PC_INFO p1 = new SagaMap.Packets.Server.SSMG_ACTOR_PC_INFO();
            Actor pc = this.map.GetActor(p.ActorID);

            if (pc == null)
            {
                return;
            }

            if (pc.type == ActorType.PC)
            {
                ActorPC a = (ActorPC)pc;
                a.WRPRanking = WRPRankingManager.Instance.GetRanking(a);
            }
            p1.Actor = pc;

            this.netIO.SendPacket(p1);
            if (pc.type == ActorType.PC)
            {
                ActorPC actor = (ActorPC)pc;
                if (actor.Ring != null)
                {
                    this.Character.e.OnActorRingUpdate(actor);
                }
            }
        }

        public void OnStatsPreCalc(Packets.Client.CSMG_PLAYER_STATS_PRE_CALC p)
        {
            //backup
            ushort str, dex, intel, agi, vit, mag;
            Packets.Server.SSMG_PLAYER_STATS_PRE_CALC p1 = new SagaMap.Packets.Server.SSMG_PLAYER_STATS_PRE_CALC();
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

            //resotre
            this.Character.Str = str;
            this.Character.Dex = dex;
            this.Character.Int = intel;
            this.Character.Agi = agi;
            this.Character.Vit = vit;
            this.Character.Mag = mag;

            StatusFactory.Instance.CalcStatus(this.Character);

            this.netIO.SendPacket(p1);
        }

        public void OnStatsUp(Packets.Client.CSMG_PLAYER_STATS_UP p)
        {
            if (Configuration.Instance.Version < SagaLib.Version.Saga13)
            {
                switch (p.Type)
                {
                    case 0:
                        if (this.Character.StatsPoint >= StatusFactory.Instance.RequiredBonusPoint(this.Character.Str))
                        {
                            this.Character.StatsPoint -= StatusFactory.Instance.RequiredBonusPoint(this.Character.Str);
                            this.Character.Str += 1;
                        }
                        break;
                    case 1:
                        if (this.Character.StatsPoint >= StatusFactory.Instance.RequiredBonusPoint(this.Character.Dex))
                        {
                            this.Character.StatsPoint -= StatusFactory.Instance.RequiredBonusPoint(this.Character.Dex);
                            this.Character.Dex += 1;
                        }
                        break;
                    case 2:
                        if (this.Character.StatsPoint >= StatusFactory.Instance.RequiredBonusPoint(this.Character.Int))
                        {
                            this.Character.StatsPoint -= StatusFactory.Instance.RequiredBonusPoint(this.Character.Int);
                            this.Character.Int += 1;
                        }
                        break;
                    case 3:
                        if (this.Character.StatsPoint >= StatusFactory.Instance.RequiredBonusPoint(this.Character.Vit))
                        {
                            this.Character.StatsPoint -= StatusFactory.Instance.RequiredBonusPoint(this.Character.Vit);
                            this.Character.Vit += 1;
                        }
                        break;
                    case 4:
                        if (this.Character.StatsPoint >= StatusFactory.Instance.RequiredBonusPoint(this.Character.Agi))
                        {
                            this.Character.StatsPoint -= StatusFactory.Instance.RequiredBonusPoint(this.Character.Agi);
                            this.Character.Agi += 1;
                        }
                        break;
                    case 5:
                        if (this.Character.StatsPoint >= StatusFactory.Instance.RequiredBonusPoint(this.Character.Mag))
                        {
                            this.Character.StatsPoint -= StatusFactory.Instance.RequiredBonusPoint(this.Character.Mag);
                            this.Character.Mag += 1;
                        }
                        break;
                }
            }
            else
            {
                if (p.Str > 0)
                {
                    for (int i = p.Str; i > 0; i--)
                    {
                        if (this.Character.StatsPoint >= StatusFactory.Instance.RequiredBonusPoint(this.Character.Str))
                        {
                            this.Character.StatsPoint -= StatusFactory.Instance.RequiredBonusPoint(this.Character.Str);
                            this.Character.Str += 1;
                        }
                    }
                }
                if (p.Dex > 0)
                {
                    for (int i = p.Dex; i > 0; i--)
                    {
                        if (this.Character.StatsPoint >= StatusFactory.Instance.RequiredBonusPoint(this.Character.Dex))
                        {
                            this.Character.StatsPoint -= StatusFactory.Instance.RequiredBonusPoint(this.Character.Dex);
                            this.Character.Dex += 1;
                        }
                    }
                }
                if (p.Int > 0)
                {
                    for (int i = p.Int; i > 0; i--)
                    {
                        if (this.Character.StatsPoint >= StatusFactory.Instance.RequiredBonusPoint(this.Character.Int))
                        {
                            this.Character.StatsPoint -= StatusFactory.Instance.RequiredBonusPoint(this.Character.Int);
                            this.Character.Int += 1;
                        }
                    }
                }
                if (p.Vit > 0)
                {
                    for (int i = p.Vit; i > 0; i--)
                    {
                        if (this.Character.StatsPoint >= StatusFactory.Instance.RequiredBonusPoint(this.Character.Vit))
                        {
                            this.Character.StatsPoint -= StatusFactory.Instance.RequiredBonusPoint(this.Character.Vit);
                            this.Character.Vit += 1;
                        }
                    }
                }
                if (p.Agi > 0)
                {
                    for (int i = p.Agi; i > 0; i--)
                    {
                        if (this.Character.StatsPoint >= StatusFactory.Instance.RequiredBonusPoint(this.Character.Agi))
                        {
                            this.Character.StatsPoint -= StatusFactory.Instance.RequiredBonusPoint(this.Character.Agi);
                            this.Character.Agi += 1;
                        }
                    }
                }

                if (p.Mag > 0)
                {
                    for (int i = p.Mag; i > 0; i--)
                    {
                        if (this.Character.StatsPoint >= StatusFactory.Instance.RequiredBonusPoint(this.Character.Mag))
                        {
                            this.Character.StatsPoint -= StatusFactory.Instance.RequiredBonusPoint(this.Character.Mag);
                            this.Character.Mag += 1;
                        }
                    }
                }

            }
            StatusFactory.Instance.CalcStatus(this.Character);
            SendActorHPMPSP(this.Character);
            SendStatus();
            SendStatusExtend();
            SendCapacity();
            //SendMaxCapacity();
            SendPlayerLevel();
        }

        public void SendWRPRanking(ActorPC pc)//ECOKEY 攻防戰 - 修正紅冠顯示
        {
            foreach (Actor actor in SagaMap.Manager.MapManager.Instance.GetMap(pc.MapID).Actors.Values)
            {
                if (actor.type == ActorType.PC)
                {
                    if (pc.WRPRanking <= 10)
                    {
                        Packets.Server.SSMG_ACTOR_WRP_RANKING p = new SagaMap.Packets.Server.SSMG_ACTOR_WRP_RANKING();
                        p.ActorID = pc.ActorID;
                        p.Ranking = pc.WRPRanking;
                        MapClient.FromActorPC((ActorPC)actor).netIO.SendPacket(p);
                    }
                    if (((ActorPC)actor).WRPRanking <= 10)
                    {
                        Packets.Server.SSMG_ACTOR_WRP_RANKING p = new SagaMap.Packets.Server.SSMG_ACTOR_WRP_RANKING();
                        p.ActorID = ((ActorPC)actor).ActorID;
                        p.Ranking = ((ActorPC)actor).WRPRanking;
                        this.netIO.SendPacket(p);
                    }
                }
            }
        }
        public void RevivePC(ActorPC pc)
        {
            pc.HP = pc.MaxHP;
            pc.MP = pc.MaxMP;
            pc.SP = pc.MaxSP;
            pc.EP = pc.MaxEP;

            if (pc.Job == PC_JOB.CARDINAL)
                pc.EP = 5000;

            if (pc.Job == PC_JOB.ASTRALIST)//魔法师
                pc.EP = 0;

            if (!pc.Status.Additions.ContainsKey("HolyVolition"))
            {
                Skill.Additions.Global.DefaultBuff skill = new Skill.Additions.Global.DefaultBuff(null, pc, "HolyVolition", 2000);
                Skill.SkillHandler.ApplyAddition(pc, skill);
            }

            if (pc.SaveMap == 0)
            {
                pc.SaveMap = 91000999;
                pc.SaveX = 21;
                pc.SaveY = 21;
            }

            pc.BattleStatus = 0;
            SendChangeStatus();

            pc.Buff.Dead = false;
            pc.Buff.TurningPurple = false;
            pc.Motion = MotionType.STAND;
            pc.MotionLoop = false;
            Skill.SkillHandler.Instance.ShowVessel(pc, (int)-pc.MaxHP);
            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.HPMPSP_UPDATE, null, pc, true);

            Skill.SkillHandler.Instance.ShowEffectByActor(pc, 5116);
            Skill.SkillHandler.Instance.CastPassiveSkills(pc);
            SendPlayerInfo();

            if (!pc.Tasks.ContainsKey("Recover"))//自然恢复
            {
                Tasks.PC.Recover reg = new Tasks.PC.Recover(FromActorPC(pc));
                pc.Tasks.Add("Recover", reg);
                reg.Activate();
            }
            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, pc, true);

            if (scriptThread != null)
                ClientManager.RemoveThread(scriptThread.Name);
            scriptThread = null;
            currentEvent = null;

            /*Scripting.Event evnt = null;
            if (evnt != null)
            {
                evnt.CurrentPC = null;
                this.scriptThread = null;
                this.currentEvent = null;
                ClientManager.RemoveThread(System.Threading.Thread.CurrentThread.Name);
                //ClientManager.LeaveCriticalArea();
            }*/
        }

        public void OnPlayerReturnHome(Packets.Client.CSMG_PLAYER_RETURN_HOME p)
        {
            if (this.Character.HP == 0)
            {
                this.Character.HP = 1;
                this.Character.MP = 1;
                this.Character.SP = 1;
            }

            if (this.Character.SaveMap == 0)
            {
                this.Character.SaveMap = 10023100;
                this.Character.SaveX = 242;
                this.Character.SaveY = 128;
            }

            this.Character.BattleStatus = 0;
            this.SendChangeStatus();
            this.Character.Buff.Dead = false;
            this.Character.Buff.TurningPurple = false;
            this.Character.Motion = MotionType.STAND;
            this.Character.MotionLoop = false;
            this.Map.SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.BUFF_CHANGE, null, this.Character, true);

            Skill.SkillHandler.Instance.CastPassiveSkills(this.Character);
            SendPlayerInfo();

            if (this.map.ID == this.Character.SaveMap)
            {
                if (this.Map.Info.Healing)
                {
                    if (!this.Character.Tasks.ContainsKey("CityRecover"))
                    {
                        Tasks.PC.CityRecover task = new SagaMap.Tasks.PC.CityRecover(this);
                        this.Character.Tasks.Add("CityRecover", task);
                        task.Activate();
                    }
                }
                else
                {
                    if (this.Character.Tasks.ContainsKey("CityRecover"))
                    {
                        this.Character.Tasks["CityRecover"].Deactivate();
                        this.Character.Tasks.Remove("CityRecover");
                    }
                }

                /*   if (this.Map.Info.Cold || this.map.Info.Hot || this.map.Info.Wet)
                    {
                        if (!this.Character.Tasks.ContainsKey("CityDown"))
                        {
                            Tasks.PC.CityDown task = new SagaMap.Tasks.PC.CityDown(this);
                            this.Character.Tasks.Add("CityDown", task);
                            task.Activate();
                        }
                    }
                    else
                    {
                        if (this.Character.Tasks.ContainsKey("CityDown"))
                        {
                            this.Character.Tasks["CityDown"].Deactivate();
                            this.Character.Tasks.Remove("CityDown");
                        }
                    }*/
            }


            #region ECOKEY 在騎士團模式中死亡，會回到準備室
            List<uint> ktmap = new List<uint>() { 10023001, 10032001, 10034001, 10042001, 10056001, 10064001, 20020001, 20080007, 20080008, 20080009, 20080010, 20200000 };//ECOKEY 騎士團新增山岳地圖
            if (this.Character.Mode == PlayerMode.KNIGHT_EAST ||
                this.Character.Mode == PlayerMode.KNIGHT_WEST ||
                this.Character.Mode == PlayerMode.KNIGHT_SOUTH ||
                this.Character.Mode == PlayerMode.KNIGHT_NORTH ||
                this.Character.Mode == PlayerMode.KNIGHT_EAST_HERO ||
                this.Character.Mode == PlayerMode.KNIGHT_WEST_HERO ||
                this.Character.Mode == PlayerMode.KNIGHT_SOUTH_HERO ||
                this.Character.Mode == PlayerMode.KNIGHT_NORTH_HERO ||
                ktmap.Contains(this.map.ID))
            {
                this.Character.Mode = PlayerMode.NORMAL;
                SagaMap.Manager.MapManager.Instance.GetMap(this.Character.MapID).SendEventToAllActorsWhoCanSeeActor(Map.EVENT_TYPE.CHAR_INFO_UPDATE, null, this.Character, true);
                if (this.map.ID == 20080007)
                {
                    Map newMap = MapManager.Instance.GetMap(20080007);
                    this.Map.SendActorToMap(this.Character, 20080007, Global.PosX8to16(25, newMap.Width), Global.PosY8to16(28, newMap.Height));
                }
                else if (this.map.ID == 20080008)
                {
                    Map newMap = MapManager.Instance.GetMap(20080008);
                    this.Map.SendActorToMap(this.Character, 20080008, Global.PosX8to16(25, newMap.Width), Global.PosY8to16(28, newMap.Height));
                }
                else if (this.map.ID == 20080009)
                {
                    Map newMap = MapManager.Instance.GetMap(20080009);
                    this.Map.SendActorToMap(this.Character, 20080009, Global.PosX8to16(25, newMap.Width), Global.PosY8to16(28, newMap.Height));
                }
                else if (this.map.ID == 20080010)
                {
                    Map newMap = MapManager.Instance.GetMap(20080010);
                    this.Map.SendActorToMap(this.Character, 20080010, Global.PosX8to16(25, newMap.Width), Global.PosY8to16(28, newMap.Height));
                }
                else if (this.Character.CInt["KNIGHT_MAP"] != 0)
                {
                    this.Character.TTime["KNIGHT"] = DateTime.Now.AddSeconds(30);
                    Map newMap = MapManager.Instance.GetMap((uint)this.Character.CInt["KNIGHT_MAP"]);
                    this.Map.SendActorToMap(this.Character, (uint)this.Character.CInt["KNIGHT_MAP"], Global.PosX8to16(25, newMap.Width), Global.PosY8to16(28, newMap.Height));
                }
                else
                {
                    Map newMap = MapManager.Instance.GetMap(20080011);
                    this.Map.SendActorToMap(this.Character, 20080011, Global.PosX8to16(25, newMap.Width), Global.PosY8to16(28, newMap.Height));
                }
            }
            //ECOKEY 在大逃殺模式中死亡，會回到準備室
            else if (this.Character.Mode == PlayerMode.BATTLE_SOUTH ||
                this.Character.Mode == PlayerMode.BATTLE_NORTH)
            {
                Map newMap = MapManager.Instance.GetMap(20080011);
                this.Map.SendActorToMap(this.Character, 20080011, Global.PosX8to16(25, newMap.Width), Global.PosY8to16(28, newMap.Height));
            }
            else if (Configuration.Instance.HostedMaps.Contains(this.Character.SaveMap))
            {
                MapInfo info = MapInfoFactory.Instance.MapInfo[this.Character.SaveMap];
                this.Map.SendActorToMap(this.Character, this.Character.SaveMap, Global.PosX8to16(this.Character.SaveX, info.width),
                    Global.PosY8to16(this.Character.SaveY, info.height));
            }
            #endregion

            //ECOKEY 修復角色卡在任務中
            /*Scripting.Event evnt = null;
            if (evnt != null)
            {
                evnt.CurrentPC = null;
                this.scriptThread = null;
                this.currentEvent = null;
                ClientManager.RemoveThread(System.Threading.Thread.CurrentThread.Name);
                ClientManager.LeaveCriticalArea();
            }*/
            if (scriptThread != null)
            {
                ClientManager.RemoveThread(scriptThread.Name);
                scriptThread = null;
                currentEvent = null;
                ClientManager.LeaveCriticalArea();
            }
        }


        public void SendDefWarChange(DefWar text)
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_DEFWAR_SET p11 = new SagaMap.Packets.Server.SSMG_DEFWAR_SET();
                p11.MapID = this.map.ID;
                p11.Data = text;
                this.netIO.SendPacket(p11);
            }
        }

        public void SendDefWarResult(byte r1, byte r2, int exp, int jobexp, int cp, byte u = 0)
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_DEFWAR_RESULT p11 = new SagaMap.Packets.Server.SSMG_DEFWAR_RESULT();
                p11.Result1 = r1;
                p11.Result2 = r2;
                p11.EXP = exp;
                p11.JOBEXP = jobexp;
                p11.CP = cp;

                p11.Unknown = u;
                this.netIO.SendPacket(p11);
            }
        }
        public void SendDefWarState(byte rate)
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_DEFWAR_STATE p1 = new SagaMap.Packets.Server.SSMG_DEFWAR_STATE();
                p1.MapID = this.map.ID;
                p1.Rate = rate;
                this.netIO.SendPacket(p1);
            }
        }

        public void SendDefWarStates(Dictionary<uint, byte> list)
        {
            if (this.Character.Online)
            {
                Packets.Server.SSMG_DEFWAR_STATES p1 = new SagaMap.Packets.Server.SSMG_DEFWAR_STATES();
                p1.List = list;
                this.netIO.SendPacket(p1);
            }
        }

        //ECOKEY-TEST
        public void SendActorFRIENDMAP(Actor actor, uint map)
        {
            if (this.Character.Online)
            {
                SagaLogin.Packets.Server.SSMG_FRIEND_MAP_UPDATE p = new SagaLogin.Packets.Server.SSMG_FRIEND_MAP_UPDATE();
                p.CharID = actor.ActorID;
                p.MapID = this.Character.MapID;
                this.netIO.SendPacket(p);
            }
        }

    }
}
