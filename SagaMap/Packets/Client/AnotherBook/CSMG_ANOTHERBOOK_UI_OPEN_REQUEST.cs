using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_ANOTHERBOOK_UI_OPEN_REQEUST : Packet
    {
        public CSMG_ANOTHERBOOK_UI_OPEN_REQEUST()
        {
            this.offset = 2;
        }
        public byte index
        {
            get
            {
                return GetByte(2);
            }
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_ANOTHERBOOK_UI_OPEN_REQEUST();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnAnotherBookUIOpen(this);
        }

    }
}