using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaLogin;
using SagaLogin.Network.Client;

namespace SagaLogin.Packets.Client
{
    public class CSMG_FRIEND_MEMO_UPDATE : Packet
    {
        public CSMG_FRIEND_MEMO_UPDATE()
        {
            this.offset = 2;
            //this.data = new byte[6];
        }

        public String Memo
        {
            get
            {
                return Global.Unicode.GetString(this.GetBytes(this.GetByte(2), 3)).Replace("\0", "");
            }
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaLogin.Packets.Client.CSMG_FRIEND_MEMO_UPDATE();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((LoginClient)(client)).SendFriendMemo(this);
        }

    }
}