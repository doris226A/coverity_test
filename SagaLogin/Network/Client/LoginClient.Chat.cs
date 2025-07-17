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
    public partial class LoginClient : SagaLib.Client
    {
        public void OnChatWhisper(Packets.Client.CSMG_CHAT_WHISPER p)
        {
            if (this.selectedChar == null) return;
            var receiver = p.Receiver;
            if (receiver != "LoginSrv")
            {
                LoginClient client = LoginClientManager.Instance.FindClient(receiver);
                if (client != null)
                {
                    Packets.Server.SSMG_CHAT_WHISPER p1 = new SagaLogin.Packets.Server.SSMG_CHAT_WHISPER();
                    p1.Sender = this.selectedChar.Name;
                    p1.Content = p.Content;
                    client.netIO.SendPacket(p1);
                    Logger logger = new Logger("玩家私聊聊天.txt");
                    string log = "\r\n玩家" + this.selectedChar.Name + "：向誰講" + p.Content + "";
                    logger.WriteLog(log);
                }
                else
                {
                    Packets.Server.SSMG_CHAT_WHISPER_FAILED p1 = new SagaLogin.Packets.Server.SSMG_CHAT_WHISPER_FAILED();
                    p1.Receiver = p.Receiver;
                    p1.Result = 0xFFFFFFFF;
                    this.netIO.SendPacket(p1);
                }
            }
            else
            {
                var args = p.Content;
                if (args == "test")
                    args = "01 3C 01 00 00 00 01 00 00 00 02 01 00 01 01 00 01 01 00 01 05 41 41 41 41 00 01 05 41 41 41 41 00";
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
                Logger.ShowInfo(str);
            }
        }
    }
}
