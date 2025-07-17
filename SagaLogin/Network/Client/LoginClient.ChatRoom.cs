using SagaDB.Actor;
using SagaLib;
using SagaLogin.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaLogin.Network.Client
{
    public partial class LoginClient : SagaLib.Client
    {
        public void OnCreateChatRoom(Packets.Client.CSMG_CHAT_CREATE_CHAT_ROOM_REQUEST p)
        {
           /* var type = p.Type;
            var name = p.ChatRoomName;
            var comment = p.Comment;
            var pass = p.Password;
            Logger.ShowInfo("Create Chat Room Request Recive.Type: " + type + ", Name: " + name + ", Comment: " + comment + ", Password: " + pass);

            var p1 = new Packets.Server.SSMG_CHAT_CREATE_CHAT_ROOM_RESULT();
            p1.Result = 0;
            this.netIO.SendPacket(p1);

            var args = "01 32 00 00 00 01 00 00 00 0A 01 00 01 00 00 02 00";
            var buf = Conversions.HexStr2Bytes(args.Replace(" ", ""));
            p1.data = buf;
            this.netIO.SendPacket(p1);

            var p2 = new Packets.Server.SSMG_CHAT_SEND_CHAT_ROOM_MESSAGE();
            p2.Sender = "";
            p2.Content = selectedChar.Name + " 进入了聊天室";
            this.netIO.SendPacket(p2);*/
        }

        public void OnEnterChatRoom(Packets.Client.CSMG_CHAT_ENTER_CHAT_ROOM_REQUEST p)
        {
          /*  var type = p.Type;
            var name = p.ChatRoomID;
            var pass = p.Password;
            Logger.ShowInfo("Enter Chat Room Request Recive.Type: " + type + ", RoomID: " + name + ", Password: " + pass);

            if (this.selectedChar == null) return;
            List<ActorPC> friendlist = LoginServer.charDB.GetFriendList2(this.selectedChar);
            foreach (ActorPC i in friendlist)
            {
                LoginClient client = LoginClientManager.Instance.FindClient(i);
                //Packets.Server.SSMG_FRIEND_DETAIL_UPDATE p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_DETAIL_UPDATE();
                //p1.CharID = this.selectedChar.CharID;
                //if (client != null)
                //{
                //    p1.Job = this.job;
                //    p1.Level = this.lv;
                //    p1.JobLevel = this.joblv;
                //    client.netIO.SendPacket(p1);
                //}
                var args1 = "";
                Packet p11 = new Packet();
                args1 = "01 32 00 00 00 01 00 00 00 0A 01 00 01 00 00 02 00";
                var buf1 = Conversions.HexStr2Bytes(args1.Replace(" ", ""));
                p11.data = buf1;
                client.netIO.SendPacket(p11);

                var p21 = new Packets.Server.SSMG_CHAT_SEND_CHAT_ROOM_MESSAGE();
                p21.Sender = "";
                p21.Content = selectedChar.Name + " 进入了聊天室";
                client.netIO.SendPacket(p21);
            }

            var args = "";
            Packet p1 = new Packet();
            args = "01 32 00 00 00 01 00 00 00 0A 01 00 01 00 00 02 00";
            var buf = Conversions.HexStr2Bytes(args.Replace(" ", ""));
            p1.data = buf;
            this.netIO.SendPacket(p1);

            var p2 = new Packets.Server.SSMG_CHAT_SEND_CHAT_ROOM_MESSAGE();
            p2.Sender = "";
            p2.Content = selectedChar.Name + " 进入了聊天室";
            this.netIO.SendPacket(p2);*/
        }

        public void OnLeaveChatRoom(Packets.Client.CSMG_CHAT_LEAVE_CHAT_ROOM_REQUEST p)
        {
         /*   if (this.selectedChar == null) return;
            List<ActorPC> friendlist = LoginServer.charDB.GetFriendList2(this.selectedChar);
            foreach (ActorPC i in friendlist)
            {
                LoginClient client = LoginClientManager.Instance.FindClient(i);
                //Packets.Server.SSMG_FRIEND_DETAIL_UPDATE p1 = new SagaLogin.Packets.Server.SSMG_FRIEND_DETAIL_UPDATE();
                //p1.CharID = this.selectedChar.CharID;
                //if (client != null)
                //{
                //    p1.Job = this.job;
                //    p1.Level = this.lv;
                //    p1.JobLevel = this.joblv;
                //    client.netIO.SendPacket(p1);
                //}
                var args1 = "";
                Packet p11 = new Packet();
                args1 = "01 32 00 00 00 01 00 00 00 0A 01 00 01 00 00 02 00";
                var buf1 = Conversions.HexStr2Bytes(args1.Replace(" ", ""));
                p11.data = buf1;
                client.netIO.SendPacket(p11);

                var p21 = new Packets.Server.SSMG_CHAT_SEND_CHAT_ROOM_MESSAGE();
                p21.Sender = "";
                p21.Content = selectedChar.Name + " 离开了聊天室";
                client.netIO.SendPacket(p21);
            }

            var args = "";
            Packet p1 = new Packet();
            args = "01 32 00 00 00 01 00 00 00 0A 01 00 01 00 00 02 00";
            var buf = Conversions.HexStr2Bytes(args.Replace(" ", ""));
            p1.data = buf;
            this.netIO.SendPacket(p1);

            var p2 = new Packets.Server.SSMG_CHAT_LEAVE_CHAT_ROOM_RESULT();
            p2.Result = 0;
            this.netIO.SendPacket(p2);*/
        }

        public void OnRequestChatRoomList(Packets.Client.CSMG_CHAT_REQUEST_CHAT_ROOM_LIST p)
        {
           /* var type = p.Type;
            var currentPage = p.CurrentPage;

            var args = "";
            if (args == "")
                args = "01 3C 01 00 00 00 00 00 00 00 02 01 00 0F 01 00 03 01 00 01 05 43 43 41 43 00 01 05 42 42 42 42 00";
            byte[] buf = Conversions.HexStr2Bytes(args.Replace(" ", ""));
            Packet p1 = new Packet();
            p1.data = buf;
            this.netIO.SendPacket(p1);

            var str = "Sending Packet : ";
            foreach (var item in p1.data)
            {
                str += item.ToString("X2") + " ";
            }
            str += "\r\n";
            Logger.ShowInfo(str);*/
        }
    }
}
