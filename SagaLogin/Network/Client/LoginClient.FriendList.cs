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
using SagaLogin;
using SagaLogin.Manager;

namespace SagaLogin.Network.Client
{
    public enum CharStatus
    {
        OFFLINE,
        ONLINE,
        募集中,
        取り込み中,
        お話し中,
        休憩中,
        退席中,
        戦闘中,
        商売中,
        憑依中,
        クエスト中,
        お祭り中,
        連絡求む
    }

    public partial class LoginClient : SagaLib.Client
    {
        public uint mapid2 = 0;
        public CharStatus currentStatus = CharStatus.ONLINE;
        public byte lv, joblv;
        public PC_JOB job;
        LoginClient friendTarget = null;

        public void OnFriendDelete(Packets.Client.CSMG_FRIEND_DELETE p)
        {
            if (this.selectedChar == null)
                return;
            LoginServer.charDB.DeleteFriend(this.selectedChar.CharID, p.CharID);
            LoginServer.charDB.DeleteFriend(p.CharID, this.selectedChar.CharID);
            Packets.Server.SSMG_FRIEND_DELETE p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_DELETE();
            p1.CharID = p.CharID;
            this.netIO.SendPacket(p1);
            LoginClient client = LoginClientManager.Instance.FindClient(p.CharID);
            if (client != null)
            {
                p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_DELETE();
                p1.CharID = this.selectedChar.CharID;
                client.netIO.SendPacket(p1);
            }
        }

        public void OnFriendAdd(Packets.Client.CSMG_FRIEND_ADD p)
        {
            LoginClient client = LoginClientManager.Instance.FindClient(p.CharID);
            //ECOKEY 拒絕好友申請設定
            ActorPC pc = LoginServer.charDB.GetChar(p.CharID, false);
            LoginServer.charDB.GetVar(pc);
            if (pc.CInt["canFriend"] == 0)
            {
                Packets.Server.SSMG_FRIEND_ADD_FAILED p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_ADD_FAILED();
                p1.AddResult = SagaLogin.Packets.Server.SSMG_FRIEND_ADD_FAILED.Result.TARGET_NO_SET;
                this.netIO.SendPacket(p1);
                return;
            }
            if (client != null)
            {
                if (!LoginServer.charDB.IsFriend(this.selectedChar.CharID, client.selectedChar.CharID))
                {
                    friendTarget = client;
                    client.friendTarget = this;
                    Packets.Server.SSMG_FRIEND_ADD p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_ADD();
                    p1.CharID = this.selectedChar.CharID;
                    p1.Name = this.selectedChar.Name;
                    client.netIO.SendPacket(p1);
                }
                else
                {
                    Packets.Server.SSMG_FRIEND_ADD_FAILED p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_ADD_FAILED();
                    p1.AddResult = SagaLogin.Packets.Server.SSMG_FRIEND_ADD_FAILED.Result.IS_FRIEND;
                    this.netIO.SendPacket(p1);
                }
            }
            else
            {
                Packets.Server.SSMG_FRIEND_ADD_FAILED p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_ADD_FAILED();
                p1.AddResult = SagaLogin.Packets.Server.SSMG_FRIEND_ADD_FAILED.Result.CANNOT_FIND_TARGET;
                this.netIO.SendPacket(p1);
            }
        }

        private int CheckFriendRegist(LoginClient client)
        {
            if (client == null)
                return -1; //相手が見付かりません
            if (LoginServer.charDB.IsFriend(this.selectedChar.CharID, client.selectedChar.CharID))
                return -2; //既に登録しています
            //return -3; //相手に拒否されました
            //return -4; //フレンドリストに空きがありません
            //return -6; //相手がフレンド招待不許可設定です
            return 0;
        }

        public void OnFriendAddReply(Packets.Client.CSMG_FRIEND_ADD_REPLY p)
        {
            if (this.friendTarget == null)
            {
                Packets.Server.SSMG_FRIEND_ADD_FAILED p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_ADD_FAILED();
                p1.AddResult = SagaLogin.Packets.Server.SSMG_FRIEND_ADD_FAILED.Result.CANNOT_FIND_TARGET;
                this.netIO.SendPacket(p1);
                return;
            }
            if (p.Reply == 1)
            {
                Packets.Server.SSMG_FRIEND_ADD_OK p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_ADD_OK();
                p1.CharID = this.friendTarget.selectedChar.CharID;
                this.netIO.SendPacket(p1);
                this.SendFriendAdd(this.friendTarget);
                LoginServer.charDB.AddFriend(this.selectedChar, this.friendTarget.selectedChar.CharID);
                p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_ADD_OK();
                p1.CharID = this.selectedChar.CharID;
                this.friendTarget.netIO.SendPacket(p1);
                this.friendTarget.SendFriendAdd(this);
                LoginServer.charDB.AddFriend(this.friendTarget.selectedChar, this.selectedChar.CharID);
            }
            else
            {
                Packets.Server.SSMG_FRIEND_ADD_FAILED p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_ADD_FAILED();
                p1.AddResult = SagaLogin.Packets.Server.SSMG_FRIEND_ADD_FAILED.Result.TARGET_REFUSED;
                this.friendTarget.netIO.SendPacket(p1);
            }
            this.friendTarget = null;
        }

        public void OnFriendMapUpdate(Packets.Client.CSMG_FRIEND_MAP_UPDATE p)
        {
            ActorPC chara = new ActorPC();
            uint mapid1 = 0;
            mapid1 += chara.MapID;
            if (this.selectedChar == null) return;
            List<ActorPC> friendlist = LoginServer.charDB.GetFriendList2(this.selectedChar);
            mapid1 = p.MapID;
            foreach (ActorPC i in friendlist)
            {
                LoginClient client = LoginClientManager.Instance.FindClient(i);
                Packets.Server.SSMG_FRIEND_MAP_UPDATE p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_MAP_UPDATE();
                p1.CharID = this.selectedChar.CharID;
                if (client != null)
                {
                    //無效
                    //ECOKEY 改
                    p1.MapID = 10049000;
                    //p1.MapID = mapid1;
                    client.netIO.SendPacket(p1);
                }
            }
        }
        public void OnFriendDetailUpdate(Packets.Client.CSMG_FRIEND_DETAIL_UPDATE p)
        {
            if (this.selectedChar == null) return;
            List<ActorPC> friendlist = LoginServer.charDB.GetFriendList2(this.selectedChar);
            this.job = p.Job;
            this.lv = p.Level;
            this.joblv = p.Level;

            foreach (ActorPC i in friendlist)
            {
                LoginClient client = LoginClientManager.Instance.FindClient(i);
                Packets.Server.SSMG_FRIEND_DETAIL_UPDATE p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_DETAIL_UPDATE();
                p1.CharID = this.selectedChar.CharID;
                if (client != null)
                {
                    p1.Job = this.job;
                    p1.Level = this.lv;
                    p1.JobLevel = this.joblv;
                    client.netIO.SendPacket(p1);
                }
            }
        }

        public void SendFriendAdd(LoginClient client)
        {
            ActorPC chara = new ActorPC();
            uint mapid1 = 0;
            mapid1 += chara.MapID;

            Packets.Server.SSMG_FRIEND_CHAR_INFO p = new SagaLogin.Packets.Server.SSMG_FRIEND_CHAR_INFO();
            p.ActorPC = client.selectedChar;
            p.MapID = mapid1;
            p.Status = client.currentStatus;
            p.Comment = "";
            this.netIO.SendPacket(p);
        }
        //ECOKEY 好友列表修正
        public void SendFriendList()
        {
            if (this.selectedChar == null) return;
            List<ActorPC> friendlist = LoginServer.charDB.GetFriendList(this.selectedChar);
            foreach (ActorPC i in friendlist)
            {
                LoginClient client = LoginClientManager.Instance.FindClient(i);

                Packets.Server.SSMG_FRIEND_CHAR_INFO p = new SagaLogin.Packets.Server.SSMG_FRIEND_CHAR_INFO();
                p.ActorPC = i;
                //這裡!!!改這句就好，修正離線還看的到地圖
                if (client != null && client.currentStatus != CharStatus.OFFLINE)
                {
                    //地圖更改
                    p.MapID = i.MapID;
                    p.Status = client.currentStatus;
                }
                p.Comment = i.FriendMemo;
                this.netIO.SendPacket(p);
            }
            Packet p2 = new Packet();
            p2.data = new byte[8];
            p2.ID = 0x00DE;
            this.netIO.SendPacket(p2);
        }
        //ECOKEY 好友列表修正
        public void SendStatusToFriends()
        {
            if (this.selectedChar == null) return;
            LoginClient clientPC = LoginClientManager.Instance.FindClient(this.selectedChar);
            List<ActorPC> friendlist = LoginServer.charDB.GetFriendList2(this.selectedChar);
            foreach (ActorPC i in friendlist)
            {
                LoginClient client = LoginClientManager.Instance.FindClient(i);
                Packets.Server.SSMG_FRIEND_STATUS_UPDATE p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_STATUS_UPDATE();
                Packets.Server.SSMG_FRIEND_DETAIL_UPDATE p2 = new SagaLogin.Packets.Server.SSMG_FRIEND_DETAIL_UPDATE();
                p1.CharID = this.selectedChar.CharID;
                p2.CharID = this.selectedChar.CharID;
                if (client != null)
                {
                    p1.Status = clientPC.currentStatus;
                    p2.Job = this.selectedChar.Job;
                    p2.Level = this.selectedChar.Level;
                    p2.JobLevel = this.selectedChar.CurrentJobLevel;
                    client.netIO.SendPacket(p1);
                    client.netIO.SendPacket(p2);
                }
            }
        }
        //ECOKEY 切換好友列表狀態
        public void SendFriendStatus(Packets.Client.CSMG_FRIEND_CharStatus_UPDATE p)
        {
            if (this.selectedChar == null) return;
            LoginClient client = LoginClientManager.Instance.FindClient(this.selectedChar);
            switch (p.Status)
            {
                case 0:
                    client.currentStatus = CharStatus.OFFLINE;
                    this.currentStatus = CharStatus.OFFLINE;
                    break;
                case 1:
                    client.currentStatus = CharStatus.ONLINE;
                    this.currentStatus = CharStatus.ONLINE;
                    break;
                case 2:
                    client.currentStatus = CharStatus.募集中;
                    this.currentStatus = CharStatus.募集中;
                    break;
                case 3:
                    client.currentStatus = CharStatus.取り込み中;
                    this.currentStatus = CharStatus.取り込み中;
                    break;
                case 4:
                    client.currentStatus = CharStatus.お話し中;
                    this.currentStatus = CharStatus.お話し中;
                    break;
                case 5:
                    client.currentStatus = CharStatus.休憩中;
                    this.currentStatus = CharStatus.休憩中;
                    break;
                case 6:
                    client.currentStatus = CharStatus.退席中;
                    this.currentStatus = CharStatus.退席中;
                    break;
                case 7:
                    client.currentStatus = CharStatus.戦闘中;
                    this.currentStatus = CharStatus.戦闘中;
                    break;
                case 8:
                    client.currentStatus = CharStatus.商売中;
                    this.currentStatus = CharStatus.商売中;
                    break;
                case 9:
                    client.currentStatus = CharStatus.憑依中;
                    this.currentStatus = CharStatus.憑依中;
                    break;
                case 10:
                    client.currentStatus = CharStatus.クエスト中;
                    this.currentStatus = CharStatus.クエスト中;
                    break;
                case 11:
                    client.currentStatus = CharStatus.お祭り中;
                    this.currentStatus = CharStatus.お祭り中;
                    break;
                case 12:
                    client.currentStatus = CharStatus.連絡求む;
                    this.currentStatus = CharStatus.連絡求む;
                    break;
            }
            SendFriendList();
            SendStatusToFriends();
        }
        //ECOKEY 好友(偽)限時動態
        public void SendFriendMemo(Packets.Client.CSMG_FRIEND_MEMO_UPDATE p)
        {
            this.selectedChar.FriendMemo = p.Memo;
            LoginServer.charDB.SaveCharfriend(this.selectedChar, this.selectedChar.MapID, this.selectedChar.FriendMemo);
            SendFriendList();
            SendStatusToFriends();
        }
    }
}
