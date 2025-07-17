using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_IRIS_CARD_REMOVE_SLOT : Packet
    {
        public CSMG_IRIS_CARD_REMOVE_SLOT()
        {
            this.offset = 2;
        }

        public short CardSlot
        {
            get
            {
                return this.GetByte(7);
            }
        }


        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_IRIS_CARD_REMOVE_SLOT();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnIrisCardRemoveSlot(this);
        }

    }
}