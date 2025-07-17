using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;
using SagaMap;
using SagaMap.Network.Client;

namespace SagaMap.Packets.Client
{
    public class CSMG_ITEM_BREAK : Packet
    {
        public CSMG_ITEM_BREAK()
        {
            this.offset = 2;
        }

        public uint Item
        {
            get
            {
                return this.GetUInt(2);
            }
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.CSMG_ITEM_BREAK();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnItemBreak(this);
        }

    }
}