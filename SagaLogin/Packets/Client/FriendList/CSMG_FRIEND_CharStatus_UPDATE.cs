using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaLogin;
using SagaLogin.Network.Client;

namespace SagaLogin.Packets.Client
{
    public class CSMG_FRIEND_CharStatus_UPDATE : Packet
    {
        public CSMG_FRIEND_CharStatus_UPDATE()
        {
            this.offset = 2;
            //this.data = new byte[6];
        }

        public uint Status
        {
            get
            {
                return this.GetByte(2);
            }
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaLogin.Packets.Client.CSMG_FRIEND_CharStatus_UPDATE();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((LoginClient)(client)).SendFriendStatus(this);
        }

    }
}