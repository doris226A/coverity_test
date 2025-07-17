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
using SagaValidation;
using SagaValidation.Manager;

namespace SagaValidation.Network.Client
{
    public partial class ValidationClient : SagaLib.Client
    {
        private string client_Version;

        private uint frontWord, backWord;

        public bool IsMapServer = false;

        //public Account account;

        public enum SESSION_STATE
        {
            LOGIN, MAP, REDIRECTING, DISCONNECTED
        }
        public SESSION_STATE state;

        public ValidationClient(Socket mSock, Dictionary<ushort, Packet> mCommandTable)
        {
            this.netIO = new NetIO(mSock, mCommandTable, this);
            this.netIO.FirstLevelLength = 2;
            this.netIO.SetMode(NetIO.Mode.Server);
            if (this.netIO.sock.Connected) this.OnConnect();
        }
        public override void OnConnect()
        {

        }
        public override void OnDisconnect()
        {
            if (ValidationClientManager.Instance.Clients.Contains(this))
                ValidationClientManager.Instance.Clients.Remove(this);
        }

      /*  public void OnLogin(Packets.Client.CSMG_LOGIN p)
        {
            try
            { 
                p.GetContent();

                // 建立 TCP 初始回應
                var ackPacket = new Packets.Server.SSMG_LOGIN_ACK
                {
                    LoginResult = Packets.Server.SSMG_LOGIN_ACK.Result.OK
                };
                this.netIO.SendPacket(ackPacket);

                // 從資料庫取得帳戶資料，並處理空帳戶
                Account tmp = ValidationServer.accountDB.GetUser(p.UserName.Replace("\\", "").Replace("'", "\\'"));
                if (tmp == null)
                {
                    SendErrorAndDisconnect(Packets.Server.SSMG_LOGIN_ACK.Result.GAME_SMSG_LOGIN_ERR_BADPASS);
                    return;
                }

                // 處理伺服器關閉邏輯
                if (Configuration.Instance.ServerClose && tmp.GMLevel <= 200)
                {
                    SendErrorAndDisconnect(Packets.Server.SSMG_LOGIN_ACK.Result.GAME_SMSG_LOGIN_ERR_IPBLOCK);
                    return;
                }

                // 驗證密碼
                if (!ValidationServer.accountDB.CheckPassword(p.UserName, p.Password, this.frontWord, this.backWord))
                {
                    SendErrorAndDisconnect(Packets.Server.SSMG_LOGIN_ACK.Result.GAME_SMSG_LOGIN_ERR_BADPASS);
                    return;
                }

                // 檢查帳戶是否被封禁
                if (tmp.Banned)
                {
                    SendErrorAndDisconnect(Packets.Server.SSMG_LOGIN_ACK.Result.GAME_SMSG_LOGIN_ERR_BFALOCK);
                    return;
                }

                // 登入成功後進行後續處理
                Logger.ShowInfo($"User {p.UserName} logged in successfully.");
            }
            catch (Exception ex)
            {
                Logger.ShowInfo($"Error in OnLogin: {ex.Message}");
                DisconnectSafely();
            }
        }

        /// <summary>
        /// 發送錯誤訊息並斷開連線
        /// </summary>
        private void SendErrorAndDisconnect(Packets.Server.SSMG_LOGIN_ACK.Result errorResult)
        {
            var errorPacket = new Packets.Server.SSMG_LOGIN_ACK
            {
                LoginResult = errorResult
            };
            this.netIO.SendPacket(errorPacket);
            DisconnectSafely();
        }

        /// <summary>
        /// 安全地斷開連線
        /// </summary>
        private void DisconnectSafely()
        {
            try
            {
                this.netIO.Disconnect();
            }
            catch (Exception ex)
            {
                Logger.ShowInfo($"Error during disconnect: {ex.Message}");
            }
        }*/

           public void OnLogin(Packets.Client.CSMG_LOGIN p)
           {
               p.GetContent();

               //Establish TCP ACK Flag at first handshake
               Packets.Server.SSMG_LOGIN_ACK p0 = new SagaValidation.Packets.Server.SSMG_LOGIN_ACK();
               p0.LoginResult = Packets.Server.SSMG_LOGIN_ACK.Result.OK;
               this.netIO.SendPacket(p0);

               Account tmp = ValidationServer.accountDB.GetUser(p.UserName.Replace("\\", "").Replace("'", "\\'"));

               if (Configuration.Instance.ServerClose == true)
               {
                   if (tmp.GMLevel <= 200)
                   {
                       Packets.Server.SSMG_LOGIN_ACK p1 = new SagaValidation.Packets.Server.SSMG_LOGIN_ACK();
                       p1.LoginResult = SagaValidation.Packets.Server.SSMG_LOGIN_ACK.Result.GAME_SMSG_LOGIN_ERR_IPBLOCK;
                       this.netIO.SendPacket(p1);
                       this.netIO.Disconnect();
                       return;
                   }
               }


               if (ValidationServer.accountDB.CheckPassword(p.UserName, p.Password, this.frontWord, this.backWord))
               {
                   if (tmp.Banned)
                   {
                       Packets.Server.SSMG_LOGIN_ACK p2 = new SagaValidation.Packets.Server.SSMG_LOGIN_ACK();
                       p2.LoginResult = SagaValidation.Packets.Server.SSMG_LOGIN_ACK.Result.GAME_SMSG_LOGIN_ERR_BFALOCK;
                       this.netIO.SendPacket(p2);
                       this.netIO.Disconnect();
                       return;
                       //TCP Connection terminated.
                   }

               }
               else
               {
                   Packets.Server.SSMG_LOGIN_ACK p1 = new SagaValidation.Packets.Server.SSMG_LOGIN_ACK();
                   p1.LoginResult = SagaValidation.Packets.Server.SSMG_LOGIN_ACK.Result.GAME_SMSG_LOGIN_ERR_BADPASS;
                   this.netIO.SendPacket(p1);
                   this.netIO.Disconnect();
                   return;
                   //TCP Connection terminated.
               }
           }
        public void OnSendVersion(Packets.Client.CSMG_SEND_VERSION p)
        {
            Logger.ShowInfo("Client(Version:" + p.GetVersion() + ") is trying to connect...");
            this.client_Version = p.GetVersion();

            string args = "FF FF E8 6A 6A CA DC E8 06 05 2B 29 F8 96 2F 86 7C AB 2A 57 AD 30";
            byte[] buf = Conversions.HexStr2Bytes(args.Replace(" ", ""));
            Packet p3 = new Packet();
            p3.data = buf;
            this.netIO.SendPacket(p3);

            Packets.Server.SSMG_VERSION_ACK p1 = new SagaValidation.Packets.Server.SSMG_VERSION_ACK();
            p1.SetResult(SagaValidation.Packets.Server.SSMG_VERSION_ACK.Result.OK);
            p1.SetVersion(this.client_Version);
            this.netIO.SendPacket(p1);

            Packets.Server.SSMG_LOGIN_ALLOWED p2 = new SagaValidation.Packets.Server.SSMG_LOGIN_ALLOWED();
            this.frontWord = (uint)Global.Random.Next();
            this.backWord = (uint)Global.Random.Next();
            p2.FrontWord = this.frontWord;
            p2.BackWord = this.backWord;
            this.netIO.SendPacket(p2);
        }
        public void OnServerLstSend(Packets.Client.CSMG_SERVERLET_ASK p)
        {
            Packets.Server.SSMG_SERVER_LST_STAER p1 = new Packets.Server.SSMG_SERVER_LST_STAER();
            this.netIO.SendPacket(p1);

            Packets.Server.SSMG_SERVER_LST_SEND p2 = new Packets.Server.SSMG_SERVER_LST_SEND();
            p2.SevName = Configuration.Instance.ServerName;
            p2.SevIP = "T" + Configuration.Instance.ServerIP + "," + Configuration.Instance.ServerIP + "," + Configuration.Instance.ServerIP + "," + Configuration.Instance.ServerIP;
            this.netIO.SendPacket(p2);

            Packets.Server.SSMG_SERVER_LST_END p3 = new Packets.Server.SSMG_SERVER_LST_END();
            this.netIO.SendPacket(p3);
        }
        public void OnPing(Packets.Client.CSMG_PING p)
        {
            Packets.Server.SSMG_PONG p1 = new SagaValidation.Packets.Server.SSMG_PONG();
            this.netIO.SendPacket(p1);
        }
        public void OnUnknownList(Packets.Client.CSMG_UNKNOWN_LIST p)
        {
            Packets.Server.SSMG_UNKNOWN_RETURN p1 = new SagaValidation.Packets.Server.SSMG_UNKNOWN_RETURN();
            this.netIO.SendPacket(p1);
        }

    }
}
